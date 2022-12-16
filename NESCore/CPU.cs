namespace NESCore
{
    public interface IRegisters
    {
        UInt16 PC { get; set; }
        byte A { get; set; }
        byte X { get; set; }
        byte Y { get; set; }
        byte STATUS { get; set; }
        byte SP { get; set; }
    }

    public class Registers : IRegisters
    {
        public UInt16 PC { get; set; } = 0;

        public byte A { get; set; } = 0;

        public byte X { get; set; } = 0;

        public byte Y { get; set; } = 0;

        public byte STATUS { get; set; } = 0;

        public byte SP { get; set; } = 0; 
    }

    public class CPU : ICPU
    {
        private readonly IBUS _bus;
        private readonly IRegisters _registers;
        private readonly Func<IBUS, IRegisters, byte>[] _instructions;

        public CPU(IBUS bus, IRegisters registers, Func<IBUS, IRegisters, byte>[] instuctions)
        {
            _bus = bus;
            _registers = registers;
            _instructions = instuctions;
            //_registers.PC = (UInt16)(_bus.Read(0xfffd) << 8 | _bus.Read(0xfffc)); // Reset vector //TODO: uncomment this after instructionset implementation
            _registers.PC = 0xc000; //nestest.nes start //TODO: delete this after instructionset implementation
        }

        public void Cycle()
        {
            throw new NotImplementedException();
        }

        public uint RunInstruction()
        {
            var instructionCode = Fetch();
            var instruction = _instructions[instructionCode];

            if(instruction == null)
            {
                throw new NotImplementedException($"{instructionCode:X}");
            }

            var elapsedCycles = instruction(_bus, _registers);
            return elapsedCycles;
        }

        private byte Fetch()
        {
            return _bus.Read(_registers.PC);
        }
    }
}