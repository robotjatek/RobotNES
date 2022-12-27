namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte JMP_ABS(IBUS bus, IRegisters registers)
        {
            var address = Fetch16(bus, registers);
            registers.PC = address;

            return 3;
        }

        private static byte JSR_ABS(IBUS bus, IRegisters registers)
        {
            var address = Fetch16(bus, registers);

            Push16(bus, registers, (ushort)(registers.PC - 1));
            registers.PC = address;

            return 6;
        }

        private static byte RTS(IBUS bus, IRegisters registers)
        {
            var address = (ushort)(Pop16(bus, registers) + 1);
            registers.PC = address;

            return 6;
        }
    }
}
