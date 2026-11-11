namespace NESCore.CPU.Instructions
{
    public partial class CpuInstructions
    {
        private static void NOP(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
        }

        private static void NOP_IMM(IBUS bus, IRegisters registers)
        {
            AddressingImmediate(bus, registers);
        }

        private static void NOP_ZERO(IBUS bus, IRegisters registers)
        {
            AddressingZero(bus, registers);
        }

        private static void NOP_ZERO_X(IBUS bus, IRegisters registers)
        { 
            AddressingZeroX(bus, registers);
        }

        private static void NOP_ABS(IBUS bus, IRegisters registers)
        {
            AddressingAbsolute(bus, registers);
        }

        private static void NOP_ABS_X(IBUS bus, IRegisters registers)
        {
            AddressingAbsoluteX(bus, registers);
        }
    }
}
