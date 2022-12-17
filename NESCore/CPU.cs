namespace NESCore
{
    public interface IRegisters
    {
        UInt16 PC { get; set; }
        byte A { get; set; }
        byte X { get; set; }
        byte Y { get; set; }

        /*
         * Bit 0 Carry
         * Bit 1 Zero
         * Bit 2 Interrupt disable
         * Bit 3 Decimal
         * Bit 4 --
         * Bit 5 --
         * Bit 6 Overflow
         * Bit 7 Negative
         */
        byte STATUS { get; set; }
        byte SP { get; set; }

        void SetCarryFlag(bool flag);
        void SetZeroFlag(bool flag);
        void SetInterruptDisableFlag(bool flag);
        void SetDecimalFlag(bool flag);
        void SetOverflowFlag(bool flag);
        void SetNegativeFlag(bool flag);

        bool GetCarryFlag();
        bool GetZeroFlag();
        bool GetInterruptDisableFlag();
        bool GetDecimalFlag();
        bool GetOverflowFlag();
        bool GetNegativeFlag();
    }

    public class FlagPositions
    {
        public const byte CARRY = 0b00000001;
        public const byte ZERO = 0b00000010;
        public const byte INTERRUPT_DISABLE = 0b00000100;
        public const byte DECIMAL = 0b00001000;
        public const byte OVERFLOW = 0b01000000;
        public const byte NEGATIVE = 0b10000000;
    }

    public class Registers : IRegisters
    {
        public UInt16 PC { get; set; } = 0;

        public byte A { get; set; } = 0;

        public byte X { get; set; } = 0;

        public byte Y { get; set; } = 0;

        public byte STATUS { get; set; } = 0;

        public byte SP { get; set; } = 0xFF;

        public void SetCarryFlag(bool flag)
        {
            if(flag)
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
        }

        private void DisableFlag(byte mask)
        {
            STATUS &= (byte)~mask;
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

    public class CPU : ICPU
    {
        private readonly IBUS _bus;
        private readonly IRegisters _registers = new Registers();
        private readonly Func<IBUS, IRegisters, byte>[] _instructions;

        public CPU(IBUS bus, Func<IBUS, IRegisters, byte>[] instuctions)
        {
            _bus = bus;
            _instructions = instuctions;
            //_registers.PC = (UInt16)(_bus.Read(0xfffd) << 8 | _bus.Read(0xfffc)); // Reset vector //TODO: uncomment this after instructionset implementation
            _registers.PC = 0xc000; //nestest.nes start //TODO: delete this after instructionset implementation
        }

        public void Cycle()
        {
            throw new NotImplementedException();
        }

        public uint RunInstruction()
        {
            var instructionCode = Fetch();
            var instruction = _instructions[instructionCode];

            if(instruction == null)
            {
                throw new NotImplementedException($"0x{instructionCode:X}@{_registers.PC-1:X}");
            }

            var elapsedCycles = instruction(_bus, _registers);
            return elapsedCycles;
        }

        private byte Fetch()
        {
            return _bus.Read(_registers.PC++);
        }
    }
}