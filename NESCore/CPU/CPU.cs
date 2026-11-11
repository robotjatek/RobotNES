using Serilog;
using Serilog.Events;

namespace NESCore.CPU
{
    public class Cpu : ICpu
    {
        private readonly IBUS _bus;
        private readonly IRegisters _registers;
        private readonly IReadOnlyList<Func<IBUS, IRegisters, byte>> _instructions;
        private readonly ILogger _logger;
        private int _cycles;
        private bool _nmi;

        private bool _dma;
        private byte _dmaCpuPage;

        public Cpu(IBUS bus, IRegisters registers, IReadOnlyList<Func<IBUS, IRegisters, byte>> instructions,
            ILogger logger)
        {
            _logger = logger;
            _bus = bus;
            _registers = registers;
            _instructions = instructions;
            _registers.PC = (ushort)(_bus.Read(0xfffd) << 8 | _bus.Read(0xfffc));
        }

        public void HandleNmi()
        {
            _nmi = true;
        }

        public void HandleDma(byte cpuPage)
        {
            _dma = true;
            _dmaCpuPage = cpuPage;
        }

        public uint RunInstruction()
        {
            if (_nmi)
            {
                return DoNmi();
            }

            if (_dma)
            {
                return DoDma(_dmaCpuPage);
            }

            var instructionCode = Fetch();
            var instruction = _instructions[instructionCode];

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                _logger.Debug("{RegistersPc:X4} {InstructionCode:X2}", _registers.PC - 1, instructionCode);
            }

            var instructionCycles = instruction(_bus, _registers);
            _cycles += instructionCycles;

            return instructionCycles;
        }

        private byte Fetch()
        {
            return _bus.Read(_registers.PC++);
        }

        private void Push16(ushort value)
        {
            var pcLow = (byte)(value & 0xff);
            var pcHigh = (byte)((value & 0xff00) >> 8);

            Push8(pcHigh);
            Push8(pcLow);
        }

        private void Push8(byte value)
        {
            _bus.Write((ushort)(0x100 | _registers.SP--), value);
        }

        private byte DoNmi()
        {
            _bus.Read(_registers.PC);
            _bus.Read(_registers.PC);
            
            Push16(_registers.PC);
            Push8(_registers.STATUS);
            _registers.SetInterruptDisableFlag(true);
            var addressLow = (ushort)_bus.Read(0xfffa);
            var addressHigh = (ushort)(_bus.Read(0xfffb) << 8);
            _registers.PC = (ushort)(addressHigh | addressLow);
            _nmi = false;
            return 7;
        }

        private uint DoDma(byte cpuPage)
        {
            _dma = false;
            _bus.Read(_registers.PC);
            if (_cycles % 2 == 1)
            {
                _bus.Read(_registers.PC);
            }
            
            var address = (ushort)(cpuPage << 8);
            for (var i = address; i < address + 0x100; i++)
            {
                _bus.PPUOamDMA(_bus.Read(i));
            }

            var penalty = _cycles % 2 == 1 ? 1 : 0;
            return (uint)(513 + penalty);
        }
    }
}