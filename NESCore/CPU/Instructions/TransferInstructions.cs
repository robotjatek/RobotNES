namespace NESCore.CPU.Instructions
{
    public partial class CpuInstructions
    {
        private static void TAX(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.X = registers.A;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
        }

        private static void TSX(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.X = registers.SP;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
        }

        private static void TAY(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.Y = registers.A;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
        }

        private static void TXA(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.A = registers.X;
            registers.SetZeroFlag(registers.A == 0);
            registers.SetNegativeFlag((registers.A & 0x80) > 0);
        }

        private static void TYA(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.A = registers.Y;
            registers.SetZeroFlag(registers.A == 0);
            registers.SetNegativeFlag((registers.A & 0x80) > 0);
        }

        private static void TXS(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SP = registers.X;
        }
    }
}
