namespace NESCore.CPU.Instructions
{
    public partial class CpuInstructions
    {
        private static byte INC(byte value, IRegisters registers)
        {
            var result = (byte)(value + 1);
            registers.SetZeroFlag(result == 0);
            registers.SetNegativeFlag((result & 0x80) > 0);
            return result;
        }

        private static void INC_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => INC(value, registers));
        }

        private static void INC_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => INC(value, registers));;
        }

        private static void INC_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => INC(value, registers));
        }

        private static void INC_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => INC(value, registers));
        }

        private static void INX(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.X++;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
        }

        private static void INY(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.Y++;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
        }

        private static byte DEC(byte value, IRegisters registers)
        {
            var result = (byte)(value - 1);
            registers.SetZeroFlag(result == 0);
            registers.SetNegativeFlag((result & 0x80) > 0);
            return result;
        }

        private static void DEC_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => DEC(value, registers));
        }

        private static void DEC_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => DEC(value, registers));;
        }

        private static void DEC_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => DEC(value, registers));
        }

        private static void DEC_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => DEC(value, registers));
        }

        private static void DEX(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.X--;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
        }

        private static void DEY(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.Y--;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
        }
    }
}
