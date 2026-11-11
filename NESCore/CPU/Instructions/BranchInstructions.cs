namespace NESCore.CPU.Instructions
{
    public partial class CpuInstructions
    {
        private static void BCC(IBUS bus, IRegisters registers)
        {
            BranchInstruction(bus, registers, r => !r.GetCarryFlag());
        }

        private static void BEQ(IBUS bus, IRegisters registers)
        {
            BranchInstruction(bus, registers, r => r.GetZeroFlag());
        }

        private static void BNE(IBUS bus, IRegisters registers)
        {
            BranchInstruction(bus, registers, r => !r.GetZeroFlag());
        }

        private static void BCS(IBUS bus, IRegisters registers)
        {
            BranchInstruction(bus, registers, r => r.GetCarryFlag());
        }

        private static void BVC(IBUS bus, IRegisters registers)
        {
            BranchInstruction(bus, registers, r => !r.GetOverflowFlag());
        }

        private static void BVS(IBUS bus, IRegisters registers)
        {
            BranchInstruction(bus, registers, r => r.GetOverflowFlag());
        }

        private static void BPL(IBUS bus, IRegisters registers)
        {
            BranchInstruction(bus, registers, r => !r.GetNegativeFlag());
        }

        private static void BMI(IBUS bus, IRegisters registers)
        {
            BranchInstruction(bus, registers, r => r.GetNegativeFlag());
        }

        private static void BranchInstruction(IBUS bus, IRegisters registers, Func<IRegisters, bool> condition)
        {
            var offset = (sbyte)Fetch(bus, registers);
            if (condition(registers))
            {
                var oldAddress = registers.PC;
                registers.PC = (ushort)(registers.PC + offset);

                var dummyAddress = (ushort)(oldAddress & 0xff00 | (registers.PC & 0x00ff));
                bus.Read(dummyAddress);
                
                if ((oldAddress & 0xff00) != (registers.PC & 0xff00))
                {
                    bus.Read(registers.PC);
                }
            }
        }
    }
}
