namespace NESCore.CPU
{
    public class FlagPositions
    {
        public const byte CARRY = 0b00000001;
        public const byte ZERO = 0b00000010;
        public const byte INTERRUPT_DISABLE = 0b00000100;
        public const byte DECIMAL = 0b00001000;
        public const byte BREAK = 0b00010000;
        public const byte OVERFLOW = 0b01000000;
        public const byte NEGATIVE = 0b10000000;
    }

    public class Registers : IRegisters
    {
        private byte _status = 0x24;  //Bit 5 is always set
        public ushort PC { get; set; } = 0;

        public byte A { get; set; } = 0;

        public byte X { get; set; } = 0;

        public byte Y { get; set; } = 0;

        public byte STATUS
        {
            get
            {
                return (byte)(_status | 0x20);
            }
            set
            {
                _status = (byte)(value | 0x20);
            }
        }

        public byte SP { get; set; } = 0xFD;

        public void SetCarryFlag(bool flag)
        {
            if (flag)
            {
                EnableFlag(FlagPositions.CARRY);
            }
            else
            {
                DisableFlag(FlagPositions.CARRY);
            }
        }

        public void SetDecimalFlag(bool flag)
        {
            if (flag)
            {
                EnableFlag(FlagPositions.DECIMAL);
            }
            else
            {
                DisableFlag(FlagPositions.DECIMAL);
            }
        }

        public void SetInterruptDisableFlag(bool flag)
        {
            if (flag)
            {
                EnableFlag(FlagPositions.INTERRUPT_DISABLE);
            }
            else
            {
                DisableFlag(FlagPositions.INTERRUPT_DISABLE);
            }
        }

        public void SetNegativeFlag(bool flag)
        {
            if (flag)
            {
                EnableFlag(FlagPositions.NEGATIVE);
            }
            else
            {
                DisableFlag(FlagPositions.NEGATIVE);
            }
        }

        public void SetOverflowFlag(bool flag)
        {
            if (flag)
            {
                EnableFlag(FlagPositions.OVERFLOW);
            }
            else
            {
                DisableFlag(FlagPositions.OVERFLOW);
            }
        }

        public void SetZeroFlag(bool flag)
        {
            if (flag)
            {
                EnableFlag(FlagPositions.ZERO);
            }
            else
            {
                DisableFlag(FlagPositions.ZERO);
            }
        }

        private void EnableFlag(byte mask)
        {
            STATUS |= mask;
            STATUS |= 0x20;
        }

        private void DisableFlag(byte mask)
        {
            STATUS &= (byte)~mask;
            STATUS |= 0x20;
        }

        public bool GetCarryFlag()
        {
            return (STATUS & FlagPositions.CARRY) > 0;
        }

        public bool GetZeroFlag()
        {
            return (STATUS & FlagPositions.ZERO) > 0;
        }

        public bool GetInterruptDisableFlag()
        {
            return (STATUS & FlagPositions.INTERRUPT_DISABLE) > 0;
        }

        public bool GetDecimalFlag()
        {
            return (STATUS & FlagPositions.DECIMAL) > 0;
        }

        public bool GetOverflowFlag()
        {
            return (STATUS & FlagPositions.OVERFLOW) > 0;
        }

        public bool GetNegativeFlag()
        {
            return (STATUS & FlagPositions.NEGATIVE) > 0;
        }
    }
}
