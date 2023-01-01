namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte LSR(byte value, IRegisters registers)
        {
            registers.SetCarryFlag((value & 0x1) == 1); // Set the carry flag if the last bit is 1
            registers.SetNegativeFlag(false);
            var result = (byte)(value >> 1);
            registers.SetZeroFlag(result == 0);
            return result;
        }

        private static byte LSR_A(IBUS bus, IRegisters registers)
        {
            registers.A = LSR(registers.A, registers);
            return 2;
        }

        private static byte LSR_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            var result = LSR(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 5;
        }

        private static byte LSR_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            var result = LSR(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return (byte)(addressingResult.Cycles + 2);
        }

        private static byte LSR_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            var result = LSR(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 6;
        }

        private static byte ASL(byte value, IRegisters registers)
        {
            registers.SetCarryFlag((value & 0x80) > 0);
            var result = (byte)(value << 1);
            registers.SetNegativeFlag((result & 0x80) > 0);
            registers.SetZeroFlag(result == 0);
            return result;
        }

        private static byte ASL_A(IBUS bus, IRegisters registers)
        {
            registers.A = ASL(registers.A, registers);
            return 2;
        }

        private static byte ASL_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            var result = ASL(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 5;
        }

        private static byte ASL_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            var result = ASL(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return (byte)(addressingResult.Cycles + 2);
        }

        private static byte ASL_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            var result = ASL(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 6;
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

        private static byte ROL_A(IBUS bus, IRegisters registers)
        {
            registers.A = ROL(registers.A, registers);
            return 2;
        }

        private static byte ROL_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            var result = ROL(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 5;
        }

        private static byte ROL_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            var result = ROL(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 6;
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

        private static byte ROR_A(IBUS bus, IRegisters registers)
        {
            registers.A = ROR(registers.A, registers);
            return 2;
        }

        private static byte ROR_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            var result = ROR(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 5;
        }
        private static byte ROR_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            var result = ROR(addressingResult.Value, registers);
            bus.Write(addressingResult.Address, result);
            return 6;
        }

    }
}
