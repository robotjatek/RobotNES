namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte INX(IBUS bus, IRegisters registers)
        {
            registers.X++;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
            return 2;
        }

        private static byte INY(IBUS bus, IRegisters registers)
        {
            registers.Y++;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
            return 2;
        }

        private static byte DEX(IBUS bus, IRegisters registers)
        {
            registers.X--;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
            return 2;
        }

        private static byte DEY(IBUS bus, IRegisters registers)
        {
            registers.Y--;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
            return 2;
        }
    }
}
