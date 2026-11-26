// using Serilog;
//
// namespace NESCore.Mappers;
//
//
// /// <summary>
// /// Notes:
// /// One nametable:
// /// Active nametable: M bit (bit 4) (0: first nametable at $2000-23ff, 1: second nametable at: $2400-27ff)
// /// PRG ROM size: up to 256 KB
// /// PRG ROM bank size: 32 KB
// /// PRG RAM: None
// /// CHR capacity: 8 KB RAM
// /// CHR bank size: Not bankswitched
// /// Nametable mirroring: Single-screen, mapper-selectable
// /// Subject to bus conflicts: AMROM/AOROM only
// ///
// /// CPU $8000-$FFFF: 32 KB switchable PRG ROM bank
// /// </summary>
// public class AoRom : IMapper
// {
//     public int Id => 7;
//     public byte PrgRomNumber { get; }
//     public int PrgRomSize { get; }
//     public byte ChrRomNumber { get; }
//     public int ChrRomSize { get; }
//     public RomBase Chr { get; }
//     public RomBase PrgRam { get; } = new Ram(8);
//
//     // TODO: merge into one "SlicedRom"
//     private Rom _lowerBank;
//     private Rom _upperBank;
//
//     private bool _mode;
//     private readonly List<Rom> _prgRoms;
//     
//     private readonly ILogger _logger;
//
//     public AoRom(List<Rom> prgRoms, List<ChrBase> chr, ILogger logger)
//     {
//         _logger = logger;
//         _prgRoms = prgRoms;
//         PrgRomNumber = (byte)prgRoms.Count;
//         PrgRomSize = PrgRomNumber * 1024 * 16;
//         ChrRomNumber = (byte)chr.Count;
//         ChrRomSize = ChrRomNumber * 1024 * 8;
//
//         Chr = chr[0];
//         _lowerBank = prgRoms[^2];
//         _upperBank = prgRoms[^1];
//     }
//     
//     public bool TryRead(ushort address, out byte value)
//     {
//         switch (address)
//         {
//             case >= 0x6000 and < 0x8000:
//                 _logger.Warning("PRG Ram read");
//                 value = PrgRam.Read((ushort)(address - 0x6000));
//                 return true;
//             case >= 0x8000 and <= 0xBFFF:
//                 value = _lowerBank.Read((ushort)(address - 0x8000));
//                 return true;
//             case >= 0xC000:
//                 value = _upperBank.Read((ushort)(address -  0xC000));
//                 return true;
//             default:
//                 value = 0;
//                 return false;
//         }
//     }
//
//     public void Write(ushort address, byte value)
//     {
//         if (address is >= 6000 and < 0x8000)
//         {
//             _logger.Warning("PRG Ram write");
//             PrgRam.Write((ushort)(address - 0x6000), value);
//         }
//         else if (address >= 0x8000)
//         {
//             // bank select register
//             _mode = (value & 0b10000) > 0;
//             
//             // switching in pairs (32k chunks)
//             var unitCount = _prgRoms.Count / 2;
//             var bankId = ((value & 0b111) % unitCount) * 2;
//             SwitchBanks(bankId);
//         }
//     }
//
//     public IMapper.MirroringMode GetMirroring()
//     {
//         return _mode ? IMapper.MirroringMode.OneScreenUpper : IMapper.MirroringMode.OneScreenLower;
//     }
//
//     private void SwitchBanks(int bankId)
//     {
//         _lowerBank = _prgRoms[bankId];
//         _upperBank = _prgRoms[bankId + 1];
//     }
// }