using System.Globalization;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Tests;

[TestSubject(typeof(NmeaDoubleFormat))]
public class NmeaDoubleFormatTest
{

        [Fact]
        public void Constructor_ValidInput_SetsFormatCorrectly()
        {
            // Arrange & Act
            var format = new NmeaDoubleFormat("0.0", 1, 1);

            // Assert
            Assert.Equal("0.0", format.Format);
        }

        [Fact]
        public void GetByteSize_NonFiniteValue_ReturnsZero()
        {
            // Arrange
            var format = new NmeaDoubleFormat("0.0", 1, 1);

            // Act
            int size1 = format.GetByteSize(double.NaN);
            int size2 = format.GetByteSize(double.PositiveInfinity);
            int size3 = format.GetByteSize(double.NegativeInfinity);

            // Assert
            Assert.Equal(0, size1);
            Assert.Equal(0, size2);
            Assert.Equal(0, size3);
        }

        [Fact]
        public void GetByteSize_FiniteValue_ReturnsMaxOfMinSizeAndActualDigits()
        {
            // Arrange
            var format = new NmeaDoubleFormat("0.00", 1, 2);

            // Act
            int size = format.GetByteSize(42.5); 

            // Assert
         
            Assert.Equal(5, size);
        }

        [Fact]
        public void GetByteSize_ZeroValue_ReturnsMinSize()
        {
            // Arrange
            var format = new NmeaDoubleFormat("0.0", 1, 1);

            // Act
            int size = format.GetByteSize(0.0);

            // Assert
            Assert.Equal(3, size);
        }

        [Fact]
        public void StaticInstances_HaveCorrectFormatsAndSizes()
        {
            // Arrange & Act & Assert
            Assert.Equal("0.0", NmeaDoubleFormat.Double1X1.Format);
            Assert.Equal(3, NmeaDoubleFormat.Double1X1.GetByteSize(5.5)); 

            Assert.Equal("0.00", NmeaDoubleFormat.Double1X2.Format);
            Assert.Equal(5, NmeaDoubleFormat.Double1X2.GetByteSize(42.5));

            Assert.Equal("000.000", NmeaDoubleFormat.Double3X3.Format);
            Assert.Equal(7, NmeaDoubleFormat.Double3X3.GetByteSize(123.456));

            Assert.Equal("000000.000000", NmeaDoubleFormat.Double6X6.Format);
            Assert.Equal(13, NmeaDoubleFormat.Double6X6.GetByteSize(123456.789012));
            
            
            Assert.Equal(4, NmeaDoubleFormat.Double1X1.GetByteSize(9.99));
            Assert.Equal("10.0",  9.99.ToString(NmeaDoubleFormat.Double1X1.Format, NumberFormatInfo.InvariantInfo));
            
            Assert.Equal(4, NmeaDoubleFormat.Double1X1.GetByteSize(-0.1));
            Assert.Equal("-0.1",  (-0.1).ToString(NmeaDoubleFormat.Double1X1.Format, NumberFormatInfo.InvariantInfo));
            
            Assert.Equal(5, NmeaDoubleFormat.Double1X1.GetByteSize(-9.99));
            Assert.Equal("-10.0",  (-9.99).ToString(NmeaDoubleFormat.Double1X1.Format, NumberFormatInfo.InvariantInfo));
        }

        [Fact]
        public void GetByteSize_AggressiveInlining_WorksAsExpected()
        {
            // Arrange
            var format = new NmeaDoubleFormat("0.00", 1, 2);
            // Act
            var size1 = format.GetByteSize(42.5);
            var size2 = format.GetByteSize(123.45);
            var size3 = format.GetByteSize(-123);

            // Assert
            Assert.Equal(5, size1); 
            Assert.Equal(6, size2); 
            Assert.Equal(7, size3);
            
        }
}