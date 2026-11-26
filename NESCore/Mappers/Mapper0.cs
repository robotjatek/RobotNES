using NESCore.Cartridge;
using Serilog;

namespace NESCore.Mappers
{
    public class Mapper0 : IMapper
    {
        public int Id => 0;
        
        private readonly SlicedRom _prgRom;

        private readonly ILogger _logger;

        public RomBase Chr { get; }

        public RomBase PrgRam { get; }

        public byte ChrRomNumber { get; }

        public int ChrRomSize { get; }
        
        private readonly ICartridgeHeader _cartridgeHeader;

        public Mapper0(SlicedRom prgRoms, List<ChrBase> chr, ILogger logger, ICartridgeHeader cartridgeHeader)
        {
            _cartridgeHeader = cartridgeHeader;
            ChrRomNumber = (byte)chr.Count;
            ChrRomSize = ChrRomNumber * 1024 * 8;
            _prgRom = prgRoms;

            if (_prgRom.NumberOfBanks is > 2 or 0)
            {
                throw new ArgumentException("Invalid number of ROMs");
            }

            Chr = chr[0];
            PrgRam = new Ram(8);
            
            prgRoms.Map(0, 0);

            if (prgRoms.NumberOfBanks == 1)
            {
                prgRoms.Map(1, 0);
            }

            _logger = logger;
        }

        public bool TryRead(ushort address, out byte value)
        {
            switch (address)
            {
                case >= 0x8000:
                    value = _prgRom.Read((ushort)(address - 0x8000));
                    return true;
                case >= 0x6000:
                    value = PrgRam.Read((ushort)(address - 0x6000));
                    return true;
                default:
                    value = 0;
                    return false;
            }
        }

        public void Write(ushort address, byte value)
        {
            if (address is >= 0x6000 and < 0x8000)
            {
                PrgRam.Write((ushort)(address - 0x6000), value);
            }
            else
            {
                _logger.Warning("Unsupported write to ROM area. ");
            }
        }

        public IMapper.MirroringMode GetMirroring()
        {
            if (_cartridgeHeader.FLAGS.FourScreenMirroring == FOUR_SCREEN_MIRRORING.ENABLED)
            {
                return IMapper.MirroringMode.FourScreen;
            }

            return _cartridgeHeader.FLAGS.Mirroring switch
            {
                MIRRORING.HORIZONTAL => IMapper.MirroringMode.Horizontal,
                MIRRORING.VERTICAL => IMapper.MirroringMode.Vertical,
                _ => throw new Exception("Unsupported mirroring mode.")
            };
        }
    }
}