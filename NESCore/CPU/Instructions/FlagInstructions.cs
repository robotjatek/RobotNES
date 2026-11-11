namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte CLC(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetCarryFlag(false);
            return 2;
        }

        private static byte CLD(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetDecimalFlag(false);
            return 2;
        }

        private static byte CLV(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetOverflowFlag(false);
            return 2;
        }

        private static byte CLI(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetInterruptDisableFlag(false);
            return 2;
        }

        private static byte SEC(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetCarryFlag(true);
            return 2;
        }

        private static byte SED(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetDecimalFlag(true);
            return 2;
        }

        private static byte SEI(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            registers.SetInterruptDisableFlag(true);
            return 2;
        }
    }
}
