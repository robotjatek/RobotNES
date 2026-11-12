namespace NESCoreTests.Unit.PPUTest;

/// <summary>
/// Decodes the loopy scroll word (0 yyy NN YYYYY XXXXX) in one place.
/// </summary>
internal static class ScrollBits
{
    public static byte FineY(ushort word) => (byte)((word & 0x7000) >> 12);

    public static byte CoarseY(ushort word) => (byte)((word & 0x03E0) >> 5);

    public static byte CoarseX(ushort word) => (byte)(word & 0x001F);

    public static byte Nametable(ushort word) => (byte)((word & 0x0C00) >> 10);
    
    public static bool HorizontalNametable(ushort word) => (word & 0x0400) != 0;
    
    public static bool VerticalNametable(ushort word) => (word & 0x0800) != 0;
    
    public static ushort VerticalHalf(ushort word) => (ushort)(word & 0x7BE0);
}
