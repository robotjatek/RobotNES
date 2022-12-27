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

        private static byte AND_IMM(IBUS bus, IRegisters registers)
        {
            var mask = Fetch(bus, registers);
            var value = (byte)(registers.A & mask);
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
            return 2;
        }

        private static byte ORA_IMM(IBUS bus, IRegisters registers)
        {
            var mask = Fetch(bus, registers);
            var value = (byte)(registers.A | mask);
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
            return 2;
        }

        private static byte EOR_IMM(IBUS bus, IRegisters registers)
        {
            var mask = Fetch(bus, registers);
            var value = (byte)(registers.A ^ mask);
            registers.A = value;
            registers.SetZeroFlag(value == 0);
            registers.SetNegativeFlag((sbyte)value < 0);
            return 2;
        }
    }
}
