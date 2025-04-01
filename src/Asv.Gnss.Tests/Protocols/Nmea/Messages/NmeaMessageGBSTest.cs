using System;
using Asv.Gnss;
using JetBrains.Annotations;
using Xunit;

namespace Asv.Gnss.Tests.Messages;

[TestSubject(typeof(NmeaMessageGbs))]
public class NmeaMessageGBSTest
{
        [Theory]
        [InlineData("$GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972*4D\r\n")]
        [InlineData("$GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972*4D\r")]
        [InlineData("$GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972*4D")]
        [InlineData("$GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972")]
        [InlineData("GPGBS,015509.00,-0.031,-0.186,0.219,19,0.000,-0.354,6.972")]
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
                TimeUtc = new TimeSpan(0, 0, 15, 50, 900),
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
            string result = NmeaProtocol.Encoding.GetString(buffer, 0, bufferSpan.Length - bufferSpan.Length);
            Assert.StartsWith("$", result); // Начинается с $
            Assert.Contains("GPGBS", result); // Содержит TalkerId и MessageId
            Assert.Contains("015509.00", result); // Время
            Assert.Contains("-0.031", result); // LatitudeError
            Assert.Contains("-0.186", result); // LongitudeError
            Assert.Contains("0.219", result); // AltitudeError
            Assert.Contains("19", result); // FailedSatelliteId
            Assert.Contains("0.000", result); // ProbabilityOfMissedDetection
            Assert.Contains("-0.354", result); // BiasEstimate
            Assert.Contains("6.972", result); // BiasEstimateStandardDeviation
            Assert.EndsWith("*4D\r\n", result); // CRC и конец сообщения (примерно, CRC может варьироваться)
        }
        
        

}