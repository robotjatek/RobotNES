namespace NESCore.CPU.Instructions
{
    public partial class CpuInstructions
    {
        private static void PHA(IBUS bus, IRegisters registers)
        {
            bus.Read(registers.PC); 
            Push8(bus, registers, registers.A);
        }

        private static void PHP(IBUS bus, IRegisters registers)
        {
            bus.Read(registers.PC); 
            Push8(bus, registers, (byte)(registers.STATUS | FlagPositions.BREAK | 0x20));
        }
        
        private static void PLA(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            bus.Read((ushort)(0x100 | registers.SP));
            var value = Pop8(bus, registers);
            registers.A = value;
            registers.SetNegativeFlag((sbyte)value < 0);
            registers.SetZeroFlag(value == 0);
        }

        private static void PLP(IBUS bus, IRegisters registers)
        {
            IdleCycle(bus, registers);
            bus.Read((ushort)(0x100 | registers.SP));
            var value = (byte)(Pop8(bus, registers) & ~FlagPositions.BREAK);
            registers.STATUS = value;
        }
    }
}
