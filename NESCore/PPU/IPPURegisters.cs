namespace NESCore.PPU
{
    public enum SpriteSize
    {
        _8x8 = 0,
        _8x16 = 1
    }

    public interface IPPURegisters
    {
        byte Status { get; set; }
        byte Control { get; set; }
        byte Mask { get; set; }

        void SetVBlankFlag(bool value);
        bool GetVBlankFlag();
        void SetOverflowFlag(bool value);
        bool GetOverflowFlag();
        void SetSpriteZeroHitFlag(bool value);

        UInt16 GetNametableAddress();
        int VRAMIncrement();
        //Only relevant in 8x8 mode. Ignored in 8x16 mode
        UInt16 GetSpritePatternTableAddress();
        UInt16 GetbackgroundPatternTableAddress();
        SpriteSize GetSpriteSize();
        bool GetEnableNMI();

        //TODO: mask register get set

    }
}