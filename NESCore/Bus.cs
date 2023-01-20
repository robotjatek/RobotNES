using NESCore.APU;
using NESCore.Cartridge;
using NESCore.PPU;

using Serilog;

namespace NESCore
{
    public class Bus : IBUS
    {
        public event OAMDMAEventHandler? OAMDMAEvent;

        private readonly ICartridge _cartridge;
        private readonly IMemory _memory;
        private readonly IPPU _ppu;
        private readonly IController _controller1;
        private readonly IController _controller2;
        private readonly IAPU _apu;
        private readonly ILogger _logger;


        public Bus(ICartridge cartridge, IMemory memory, IPPU ppu, IController controller1, IController controller2, IAPU apu, ILogger logger)
        {
            _logger = logger;
            _cartridge = cartridge;
            _memory = memory;
            _ppu = ppu;
            _controller1 = controller1;
            _controller2 = controller2;
            _apu = apu;
        }

        public byte Read(ushort address)
        {
            if (IsInInternalMemory(address))
            {
                return InternalMemoryRead(address);
            }
            else if (IsInCartridgeArea(address))
            {
                return _cartridge.Read(address);
            }
            else if (IsInPPUArea(address))
            {
                return PPURead(address);
            }
            else if (address == 0x4016)
            {
                return _controller1.ReadNextButton();
            }
            else if (address == 0x4017)
            {
                return _controller2.ReadNextButton();
            }
            else if (IsInApuArea(address))
            {
                _apu.Read(address);
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
            else if (IsInCartridgeArea(address))
            {
                _cartridge.Write(address, data);
            }
            else if (IsInPPUArea(address))
            {
                PPUWrite(address, data);
            }
            else if (address == 0x4014)
            {
                OAMDMAEvent?.Invoke(data);
            }
            else if (address == 0x4016)
            {
                _controller1.Reset();
            }
            else if (address == 0x4017)
            {
                _controller2.Reset();
            }
            else if (IsInApuArea(address))
            {
                _apu.Write(address, data);
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

        private static bool IsInApuArea(UInt16 address)
        {
            return (address >= 0x4000 && address <= 0x4013) || address == 0x4015;
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
            if (address != mirroredAddress)
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