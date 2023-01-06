namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte NOP(IBUS bus, IRegisters registers)
        {
            return 2;
        }

        private static byte NOP_IMM(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingImmediate(bus, registers);
            return addressingResult.Cycles;
        }

        private static byte NOP_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZero(bus, registers);
            return addressingResult.Cycles;
        }

        private static byte NOP_ZERO_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroX(bus, registers);
            return addressingResult.Cycles;
        }

        private static byte NOP_ABS(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsolute(bus, registers);
            return addressingResult.Cycles;
        }

        private static byte NOP_ABS_X(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingAbsoluteX(bus, registers);
            return addressingResult.Cycles;
        }
    }
}
