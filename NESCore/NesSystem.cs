﻿using NESCore.Cartridge;
using NESCore.CPU;
using NESCore.CPU.Instructions;
using NESCore.Mappers;
using NESCore.PPU;

using Serilog;

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

        public NesSystem(string cartridgePath, ILogger logger)
        {
            _logger = logger;
            _cartridge = LoadCartridge(cartridgePath); //TODO: move cartridge load out of this class
            _memory = new Memory();
            _ppu = new PPU.PPU(new PPURegisters(), _logger);
            _bus = new Bus(_cartridge, _memory, _ppu, _logger);

            //TODO: Move this CPU creation block to a factory
            var instructions = new CPUInstructions().InstructionSet;
            _cpu = new CPU.CPU(_bus, instructions, _logger);
            _ppu.NMIEvent += _cpu.HandleNMI;
        }

        public void Run()
        {
            _running = true;
            while (_running)
            {
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

        //Runs one internal cycle
        private void Cycle()
        {
            //TODO: catch-up mechanism instead: run the cpu up until the ppu is accessed or a vblank happens then catch up with the ppu (run the cpu in a per instruction basis instead of a per cycle basis)
            unchecked
            {
                _cycle++;
            }

            _cpu.Cycle();
        }

        private ICartridge LoadCartridge(string path)
        {
            var parser = new CartridgeParser(new MapperFactory(_logger));
            return parser.LoadCartridge(path);
        }
    }
}
