using Serilog;

namespace NESCore.Mappers
{
    public class Mapper0 : IMapper
    {
        public int ID => 0;

        public byte PRGRomNumber { get; private set; }

        public int PRGRomSize { get; private set; }

        private readonly Rom _lowerBank;

        private readonly Rom _upperBank;

        private readonly ILogger _logger;

        public RomBase Chr { get; private set; }

        public RomBase PrgRam { get; private set; }

        public byte CHRROMNumber { get; private set; }

        public int CHRROMSize { get; private set; }

        public Mapper0(List<Rom> prgRoms, List<ChrBase> chr, ILogger logger)
        {
            PRGRomNumber = (byte)prgRoms.Count;
            PRGRomSize = PRGRomNumber * 1024 * 16;
            CHRROMNumber = (byte)chr.Count;
            CHRROMSize = CHRROMNumber * 1024 * 8;

            if (PRGRomNumber > 2 || PRGRomNumber == 0)
            {
                throw new ArgumentException("Invalid number of ROMs");
            }

            Chr = chr[0];
            PrgRam = new Ram(8);
            _lowerBank = prgRoms[0];

            if (PRGRomNumber > 1)
            {
                _upperBank = prgRoms[1];
            }
            else
            {
                _upperBank = prgRoms[0];
            }

            _logger = logger;
        }

        public bool TryRead(ushort address, out byte value)
        {
            if (address is >= 0x8000 and <= 0xBFFF)
            {
                value = _lowerBank.Read((ushort)(address - 0x8000));
                return true;
            }
            else if (address >= 0xC000)
            {
                value = _upperBank.Read((ushort)(address - 0xC000));
                return true;
            }
            else if (address is >= 0x6000 and < 0x8000)
            {
                value = PrgRam.Read((ushort)(address - 0x6000));
                return true;
            }

            value = 0;
            return false;
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
    }
}