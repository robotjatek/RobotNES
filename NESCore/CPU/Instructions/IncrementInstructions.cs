namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte INC(byte value, IRegisters registers)
        {
            var result = (byte)(value + 1);
            registers.SetZeroFlag(result == 0);
            registers.SetNegativeFlag((result & 0x80) > 0);
            return result;
        }

        private static byte INC_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            var result = INC(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 5;
        }

        private static byte INC_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            var result = INC(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return (byte)(addressingResult.Cycles + 2);
        }

        private static byte INC_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            var result = INC(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 6;
        }

        private static byte INC_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            var result = INC(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 7;
        }

        private static byte INX(IBUS bus, IRegisters registers)
        {
            registers.X++;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
            return 2;
        }

        private static byte INY(IBUS bus, IRegisters registers)
        {
            registers.Y++;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
            return 2;
        }

        private static byte DEC(byte value, IRegisters registers)
        {
            var result = (byte)(value - 1);
            registers.SetZeroFlag(result == 0);
            registers.SetNegativeFlag((result & 0x80) > 0);
            return result;
        }

        private static byte DEC_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            var result = DEC(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 5;
        }

        private static byte DEC_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            var result = DEC(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return (byte)(addressingResult.Cycles + 2);
        }

        private static byte DEC_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            var result = DEC(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 6;
        }

        private static byte DEX(IBUS bus, IRegisters registers)
        {
            registers.X--;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
            return 2;
        }

        private static byte DEY(IBUS bus, IRegisters registers)
        {
            registers.Y--;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
            return 2;
        }
    }
}
