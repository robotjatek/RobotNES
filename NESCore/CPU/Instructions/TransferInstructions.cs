namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte TAX(IBUS bus, IRegisters registers)
        {
            registers.X = registers.A;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
            return 2;
        }

        private static byte TSX(IBUS bus, IRegisters registers)
        {
            registers.X = registers.SP;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
            return 2;
        }

        private static byte TAY(IBUS bus, IRegisters registers)
        {
            registers.Y = registers.A;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
            return 2;
        }

        private static byte TXA(IBUS bus, IRegisters registers)
        {
            registers.A = registers.X;
            registers.SetZeroFlag(registers.A == 0);
            registers.SetNegativeFlag((registers.A & 0x80) > 0);
            return 2;
        }

        private static byte TYA(IBUS bus, IRegisters registers)
        {
            registers.A = registers.Y;
            registers.SetZeroFlag(registers.A == 0);
            registers.SetNegativeFlag((registers.A & 0x80) > 0);
            return 2;
        }

        private static byte TXS(IBUS bus, IRegisters registers)
        {
            registers.SP = registers.X;
            return 2;
        }
    }
}
