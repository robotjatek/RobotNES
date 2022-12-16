using NESCore.Cartridge;
using Serilog;

namespace NESCore
{
    public class Bus : IBUS
    {
        private readonly ICartridge _cartridge;
        private readonly IMemory _memory;
        private readonly ILogger _logger;

        //TODO: PPU
        //TODO: APU (0x4000-0x4020)

        public Bus(ICartridge cartridge, IMemory memory, ILogger logger)
        {
            _logger = logger;
            _cartridge = cartridge;
            _memory = memory;
        }

        //TODO: ha a felső 3 bit 0 => RAM, 001 => I/O
        //TODO: 0x6000-0x7FFF cartridge ram (battery ram/mapper dependent ram)
        //TODO: 0x2000-0x4000 (PPU) IO memory (0x2000-0x2007 [3bit]) + mirrors
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
            else
            {
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

        private byte InternalMemoryRead(UInt16 address)
        {
            var mirroredAddress = address &= 0x7ff;
            if (address != mirroredAddress)
            {
                _logger.Warning("Mirrored internal memory address accessed");
            }

            return _memory.Read(mirroredAddress);
        }

        private void InternalMemoryWrite(UInt16 address, byte data)
        {
            var mirroredAddress = address &= 0x7ff;
            if (address != mirroredAddress)
            {
                _logger.Warning("Mirrored internal memory address accessed");
            }
            _memory.Write(mirroredAddress, data);
        }
    }
}