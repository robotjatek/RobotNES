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

        private static byte JMP_INDIRECT(IBUS bus, IRegisters registers)
        {
            var tl = Fetch(bus, registers);
            var th = Fetch(bus, registers);

            byte low = bus.Read((UInt16)((th << 8) | tl));
            byte high = bus.Read((UInt16)((th << 8) | ((tl+1)&0xff))); //This is done this way because 6502 doesn't increment the address correctly on page boundaries

            var address = (UInt16)((high << 8) | low);

            registers.PC = address;
            return 5;
        }

        private static byte JSR_ABS(IBUS bus, IRegisters registers)
        {
            var address = Fetch16(bus, registers);

            Push16(bus, registers, (ushort)(registers.PC - 1));
            registers.PC = address;

            return 6;
        }

        private static byte RTI(IBUS bus, IRegisters registers)
        {
            registers.STATUS = Pop8(bus, registers);
            registers.PC = Pop16(bus, registers);

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
