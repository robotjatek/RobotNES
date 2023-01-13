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
            if(value == true)
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
    }
}
