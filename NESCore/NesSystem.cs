using NESCore.Cartridge;
using NESCore.CPU;
using NESCore.CPU.Instructions;
using NESCore.Mappers;

using Serilog;

namespace NESCore
{
    interface IPPU { }


    public class NesSystem
    {
        private bool _running = false;
        private ulong _cycle = 0;

        private readonly ILogger _logger;
        private readonly ICartridge _cartridge;
        private readonly IBUS _bus;
        private readonly ICPU _cpu;
        private readonly IMemory _memory;
        private readonly IPPU _ppu;

        public NesSystem(string cartridgePath)
        {
            _logger = CreateLogger(); //TODO: move logger creation out of this class
            _cartridge = LoadCartridge(cartridgePath); //TODO: move cartridge load out of this class
            _memory = new Memory();
            _bus = new Bus(_cartridge, _memory, _logger);

            //TODO: Move this CPU creation block to a factory
            var instructions = new CPUInstructions().InstructionSet;
            _cpu = new CPU.CPU(_bus, instructions, _logger);
        }

        public void Run()
        {
            _running = true;
            while (_running)
            {
                //Cycle();
                _cycle += _cpu.RunInstruction();
            }
        }

        public void Stop()
        {
            _running = false;
        }

        //Runs one internal cycle
        private void Cycle()
        {
            //TODO: for the moment 1 internal cycle is 1 cpu cycle. When the PPU gets implemented it gets 1 CPU cycle = 3 PPU cycles
            //TODO: catch-up mechanism instead: run the cpu up until the ppu is accessed or a vblank happens then catch up with the ppu (run the cpu in a per instruction basis instead of a per cycle basis)
            unchecked
            {
                _cycle++;
            }

            _cpu.Cycle();
        }

        private static ILogger CreateLogger()
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo
                .Console()
                .WriteTo.File("RobotNES.log")
                .CreateLogger();

            return logger;
        }

        private ICartridge LoadCartridge(string path)
        {
            var parser = new CartridgeParser(new MapperFactory(_logger));
            return parser.LoadCartridge(path);
        }
    }
}
