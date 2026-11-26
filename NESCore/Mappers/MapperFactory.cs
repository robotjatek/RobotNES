using NESCore.Cartridge;
using NESCore.Mappers.MMC1;
using Serilog;

namespace NESCore.Mappers
{
    public interface IMapperFactory
    {
        IMapper CreateMapper(ICartridgeHeader header, byte[] prgRoms, byte[] chrRoms, Func<IBUS> getBus);
    }

    public class MapperFactory(ILogger logger) : IMapperFactory
    {
        public IMapper CreateMapper(ICartridgeHeader header, byte[] prgRoms, byte[] chrRoms, Func<IBUS> getBus)
        {
            logger.Information("Creating mapper {HeaderMapperId}", header.MapperID);

            switch (header.MapperID)
            {
                case 0:
                {
                    var chr = CreateChr(chrRoms);
                    var prg = new SlicedRom(16, prgRoms, true);
                    return new Mapper0(prg, chr, logger, header);
                }
                case 1:
                {
                    var chr = chrRoms.Length == 0 ? new SlicedRom(4, new byte[8*1024], false)
                        : new SlicedRom(4, chrRoms, true);
                    var prg = new SlicedRom(16, prgRoms, true);
                    return new Mmc1(prg, chr, getBus, logger);
                }
                case 2:
                {
                    // I don't think unrom should support chr ram
                    var chr = CreateChr(chrRoms);
                    var prg = new SlicedRom(16, prgRoms, true);
                    return new UnRom(prg, chr, logger, header);
                }
                case 3:
                {
                    var chr = CreateChr(chrRoms);
                    var prg = new SlicedRom(16, prgRoms, true);
                    return new CnRom(prg, chr, logger, header);
                }
                case 7:
                {
                    // BattleToads: NES2.0 8k CHR-RAM size: 8192
                    var chr = CreateChr(chrRoms);
                    var prg = new SlicedRom(16, prgRoms, true);
                    return new AoRom(prg, chr, logger);
                }
                default:
                    throw new UnknownMapperException(header.MapperID);
            }
        }

        private SlicedRom CreateChr(byte[] rawBytes)
        {
            return rawBytes.Length == 0 ? new SlicedRom(8, new byte[8*1024], false) : new SlicedRom(8, rawBytes, true);
        }
    }
}