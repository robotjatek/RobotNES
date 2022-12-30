namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte INC_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroWithValue(bus, registers);
            var result = (byte)(addressingResult.Value + 1);
            registers.SetZeroFlag(result == 0);
            registers.SetNegativeFlag((result & 0x80) > 0);
            bus.Write(addressingResult.Address, result);
            return 5;
        }

        private static byte INX(IBUS bus, IRegisters registers)
        {
            registers.X++;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
            return 2;
        }

        private static byte INY(IBUS bus, IRegisters registers)
        {
            registers.Y++;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
            return 2;
        }

        private static byte DEC_ZERO(IBUS bus, IRegisters registers)
        {
            var addressingResult = AddressingZeroWithValue(bus, registers);
            var result = (byte)(addressingResult.Value - 1);
            registers.SetZeroFlag(result == 0);
            registers.SetNegativeFlag((result & 0x80) > 0);
            bus.Write(addressingResult.Address, result);
            return 5;
        }

        private static byte DEX(IBUS bus, IRegisters registers)
        {
            registers.X--;
            registers.SetZeroFlag(registers.X == 0);
            registers.SetNegativeFlag((registers.X & 0x80) > 0);
            return 2;
        }

        private static byte DEY(IBUS bus, IRegisters registers)
        {
            registers.Y--;
            registers.SetZeroFlag(registers.Y == 0);
            registers.SetNegativeFlag((registers.Y & 0x80) > 0);
            return 2;
        }
    }
}
