namespace NESCore.CPU.Instructions
{
    public partial class CpuInstructions
    {
        private static byte LSR(byte value, IRegisters registers)
        {
            registers.SetCarryFlag((value & 0x1) == 1); // Set the carry flag if the last bit is 1
            registers.SetNegativeFlag(false);
            var result = (byte)(value >> 1);
            registers.SetZeroFlag(result == 0);
            return result;
        }

        private static void LSR_A(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.A = LSR(registers.A, registers);
        }

        private static void LSR_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => LSR(value, registers));
        }

        private static void LSR_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => LSR(value, registers));
        }

        private static void LSR_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => LSR(value, registers));
        }

        private static void LSR_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => LSR(value, registers));
        }

        private static byte ASL(byte value, IRegisters registers)
        {
            registers.SetCarryFlag((value & 0x80) > 0);
            var result = (byte)(value << 1);
            registers.SetNegativeFlag((result & 0x80) > 0);
            registers.SetZeroFlag(result == 0);
            return result;
        }

        private static void ASL_A(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.A = ASL(registers.A, registers);
        }

        private static void ASL_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => ASL(value, registers));
        }

        private static void ASL_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => ASL(value, registers));
        }

        private static void ASL_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => ASL(value, registers));
        }

        private static void ASL_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => ASL(value, registers));
        }

        private static byte ROL(byte value, IRegisters registers)
        {
            var carryAfterShift = (value & 0x80) > 0;
            var result = (byte)(value << 1);
            if(registers.GetCarryFlag())
            {
                result |= 0x1;
            }
            registers.SetNegativeFlag((result & 0x80) > 0);
            registers.SetZeroFlag(result == 0);
            registers.SetCarryFlag(carryAfterShift);
            return result;
        }

        private static void ROL_A(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.A = ROL(registers.A, registers);
        }

        private static void ROL_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => ROL(value, registers));
        }

        private static void ROL_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => ROL(value, registers));
        }

        private static void ROL_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => ROL(value, registers));
        }

        private static void ROL_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => ROL(value, registers));
        }

        private static byte ROR(byte value, IRegisters registers)
        {
            var carryAfterShift = (value & 0x1) > 0;
            var result = (byte)(value >> 1);
            if(registers.GetCarryFlag())
            {
                result |= 0x80;
            }
            registers.SetCarryFlag(carryAfterShift);
            registers.SetZeroFlag(result == 0);
            registers.SetNegativeFlag((result & 0x80) > 0);
            return result;
        }

        private static void ROR_A(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.A = ROR(registers.A, registers);
        }

        private static void ROR_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => ROR(value, registers));
        }

        private static void ROR_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => ROR(value, registers));
        }

        private static void ROR_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => ROR(value, registers));
        }

        private static void ROR_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            addressingResult.ReadModifyWrite(bus, value => ROR(value, registers));
        }

    }
}
