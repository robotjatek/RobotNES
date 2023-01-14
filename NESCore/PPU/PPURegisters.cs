namespace NESCore.PPU
{
    public class PPUStatusFlagPositions
    {
        public const byte OPENBUS = 0b00011111;
        public const byte SPRITEOVERFLOW = 0b00100000;
        public const byte SPRITE0HIT = 0b01000000;
        public const byte VBLANK = 0b10000000;
    }

    public class PPUControlFlagPositions
    {
        public const byte NAMETABLE_ADDRESS = 0b00000011;
        public const byte VRAM_INCREMENT = 0b100;
        public const byte SPRITE_PATTERNTABLE_ADDRESS = 0x8;
        public const byte BACKGROUND_PATTERNTABLE_ADDRESS = 0x10;
        public const byte SPRITE_SIZE = 0x20;
        public const byte NMI = 0x80;
    }

    public class PPUMaskFlagPositions
    {
        public const byte GRAY_SCALE = 0x1;
        public const byte SHOW_BACKGROUND_IN_LEFT_COLUMN = 0x2;
        public const byte SHOW_SPRITES_IN_LEFT_COLUMN = 0x4;
        public const byte SHOW_BACKGROUND = 0x8;
        public const byte SHOW_SPRITES = 0x10;
        public const byte EMPHASIZE_RED = 0x20;
        public const byte EMPHASIZE_GREEN = 0x40;
        public const byte EMPHASIZE_BLUE = 0x80;
    }
    
    public class PPURegisters : IPPURegisters
    {
        public byte Status { get; set; } = 0;
        public byte Control { get; set; } = 0;
        public byte Mask { get; set; } = 0;

        #region STATUS_REGISTER
        public void SetVBlankFlag(bool value)
        {
            if (value == true)
            {
                EnableStatusFlag(PPUStatusFlagPositions.VBLANK);
            }
            else
            {
                DisableStatusFlag(PPUStatusFlagPositions.VBLANK);
            }
        }

        public bool GetVBlankFlag()
        {
            return (Status & PPUStatusFlagPositions.VBLANK) > 0;
        }

        public void SetOverflowFlag(bool value)
        {
            if (value == true)
            {
                EnableStatusFlag(PPUStatusFlagPositions.SPRITEOVERFLOW);
            }
            else
            {
                DisableStatusFlag(PPUStatusFlagPositions.SPRITEOVERFLOW);
            }
        }

        public bool GetOverflowFlag()
        {
            return (Status & PPUStatusFlagPositions.SPRITEOVERFLOW) > 0;
        }

        public void SetSpriteZeroHitFlag(bool value)
        {
            if (value == true)
            {
                EnableStatusFlag(PPUStatusFlagPositions.SPRITE0HIT);
            }
            else
            {
                DisableStatusFlag(PPUStatusFlagPositions.SPRITE0HIT);
            }
        }

        public bool GetSpriteZeroHitFlag()
        {
            return (Status & PPUStatusFlagPositions.SPRITE0HIT) > 0;
        }

        private void EnableStatusFlag(byte mask)
        {
            Status |= mask;
        }

        private void DisableStatusFlag(byte mask)
        {
            Status &= (byte)~mask;
        }
        #endregion

        #region CONTROL_REGISTER
        public UInt16 GetNametableAddress()
        {
            var map = new UInt16[4] { 0x2000, 0x2400, 0x2800, 0x2C00 };
            var index = Control & PPUControlFlagPositions.NAMETABLE_ADDRESS;

            return map[index];
        }

        public int VRAMIncrement()
        {
            return (Control & PPUControlFlagPositions.VRAM_INCREMENT) == 0 ? 1 : 32;
        }

        public UInt16 GetSpritePatternTableAddress()
        {
            return (Control & PPUControlFlagPositions.SPRITE_PATTERNTABLE_ADDRESS) == 0 ? (UInt16)0 : (UInt16)0x1000;
        }

        public UInt16 GetbackgroundPatternTableAddress()
        {
            return (Control & PPUControlFlagPositions.BACKGROUND_PATTERNTABLE_ADDRESS) == 0 ? (UInt16)0 : (UInt16)0x1000;
        }

        public SpriteSize GetSpriteSize()
        {
            return (Control&PPUControlFlagPositions.SPRITE_SIZE) == 0 ? SpriteSize._8x8 : SpriteSize._8x16;
        }

        public bool GetEnableNMI()
        {
            return (Control & PPUControlFlagPositions.NMI) > 0;
        }
        #endregion

        #region MASK_REGISTER
        public bool IsGrayScale()
        {
            return (Mask & PPUMaskFlagPositions.GRAY_SCALE) > 0;
        }

        public bool ShowBackgroundInLeftColumn()
        {
            return (Mask & PPUMaskFlagPositions.SHOW_BACKGROUND_IN_LEFT_COLUMN) > 0;
        }

        public bool ShowSpritesInLeftColumn()
        {
            return (Mask & PPUMaskFlagPositions.SHOW_SPRITES_IN_LEFT_COLUMN) > 0;
        }

        public bool ShowBackground()
        {
            return (Mask & PPUMaskFlagPositions.SHOW_BACKGROUND) > 0;
        }

        public bool ShowSprites()
        {
            return (Mask & PPUMaskFlagPositions.SHOW_SPRITES) > 0;
        }

        public bool EmphasizeRed()
        {
            return (Mask & PPUMaskFlagPositions.EMPHASIZE_RED) > 0;
        }

        public bool EmphasizeGreen()
        {
            return (Mask & PPUMaskFlagPositions.EMPHASIZE_GREEN) > 0;
        }

        public bool EmphasizeBlue()
        {
            return (Mask & PPUMaskFlagPositions.EMPHASIZE_BLUE) > 0;
        }
        #endregion
    }
}
