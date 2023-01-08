using Serilog;

namespace NESCore.CPU
{
    public class CPU : ICPU
    {
        private readonly IBUS _bus;
        private readonly IRegisters _registers = new Registers();
        private readonly IReadOnlyList<Func<IBUS, IRegisters, byte>> _instructions;
        private readonly ILogger _logger;

        public CPU(IBUS bus, IReadOnlyList<Func<IBUS, IRegisters, byte>> instuctions, ILogger logger)
        {
            _logger = logger;
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

            _logger.Debug($"{_registers.PC - 1:X4} {instructionCode:X2}");

            var elapsedCycles = instruction(_bus, _registers);
            return elapsedCycles;
        }

        private byte Fetch()
        {
            return _bus.Read(_registers.PC++);
        }
    }
}