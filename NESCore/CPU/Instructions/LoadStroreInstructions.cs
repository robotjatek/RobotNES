namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static void LDA(byte value, IRegisters registers)
        {
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static byte LDA_IMM(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingImmediate(bus, registers);
            LDA(addressingResult.Value, registers);
            return 2;
        }

        private static byte LDA_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            LDA(addressingResult.Value, registers);
            return 4;
        }

        private static byte LDA_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            LDA(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte LDA_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            LDA(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte LDA_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            LDA(addressingResult.Value, registers);
            return 3;
        }

        private static byte LDA_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            LDA(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte LDA_IND_X(IBUS bus, IRegisters registers)
        {
            var value = AddressingIndirectX(bus, registers).Value;
            LDA(value, registers);
            return 6;
        }

        private static byte LDA_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            LDA(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static void LAX(byte value, IRegisters registers)
        {
            LDA(value, registers);
            LDX(value, registers);
        }

        private static byte LAX_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectX(bus, registers);
            LAX(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte LAX_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            LAX(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte LAX_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            LAX(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte LAX_ZERO_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroY(bus, registers);
            LAX(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte LAX_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            LAX(addressingResult.Value , registers);
            return addressingResult.Cycles;
        }

        private static byte LAX_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            LAX(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static void LDX(byte value, IRegisters registers)
        {
            registers.X = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static byte LDX_IMM(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingImmediate(bus, registers);
            LDX(addressingResult.Value, registers);
            return 2;
        }

        private static byte LDX_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            LDX(addressingResult.Value, registers);
            return 4;
        }

        private static byte LDX_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            LDX(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte LDX_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            LDX(addressingResult.Value, registers);
            return 3;
        }

        private static byte LDX_ZERO_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroY(bus, registers);
            LDX(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static void LDY(byte value, IRegisters registers)
        {
            registers.Y = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static byte LDY_IMM(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingImmediate(bus, registers);
            LDY(addressingResult.Value, registers);
            return 2;
        }

        private static byte LDY_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            LDY(addressingResult.Value, registers);
            return 3;
        }

        private static byte LDY_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            LDY(addressingResult.Value, registers);
            return 4;
        }

        private static byte LDY_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            LDY(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte LDY_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            LDY(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }


        private static void STA(ushort address, IBUS bus, IRegisters registers)
        {
            bus.Write(address, registers.A);
        }

        private static byte STA_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            STA(addressingResult.Address, bus, registers);
            return 3;
        }

        private static byte STA_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            STA(addressingResult.Address, bus, registers);
            return addressingResult.Cycles;
        }

        private static byte STA_ABS(IBUS bus, IRegisters registers)
        {
            var address = AddressingAbsolute(bus, registers).Address;
            STA(address, bus, registers);
            return 4;
        }

        private static byte STA_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            STA(addressingResult.Address, bus, registers);
            return 5; //According to https://www.pagetable.com/c64ref/6502/?tab=3#a16,Y STA_ABS_X and STA_ABS_Y are always 5 cycles regardless of page boundary crossing
        }

        private static byte STA_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            STA(addressingResult.Address,  bus, registers);
            return 5; //According to https://www.pagetable.com/c64ref/6502/?tab=3#a16,Y STA_ABS_X and STA_ABS_Y are always 5 cycles regardless of page boundary crossing
        }

        private static byte STA_IND_X(IBUS bus, IRegisters registers)
        {
            var address = AddressingIndirectX(bus, registers).Address;
            STA(address, bus, registers);
            return 6;
        }

        private static byte STA_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            STA(addressingResult.Address, bus, registers);
            return addressingResult.Cycles;
        }

        private static byte STX_ZERO(IBUS bus, IRegisters registers)
        {
            var address = AddressingZero(bus, registers).Address;
            bus.Write(address, registers.X);
            return 3; //1(opcode fetch) + 1 (1 byte fetch from memory) + 1 (1 byte write to memory)
        }

        private static byte STX_ZERO_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroY(bus, registers);
            bus.Write(addressingResult.Address, registers.X);
            return addressingResult.Cycles;
        }

        private static byte STX_ABS(IBUS bus, IRegisters registers)
        {
            var address = AddressingAbsolute(bus, registers).Address;
            bus.Write(address, registers.X);
            return 4; //1(opcode fetch) + 2 (2 byte fetch from memory) + 1 (1 byte write to memory)
        }

        private static byte STY_ZERO(IBUS bus, IRegisters registers)
        {
            var address = AddressingZero(bus, registers).Address;
            bus.Write(address, registers.Y);
            return 3;
        }

        private static byte STY_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            bus.Write(addressingResult.Address, registers.Y);
            return addressingResult.Cycles;
        }

        private static byte STY_ABS(IBUS bus, IRegisters registers)
        {
            var address = AddressingAbsolute(bus, registers).Address;
            bus.Write(address, registers.Y);
            return 4;
        }

        private static byte SAX(byte value1, byte value2)
        {
            return (byte)(value1 & value2);
        }

        private static byte SAX_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectX(bus, registers);
            bus.Write(addressingResult.Address, SAX(registers.A, registers.X));
            return addressingResult.Cycles;
        }

        private static byte SAX_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            bus.Write(addressingResult.Address, SAX(registers.A, registers.X));
            return addressingResult.Cycles;
        }

        private static byte SAX_ZERO_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroY(bus, registers);
            bus.Write(addressingResult.Address, SAX(registers.A, registers.X));
            return addressingResult.Cycles;
        }

        private static byte SAX_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            bus.Write(addressingResult.Address, SAX(registers.A, registers.X));
            return addressingResult.Cycles;
        }
    }
}
