using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Test;

[TestSubject(typeof(NmeaIntFormat))]
public class NmeaIntFormatTest
{


        [Fact]
        public void GetByteSize_NullValue_ReturnsZero()
        {
            // Arrange
            var format = new NmeaIntFormat("000", 3);

            // Act
            int size = format.GetByteSize(null);

            // Assert
            Assert.Equal(0, size);
        }

        [Fact]
        public void GetByteSize_NonNullValue_ReturnsMaxOfMinSizeAndDigitCount()
        {
            // Arrange
            var format = new NmeaIntFormat("000", 3);

            // Act
            int size = format.GetByteSize(42); 

            // Assert
            Assert.Equal(3, size); 
        }
        
        [Fact]
        public void GetByteSize_NegativeValue_ReturnsValidDigitCount()
        {
            // Arrange
            var format = new NmeaIntFormat("0", 1);

            // Act
            int size = format.GetByteSize(-42); 

            // Assert
            Assert.Equal(3, size); 
        }

        [Fact]
        public void GetByteSize_ZeroValue_ReturnsMinSize()
        {
            // Arrange
            var format = new NmeaIntFormat("0", 1);

            // Act
            int size = format.GetByteSize(0);

            // Assert
            Assert.Equal(1, size); // 0 имеет 1 цифру, но минимум — 1
        }

        [Fact]
        public void StaticInstances_HaveCorrectFormatsAndSizes()
        {
            // Arrange & Act & Assert
            Assert.Equal("0", NmeaIntFormat.IntD1.Format);
            Assert.Equal(1, NmeaIntFormat.IntD1.GetByteSize(5)); 

            Assert.Equal("00", NmeaIntFormat.IntD2.Format);
            Assert.Equal(2, NmeaIntFormat.IntD2.GetByteSize(42));

            Assert.Equal("000", NmeaIntFormat.IntD3.Format);
            Assert.Equal(3, NmeaIntFormat.IntD3.GetByteSize(123));

            Assert.Equal("0000", NmeaIntFormat.IntD4.Format);
            Assert.Equal(4, NmeaIntFormat.IntD4.GetByteSize(1234));

            Assert.Equal("00000", NmeaIntFormat.IntD5.Format);
            Assert.Equal(5, NmeaIntFormat.IntD5.GetByteSize(12345));
        }

        [Fact]
        public void GetByteSize_AggressiveInlining_WorksAsExpected()
        {
            // Arrange
            var format = new NmeaIntFormat("000", 3);

            // Act
            int size1 = format.GetByteSize(42);
            int size2 = format.GetByteSize(123);

            // Assert
            Assert.Equal(3, size1); // max(3, 2) = 3
            Assert.Equal(3, size2); // max(3, 3) = 3
        }

}