using Serilog;

namespace NESCore.CPU
{
    public class CPU : ICPU
    {
        private readonly IBUS _bus;
        private readonly IRegisters _registers;
        private readonly IReadOnlyList<Func<IBUS, IRegisters, byte>> _instructions;
        private readonly ILogger _logger;
        private int _cycles = 0;
        private bool _nmi = false;

        private bool _dma = false;
        private byte _dmaCpuPage = 0;

        public CPU(IBUS bus, IRegisters registers, IReadOnlyList<Func<IBUS, IRegisters, byte>> instructions, ILogger logger)
        {
            _logger = logger;
            _bus = bus;
            _registers = registers;
            _instructions = instructions;
            _registers.PC = (UInt16)(_bus.Read(0xfffd) << 8 | _bus.Read(0xfffc));
        }

        public void HandleNMI()
        {
            _nmi = true;
        }

        public void HandleDMA(byte cpuPage)
        {
            _dma = true;
            _dmaCpuPage = cpuPage;
        }

        public uint RunInstruction()
        {
            if (_nmi)
            {
                return DoNMI();
            }

            if (_dma)
            {
                return DoDMA(_dmaCpuPage);
            }

            var instructionCode = Fetch();
            var instruction = _instructions[instructionCode];

            _logger.Debug($"{_registers.PC - 1:X4} {instructionCode:X2}");
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

        private byte DoNMI()
        {
            Push16(_registers.PC);
            Push8((byte)(_registers.STATUS | FlagPositions.INTERRUPT_DISABLE));
            _registers.PC = (UInt16)((_bus.Read(0xFFFB) << 8) | _bus.Read(0xFFFA));
            _nmi = false;
            return 7;
        }

        private byte DoDMA(byte cpuPage)
        {
            _dma = false;
            UInt16 address = (UInt16)(cpuPage << 8);
            for (UInt16 i = address; i < address + 0xff; i++)
            {
                _bus.Write(0x2004, _bus.Read(i));
            }

            var penalty = _cycles % 2 == 1 ? 1 : 0;
            return (byte)(513 + penalty);
        }
    }
}