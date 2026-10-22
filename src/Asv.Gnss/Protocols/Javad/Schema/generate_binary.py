#!/usr/bin/env python3
"""Generate typed GREIS binary models from the reviewed, maintained JSON schema.

The specification is an input to schema review, never a runtime/build dependency.
Run from any directory; --check verifies checked-in generated source is current.
"""
from pathlib import Path
import argparse
import html
import json
import re

ROOT = Path(__file__).resolve().parent.parent
SCHEMA = json.loads((ROOT / 'Schema/binary-messages.json').read_text())
TYPES = {'u1':('byte',1,'U1'),'i1':('sbyte',1,'I1'),'u2':('ushort',2,'U2'),'i2':('short',2,'I2'),'u4':('uint',4,'U4'),'i4':('int',4,'I4'),'f4':('float',4,'F4'),'f8':('double',8,'F8'),'a1':('char',1,'U1')}
RECORDS = {r['name']:r for r in SCHEMA['records']}
CUSTOM = {'IqSamples','Spectrum','ExtendedSpectrum','SingleSpectrum','LoggingHistory','BeiDouEphemeris'}
VISITOR_TYPES = {'u1':'UInt8', 'i1':'Int8', 'u2':'UInt16', 'i2':'Int16', 'u4':'UInt32', 'i4':'Int32', 'f4':'Float', 'f8':'Double', 'a1':'Char'}
UNITS = {
    's':'s', 'm':'m', 'm/s':'m/s', 'rad':'rad', 'ms':'ms', 's/s':'s/s',
    'm^2':'m^2', '(m/s)^2':'(m/s)^2', 'semi-circles':'semi-circles', 'meters':'m',
    'rad/sec':'rad/s', 'deg':'deg', 'g':'g', 'semi-circle/s':'semi-circle/s', 'Hz':'Hz',
    's/semicircles':'s/semicircles', 's/semicircles2':'s/semicircles^2',
    's/semicircles3':'s/semicircles^3', 'Hz*104':'Hz*10^4', 'rad/s':'rad/s',
    'm/sec2':'m/s^2', 'm^0.5':'m^0.5', 'm/s^2':'m/s^2', 'm/sec^2':'m/s^2',
    'sec':'s', 'cycles':'cycles', 'percents':'%', 'Hz*10-4':'Hz*10^-4',
    '(1/256)*dB*Hz':'(1/256)*dB*Hz', 's/(s^2)':'s/s^2', 's/s/s':'s/s^2',
    'degrees':'deg', 'degrees*2':'deg*2', 'Seconds * 1e11':'s*10^11',
    's*10-11':'s*10^-11', 'cycles/1024':'cycles/1024', 's*2-40':'s*2^-40',
    'cycles * 1024':'cycles*1024', 'dB*Hz':'dB*Hz', '0.25*dB*Hz':'0.25*dB*Hz',
    'radians':'rad', 's/period^2':'s/period^2', 'days':'day', 'km':'km', 'km/s':'km/s',
    'dBm':'dBm', 'cm/sec2':'cm/s^2', 'deg C':'degC',
}

def cstype(field):
    if field['type']=='a1' and 'count'in field:return 'string'
    t=TYPES.get(field['type'],('Javad'+field['type'],))[0]
    if 'count'in field:t+='[]'
    elif field.get('nullable')or'optional'in field:t+='?'
    return t

def literal(s):return json.dumps(s,ensure_ascii=False)
def numeric_count(count):
    count=count.replace('(','').replace(')','')
    return int(count)if count.isdecimal()else None

def record_size(name):
    rec=RECORDS[name]
    if rec.get('custom'):return None
    result=0
    for field in rec['fields']:
        size=TYPES[field['type']][1]if field['type']in TYPES else record_size(field['type'])
        if size is None:return None
        if 'count'in field:
            n=numeric_count(field['count'])
            if n is None:return None
            size*=n
        result+=size
    return result

def byte_size(field):return TYPES[field['type']][1]if field['type']in TYPES else record_size(field['type'])
def default(field):
    if 'count'in field:
        if field['type']=='a1':return literal(field.get('default',''))
        n=numeric_count(field['count'])
        if n is not None:
            t=cstype(field)[:-2]
            if field['type']not in TYPES:return '['+', '.join('new()'for _ in range(n))+']'
            return f'new {t}[{n}]'
        return '[]'
    if field.get('nullable')or'optional'in field:return None
    if field['type']not in TYPES:return 'new()'
    if 'default'in field:
        return "'"+field['default']+"'"if field['type']=='a1'else str(field['default'])
    return None

def counts(fields):
    result={}
    for array in fields:
        count=array.get('count')
        if not count or count=='remaining':continue
        var=count.replace('(','').replace(')','')
        for scalar in fields:
            if scalar.get('wireName')==var and scalar['type']in TYPES and 'count'not in scalar:
                result[scalar['name']]=array['name']
    return result

def expression(count, fields, reading):
    refs={x.get('wireName',x['name']):('wire'+x['name']if reading and x['name']in counts(fields)else x['name'])for x in fields}
    refs.update({x['name']:x['name']for x in fields})
    return re.sub(r'[a-zA-Z_]\w*',lambda m:refs.get(m[0],m[0]),count)

def read_expr(field):
    t=field['type'];order=''if TYPES[t][1]==1 else ', order'
    e=f'JavadBinary.Read{TYPES[t][2]}(ref buffer{order})'
    return '(char)'+e if t=='a1'else e

def write_stmt(field,value):
    t=field['type'];order=''if TYPES[t][1]==1 else ', order'
    if t=='a1':value=f'checked((byte){value})'
    return f'JavadBinary.Write{TYPES[t][2]}(ref buffer, {value}{order});'

def backing(field):return '_'+field['name'][0].lower()+field['name'][1:]

def visitor_primitive(field):
    kind=VISITOR_TYPES[field['type']]
    if field.get('nullable')or'optional'in field:kind+='Optional'
    return kind+'Type'

def visitor_type(field):
    typ=field['type'];count=field.get('count')
    if typ=='bool':return 'BoolType.Default'
    if typ=='a1' and count is not None:
        maximum=numeric_count(count)
        return f'new StringType(EncodingId.Ascii, 0, {maximum if maximum is not None else "JavadProtocol.MaxBodySize"})'
    if typ in TYPES:
        base=visitor_primitive(field)+('.Ascii'if typ=='a1'else'.Default')
    else:base='Javad'+typ+'.StructType'
    if count is not None:
        length=numeric_count(count)
        if length is not None:return f'new ArrayType({base}, {length})'
        size=byte_size(field)
        maximum=f'JavadProtocol.MaxBodySize / {size}'if size and size>1 else'JavadProtocol.MaxBodySize'
        return f'new ListType({base}, 0, {maximum})'
    if typ not in TYPES and (field.get('nullable')or'optional'in field):return f'new OptionalStructType({base})'
    return base

def visitor(item,message):
    computed=counts(item['fields'])if not item.get('custom')else{}
    fields=item['fields']+legacy_presence_fields(item)+item.get('visitorFields',[])
    lines=[]
    for field in fields:
        name=field['name'];readonly=name in computed or field.get('readOnly')
        desc=field.get('description')or f'GREIS {field.get("wireName",name)} field; specification page {item["page"]}.'
        if field.get('missing'):desc+=' The maximum integer value or quiet NaN denotes unavailable data; the raw wire value is preserved.'
        if readonly:desc+=' Read-only value derived from the payload; edit its source fields instead.'
        units=field.get('units')
        if units is None:
            units=next((UNITS[value.strip()]for value in re.findall(r'\[([^]]*)\]',desc)if value.strip()in UNITS),None)
        method='ReadOnlyField'if readonly else'Field'
        lines.append(f'    private static readonly Field {name}Field = JavadVisitor.{method}(nameof({name}), {visitor_type(field)}, {literal(desc)}'+(', '+literal(units)if units else'')+');')
    names=(['ExtensionDataField']if message else[])+[f['name']+'Field'for f in fields]
    lines+=['','    /// <summary>Cached field metadata for the GREIS payload.</summary>',f'    public static StructType StructType {{ get; }} = new([{", ".join(names)}]);','','    /// <inheritdoc />','    public '+('override 'if message else'')+'void Accept(IVisitor visitor)','    {']
    lines.append('        base.Accept(visitor);'if message else'        ArgumentNullException.ThrowIfNull(visitor);')
    for field in fields:
        name=field['name'];typ=field['type'];member=backing(field);metadata=name+'Field'
        if name in computed or field.get('readOnly'):
            value=f'{computed[name]} is null ? default : {name}'if name in computed else name
            lines +=[f'        var readOnly{name} = {value};',f'        var original{name} = readOnly{name};',f'        {visitor_primitive(field)}.Accept(visitor, {metadata}, ref readOnly{name});',f'        if (readOnly{name} != original{name})',f'            throw new InvalidOperationException("{name} is derived from other GREIS fields and cannot be edited directly.");']
        elif typ=='a1' and 'count'in field:
            lines.append(f'        StringType.Accept(visitor, {metadata}, ref {member});')
        elif 'count'in field:
            element=cstype(field)[:-2]
            callback=f'{visitor_primitive(field)}.Accept(v, f, t, ref value);'if typ in TYPES else'if (value is null) v.VisitUnknown(f, t); else Asv.IO.StructType.Accept(v, f, t, value);'
            create='default'if typ in TYPES else'new()'
            lines.extend([f'        JavadVisitor.VisitArray(visitor, {metadata}, ref {member}, static () => {create},',f'            static (IVisitor v, Field f, IFieldType t, ref {element} value) => {{ {callback} }});'])
        elif typ=='bool':
            lines.append(f'        BoolType.Accept(visitor, {metadata}, ref {member});')
        elif typ in TYPES:
            lines.append(f'        {visitor_primitive(field)}.Accept(visitor, {metadata}, ref {member});')
        elif field.get('nullable')or'optional'in field:
            lines.append(f'        OptionalStructType.Accept(visitor, {metadata}, ref {member});')
        else:
            lines.extend([f'        if ({member} is null)',f'            visitor.VisitUnknown({metadata}, {metadata}.DataType);','        else',f'            Asv.IO.StructType.Accept(visitor, {metadata}, {member});'])
    lines.append('    }')
    return lines

def legacy_presence_fields(item):
    return [{'name':'Has'+field['name'], 'type':'bool',
             'description':f'Whether {field["name"]} is included in the wire payload. True for new messages; false for historical frames that predate this trailing field. Set true before serializing a newly assigned value.'}
            for field in item['fields']if field.get('legacyOptionalTail')]

def properties(item):
    lines=[];computed=counts(item['fields'])if not item.get('custom')else{}
    for field in item['fields']:
        name=field['name'];desc=field.get('description','')
        if field.get('missing'):desc+=' The maximum integer value or quiet NaN denotes unavailable data; the wire value is preserved.'
        if name not in computed:
            d=default(field)
            lines.append(f'    private {cstype(field)} {backing(field)}'+(' = '+d if d is not None else'')+';')
        if desc:lines.append('    /// <summary>'+html.escape(desc,quote=False)+'</summary>')
        if name in computed:lines.append(f'    public {cstype(field)} {name} => checked(({cstype(field)}){computed[name]}.Length);')
        else:
            member=backing(field)
            lines.append(f'    public {cstype(field)} {name} {{ get => {member}; set => {member} = value; }}')
    for field in legacy_presence_fields(item):
        member=backing(field);name=field['name']
        lines +=[f'    private bool {member} = true;',
                 '    /// <summary>'+html.escape(field['description'],quote=False)+'</summary>',
                 f'    public bool {name} {{ get => {member}; set => {member} = value; }}']
    return lines

def codec(item,message):
    fields=item['fields'];computed=counts(fields);read=[];write=[];size=[]
    for i,field in enumerate(fields):
        name=field['name'];typ=field['type'];n=field.get('count')
        if field.get('legacyOptionalTail'):
            if i!=len(fields)-1 or typ not in TYPES or n is not None or not message:
                raise ValueError('A legacy optional tail must be the final primitive field of a message.')
            present='Has'+name
            read.extend([f'{present} = !buffer.IsEmpty;',f'{name} = {present} ? {read_expr(field)} : default;'])
            write.extend([f'if ({present})',f'    {write_stmt(field,name)}'])
            size.extend([f'if (!{present} && ExtensionData.Length != 0)',
                         f'    throw new InvalidOperationException("Extension data cannot follow an absent {name} field.");',
                         f'size += {present} ? {byte_size(field)} : 0;'])
            continue
        if name in computed:
            read.append(f'var wire{name} = {read_expr(field)};')
            write.append(write_stmt(field,name));size.append(f'_ = {name};')
            size.append(f'size += {byte_size(field)};');continue
        if 'optional'in field:
            read.extend([f'{name} = null;',f'if (buffer.Length >= {field["optional"]})','{',f'    {name} = new();',f'    {name}.Deserialize(ref buffer, order);','}'])
            write.append(f'{name}?.Serialize(ref buffer, order);')
            size.append(f'size += {name}?.GetByteSize() ?? 0;');continue
        if n is not None:
            if n=='remaining':
                tail=sum(byte_size(x)for x in fields[i+1:])
                divisor=byte_size(field)
                read.extend([f'var count{name} = JavadBinaryPayload.GetRemainingCount(buffer.Length, {tail}, {divisor});'])
                c='count'+name
            else:c=expression(n,fields,True)
            if byte_size(field) is not None:
                read.append(f'JavadBinaryPayload.RequireAvailable(buffer.Length, {c}, {byte_size(field)});')
            if typ=='a1':
                read.append(f'{name} = JavadBinary.ReadAscii(ref buffer, {c});')
                wc=f'{name}.Length'if n=='remaining'else expression(n,fields,False)
                write.append(f'JavadBinary.WriteAscii(ref buffer, {name}, {wc});')
                size.extend([f'JavadBinaryPayload.RequireAscii({name}, {wc}, nameof({name}));',f'size += {wc};'])
            else:
                read.extend([f'{name} = new {cstype(field)[:-2]}[{c}];',f'for (var index = 0; index < {name}.Length; index++)','{'])
                if typ in TYPES:read.append(f'    {name}[index] = {read_expr(field)};')
                else:read.extend([f'    {name}[index] = new();',f'    {name}[index].Deserialize(ref buffer, order);'])
                read.append('}')
                write.extend([f'foreach (var value in {name})','{',('    '+write_stmt(field,'value'))if typ in TYPES else'    value.Serialize(ref buffer, order);','}'])
                count_expr=f'{name}.Length'if n=='remaining'else expression(n,fields,False)
                size.append(f'JavadBinaryPayload.RequireLength({name}, {count_expr}, nameof({name}));')
                sz=byte_size(field)
                if sz is not None:size.append(f'size = checked(size + {name}.Length * {sz});')
                else:size.extend([f'foreach (var value in {name})',f'    size = checked(size + value.GetByteSize());'])
            continue
        if typ in TYPES:
            read.append(f'{name} = {read_expr(field)};');write.append(write_stmt(field,name));size.append(f'size += {byte_size(field)};')
            if typ=='a1':size.append(f'JavadBinaryPayload.RequireAscii({name}.ToString(), 1, nameof({name}));')
        else:
            read.extend([f'{name} = new();',f'{name}.Deserialize(ref buffer, order);'])
            write.append(f'{name}.Serialize(ref buffer, order);');size.append(f'size += {name}.GetByteSize();')
    if item.get('validation'):
        read.append(item['validation']+';')
        size.insert(0,item['validation']+';')
    required = [f'ArgumentNullException.ThrowIfNull({f["name"]});' for f in fields if ('count' in f or f['type'] not in TYPES) and not f.get('nullable') and 'optional' not in f]
    size = required + size
    lines=[]
    signatures=[('protected override void DeserializePayload(ref ReadOnlySpan<byte> buffer)'if message else'internal void Deserialize(ref ReadOnlySpan<byte> buffer, JavadByteOrder order)',read),('protected override void SerializePayload(ref Span<byte> buffer)'if message else'internal void Serialize(ref Span<byte> buffer, JavadByteOrder order)',write)]
    for signature,body in signatures:
        lines+=['',f'    {signature}','    {']
        if message:lines.append('        var order = ByteOrder;')
        lines += ['        '+line for line in body];lines.append('    }')
    lines+=['',('    protected override int GetPayloadByteSize()'if message else'    internal int GetByteSize()'),'    {','        var size = 0;']+['        '+x for x in size]+['        return size;','    }']
    return lines

def generate():
    lines=['// <auto-generated />','// Source: Schema/binary-messages.json; regenerate with Schema/generate_binary.py.','#nullable enable','using System;','using Asv.IO;','','namespace Asv.Gnss;']
    for item in SCHEMA['records']:
        lines +=['',f'/// <summary>GREIS {item["name"]} record, specification page {item["page"]}.</summary>',f'public sealed partial class Javad{item["name"]} : IVisitable','{']+properties(item)+['']+visitor(item,False)
        if not item.get('custom'):lines+=codec(item,False)
        lines.append('}')
    classes=[]
    for item in SCHEMA['messages']:
        ids=item['ids'];name=item['name']
        classname='Javad'+name+('Payload'if len(ids)>1 else'Message')
        abstract='abstract 'if len(ids)>1 else''
        lines+=['',f'/// <summary>GREIS {name}; {", ".join(ids)}. Specification page {item["page"]}.</summary>',f'public {abstract}partial class {classname} : JavadMessageBase','{']
        if len(ids)==1:lines +=[f'    public const string MessageId = {literal(ids[0])};','    public override string Id => MessageId;']
        lines +=[f'    public override string Name => {literal("GREIS "+name)};',f'    public override JavadChecksumKind ChecksumKind => JavadChecksumKind.{item["checksum"]};']+properties(item)+['']+visitor(item,True)
        if not item.get('custom'):lines+=codec(item,True)
        lines.append('}')
        if len(ids)==1:classes.append(classname)
        else:
            for id in ids:
                cl='Javad'+name+id+'Message'
                lines +=['',f'/// <summary>GREIS {id}; specification page {item["page"]}.</summary>',f'public sealed class {cl} : {classname}','{',f'    public const string MessageId = {literal(id)};','    public override string Id => MessageId;','}'];classes.append(cl)
    lines+=['','/// <summary>Registers every binary receiver message from GREIS 4.2.01, including obsolete SI.</summary>','public static class JavadBinaryMessageFactory','{','    public static void Register(IProtocolMessageFactoryBuilder<JavadMessageBase, string> builder)','    {','        ArgumentNullException.ThrowIfNull(builder);']+[f'        builder.Add<{cl}>();'for cl in classes]+['    }','}']
    return '\n'.join(lines)+'\n'

def main():
    parser=argparse.ArgumentParser();parser.add_argument('--check',action='store_true');args=parser.parse_args()
    output=ROOT/'Generated/JavadBinaryMessages.g.cs';text=generate()
    if args.check:
        if not output.exists()or output.read_text()!=text:raise SystemExit('Generated GREIS binary models are out of date.')
    else:output.write_text(text)
    print(f'{sum(len(m["ids"])for m in SCHEMA["messages"])} binary message IDs, {len(SCHEMA["records"])} shared records.')

if __name__=='__main__':main()
