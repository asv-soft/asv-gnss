using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;

namespace Asv.Gnss.Test;

public abstract class NmeaMessageTestBase<TMessage>(ITestOutputHelper output, Dictionary<string, string> testMessages)
    where TMessage : NmeaMessageBase, new()
{
    [Fact]
    public void Deserialize_ShouldParseRuntimeCorrectly_WithCompleteData()
    {
        output.WriteLine(@"1) Test raw messages");
        foreach (var dataString in testMessages.Keys)
        {
            try
            {
                ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
                var msg = new TMessage();
                msg.Deserialize(ref data);
            }
            catch (Exception e)
            {
                output.WriteLine($"ERROR:{dataString} : {e.Message}");
                throw;
            }
            
        }
        output.WriteLine(@"2) Test append \r\n to end");
        foreach (var origin in testMessages.Keys)
        {
            try
            {
                var dataString= $"{origin}\r\n";
                ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
                var msg = new TMessage();
                msg.Deserialize(ref data);
            }
            catch (Exception)
            {
                output.WriteLine($"ERROR:{origin}");
                throw;
            }
           
        }
        
        output.WriteLine(@"3) Test replace $=>!");
        foreach (var origin in testMessages.Keys)
        {
            try
            {
                var dataString= origin.Replace("$", "!");
                ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
                var msg = new TMessage();
                msg.Deserialize(ref data);
            }
            catch (Exception)
            {
                output.WriteLine($"ERROR:{origin}");
                throw;
            }
            
        }
        
        output.WriteLine(@"4) Test remove $");
        foreach (var origin in testMessages.Keys)
        {
            try
            {
                var dataString= origin.Replace("$", "");
                ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
                var msg = new TMessage();
                msg.Deserialize(ref data);
            }
            catch (Exception)
            {
                output.WriteLine($"ERROR:{origin}");
                throw;
            }
        }
        
        output.WriteLine(@"5) Test Append spaces");
        foreach (var origin in testMessages.Keys)
        {
            try
            {
                var dataString= $"  {origin}    ";
                ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString);
                var msg = new TMessage();
                msg.Deserialize(ref data);
            }
            catch (Exception)
            {
                output.WriteLine($"ERROR:{origin}");
                throw;
            }
        }
        
    }


    [Fact]
    public void Serialize_And_Deserialize_ShouldReturnEqualData()
    {
        int index = 0;
        foreach (var dataString in testMessages)
        {
            try
            {
                ReadOnlySpan<byte> data = NmeaProtocol.Encoding.GetBytes(dataString.Key);
                var origin = new TMessage();
                origin.Deserialize(ref data);

                var serialized = new byte[origin.GetByteSize()];
                var span = new Span<byte>(serialized);
                origin.Serialize(ref span);
                var serializedString = NmeaProtocol.Encoding.GetString(serialized);
                var substring = serializedString[..serializedString.IndexOf(NmeaProtocol.StartCrcChar)];
                Assert.Equal(dataString.Value, substring);
                index++;
            }
            catch (Exception)
            {
                output.WriteLine($"ERROR {index}:{dataString}");
                throw;
            }
            
        }
    }
}