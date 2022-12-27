namespace NESCore.CPU.Instructions
{
    public partial class CPUInstructions
    {
        private static byte BCC(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetCarryFlag() == false;
            });
        }

        private static byte BEQ(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetZeroFlag() == true;
            });
        }

        private static byte BNE(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetZeroFlag() == false;
            });
        }

        private static byte BCS(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetCarryFlag() == true;
            });
        }

        private static byte BVC(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetOverflowFlag() == false;
            });
        }

        private static byte BVS(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetOverflowFlag() == true;
            });
        }

        private static byte BPL(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetNegativeFlag() == false;
            });
        }

        private static byte BMI(IBUS bus, IRegisters registers)
        {
            return BranchInstruction(bus, registers, (r) =>
            {
                return r.GetNegativeFlag() == true;
            });
        }

        private static byte BranchInstruction(IBUS bus, IRegisters registers, Func<IRegisters, bool> condition)
        {
            byte cycles = 2;
            sbyte offset = (sbyte)Fetch(bus, registers);
            if (condition(registers))
            {
                var oldAddress = registers.PC;
                registers.PC = (ushort)(registers.PC + offset);
                cycles++;
                if ((oldAddress & 0xff00) != (registers.PC & 0xff00))
                {
                    cycles++;
                }
            }

            return cycles;
        }
    }
}
