using NESCore.APU;
using NESCore.Cartridge;
using NESCore.CPU;
using NESCore.CPU.Instructions;
using NESCore.Mappers;
using NESCore.PPU;

using Serilog;

//TODO: output fake noise from ppu
//TODO: cycle correct run: (3 ppu.cycle, 1 cpu.cycle [cpu: 1st cycle: fetch intruction code&determine length, execute instruction on the last cycle only])

namespace NESCore
{
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
        private readonly IController _contoller1;
        private readonly IController _contoller2;
        private readonly IAPU _apu;

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
            _ppu.NMIEvent += _cpu.HandleNMI;
            _bus.OAMDMAEvent += _cpu.HandleDMA;
        }

        public void Run()
        {
            _running = true;
            while (_running)
            {
                //TODO: catch-up mechanism instead: run the cpu up until the ppu is accessed or a vblank happens then catch up with the ppu (run the cpu in a per instruction basis instead of a per cycle basis)
                _cycle += _cpu.RunInstruction();
                if(_cycle > 133) //TODO: this is a temporary hack to get the ppu running
                {
                    _ppu.Run(133 * 3);
                    _cycle = 0;
                }
            }
        }

        public void Stop()
        {
            _running = false;
        }

        private ICartridge LoadCartridge(string path)
        {
            var parser = new CartridgeParser(new MapperFactory(_logger));
            return parser.LoadCartridge(path);
        }
    }
}
