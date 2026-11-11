namespace NESCore.CPU.Instructions
{
    public partial class CpuInstructions
    {
        private static void CLC(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetCarryFlag(false);
        }

        private static void CLD(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetDecimalFlag(false);
        }

        private static void CLV(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetOverflowFlag(false);
        }

        private static void CLI(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetInterruptDisableFlag(false);
        }

        private static void SEC(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetCarryFlag(true);
        }

        private static void SED(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetDecimalFlag(true);
        }

        private static void SEI(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetInterruptDisableFlag(true);
        }
    }
}
