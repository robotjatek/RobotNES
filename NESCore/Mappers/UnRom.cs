// using NESCore.Cartridge;
// using Serilog;
//
// namespace NESCore.Mappers;
// /// <summary>
// /// UxROM/UNROM
// /// Mapper 002
// /// Ducktales, Contra, Metal Gear, Mega Man, Castlevania
// /// https://www.nesdev.org/wiki/UxROM
// /// 2 banks, lower is switchable
// ///  0x8000-0xBFFF: 16k switchable
// ///  0xC000-0xFFFF: 16k fixed to the last bank
// ///
// /// Bank select register: 0x8000-0xFFFF write low 3 or 4 bits
// /// </summary>
// public class UnRom : IMapper
// {
//     public int Id => 2;
//     public byte PrgRomNumber { get; }
//     public int PrgRomSize { get; }
//     public byte ChrRomNumber { get; }
//     public int ChrRomSize { get; }
//     public RomBase Chr { get; }
//     public RomBase PrgRam { get; }
//     private readonly ILogger _logger;
//     private readonly ICartridgeHeader _cartridgeHeader;
//
//     private readonly List<Rom> _prgRoms;
//     private Rom _lowerBank; // switchable 0x8000-0xBFFF
//     private readonly Rom _upperBank; // fixed 0xC000-0xFFFF, Should be initialized to the LAST ROM
//
//     public UnRom(List<Rom> prgRoms, List<ChrBase> chr, ILogger logger,  ICartridgeHeader cartridgeHeader)
//     {
//         _cartridgeHeader = cartridgeHeader;
//         _logger = logger;
//         _prgRoms = prgRoms;
//         PrgRomNumber =  (byte)prgRoms.Count; // window 16k + 16k fixed
//         PrgRomSize = PrgRomNumber * 1024 * 16;
//         ChrRomNumber = (byte)chr.Count;
//         ChrRomSize = ChrRomNumber * 1024 * 8; // 8k fix
//         
//         Chr = chr[0];
//         PrgRam = new Ram(8); // According to wiki this should be 0, but keep it safe
//         _lowerBank = prgRoms[0];
//         _upperBank = prgRoms[^1];
//     }
//     
//     
//     public bool TryRead(ushort address, out byte value)
//     {
//         if (address is >= 0x8000 and <= 0xBFFF)
//         {
//             value = _lowerBank.Read((ushort)(address - 0x8000));
//             return true;
//         }
//         else if  (address >= 0xC000)
//         {
//             value = _upperBank.Read((ushort)(address -  0xC000));
//             return true;
//         }
//         else if (address is >= 0x6000 and < 0x8000)
//         {
//             value = PrgRam.Read((ushort)(address - 0x6000));
//             return true;
//         }
//
//         value = 0;
//         return false;
//     }
//
//     public void Write(ushort address, byte value)
//     {
//         if (address is >= 0x6000 and < 0x8000)
//         {
//             _logger.Warning("Write to UnROM ram. UnRom shouldn't have RAM isn't it? ({Address:X4} 0x{Value})", address, value);
//             PrgRam.Write((ushort)(address - 0x6000), value);
//         }
//         else if (address >= 0x8000)
//         {
//             // bank switch
//             // NesDev wiki: Emulator implementations of iNES mapper 2 treat this as a full 8-bit bank select register,
//             // without bus conflicts. This allows the mapper to be used for similar boards that are compatible.
//             var selectedBank = value % _prgRoms.Count;
//             _lowerBank = _prgRoms[selectedBank];
//         }
//         else
//         {
//             _logger.Warning("Unsupported write to UNROM ROM area 0x{Address:X4} 0x{Value:X}", address, value);
//         }
//     }
//     
//     public IMapper.MirroringMode GetMirroring()
//     {
//         if (_cartridgeHeader.FLAGS.FourScreenMirroring == FOUR_SCREEN_MIRRORING.ENABLED)
//         {
//             return IMapper.MirroringMode.FourScreen;
//         }
//
//         return _cartridgeHeader.FLAGS.Mirroring switch
//         {
//             MIRRORING.HORIZONTAL => IMapper.MirroringMode.Horizontal,
//             MIRRORING.VERTICAL => IMapper.MirroringMode.Vertical,
//             _ => throw new Exception("Unsupported mirroring mode.")
//         };
//     }
// }