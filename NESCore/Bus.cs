using NESCore.Cartridge;
using NESCore.PPU;

using Serilog;

namespace NESCore
{
    public class Bus : IBUS
    {
        private readonly ICartridge _cartridge;
        private readonly IMemory _memory;
        private readonly IPPU _ppu;
        private readonly ILogger _logger;

        //TODO: APU (0x4000-0x4013)
        //TODO: OAM DMA (0x4014)
        //TODO: Sound channel enable (0x4015)
        //TODO: JOY1 (0x4016)
        //TODO: JOY2 (0x4017)

        public Bus(ICartridge cartridge, IMemory memory, IPPU ppu, ILogger logger)
        {
            _logger = logger;
            _cartridge = cartridge;
            _memory = memory;
            _ppu = ppu;
        }

        //TODO: 0x6000-0x7FFF cartridge ram (battery ram/mapper dependent ram)
        //TODO: 0x4000-0x4020 APU IO memory
        public byte Read(ushort address)
        {
            if (IsInInternalMemory(address))
            {
                return InternalMemoryRead(address);
            }
            else if(IsInCartridgeArea(address))
            {
                return _cartridge.Read(address);
            }
            else if(IsInPPUArea(address))
            {
                return PPURead(address);
            }

            _logger.Warning($"Unsupported read from 0x{address:X4}");
            throw new NotImplementedException();
        }

        public void Write(ushort address, byte data)
        {
            if (IsInInternalMemory(address))
            {
               InternalMemoryWrite(address, data);
            }
            else if(IsInCartridgeArea(address))
            {
                _cartridge.Write(address, data);
            }
            else if(IsInPPUArea(address))
            {
                PPUWrite(address, data);
            }
            else
            {
                _logger.Warning($"Unsupported write to 0x{address:X4}");
                throw new NotImplementedException();
            }
        }

        private static bool IsInInternalMemory(UInt16 address)
        {
            return address >= 0 && address <= 0x1fff;
        }

        private static bool IsInCartridgeArea(UInt16 address)
        {
            return address >= 0x4020 && address <= 0xFFFF;
        }

        private static bool IsInPPUArea(UInt16 address)
        {
            return address >= 0x2000 && address <= 0x3FFF;
        }

        private byte InternalMemoryRead(UInt16 address)
        {
            var mirroredAddress = address &= 0x7ff;
            if (address != mirroredAddress)
            {
                _logger.Warning($"Mirrored internal memory address accessed: {address} => {mirroredAddress}");
            }

            return _memory.Read(mirroredAddress);
        }

        private void InternalMemoryWrite(UInt16 address, byte data)
        {
            var mirroredAddress = address &= 0x7ff;
            if (address != mirroredAddress)
            {
                _logger.Warning($"Mirrored internal memory address accessed: {address} => {mirroredAddress}");
            }
            _memory.Write(mirroredAddress, data);
        }

        private byte PPURead(UInt16 address)
        {
            var mirroredAddress = address &= 0x2007;
            if(address != mirroredAddress)
            {
                _logger.Warning($"Mirrored ppu address accessed: {address} => {mirroredAddress}");
            }
            return _ppu.Read(address);
        }

        private void PPUWrite(UInt16 address, byte data)
        {
            var mirroredAddress = address &= 0x2007;
            if (address != mirroredAddress)
            {
                _logger.Warning($"Mirrored ppu address accessed: {address} => {mirroredAddress}");
            }
            _ppu.Write(mirroredAddress, data);
        }
    }
}