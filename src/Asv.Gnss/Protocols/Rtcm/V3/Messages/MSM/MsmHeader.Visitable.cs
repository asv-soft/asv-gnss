using System;
using System.Collections.Generic;
using Asv.IO;

namespace Asv.Gnss;

public partial struct MsmHeader : IVisitable
{
    private static readonly Field Rtcmtime_sField = RtcmV3Visitor.Field(nameof(time_s), Int32Type.Default, "time_s");
    private static readonly Field Rtcmclk_strField = RtcmV3Visitor.Field(nameof(clk_str), Int32Type.Default, "clk_str");
    private static readonly Field Rtcmclk_extField = RtcmV3Visitor.Field(nameof(clk_ext), Int32Type.Default, "clk_ext");
    private static readonly Field RtcmsmoothField = RtcmV3Visitor.Field(nameof(smooth), Int32Type.Default, "smooth");
    private static readonly Field Rtcmtint_sField = RtcmV3Visitor.Field(nameof(tint_s), Int32Type.Default, "tint_s");
    private static readonly Field RtcmsatsField = RtcmV3Visitor.Field(nameof(sats), new ListType(UInt8Type.Default, 0, RtcmV3Visitor.MaximumElements), "sats");
    private static readonly Field RtcmnsatField = RtcmV3Visitor.Field(nameof(nsat), Int32Type.Default, "nsat");
    private static readonly Field RtcmsigsField = RtcmV3Visitor.Field(nameof(sigs), new ListType(UInt8Type.Default, 0, RtcmV3Visitor.MaximumElements), "sigs");
    private static readonly Field RtcmnsigField = RtcmV3Visitor.Field(nameof(nsig), Int32Type.Default, "nsig");
    private static readonly Field RtcmcellmaskField = RtcmV3Visitor.Field(nameof(cellmask), new ListType(UInt8Type.Default, 0, RtcmV3Visitor.MaximumElements), "cellmask");

    internal static readonly StructType RtcmVisitorType = new([Rtcmtime_sField, Rtcmclk_strField, Rtcmclk_extField, RtcmsmoothField, Rtcmtint_sField, RtcmsatsField, RtcmnsatField, RtcmsigsField, RtcmnsigField, RtcmcellmaskField]);

    /// <inheritdoc />
    public void Accept(IVisitor visitor)
    {
        ArgumentNullException.ThrowIfNull(visitor);

        var valuetime_s = time_s;
        Int32Type.Accept(visitor, Rtcmtime_sField, Rtcmtime_sField.DataType, ref valuetime_s);
        if (!EqualityComparer<int>.Default.Equals(valuetime_s, time_s))
        {
            time_s = valuetime_s;
        }

        var valueclk_str = clk_str;
        Int32Type.Accept(visitor, Rtcmclk_strField, Rtcmclk_strField.DataType, ref valueclk_str);
        if (!EqualityComparer<int>.Default.Equals(valueclk_str, clk_str))
        {
            clk_str = valueclk_str;
        }

        var valueclk_ext = clk_ext;
        Int32Type.Accept(visitor, Rtcmclk_extField, Rtcmclk_extField.DataType, ref valueclk_ext);
        if (!EqualityComparer<int>.Default.Equals(valueclk_ext, clk_ext))
        {
            clk_ext = valueclk_ext;
        }

        var valuesmooth = smooth;
        Int32Type.Accept(visitor, RtcmsmoothField, RtcmsmoothField.DataType, ref valuesmooth);
        if (!EqualityComparer<int>.Default.Equals(valuesmooth, smooth))
        {
            smooth = valuesmooth;
        }

        var valuetint_s = tint_s;
        Int32Type.Accept(visitor, Rtcmtint_sField, Rtcmtint_sField.DataType, ref valuetint_s);
        if (!EqualityComparer<int>.Default.Equals(valuetint_s, tint_s))
        {
            tint_s = valuetint_s;
        }

        var valuesats = sats;
        RtcmV3Visitor.VisitArray(visitor, RtcmsatsField, RtcmsatsField.DataType, ref valuesats,
            static () => default(byte),
            static (IVisitor v, Field f, IFieldType t, ref byte item) =>
            {
                UInt8Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valuesats, sats))
        {
            sats = valuesats!;
        }

        var valuensat = nsat;
        Int32Type.Accept(visitor, RtcmnsatField, RtcmnsatField.DataType, ref valuensat);
        if (!EqualityComparer<int>.Default.Equals(valuensat, nsat))
        {
            nsat = valuensat;
        }

        var valuesigs = sigs;
        RtcmV3Visitor.VisitArray(visitor, RtcmsigsField, RtcmsigsField.DataType, ref valuesigs,
            static () => default(byte),
            static (IVisitor v, Field f, IFieldType t, ref byte item) =>
            {
                UInt8Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valuesigs, sigs))
        {
            sigs = valuesigs!;
        }

        var valuensig = nsig;
        Int32Type.Accept(visitor, RtcmnsigField, RtcmnsigField.DataType, ref valuensig);
        if (!EqualityComparer<int>.Default.Equals(valuensig, nsig))
        {
            nsig = valuensig;
        }

        var valuecellmask = cellmask;
        RtcmV3Visitor.VisitArray(visitor, RtcmcellmaskField, RtcmcellmaskField.DataType, ref valuecellmask,
            static () => default(byte),
            static (IVisitor v, Field f, IFieldType t, ref byte item) =>
            {
                UInt8Type.Accept(v, f, t, ref item);
            });
        if (!ReferenceEquals(valuecellmask, cellmask))
        {
            cellmask = valuecellmask!;
        }
    }
}
