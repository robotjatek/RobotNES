namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte PHA(IBUS bus, IRegisters registers)
        {
            bus.Read(registers.PC); 
            Push8(bus, registers, registers.A);
            return 3;
        }

        private static byte PHP(IBUS bus, IRegisters registers)
        {
            bus.Read(registers.PC); 
            Push8(bus, registers, registers.STATUS);
            return 3;
        }

        private static byte PLA(IBUS bus, IRegisters registers)
        {
            bus.Read((ushort)(0x100 | registers.SP));
            var value = Pop8(bus, registers);
            registers.A = value;
            registers.SetNegativeFlag((sbyte)value < 0);
            registers.SetZeroFlag(value == 0);
            return 4;
        }

        private static byte PLP(IBUS bus, IRegisters registers)
        {
            bus.Read((ushort)(0x100 | registers.SP));
            var value = Pop8(bus, registers) & ~FlagPositions.BREAK;
            registers.STATUS = (byte)value;
            return 4;
        }
    }
}
