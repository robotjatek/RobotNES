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
        //TODO: OAM address register (0x2003)
        //TODO: OAM data register (0x2004)
        byte Scroll { get; set; }
        byte Address { get; set; }


        #region STATUS_REGISTER
        void SetVBlankFlag(bool value);
        bool GetVBlankFlag();
        void SetOverflowFlag(bool value);
        bool GetOverflowFlag();
        void SetSpriteZeroHitFlag(bool value);
        #endregion

        #region CONTROL_REGISTER
        UInt16 GetNametableAddress();
        int VRAMIncrement();
        //Only relevant in 8x8 mode. Ignored in 8x16 mode
        UInt16 GetSpritePatternTableAddress();
        UInt16 GetbackgroundPatternTableAddress();
        SpriteSize GetSpriteSize();
        bool GetEnableNMI();
        #endregion

        #region MASK_REGISTER
        bool IsGrayScale();
        bool ShowBackgroundInLeftColumn();
        bool ShowSpritesInLeftColumn();
        bool ShowBackground();
        bool ShowSprites();
        bool EmphasizeRed();
        bool EmphasizeGreen();
        bool EmphasizeBlue();
        #endregion
    }
}