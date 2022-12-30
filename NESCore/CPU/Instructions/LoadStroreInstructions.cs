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
            var addressingResult = AddressingAbsoluteWithValue(bus, registers);
            LDA(addressingResult.Value, registers);
            return 4;
        }

        private static byte LDA_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroWithValue(bus, registers);
            LDA(addressingResult.Value, registers);
            return 3;
        }

        private static byte LDA_IND_X(IBUS bus, IRegisters registers)
        {
            var value = AddressingIndirectXWithValue(bus, registers).Value;
            LDA(value, registers);
            return 6;
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
            var addressingResult = AddressingAbsoluteWithValue(bus, registers);
            LDX(addressingResult.Value, registers);
            return 4;
        }

        private static byte LDX_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroWithValue(bus, registers);
            LDX(addressingResult.Value, registers);
            return 3;
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
            var addressingResult = AddressingZeroWithValue(bus, registers);
            LDY(addressingResult.Value, registers);
            return 3;
        }


        private static void STA(ushort address, IBUS bus, IRegisters registers)
        {
            bus.Write(address, registers.A);
        }

        private static byte STA_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroAddressOnly(bus, registers);
            STA(addressingResult.Address, bus, registers);
            return 3;
        }

        private static byte STA_ABS(IBUS bus, IRegisters registers)
        {
            var address = AddressingAbsoulteAddressOnly(bus, registers).Address;
            STA(address, bus, registers);
            return 4;
        }

        private static byte STA_IND_X(IBUS bus, IRegisters registers)
        {
            var address = AddressingIndirectXAddressOnly(bus, registers).Address;
            STA(address, bus, registers);
            return 6;
        }

        private static byte STX_ZERO(IBUS bus, IRegisters registers)
        {
            var address = AddressingZeroAddressOnly(bus, registers).Address;
            bus.Write(address, registers.X);
            return 3; //1(opcode fetch) + 1 (1 byte fetch from memory) + 1 (1 byte write to memory)
        }

        private static byte STX_ABS(IBUS bus, IRegisters registers)
        {
            var address = AddressingAbsoulteAddressOnly(bus, registers).Address;
            bus.Write(address, registers.X);
            return 4; //1(opcode fetch) + 2 (2 byte fetch from memory) + 1 (1 byte write to memory)
        }

        private static byte STY_ZERO(IBUS bus, IRegisters registers)
        {
            var address = AddressingZeroAddressOnly(bus, registers).Address;
            bus.Write(address, registers.Y);
            return 3;
        }
    }
}
