using System;
using Asv.IO;

namespace Asv.Gnss
{
    public enum BackupCreationEnum
    {
        NotAcknowledged = 0,
        Acknowledged = 1
    }

    public enum RestoredFromBackupEnum
    {
        Unknown = 0,
        FailedRestoring = 1,
        RestoredOk = 2,
        NoBackup = 3
    }

    public class UbxBackupRestoreStatusResult : UbxMessageBase
    {
        public override byte Class => 0x09;
        public override byte SubClass => 0x14;
        public override string Name => "BackupRestoreStatus";

        protected override void SerializeContent(ref Span<byte> buffer)
        {
            throw new NotImplementedException();
        }

        protected override void DeserializeContent(ref ReadOnlySpan<byte> buffer, int payloadByteSize)
        {
            var command = BinSerialize.ReadByte(ref buffer);
            buffer = buffer.Slice(3);

            switch (command)
            {
                case 2:
                    BackupCreation = (BackupCreationEnum)BinSerialize.ReadByte(ref buffer);
                    break;
                case 3:
                    RestoredFromBackup = (RestoredFromBackupEnum)BinSerialize.ReadByte(ref buffer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            buffer = buffer.Slice(3);
        }

        protected override int GetContentByteSize() => 8;

        public BackupCreationEnum? BackupCreation { get; private set; }

        public RestoredFromBackupEnum? RestoredFromBackup { get; private set; }


    }

}