using Microsoft.Win32;

namespace NESCore.CPU.Instructions
{
    public partial class CpuInstructions
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

        private static void BIT_ZERO(IBUS bus, IRegisters registers)
        {
            var value = AddressingZero(bus, registers).Value;
            BIT(value, registers);
        }

        private static void BIT_ABS(IBUS bus, IRegisters registers)
        {
            var value = AddressingAbsolute(bus, registers).Value;
            BIT(value, registers);
        }

        private static void AND(byte mask, IRegisters registers)
        {
            var value = (byte)(registers.A & mask);
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static void AND_IMM(IBUS bus, IRegisters registers)
        {
            var mask = AddressingImmediate(bus, registers).Value;
            AND(mask, registers);
        }

        private static void AND_IND_X(IBUS bus, IRegisters registers)
        {
            var mask = AddressingIndirectX(bus, registers).Value;
            AND(mask, registers);
        }

        private static void AND_ZERO(IBUS bus, IRegisters registers)
        {
            var mask = AddressingZero(bus, registers).Value;
            AND(mask, registers);
        }

        private static void AND_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            AND(addressingResult.Value, registers);
        }

        private static void AND_ABS(IBUS bus, IRegisters registers)
        {
            var mask = AddressingAbsolute(bus, registers).Value;
            AND(mask, registers);
        }

        private static void AND_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            AND(addressingResult.Value, registers);
        }

        private static void AND_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            AND(addressingResult.Value, registers);
        }

        private static void AND_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            AND(addressingResult.Value, registers);
        }

        private static void ORA(byte value, IRegisters registers)
        {
            var result = (byte)(registers.A | value);
            registers.A = result;
            registers.SetZeroFlag(result == 0);
            registers.SetNegativeFlag((sbyte)result < 0);
        }

        private static void ORA_IMM(IBUS bus, IRegisters registers)
        {
            var mask = AddressingImmediate(bus, registers).Value;
            ORA(mask, registers);
        }

        private static void ORA_ZERO(IBUS bus, IRegisters registers)
        {
            var mask = AddressingZero(bus, registers).Value;
            ORA(mask, registers);
        }

        private static void ORA_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            ORA(addressingResult.Value, registers);
        }

        private static void ORA_IND_X(IBUS bus, IRegisters registers)
        {
            var mask = AddressingIndirectX(bus, registers).Value;
            ORA(mask, registers);
        }

        private static void ORA_ABS(IBUS bus, IRegisters registers)
        {
            var mask = AddressingAbsolute(bus, registers).Value;
            ORA(mask, registers);
        }

        private static void ORA_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            ORA(addressingResult.Value, registers);
        }

        private static void ORA_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            ORA(addressingResult.Value, registers);
        }

        private static void ORA_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            ORA(addressingResult.Value, registers);
        }

        private static void EOR(byte mask, IRegisters registers)
        {
            var value = (byte)(registers.A ^ mask);
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
        }

        private static void EOR_IMM(IBUS bus, IRegisters registers)
        {
            var mask = AddressingImmediate(bus, registers).Value;
            EOR(mask, registers);
        }

        private static void EOR_IND_X(IBUS bus, IRegisters registers)
        {
            var mask = AddressingIndirectX(bus, registers).Value;
            EOR(mask, registers);
        }

        private static void EOR_ZERO(IBUS bus, IRegisters registers)
        {
            var mask = AddressingZero(bus, registers).Value;
            EOR(mask, registers);
        }

        private static void EOR_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            EOR(addressingResult.Value, registers);
        }

        private static void EOR_ABS(IBUS bus, IRegisters registers)
        {
            var mask = AddressingAbsolute(bus, registers).Value;
            EOR(mask, registers);
        }

        private static void EOR_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            EOR(addressingResult.Value, registers);
        }

        private static void EOR_ABS_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteY(bus, registers);
            EOR(addressingResult.Value, registers);
        }

        private static void EOR_IND_Y(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingIndirectY(bus, registers);
            EOR(addressingResult.Value, registers);
        }
    }
}
