namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte LSR(byte value, IRegisters registers)
        {
            registers.SetCarryFlag((value & 0x1) == 1); // Set the carry flag if the last bit is 1
            registers.SetNegativeFlag(false);
            var result = (byte)(value >> 1);
            registers.SetZeroFlag(result == 0);
            return (byte)result;
        }

        private static byte LSR_A(IBUS bus, IRegisters registers)
        {
            registers.A = LSR(registers.A, registers);
            return 2;
        }

        private static byte ASL(byte value, IRegisters registers)
        {
            registers.SetCarryFlag((value & 0x80) > 0);
            var result = (byte)(value << 1);
            registers.SetNegativeFlag((result & 0x80) > 0);
            registers.SetZeroFlag(result == 0);
            return (byte)result;
        }

        private static byte ASL_A(IBUS bus, IRegisters registers)
        {
            registers.A = ASL(registers.A, registers);
            return 2;
        }
    }
}
