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
        
        private readonly BusData _busData = new();
        
        private class BusData
        {
            public byte Value { get; private set; }

            public byte Drive(byte value)
            {
                Value = value;
                return value;
            }
        }

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
                return _busData.Drive(_cartridge.TryRead(address, out var value) ? value : _busData.Value);
            }
            else if (IsInPpuArea(address))
            {
                return PpuRead(address);
            }
            else if (address == 0x4016)
            {
                return ReadController(_controller1.ReadNextButton());
            }
            else if (address == 0x4017)
            {
                return ReadController(_controller2.ReadNextButton());
            }
            else if (IsInApuArea(address))
            {
                _apu.Read(address); // ignore apu reads
                return _busData.Value; // return openbus
            }

            _logger.Warning("Unsupported read from 0x{Address:X4}", address);
            return _busData.Value;
        }

        private byte ReadController(byte buttons)
        {
            // The shift register only drives bits 4-0. Bits 7-5 are left floating and repeat
            // whatever was on them before, which is why an idle port usually reads $40 and a
            // pressed one $41.
            return _busData.Drive((byte)((_busData.Value & 0xF8) | (buttons & 0x07)));
        }

        public void Write(ushort address, byte data)
        {
            _busData.Drive(data);
            
            if (IsInInternalMemory(address))
            {
                InternalMemoryWrite(address, data);
            }
            else if (IsInCartridgeArea(address))
            {
                _cartridge.Write(address, data);
            }
            else if (IsInPpuArea(address))
            {
                PpuWrite(address, data);
            }
            else if (address == 0x4014)
            {
                OAMDMAEvent?.Invoke(data);
            }
            else if (address == 0x4016)
            {
                _controller1.StrobeWrite(data);
                _controller2.StrobeWrite(data);
            }
            else if (address == 0x4017)
            {
                // TODO: apu
            }
            else if (IsInApuArea(address))
            {
                _apu.Write(address, data);
            }
            else
            {
                _logger.Warning("Unsupported write to 0x{Address:X4}", address);
                throw new NotImplementedException();
            }
        }
        
        public void PPUOamDMA(byte value)
        {
            _ppu.OamDMAWrite(value);
        }

        private static bool IsInInternalMemory(ushort address)
        {
            return address <= 0x1fff;
        }

        private static bool IsInCartridgeArea(ushort address)
        {
            return address >= 0x4020;
        }

        private static bool IsInPpuArea(ushort address)
        {
            return address is >= 0x2000 and <= 0x3FFF;
        }

        private static bool IsInApuArea(ushort address)
        {
            return address is >= 0x4000 and <= 0x4013 or 0x4015;
        }

        private byte InternalMemoryRead(ushort address)
        {
            var mirroredAddress = (ushort)(address & 0x7ff);
            if (address != mirroredAddress)
            {
                _logger.Warning("Mirrored internal memory address accessed: {Address} => {MirroredAddress}", address, mirroredAddress);
            }

            return _busData.Drive(_memory.Read(mirroredAddress));
        }

        private void InternalMemoryWrite(UInt16 address, byte data)
        {
            var mirroredAddress = (ushort)(address & 0x7ff);
            if (address != mirroredAddress)
            {
                _logger.Warning("Mirrored internal memory address accessed: {Address} => {MirroredAddress}", address, mirroredAddress);
            }
            _memory.Write(mirroredAddress, data);
        }

        private byte PpuRead(ushort address)
        {
            var mirroredAddress = (ushort)(0x2000 + (address & 0x0007));
            if (address != mirroredAddress)
            {
                _logger.Warning("Mirrored ppu address accessed: {Address} => {MirroredAddress}", address, mirroredAddress);
            }
            return _busData.Drive(_ppu.Read(mirroredAddress)); // TODO: tryread for ppu
        }

        private void PpuWrite(ushort address, byte data)
        {
            var mirroredAddress = (ushort)(0x2000 + (address & 0x0007));
            if (address != mirroredAddress)
            {
                _logger.Warning("Mirrored ppu address accessed: {Address} => {MirroredAddress}", address, mirroredAddress);
            }
            _ppu.Write(mirroredAddress, data);
        }
    }
}