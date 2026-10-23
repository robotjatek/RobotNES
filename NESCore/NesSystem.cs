using System.Diagnostics;
using NESCore.APU;
using NESCore.Cartridge;
using NESCore.CPU;
using NESCore.CPU.Instructions;
using NESCore.Mappers;
using NESCore.PPU;
using Serilog;

namespace NESCore
{
    public delegate void FrameBufferReadyEvent(byte[] frameBuffer);

    public class NesSystem
    {
        public event FrameBufferReadyEvent? FrameBufferReady;

        private volatile bool _running = false;
        private ulong _cycle = 0;

        private readonly ILogger _logger;
        private readonly ICartridge _cartridge;
        private readonly IBUS _bus;
        private readonly ICPU _cpu;
        private readonly IMemory _memory;
        private readonly IPPU _ppu;
        private readonly IController _contoller1;
        private readonly IController _contoller2;
        private readonly IAPU _apu;
        private long _nextFrameTimestamp;
        private static readonly long FrameIntervalTicks = Stopwatch.Frequency / 60;

        public NesSystem(string cartridgePath, ILogger logger)
        {
            _logger = logger;
            _cartridge = LoadCartridge(cartridgePath); //TODO: move cartridge load out of this class

            _contoller1 = new Controller(_logger);
            _contoller2 = new Controller(_logger);
            _apu = new APU.APU(_logger);

            _memory = new Memory();
            _ppu = new PPU.PPU(new PPURegisters(), new PPUMemory(_cartridge, _logger), _logger);
            _bus = new Bus(_cartridge, _memory, _ppu, _contoller1, _contoller2, _apu, _logger);

            //TODO: Move this CPU creation block to a factory
            var instructions = new CPUInstructions().InstructionSet;
            var registers = new Registers();
            _cpu = new CPU.CPU(_bus, registers, instructions, _logger);
            _bus.OAMDMAEvent += _cpu.HandleDMA;

            _ppu.NMIEvent += OnVBlank;
        }

        public void Run()
        {
            _nextFrameTimestamp = Stopwatch.GetTimestamp() + FrameIntervalTicks;
            _running = true;
            while (_running)
            {
                var cycles = _cpu.RunInstruction();
                _ppu.Run(cycles * 3);
            }
        }

        public void Stop()
        {
            _running = false;
            _logger.Information("Stopping emulation...");
        }

        public void PressControllerButton(IController.Keys key)
        {
            _contoller1.PressKey(key);
        }

        public void ReleaseControllerButton(IController.Keys key)
        {
            _contoller1.ReleaseKey(key);
        }

        private void OnVBlank(byte[] framebuffer)
        {
            _cpu.HandleNMI();
            FrameBufferReady?.Invoke(framebuffer);
            LimitFrameRate();
        }

        private ICartridge LoadCartridge(string path)
        {
            var parser = new CartridgeParser(new MapperFactory(_logger));
            return parser.LoadCartridge(path);
        }

        private void LimitFrameRate()
        {
            var remainingTicks = _nextFrameTimestamp - Stopwatch.GetTimestamp();
            if (remainingTicks > 0)
            {
                var remainingMilliseconds = (int)Math.Ceiling(remainingTicks * 1000.0 / Stopwatch.Frequency);
                Task.Delay(remainingMilliseconds).GetAwaiter().GetResult();
            }

            _nextFrameTimestamp += FrameIntervalTicks;
        }
    }
}