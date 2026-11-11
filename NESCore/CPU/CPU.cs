using Serilog;
using Serilog.Events;

namespace NESCore.CPU
{
    public class Cpu : ICpu
    {
        private readonly IBUS _bus;
        private readonly IRegisters _registers;
        private readonly IReadOnlyList<Action<IBUS, IRegisters>> _instructions;
        private readonly ILogger _logger;
        private bool _nmi;

        private bool _dma;
        private byte _dmaCpuPage;

        public Cpu(IBUS bus, IRegisters registers, IReadOnlyList<Action<IBUS, IRegisters>> instructions,
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

        public void RunInstruction()
        {
            if (_nmi)
            {
                DoNmi();
                return;
            }

            if (_dma)
            {
                DoDma(_dmaCpuPage);
                return;
            }

            var instructionCode = Fetch();
            var instruction = _instructions[instructionCode];

            if (_logger.IsEnabled(LogEventLevel.Debug))
            {
                _logger.Debug("{RegistersPc:X4} {InstructionCode:X2}", _registers.PC - 1, instructionCode);
            }

            instruction(_bus, _registers);
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

        private void DoNmi()
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
        }

        private void DoDma(byte cpuPage)
        {
            _dma = false;
            var startedOnOdd = _bus.Cycles % 2 == 1;
            _bus.Read(_registers.PC);
            if (startedOnOdd)
            {
                _bus.Read(_registers.PC);
            }
            
            var address = (ushort)(cpuPage << 8);
            for (var i = address; i < address + 0x100; i++)
            {
                _bus.PPUOamDMA(_bus.Read(i));
            }
        }
    }
}