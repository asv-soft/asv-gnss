using Asv.IO;

namespace Asv.Gnss;

public partial class GpsSubframe4
{
    public partial class Ionospheric : IVisitable
    {
        private static readonly Field Alpha0Field = GpsNavigationVisitor.Field(
            nameof(Alpha0),
            DoubleType.Default
        );
        private static readonly Field Alpha1Field = GpsNavigationVisitor.Field(
            nameof(Alpha1),
            DoubleType.Default
        );
        private static readonly Field Alpha2Field = GpsNavigationVisitor.Field(
            nameof(Alpha2),
            DoubleType.Default
        );
        private static readonly Field Alpha3Field = GpsNavigationVisitor.Field(
            nameof(Alpha3),
            DoubleType.Default
        );
        private static readonly Field Betta0Field = GpsNavigationVisitor.Field(
            nameof(Betta0),
            DoubleType.Default
        );
        private static readonly Field Betta1Field = GpsNavigationVisitor.Field(
            nameof(Betta1),
            DoubleType.Default
        );
        private static readonly Field Betta2Field = GpsNavigationVisitor.Field(
            nameof(Betta2),
            DoubleType.Default
        );
        private static readonly Field Betta3Field = GpsNavigationVisitor.Field(
            nameof(Betta3),
            DoubleType.Default
        );
        internal static StructType Fields { get; } =
            new([
                Alpha0Field,
                Alpha1Field,
                Alpha2Field,
                Alpha3Field,
                Betta0Field,
                Betta1Field,
                Betta2Field,
                Betta3Field,
            ]);

        public void Accept(IVisitor visitor)
        {
            DoubleType.Accept(visitor, Alpha0Field, Alpha0Field.DataType, ref _alpha0);
            DoubleType.Accept(visitor, Alpha1Field, Alpha1Field.DataType, ref _alpha1);
            DoubleType.Accept(visitor, Alpha2Field, Alpha2Field.DataType, ref _alpha2);
            DoubleType.Accept(visitor, Alpha3Field, Alpha3Field.DataType, ref _alpha3);
            DoubleType.Accept(visitor, Betta0Field, Betta0Field.DataType, ref _betta0);
            DoubleType.Accept(visitor, Betta1Field, Betta1Field.DataType, ref _betta1);
            DoubleType.Accept(visitor, Betta2Field, Betta2Field.DataType, ref _betta2);
            DoubleType.Accept(visitor, Betta3Field, Betta3Field.DataType, ref _betta3);
        }
    }

    public partial class UtcGpsRelationship : IVisitable
    {
        private static readonly Field A0Field = GpsNavigationVisitor.Field(
            nameof(A0),
            DoubleType.Default
        );
        private static readonly Field A1Field = GpsNavigationVisitor.Field(
            nameof(A1),
            DoubleType.Default
        );
        private static readonly Field TotField = GpsNavigationVisitor.Field(
            nameof(Tot),
            DoubleType.Default
        );
        private static readonly Field WNtField = GpsNavigationVisitor.Field(
            nameof(WNt),
            Int32Type.Default
        );
        private static readonly Field DeltaT_LSField = GpsNavigationVisitor.Field(
            nameof(DeltaT_LS),
            DoubleType.Default
        );
        private static readonly Field WN_LSFField = GpsNavigationVisitor.Field(nameof(WN_LSF), Int32Type.Default);
        private static readonly Field DNField = GpsNavigationVisitor.Field(nameof(DN), Int32Type.Default);
        private static readonly Field DeltaT_LSFField = GpsNavigationVisitor.Field(nameof(DeltaT_LSF), Int32Type.Default);
        internal static StructType Fields { get; } =
            new([A0Field, A1Field, TotField, WNtField, DeltaT_LSField, WN_LSFField, DNField, DeltaT_LSFField]);

        public void Accept(IVisitor visitor)
        {
            DoubleType.Accept(visitor, A0Field, A0Field.DataType, ref _a0);
            DoubleType.Accept(visitor, A1Field, A1Field.DataType, ref _a1);
            DoubleType.Accept(visitor, TotField, TotField.DataType, ref _tot);
            Int32Type.Accept(visitor, WNtField, WNtField.DataType, ref _wNt);
            DoubleType.Accept(visitor, DeltaT_LSField, DeltaT_LSField.DataType, ref _deltaT_LS);
            Int32Type.Accept(visitor, WN_LSFField, WN_LSFField.DataType, ref _wN_LSF);
            Int32Type.Accept(visitor, DNField, DNField.DataType, ref _dN);
            Int32Type.Accept(visitor, DeltaT_LSFField, DeltaT_LSFField.DataType, ref _deltaT_LSF);
        }
    }
}
