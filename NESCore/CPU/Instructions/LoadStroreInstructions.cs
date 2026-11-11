namespace NESCore.CPU.Instructions
{
    public partial class CpuInstructions
    {
        private static void LDA(byte value, IRegisters registers)
        {
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static void LDA_IMM(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingImmediate(bus, registers);
            LDA(addressingResult.Value, registers);
        }

        private static void LDA_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            LDA(addressingResult.Value, registers);
        }

        private static void LDA_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            LDA(addressingResult.Value, registers);
        }

        private static void LDA_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            LDA(addressingResult.Value, registers);
        }

        private static void LDA_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            LDA(addressingResult.Value, registers);
        }

        private static void LDA_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            LDA(addressingResult.Value, registers);
        }

        private static void LDA_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectX(bus, registers);
            LDA(addressingResult.Value, registers);
        }

        private static void LDA_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            LDA(addressingResult.Value, registers);
        }

        private static void LAX(byte value, IRegisters registers)
        {
            LDA(value, registers);
            LDX(value, registers);
        }

        private static void LAX_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectX(bus, registers);
            LAX(addressingResult.Value, registers);
        }

        private static void LAX_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            LAX(addressingResult.Value, registers);
        }

        private static void LAX_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            LAX(addressingResult.Value, registers);
        }

        private static void LAX_ZERO_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroY(bus, registers);
            LAX(addressingResult.Value, registers);
        }

        private static void LAX_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            LAX(addressingResult.Value , registers);
        }

        private static void LAX_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            LAX(addressingResult.Value, registers);
        }

        private static void LDX(byte value, IRegisters registers)
        {
            registers.X = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static void LDX_IMM(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingImmediate(bus, registers);
            LDX(addressingResult.Value, registers);
        }

        private static void LDX_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            LDX(addressingResult.Value, registers);
        }

        private static void LDX_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            LDX(addressingResult.Value, registers);
        }

        private static void LDX_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            LDX(addressingResult.Value, registers);
        }

        private static void LDX_ZERO_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroY(bus, registers);
            LDX(addressingResult.Value, registers);
        }

        private static void LDY(byte value, IRegisters registers)
        {
            registers.Y = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static void LDY_IMM(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingImmediate(bus, registers);
            LDY(addressingResult.Value, registers);
        }

        private static void LDY_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            LDY(addressingResult.Value, registers);
        }

        private static void LDY_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            LDY(addressingResult.Value, registers);
        }

        private static void LDY_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            LDY(addressingResult.Value, registers);
        }

        private static void LDY_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            LDY(addressingResult.Value, registers);
        }


        private static void STA(ushort address, IBUS bus, IRegisters registers)
        {
            bus.Write(address, registers.A);
        }

        private static void STA_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroAddressOnly(bus, registers);
            STA(addressingResult.Address, bus, registers);
        }

        private static void STA_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroXAddressOnly(bus, registers);
            var dummyAddress = addressingResult.DummyAddress ?? addressingResult.Address;
            bus.Read(dummyAddress);
            STA(addressingResult.Address, bus, registers);
        }

        private static void STA_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteAddressOnly(bus, registers);
            STA(addressingResult.Address, bus, registers);
        }

        private static void STA_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteXAddressOnly(bus, registers);
            var dummyAddress = addressingResult.DummyAddress ?? addressingResult.Address;
            bus.Read(dummyAddress);
            STA(addressingResult.Address, bus, registers);
        }

        private static void STA_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteYAddressOnly(bus, registers);
            var dummyAddress = addressingResult.DummyAddress ?? addressingResult.Address;
            bus.Read(dummyAddress);
            STA(addressingResult.Address,  bus, registers);
        }

        private static void STA_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectXAddressOnly(bus, registers);
            STA(addressingResult.Address, bus, registers);
        }

        private static void STA_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectYAddressOnly(bus, registers);
            var dummyAddress = addressingResult.DummyAddress ?? addressingResult.Address;
            bus.Read(dummyAddress);
            STA(addressingResult.Address, bus, registers);
        }

        private static void STX_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroAddressOnly(bus, registers);
            bus.Write(addressingResult.Address, registers.X);
        }

        private static void STX_ZERO_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroYAddressOnly(bus, registers);
            var dummyAddress = addressingResult.DummyAddress ?? addressingResult.Address;
            bus.Read(dummyAddress);
            bus.Write(addressingResult.Address, registers.X);
        }

        private static void STX_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteAddressOnly(bus, registers);
            bus.Write(addressingResult.Address, registers.X);
        }

        private static void STY_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroAddressOnly(bus, registers);
            bus.Write(addressingResult.Address, registers.Y);
        }

        private static void STY_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroXAddressOnly(bus, registers);
            var dummyAddress = addressingResult.DummyAddress ?? addressingResult.Address;
            bus.Read(dummyAddress);
            bus.Write(addressingResult.Address, registers.Y);
        }

        private static void STY_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteAddressOnly(bus, registers);
            bus.Write(addressingResult.Address, registers.Y);
        }

        private static byte SAX(byte value1, byte value2)
        {
            return (byte)(value1 & value2);
        }

        private static void SAX_IND_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectXAddressOnly(bus, registers);
            bus.Write(addressingResult.Address, SAX(registers.A, registers.X));
        }

        private static void SAX_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroAddressOnly(bus, registers);
            bus.Write(addressingResult.Address, SAX(registers.A, registers.X));
        }

        private static void SAX_ZERO_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroYAddressOnly(bus, registers);
            var dummyAddress = addressingResult.DummyAddress ?? addressingResult.Address;
            bus.Read(dummyAddress);
            bus.Write(addressingResult.Address, SAX(registers.A, registers.X));
        }

        private static void SAX_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteAddressOnly(bus, registers);
            bus.Write(addressingResult.Address, SAX(registers.A, registers.X));
        }
    }
}
