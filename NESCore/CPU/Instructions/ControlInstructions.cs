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
            
            var low = bus.Read(0xFFFE);
            var high = bus.Read(0xFFFF);
            registers.PC = (ushort)((high << 8) | low);
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

            var low = bus.Read((ushort)((th << 8) | tl));
            var high = bus.Read((ushort)((th << 8) | ((tl+1)&0xff))); //This is done this way because 6502 doesn't increment the address correctly on page boundaries

            var address = (ushort)((high << 8) | low);

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
            bus.Read(registers.PC);
            bus.Read((ushort)(0x100 | registers.SP));
            
            registers.STATUS = Pop8(bus, registers);;
            registers.PC = Pop16(bus, registers);

            return 6;
        }

        private static byte RTS(IBUS bus, IRegisters registers)
        {
            bus.Read(registers.PC);
            bus.Read((ushort)(0x100 | registers.SP));
            
            var address = Pop16(bus, registers);
            bus.Read(address);
            registers.PC = (ushort)(address + 1);

            return 6;
        }
    }
}
