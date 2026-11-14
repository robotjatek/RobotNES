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
        public bool Running => _running;

        private volatile bool _running ;

        private readonly ILogger _logger;
        private readonly IBUS _bus;
        private readonly ICpu _cpu;
        private readonly IPPU _ppu;
        private readonly IController _controller1;
        private long _nextFrameTimestamp;
        private static readonly long FrameIntervalTicks = Stopwatch.Frequency / 60;

        public NesSystem(string cartridgePath, ILogger logger)
        {
            _logger = logger;
            var cartridge = LoadCartridge(cartridgePath); //TODO: move cartridge load out of this class

            _controller1 = new Controller(_logger);
            IController controller2 = new Controller(_logger);
            IApu apu = new Apu(_logger);

            IMemory memory = new Memory();
            _ppu = new Ppu(new PPURegisters(), new PpuMemory(cartridge, _logger), _logger);
            _bus = new Bus(cartridge, memory, _ppu, _controller1, controller2, apu, _logger);

            //TODO: Move this CPU creation block to a factory
            var instructions = new CpuInstructions().InstructionSet;
            var registers = new Registers();
            _cpu = new Cpu(_bus, registers, instructions, _logger);
            _bus.OamDmaEvent += _cpu.HandleDma;

            _ppu.NMIEvent += OnVBlank;
        }

        public void Run()
        {
            _nextFrameTimestamp = Stopwatch.GetTimestamp() + FrameIntervalTicks;
            _running = true;
            while (_running)
            {
                _cpu.RunInstruction();
            }
        }

        public void Stop()
        {
            _running = false;
            _logger.Information("Stopping emulation...");
        }

        public void PressControllerButton(IController.Keys key)
        {
            _controller1.PressKey(key);
        }

        public void ReleaseControllerButton(IController.Keys key)
        {
            _controller1.ReleaseKey(key);
        }

        private void OnVBlank(byte[] framebuffer)
        {
            _cpu.HandleNmi();
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