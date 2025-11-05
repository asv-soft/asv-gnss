using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using Asv.IO;

namespace Asv.Gnss;

public class AsterixMessageParser : ProtocolParser<AsterixMessage, byte>
{
    #region State
    
    private enum State
    {
        Category,
        Length1,
        Length2,
        Data
    }
    
    #endregion
    
    private State _state;
    private readonly byte[] _buffer = new byte[65536];
    private ushort _length;
    private int _index;

    public AsterixMessageParser(IProtocolMessageFactory<AsterixMessage, byte> messageFactory, IProtocolContext context, IStatisticHandler? statisticHandler) 
        : base(messageFactory, context, statisticHandler)
    {
    }

    public override bool Push(byte data)
    {
        switch (_state)
        {
            case State.Category:
                _buffer[0] = data; 
                _state = State.Length1;
                break;
            case State.Length1:
                _buffer[1] = data;
                _state = State.Length2;
                break;
            case State.Length2:
                _buffer[2] = data;
                var lengthSpan = new ReadOnlySpan<byte>(_buffer, 1, 2);
                _length = AsterixProtocol.ReadLength(ref lengthSpan);
                _index = 3;
                _state = State.Data;
                break;
            case State.Data:
                _buffer[_index] = data;
                _index++;
                if (_index >= _length)
                {
                    _state = State.Category;
                    var dataSpan = new ReadOnlySpan<byte>(_buffer, 0, _length);
                    InternalParsePacket(_buffer[0],ref dataSpan);
                    return true;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Reset()
    {
        _state = State.Category;
    }

    public override ProtocolInfo Info => AsterixProtocol.Info;
}