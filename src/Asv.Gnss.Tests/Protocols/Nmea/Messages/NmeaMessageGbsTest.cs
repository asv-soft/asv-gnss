using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Tests.Messages;

[TestSubject(typeof(NmeaMessageGbs))]
public class NmeaMessageGbsTest
{
        [Theory]
        [InlineData("$GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972*4D\r\n")]
        [InlineData("$GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972*4D\r")]
        [InlineData("$GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972*4D")]
        [InlineData("$GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972")]
        [InlineData("GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972")]
        [InlineData("GPGBS,015509.00,-0.031,-0.186,0.219,019,0.000,-0.354,6.972")]
        public void Deserialize_ShouldParseCorrectly_WithCompleteData(string dataString)
        {
            ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
            var msg = new NmeaMessageGbs();
            msg.Deserialize(ref data);
            Assert.Equal(0, data.Length);
            Assert.Equal(new TimeSpan(0,01,55,09,00), msg.TimeUtc);
            Assert.Equal(-0.031, msg.LatitudeError);
            Assert.Equal(-0.186, msg.LongitudeError);
            Assert.Equal(0.219, msg.AltitudeError);
            Assert.Equal(19, msg.FailedSatelliteId);
            Assert.Equal(0.000, msg.ProbabilityOfMissedDetection);
            Assert.Equal(-0.354, msg.BiasEstimate);
            Assert.Equal(6.972, msg.BiasEstimateStandardDeviation);


        }
        
        [Fact]
        public void Serialize_ValidObject_ProducesCorrectNmeaString()
        {
            // Arrange
            var message = new NmeaMessageGbs
            {
                TalkerId = new NmeaTalkerId("GP"),
                TimeUtc = new TimeSpan(0, 01, 55, 09, 900),
                LatitudeError = -0.031,
                LongitudeError = -0.186,
                AltitudeError = 0.219,
                FailedSatelliteId = 19,
                ProbabilityOfMissedDetection = 0.000,
                BiasEstimate = -0.354,
                BiasEstimateStandardDeviation = 6.972
            };

            var buffer = new byte[message.GetByteSize()]; // Достаточно большой буфер
            var bufferSpan = buffer.AsSpan();

            // Act
            message.Serialize(ref bufferSpan);

            // Assert
            Assert.Equal(0,bufferSpan.Length);
            var result = NmeaProtocol.Encoding.GetString(buffer);
            Assert.Equal("$GPGBS,015509.900,-0.031,-0.186,0.219,19,0.000,-0.354,6.972*12\r\n", result);
        }
        
        

}