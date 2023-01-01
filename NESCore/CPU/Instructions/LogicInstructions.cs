using Microsoft.Win32;

namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static void BIT(byte value, IRegisters registers)
        {
            var isNegative = (value & 0x80) == 0x80;
            registers.SetNegativeFlag(isNegative);

            var overflow = (value & 0x40) == 0x40;
            registers.SetOverflowFlag(overflow);

            var isZero = (value & registers.A) == 0;
            registers.SetZeroFlag(isZero);
        }

        private static byte BIT_ZERO(IBUS bus, IRegisters registers)
        {
            var value = AddressingZero(bus, registers).Value;
            BIT(value, registers);            

            return 3;
        }

        private static byte BIT_ABS(IBUS bus, IRegisters registers)
        {
            var value = AddressingAbsolute(bus, registers).Value;
            BIT(value, registers);

            return 4;
        }

        private static void AND(byte mask, IRegisters registers)
        {
            var value = (byte)(registers.A & mask);
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static byte AND_IMM(IBUS bus, IRegisters registers)
        {
            var mask = AddressingImmediate(bus, registers).Value;
            AND(mask, registers);

            return 2;
        }

        private static byte AND_IND_X(IBUS bus, IRegisters registers)
        {
            var mask = AddressingIndirectX(bus, registers).Value;
            AND(mask, registers);

            return 6;
        }

        private static byte AND_ZERO(IBUS bus, IRegisters registers)
        {
            var mask = AddressingZero(bus, registers).Value;
            AND(mask, registers);

            return 3;
        }

        private static byte AND_ABS(IBUS bus, IRegisters registers)
        {
            var mask = AddressingAbsolute(bus, registers).Value;
            AND(mask, registers);

            return 4;
        }

        private static byte AND_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            AND(addressingResult.Value, registers);

            return addressingResult.Cycles;
        }

        private static byte AND_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            AND(addressingResult.Value, registers);

            return addressingResult.Cycles;
        }

        private static void ORA(byte value, IRegisters registers)
        {
            var result = (byte)(registers.A | value);
            registers.A = result;
            registers.SetZeroFlag(result == 0);
            registers.SetNegativeFlag((sbyte)result < 0);
        }

        private static byte ORA_IMM(IBUS bus, IRegisters registers)
        {
            var mask = AddressingImmediate(bus, registers).Value;
            ORA(mask, registers);

            return 2;
        }

        private static byte ORA_ZERO(IBUS bus, IRegisters registers)
        {
            var mask = AddressingZero(bus, registers).Value;
            ORA(mask, registers);

            return 3;
        }

        private static byte ORA_IND_X(IBUS bus, IRegisters registers)
        {
            var mask = AddressingIndirectX(bus, registers).Value;
            ORA(mask, registers);

            return 6;
        }

        private static byte ORA_ABS(IBUS bus, IRegisters registers)
        {
            var mask = AddressingAbsolute(bus, registers).Value;
            ORA(mask, registers);

            return 4;
        }

        private static byte ORA_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            ORA(addressingResult.Value, registers);

            return addressingResult.Cycles;
        }

        private static byte ORA_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            ORA(addressingResult.Value, registers);

            return addressingResult.Cycles;
        }

        private static void EOR(byte mask, IRegisters registers)
        {
            var value = (byte)(registers.A ^ mask);
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static byte EOR_IMM(IBUS bus, IRegisters registers)
        {
            var mask = AddressingImmediate(bus, registers).Value;
            EOR(mask, registers);
            return 2;
        }

        private static byte EOR_IND_X(IBUS bus, IRegisters registers)
        {
            var mask = AddressingIndirectX(bus, registers).Value;
            EOR(mask, registers);
            return 6;
        }

        private static byte EOR_ZERO(IBUS bus, IRegisters registers)
        {
            var mask = AddressingZero(bus, registers).Value;
            EOR(mask, registers);
            return 3;
        }

        private static byte EOR_ABS(IBUS bus, IRegisters registers)
        {
            var mask = AddressingAbsolute(bus, registers).Value;
            EOR(mask, registers);
            return 4;
        }

        private static byte EOR_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            EOR(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }

        private static byte EOR_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            EOR(addressingResult.Value, registers);
            return addressingResult.Cycles;
        }
    }
}
