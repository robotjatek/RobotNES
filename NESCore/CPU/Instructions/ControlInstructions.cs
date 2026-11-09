namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte BRK(IBUS bus, IRegisters registers)
        {
            var pcToPush = registers.PC;
            bus.Read(registers.PC++);
            Push16(bus, registers, pcToPush);
            var statusToPush = (byte)(registers.STATUS | FlagPositions.BREAK | 0x20);
            Push8(bus, registers, statusToPush);
            registers.SetInterruptDisableFlag(true);
            
            registers.PC = (ushort)((bus.Read(0xFFFF) << 8) | bus.Read(0xFFFE));
            return 7;
        }

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
            var lowAddress = Fetch(bus, registers);
            bus.Read((ushort)(0x100 | registers.SP)); // dummy read
            Push16(bus, registers, registers.PC);
            var highAddress = Fetch(bus, registers);
            registers.PC = (ushort)((highAddress << 8) | lowAddress);

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
