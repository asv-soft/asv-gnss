using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Asv.IO;
using System.Buffers.Binary;
using Xunit;

namespace Asv.Gnss;

public class AsterixVisitorTest
{
    private static readonly string[] ModelNames =
    [
        "AsterixCategoryVersion",
        "AsterixFieldI001Frn001Type010",
        "AsterixFieldI001Frn002Type020",
        "AsterixFieldI001Frn003Type040",
        "AsterixFieldI001Frn004Type070",
        "AsterixFieldI001Frn005Type090",
        "AsterixFieldI001Frn006Type130",
        "AsterixFieldI001Frn007Type141",
        "AsterixFieldI002Frn001Type010",
        "AsterixFieldI002Frn002Type000",
        "AsterixFieldI002Frn003Type020",
        "AsterixFieldI002Frn004Type030",
        "AsterixFieldI002Frn005Type041",
        "AsterixFieldI002Frn008Type070",
        "AsterixFieldI002Frn009Type100",
        "AsterixFieldI002Frn010Type090",
        "AsterixFieldI002Frn013TypeSp",
        "AsterixFieldI004Frn001Type010",
        "AsterixFieldI004Frn002Type000",
        "AsterixFieldI004Frn003Type015",
        "AsterixFieldI004Frn004Type020",
        "AsterixFieldI004Frn005Type040",
        "AsterixFieldI004Frn006Type045",
        "AsterixFieldI004Frn007Type060",
        "AsterixFieldI004Frn012Type076",
        "AsterixFieldI004Frn013Type074",
        "AsterixFieldI004Frn014Type075",
        "AsterixFieldI004Frn018Type110",
        "AsterixFieldI010Frn001Type010",
        "AsterixFieldI010Frn002Type000",
        "AsterixFieldI010Frn003Type020",
        "AsterixFieldI010Frn004Type140",
        "AsterixFieldI010Frn005Type041",
        "AsterixFieldI010Frn006Type040",
        "AsterixFieldI010Frn007Type042",
        "AsterixFieldI010Frn008Type200",
        "AsterixFieldI010Frn009Type202",
        "AsterixFieldI010Frn010Type161",
        "AsterixFieldI010Frn011Type170",
        "AsterixFieldI010Frn012Type060",
        "AsterixFieldI010Frn013Type220",
        "AsterixFieldI010Frn014Type245",
        "AsterixFieldI010Frn015Type250",
        "AsterixFieldI010Frn016Type300",
        "AsterixFieldI010Frn017Type090",
        "AsterixFieldI010Frn018Type091",
        "AsterixFieldI010Frn019Type270",
        "AsterixFieldI010Frn020Type550",
        "AsterixFieldI010Frn021Type310",
        "AsterixFieldI010Frn022Type500",
        "AsterixFieldI010Frn023Type280",
        "AsterixFieldI010Frn024Type131",
        "AsterixFieldI010Frn025Type210",
        "AsterixFieldI010Frn026TypeSp",
        "AsterixFieldI010Frn027TypeRe",
        "AsterixFieldI020Frn027TypeRe",
        "AsterixFieldI021Frn001Type010",
        "AsterixFieldI021Frn002Type040",
        "AsterixFieldI021Frn003Type161",
        "AsterixFieldI021Frn004Type015",
        "AsterixFieldI021Frn006Type130",
        "AsterixFieldI021Frn007Type131",
        "AsterixFieldI021Frn010Type150",
        "AsterixFieldI021Frn011Type151",
        "AsterixFieldI021Frn012Type080",
        "AsterixFieldI021Frn018Type140",
        "AsterixFieldI021Frn019Type090",
        "AsterixFieldI021Frn020Type210",
        "AsterixFieldI021Frn021Type070",
        "AsterixFieldI021Frn022Type230",
        "AsterixFieldI021Frn023Type145",
        "AsterixFieldI021Frn025Type152",
        "AsterixFieldI021Frn026Type200",
        "AsterixFieldI021Frn029Type160",
        "AsterixFieldI021Frn030Type165",
        "AsterixFieldI021Frn033Type170",
        "AsterixFieldI021Frn034Type020",
        "AsterixFieldI021Frn035Type220",
        "AsterixFieldI021Frn037Type148",
        "AsterixFieldI021Frn039Type016",
        "AsterixFieldI021Frn041Type008",
        "AsterixFieldI021Frn042Type271",
        "AsterixFieldI021Frn043Type132",
        "AsterixFieldI021Frn044Type250",
        "AsterixFieldI021Frn045Type260",
        "AsterixFieldI021Frn046Type400",
        "AsterixFieldI034Frn001Type010",
        "AsterixFieldI034Frn002Type000",
        "AsterixFieldI034Frn003Type030",
        "AsterixFieldI034Frn004Type020",
        "AsterixFieldI034Frn005Type041",
        "AsterixFieldI034Frn006Type050",
        "AsterixFieldI034Frn007Type060",
        "AsterixFieldI034Frn008Type070",
        "AsterixFieldI034Frn009Type100",
        "AsterixFieldI034Frn010Type110",
        "AsterixFieldI034Frn012Type090",
        "AsterixFieldI034Frn013TypeRe",
        "AsterixFieldI034Frn014TypeSp",
        "AsterixFieldI048Frn001Type010",
        "AsterixFieldI048Frn002Type140",
        "AsterixFieldI048Frn003Type020",
        "AsterixFieldI048Frn004Type040",
        "AsterixFieldI048Frn005Type070",
        "AsterixFieldI048Frn006Type090",
        "AsterixFieldI048Frn007Type130",
        "AsterixFieldI048Frn008Type220",
        "AsterixFieldI048Frn009Type240",
        "AsterixFieldI048Frn010Type250",
        "AsterixFieldI048Frn011Type161",
        "AsterixFieldI048Frn013Type200",
        "AsterixFieldI048Frn014Type170",
        "AsterixFieldI048Frn016Type030",
        "AsterixFieldI048Frn020Type120",
        "AsterixFieldI048Frn021Type230",
        "AsterixFieldI048Frn027TypeSp",
        "AsterixFieldI048Frn028TypeRe",
        "AsterixFieldI247Frn001Type010",
        "AsterixFieldI247Frn002Type015",
        "AsterixFieldI247Frn003Type140",
        "AsterixFieldI247Frn004Type550",
        "AsterixFieldI247Frn006TypeSp",
        "AsterixFieldI247Frn007TypeRe",
        "AsterixI002PlotCountValue",
        "AsterixI004DataSourceIdentifier",
        "AsterixI004FdpsSectorControl",
        "AsterixI010ModeSMbData",
        "AsterixI010PresenceItem",
        "AsterixI021ModeSData",
        "AsterixI034MessageCountValue",
        "AsterixI034StatusSubfieldSelector",
        "AsterixI034SystemConfigurationCommon",
        "AsterixI034SystemConfigurationModeS",
        "AsterixI034SystemConfigurationPsr",
        "AsterixI034SystemProcessingCommon",
        "AsterixI034SystemProcessingModeS",
        "AsterixI034SystemProcessingPsr",
        "AsterixI048ModeSMbData",
        "AsterixRecordI001",
        "AsterixRecordI002",
        "AsterixRecordI004",
        "AsterixRecordI010",
        "AsterixRecordI020",
        "AsterixRecordI021",
        "AsterixRecordI034",
        "AsterixRecordI048",
        "AsterixRecordI247",
        "RadarPlotCharacteristic",
    ];
    public static IEnumerable<object[]> Models() => ModelNames.Select(name => new object[] { typeof(AsterixMessage).Assembly.GetType("Asv.Gnss." + name, true)! });

    [Theory]
    [MemberData(nameof(Models))]
    public void Accept_PayloadModels_ExposeTypedValuesAndPreserveStorage(Type type)
    {
        var model = Activator.CreateInstance(type)!;
        // Record optionals remain absent here; integration fixtures exercise present data items.
        if (!typeof(AsterixRecord).IsAssignableFrom(type)) Populate(model);
        var expected = ExpectedValues(model).Order().ToArray();
        var before = Snapshot(model);
        var visitor = Assert.IsAssignableFrom<IVisitable>(model);
        var first = new Reader();
        var second = new Reader();
        visitor.Accept(first);
        visitor.Accept(second);
        Assert.Equal(PayloadMembers(type).Select(m => m.Name).Order(), first.TopFields.Select(f => f.Name).Order());
        Assert.Equal(expected, first.Values.Select(ValueKey).Order());
        Assert.Equal(0, first.Depth);
        Assert.Equal(first.Fields.Count, second.Fields.Count);
        for (var i = 0; i < first.Fields.Count; i++) Assert.Same(first.Fields[i], second.Fields[i]);
        var after = Snapshot(model);
        Assert.Equal(before.Values, after.Values);
        foreach (var (key, reference) in before.References) Assert.Same(reference, after.References[key]);
        Assert.Throws<ArgumentNullException>(() => visitor.Accept(null!));
    }

    [Fact]
    public void Accept_RecordGraph_VisitsPresentFieldsAndSupportsTypedEdits()
    {
        var field = new AsterixFieldI001Frn001Type010 { Sac = (SystemAreaCode)7, Sic = 42 };
        var record = new AsterixRecordI001 { DataSourceIdentifier = field };
        var message = new AsterixMessageI001();
        message.Add(record);
        var reader = new Reader();
        message.Accept(reader);
        Assert.Contains(("Sac", (object)(byte)7), reader.Values);
        Assert.Contains(("Sic", (object)(byte)42), reader.Values);
        message.Accept(new Editor());
        Assert.Equal((byte)43, field.Sic);
        Assert.Same(record, message.Single());
        Assert.Same(field, record.DataSourceIdentifier);
    }

    [Fact]
    public void Accept_ImmutableRecordsAndFixedBuffers_PreserveValuesAndRejectResize()
    {
        var immutable = new AsterixI010PresenceItem(-12, 23);
        var reader = new Reader();
        immutable.Accept(reader);
        Assert.Contains(("Drho", (object)(sbyte)-12), reader.Values);
        Assert.Contains(("Dtheta", (object)(sbyte)23), reader.Values);
        var fixedData = new AsterixI021ModeSData();
        var storage = fixedData.MbData;
        Assert.Throws<ArgumentOutOfRangeException>(() => fixedData.Accept(new ResizeEditor()));
        Assert.Same(storage, fixedData.MbData);
        Assert.Equal(7, storage.Length);
        var recordData = new AsterixI010ModeSMbData([1, 2, 3, 4, 5, 6, 7, 8]);
        Assert.Throws<ArgumentOutOfRangeException>(() => recordData.Accept(new ResizeEditor()));
        Assert.Equal(8, recordData.Data.Length);
    }

    [Fact]
    public void Accept_RecordedAsterixStream_PreservesBytesAndRecordIdentity()
    {
        var bytes = File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Resources", "Asterix", "2025-07-09-07.stream"));
        var position = 64;
        var count = 0;
        while (position < bytes.Length)
        {
            var size = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(position));
            position += 8;
            AsterixMessage? message = bytes[position] switch { 21 => new AsterixMessageI021(), 247 => new AsterixMessageI247(), _ => null };
            if (message != null)
            {
                ReadOnlySpan<byte> input = bytes.AsSpan(position, size);
                message.Deserialize(ref input);
                var original = Serialize(message);
                var snapshot = message is AsterixMessageI021 cat21 ? cat21.Cast<object>().ToArray() : ((AsterixMessageI247)message).Cast<object>().ToArray();
                message.Accept(new Reader());
                Assert.Equal(original, Serialize(message));
                var after = message is AsterixMessageI021 cat21After ? cat21After.Cast<object>().ToArray() : ((AsterixMessageI247)message).Cast<object>().ToArray();
                Assert.Equal(snapshot.Length, after.Length);
                for (var i = 0; i < snapshot.Length; i++) Assert.Same(snapshot[i], after[i]);
                count++;
            }
            position += size;
        }
        Assert.Equal(22941, count);
    }

    private static byte[] Serialize(AsterixMessage message)
    {
        var bytes = new byte[message.GetByteSize()];
        Span<byte> span = bytes;
        message.Serialize(ref span);
        Assert.True(span.IsEmpty);
        return bytes;
    }

    private static IEnumerable<MemberInfo> PayloadMembers(Type type)
    {
        var isField = typeof(AsterixField).IsAssignableFrom(type);
        var isRecord = typeof(AsterixRecord).IsAssignableFrom(type);
        return type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Where(member => member is PropertyInfo { GetMethod.IsPublic: true } p && p.GetIndexParameters().Length == 0
                || member is FieldInfo { IsStatic: false })
            .Where(member => !isField || member.Name is not ("Name" or "Category" or "FieldReferenceNumber"))
            .Where(member => !isRecord || member.Name != "Category");
    }

    private static Type MemberType(MemberInfo member) => member is PropertyInfo property ? property.PropertyType : ((FieldInfo)member).FieldType;
    private static object? Get(MemberInfo member, object value) => member is PropertyInfo property ? property.GetValue(value) : ((FieldInfo)member).GetValue(value);
    private static void Set(MemberInfo member, object value, object? item)
    {
        if (member is PropertyInfo property) property.SetValue(value, item);
        else ((FieldInfo)member).SetValue(value, item);
    }

    private static object? Sample(Type type, int seed)
    {
        if (Nullable.GetUnderlyingType(type) is { } underlying) return Sample(underlying, seed);
        if (type == typeof(string)) return "sample-" + seed;
        if (type == typeof(DateTime)) return new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc).AddSeconds(seed);
        if (type == typeof(bool)) return seed % 2 == 0;
        if (type.IsEnum) return Enum.ToObject(type, 1);
        if (type.IsPrimitive) return Convert.ChangeType(seed, type, CultureInfo.InvariantCulture);
        if (type.IsArray)
        {
            var items = Array.CreateInstance(type.GetElementType()!, 2);
            for (var index = 0; index < 2; index++) items.SetValue(Sample(type.GetElementType()!, seed + index), index);
            return items;
        }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var list = (System.Collections.IList)Activator.CreateInstance(type)!;
            for (var i = 0; i < 2; i++) list.Add(Sample(type.GetGenericArguments()[0], seed + i));
            return list;
        }
        var result = Activator.CreateInstance(type, true)!;
        Populate(result);
        return result;
    }

    private static void Populate(object value)
    {
        foreach (var member in PayloadMembers(value.GetType()))
        {
            var type = MemberType(member);
            if (member is FieldInfo || member is PropertyInfo { SetMethod: not null }) Set(member, value, Sample(type, 1 + member.Name.Sum(character => (int)character) % 90));
            else if (Get(member, value) is Array fixedArray)
            {
                for (var i = 0; i < fixedArray.Length; i++) fixedArray.SetValue(Sample(type.GetElementType()!, i + 17), i);
            }
            else if (Get(member, value) is System.Collections.IList list)
            {
                var element = type.GetGenericArguments()[0];
                list.Add(Sample(element, 12));
                list.Add(Sample(element, 23));
            }
            else if (!type.IsValueType && type != typeof(string) && Get(member, value) is { } nested) Populate(nested);
        }
    }

    private static string ValueKey((string Name, object? Value) value)
    {
        var text = value.Value is IFormattable formatted
            ? formatted.ToString(null, CultureInfo.InvariantCulture) : value.Value?.ToString() ?? "<null>";
        return $"{value.Name}:{value.Value?.GetType().Name}:{text}";
    }

    private static IEnumerable<string> ExpectedValues(object model)
    {
        IEnumerable<string> Read(object? value, Type type, string name)
        {
            if (value == null)
            {
                if (type == typeof(string) || Nullable.GetUnderlyingType(type) != null)
                    yield return ValueKey((name, null));
                yield break;
            }
            if (type.IsEnum)
            {
                yield return ValueKey((name, Convert.ChangeType(value, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture)));
                yield break;
            }
            if (value is string or DateTime || type.IsPrimitive || Nullable.GetUnderlyingType(type) != null)
            {
                yield return ValueKey((name, value));
                yield break;
            }
            if (value is System.Collections.IList array)
            {
                var elementType = type.IsArray ? type.GetElementType()! : type.GetGenericArguments()[0];
                foreach (var element in array)
                    foreach (var item in Read(element, elementType, name))
                        yield return item;
                yield break;
            }
            foreach (var member in PayloadMembers(type))
                foreach (var item in Read(Get(member, value), MemberType(member), member.Name))
                    yield return item;
        }
        return Read(model, model.GetType(), string.Empty);
    }

    private static (Dictionary<string, object?> Values, Dictionary<string, object> References) Snapshot(object value)
    {
        var values = new Dictionary<string, object?>();
        var references = new Dictionary<string, object>();
        void Read(object? item, string path)
        {
            if (item is null || item is string || item.GetType().IsPrimitive || item.GetType().IsEnum || item is DateTime)
            {
                values[path] = item;
                return;
            }
            if (!item.GetType().IsValueType) references[path] = item;
            if (item is System.Collections.IList array)
            {
                values[path + ".Length"] = array.Count;
                for (var index = 0; index < array.Count; index++) Read(array[index], path + $"[{index}]");
                return;
            }
            foreach (var member in PayloadMembers(item.GetType())) Read(Get(member, item), path + "." + member.Name);
        }
        Read(value, "$root");
        return (values, references);
    }

    private class Reader : DoubleType.IVisitor, BoolType.IVisitor, UInt8Type.IVisitor, UInt16Type.IVisitor,
        UInt32Type.IVisitor, Int32Type.IVisitor, UInt32OptionalType.IVisitor, StringOptionalType.IVisitor,
        DateTimeType.IVisitor, ListType.IVisitor, StructType.IVisitor, OptionalStructType.IVisitor,
        Int8Type.IVisitor, Int8OptionalType.IVisitor, UInt8OptionalType.IVisitor, Int16OptionalType.IVisitor, DoubleOptionalType.IVisitor
    {
        public int Depth { get; private set; }
        public List<Field> Fields { get; } = [];
        public List<Field> TopFields { get; } = [];
        public List<(string Name, object? Value)> Values { get; } = [];
        private void Seen(Field field)
        {
            Fields.Add(field);
            if (Depth == 0) TopFields.Add(field);
        }
        private void Scalar(Field field, object? value) { Seen(field); Values.Add((field.Name, value)); }
        public virtual void Visit(Field field, DoubleType type, ref double value) => Scalar(field, value);
        public virtual void Visit(Field field, BoolType type, ref bool value) => Scalar(field, value);
        public virtual void Visit(Field field, UInt8Type type, ref byte value) => Scalar(field, value);
        public virtual void Visit(Field field, UInt16Type type, ref ushort value) => Scalar(field, value);
        public virtual void Visit(Field field, UInt32Type type, ref uint value) => Scalar(field, value);
        public virtual void Visit(Field field, Int32Type type, ref int value) => Scalar(field, value);
        public virtual void Visit(Field field, UInt32OptionalType type, ref uint? value) => Scalar(field, value);
        public virtual void Visit(Field field, StringOptionalType type, ref string? value) => Scalar(field, value);
        public virtual void Visit(Field field, DateTimeType type, ref DateTime value) => Scalar(field, value);
        public virtual void Visit(Field field, Int8Type type, ref sbyte value) => Scalar(field, value);
        public virtual void Visit(Field field, Int8OptionalType type, ref sbyte? value) => Scalar(field, value);
        public virtual void Visit(Field field, UInt8OptionalType type, ref byte? value) => Scalar(field, value);
        public virtual void Visit(Field field, Int16OptionalType type, ref short? value) => Scalar(field, value);
        public virtual void Visit(Field field, DoubleOptionalType type, ref double? value) => Scalar(field, value);
        public void BeginOptionalStruct(Field field, OptionalStructType type, bool isPresent, out bool createNew) { Seen(field); Depth++; createNew = false; }
        public void EndOptionalStruct(bool isPresent) => Depth--;
        public virtual void BeginList(Field field, ListType type, ref uint size) { Seen(field); Depth++; }
        public void EndList() => Depth--;
        public void BeginStruct(Field field, StructType type) { Seen(field); Depth++; }
        public void EndStruct() => Depth--;
        public void VisitUnknown(Field field, IFieldType type) => throw new InvalidOperationException($"Unsupported {field.Name}: {type.Name}");
    }


    private sealed class ResizeEditor : Reader
    {
        public override void BeginList(Field field, ListType type, ref uint size) => size++;
    }

    private sealed class Editor : Reader
    {
        public override void Visit(Field field, UInt8Type type, ref byte value) { if (field.Name == "Sic") value++; }
    }
}
