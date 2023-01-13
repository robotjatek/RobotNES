namespace NESCore.PPU
{
    public class PPUStatusFlagPositions
    {
        public const byte OPENBUS = 0b00011111;
        public const byte SPRITEOVERFLOW = 0b00100000;
        public const byte SPRITE0HIT = 0b01000000;
        public const byte VBLANK = 0b10000000;
    }

    public class PPURegisters : IPPURegisters
    {
        public byte Status { get; set; } = 0;
        public byte Control { get; set; } = 0;
        public byte Mask { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

        public UInt16 GetNametableAddress()
        {
            var map = new UInt16[4] { 0x2000, 0x2400, 0x2800, 0x2C00 };
            var index = Control & 0b00000011;

            return map[index];
        }

        public int VRAMIncrement()
        {
            return (Control & 0b100) == 0 ? 1 : 32;
        }

        public UInt16 GetSpritePatternTableAddress()
        {
            return (Control & 0x8) == 0 ? (UInt16)0 : (UInt16)0x1000;
        }

        public UInt16 GetbackgroundPatternTableAddress()
        {
            return (Control & 0x10) == 0 ? (UInt16)0 : (UInt16)0x1000;
        }

        public SpriteSize GetSpriteSize()
        {
            return (Control&0x20) == 0 ? SpriteSize._8x8 : SpriteSize._8x16;
        }

        public bool GetEnableNMI()
        {
            return (Control & 0x80) > 0 ? true : false;
        }
    }
}
