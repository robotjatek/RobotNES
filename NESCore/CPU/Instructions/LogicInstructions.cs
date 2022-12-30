namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte BIT_ZERO(IBUS bus, IRegisters registers)
        {
            var value = AddressingZeroWithValue(bus, registers).Value;

            var isNegative = (value & 0x80) == 0x80;
            registers.SetNegativeFlag(isNegative);

            var overflow = (value & 0x40) == 0x40;
            registers.SetOverflowFlag(overflow);

            var isZero = (value & registers.A) == 0;
            registers.SetZeroFlag(isZero);

            return 3;
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
            var mask = AddressingIndirectXWithValue(bus, registers).Value;
            AND(mask, registers);

            return 6;
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

        private static byte ORA_IND_X(IBUS bus, IRegisters registers)
        {
            var mask = AddressingIndirectXWithValue(bus, registers).Value;
            ORA(mask, registers);

            return 6;
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
            var mask = AddressingIndirectXWithValue(bus, registers).Value;
            EOR(mask, registers);
            return 6;
        }
    }
}
