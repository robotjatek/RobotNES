namespace NESCore.PPU;

public readonly record struct OamEntry(byte Y, byte TileIndex, byte Attributes, byte X, int Index)
{
    public bool IsSpriteZero => Index == 0;
    public int ScreenY => Y + 1;
    public int ScreenX => X;
    public int PaletteIndex => Attributes & 0b11;
    public bool VerticalFlip => (Attributes & 0x80) != 0;
    public bool HorizontalFlip => (Attributes & 0x40) != 0;
    public bool BehindBackground => (Attributes & 0x20) != 0; // false: Front of background, true: Behind background

    public ushort GetPatternTableAddress(SpriteSize spriteSize, ushort baseAddress, int row)
    {
        return spriteSize switch
        {
            SpriteSize._8x8 => (ushort)(baseAddress + (TileIndex * 16) + row),
            SpriteSize._8x16 => _8x16(),
            _ => throw new ArgumentOutOfRangeException($"Invalid sprite size")
        };
    }

    private ushort _8x16()
    {
        // TODO: 8x16 mode
        var patternTable = (TileIndex & 0x1) * 0x1000;
        var tileIndex = TileIndex & 0xFE;
        throw new NotImplementedException("8x16 sprite mode is not implemented");
    }

public readonly record struct SpriteAttribute(byte Palette, bool Priority, bool VerticalFlip, bool HorizontalFlip)
    {
        public byte ToByte()
        {
            return (byte)(Palette & 0b11 // Bits 0-1
                          | (Priority ? 0x20 : 0) // bit 5
                          | (HorizontalFlip ? 0x40 : 0)
                          | (VerticalFlip ? 0x80 : 0));
        }
    }
}