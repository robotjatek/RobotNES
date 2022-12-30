namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte PHA(IBUS bus, IRegisters registers)
        {
            Push8(bus, registers, registers.A);
            return 3;
        }

        private static byte PHP(IBUS bus, IRegisters registers)
        {
            Push8(bus, registers, registers.STATUS);
            return 3;
        }

        private static byte PLA(IBUS bus, IRegisters registers)
        {
            var value = Pop8(bus, registers);
            registers.A = value;
            registers.SetNegativeFlag((sbyte)value < 0);
            registers.SetZeroFlag(value == 0);
            return 4;
        }

        private static byte PLP(IBUS bus, IRegisters registers)
        {
            var value = Pop8(bus, registers) & ~FlagPositions.BREAK;
            registers.STATUS = (byte)value;
            return 4;
        }
    }
}
