namespace NESCore.PPU
{
    public interface IPPURegisters
    {
        byte Status { get; set; }
        byte Control { get; set; }

        void SetVBlankFlag(bool value);
        bool GetVBlankFlag();
        void SetOverflowFlag(bool value);
        bool GetOverflowFlag();
        void SetSpriteZeroHitFlag(bool value);
    }
}