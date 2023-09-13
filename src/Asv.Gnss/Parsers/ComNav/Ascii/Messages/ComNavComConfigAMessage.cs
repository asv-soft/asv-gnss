using System;

namespace Asv.Gnss
{
    public class ComNavPortCfg
    {
        public ComNavPortEnum Port { get; set; }
        public uint BaudRate { get; set; }
        public uint[] OtherParams { get; } = new uint[8];
    }

    public class ComNavComConfigAMessage : ComNavAsciiMessageBase
    {
        public const string ComNavMessageId = "COMCONFIGA";
        public override string MessageId => ComNavMessageId;
        public override string Name => ComNavMessageId;
        public ComNavPortCfg[] Ports { get; set; }
        protected override void InternalContentDeserialize(string[] msg)
        {
            var portCnt = msg.Length / 10;
            Ports = new ComNavPortCfg[portCnt];

            for (var i = 0; i < portCnt; i++)
            {
                var port = new ComNavPortCfg();

#if NETFRAMEWORK
                port.Port = Enum.TryParse(msg[0 + i * 10], true, out ComNavPortEnum dir) ? dir : ComNavPortEnum.NO_PORTS;
#else
				if (Enum.TryParse(typeof(ComNavPortEnum), msg[0 + i * 10], true, out var dir))
					 port.Port = (ComNavPortEnum)dir;
				else port.Port = ComNavPortEnum.NO_PORTS;
#endif
                port.BaudRate = uint.TryParse(msg[1 + i * 10], out var baudRate) ? baudRate : 0;
                port.OtherParams[0] = uint.TryParse(msg[2 + i * 10], out var param1) ? param1 : 0;
                port.OtherParams[1] = uint.TryParse(msg[3 + i * 10], out var param2) ? param2 : 0;
                port.OtherParams[2] = uint.TryParse(msg[4 + i * 10], out var param3) ? param3 : 0;
                port.OtherParams[3] = uint.TryParse(msg[5 + i * 10], out var param4) ? param4 : 0;
                port.OtherParams[4] = uint.TryParse(msg[6 + i * 10], out var param5) ? param5 : 0;
                port.OtherParams[5] = uint.TryParse(msg[7 + i * 10], out var param6) ? param6 : 0;
                port.OtherParams[6] = uint.TryParse(msg[8 + i * 10], out var param7) ? param7 : 0;
                port.OtherParams[7] = uint.TryParse(msg[9 + i * 10], out var param8) ? param8 : 0;
                Ports[i] = port;
            }
        }

    }
}