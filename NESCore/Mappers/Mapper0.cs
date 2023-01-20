using Serilog;

namespace NESCore.Mappers
{
    public class Mapper0 : IMapper
    {
        public int ID => 0;

        public byte PRGRomNumber { get; private set; }

        public int PRGRomSize { get; private set; }

        private readonly ROM _lowerBank;

        private readonly ROM _upperBank;

        private readonly ILogger _logger;

        public VROM CHRROM { get; private set; }

        public byte CHRROMNumber { get; private set; }

        public int CHRROMSize { get; private set; }

        public Mapper0(List<ROM> prgRoms, List<VROM> chrROMS, ILogger logger)
        {
            PRGRomNumber = (byte)prgRoms.Count;
            PRGRomSize = PRGRomNumber * 1024 * 16;
            CHRROMNumber = (byte)chrROMS.Count;
            CHRROMSize = CHRROMNumber * 1024 * 8;

            CHRROM = chrROMS[0];

            if (PRGRomNumber > 2 || PRGRomNumber == 0)
            {
                throw new ArgumentException("Invalid number of ROMs");
            }

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

        public byte Read(UInt16 address)
        {
            if (address >= 0x8000 && address <= 0xBFFF)
            {
                return _lowerBank.Read((UInt16)(address - 0x8000));
            }
            else if (address >= 0xC000)
            {
                return _upperBank.Read((UInt16)(address - 0xC000));
            }

            //TODO: PRG RAM read

            throw new NotImplementedException($"Unsupported read from address {address:X}");
        }

        public void Write(UInt16 address, byte value)
        {
            if (address < 0x8000)
            {
                //TODO: possible PRG RAM writes
                throw new NotImplementedException();
            }

            _logger.Warning("Unsupported write to ROM area. ");
        }
    }
}
