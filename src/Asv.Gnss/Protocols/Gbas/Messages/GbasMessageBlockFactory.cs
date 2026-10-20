using System;
namespace Asv.Gnss;

public static class GbasMessageBlockFactory
{
    public static GbasMessageBlockBase CreateGbasMessageBlock(byte messageBlockId)
    {
        return messageBlockId switch
        {
            MessageBlockType1.GbasMessageTypeId => new MessageBlockType1(),
            MessageBlockType101.GbasMessageTypeId => new MessageBlockType101(),
            MessageBlockType11.GbasMessageTypeId => new MessageBlockType11(),
            MessageBlockType2.GbasMessageTypeId => new MessageBlockType2(),
            MessageBlockType3.GbasMessageTypeId => new MessageBlockType3(),
            MessageBlockType4.GbasMessageTypeId => new MessageBlockType4(),
            MessageBlockType5.GbasMessageTypeId => new MessageBlockType5(),
            _ => throw new ArgumentOutOfRangeException(nameof(messageBlockId))
        };
    }

    public static AdditionalDataBlock CreateAdditionalDataBlockFromMsg2(byte blockType)
    {
        return (blockType) switch
        {
            AdditionalDataBlock1.BlockNumber => new AdditionalDataBlock1(),
            AdditionalDataBlock2.BlockNumber => new AdditionalDataBlock2(),
            AdditionalDataBlock3.BlockNumber => new AdditionalDataBlock3(),
            AdditionalDataBlock4.BlockNumber => new AdditionalDataBlock4(),
            _ => throw new ArgumentOutOfRangeException(nameof(blockType))
        };
    }
}
