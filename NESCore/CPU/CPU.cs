using Serilog;

namespace NESCore.CPU
{
    public class CPU : ICPU
    {
        private readonly IBUS _bus;
        private readonly IRegisters _registers = new Registers();
        private readonly IReadOnlyList<Func<IBUS, IRegisters, byte>> _instructions;
        private readonly ILogger _logger;
        private bool _nmi = false;

        public CPU(IBUS bus, IReadOnlyList<Func<IBUS, IRegisters, byte>> instuctions, ILogger logger)
        {
            _logger = logger;
            _bus = bus;
            _instructions = instuctions;
            _registers.PC = (UInt16)(_bus.Read(0xfffd) << 8 | _bus.Read(0xfffc));
        }

        public void HandleNMI()
        {
            _nmi = true;
        }

        public uint RunInstruction()
        {
            if(_nmi)
            {
                return DoNMI();
            }

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
    }
}