namespace NESCore.CPU
{
    public class CPU : ICPU
    {
        private readonly IBUS _bus;
        private readonly IRegisters _registers = new Registers();
        private readonly Func<IBUS, IRegisters, byte>[] _instructions;
        private readonly System.IO.StreamWriter _wr = new StreamWriter("instlog.txt");

        public CPU(IBUS bus, Func<IBUS, IRegisters, byte>[] instuctions)
        {
            _bus = bus;
            _instructions = instuctions;
            _registers.PC = (UInt16)(_bus.Read(0xfffd) << 8 | _bus.Read(0xfffc));
        }

        public void Cycle()
        {
            throw new NotImplementedException();
        }

        public uint RunInstruction()
        {
            var instructionCode = Fetch();
            var instruction = _instructions[instructionCode];

            if (instruction == null)
            {
                _wr.Close();
                throw new NotImplementedException($"0x{instructionCode:X}@0x{_registers.PC - 1:X2}");
            }

            _wr.WriteLine($"{_registers.PC - 1:X4} {instructionCode:X2}");
            var elapsedCycles = instruction(_bus, _registers);
            return elapsedCycles;
        }

        private byte Fetch()
        {
            return _bus.Read(_registers.PC++);
        }
    }
}