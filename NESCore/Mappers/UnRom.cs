using NESCore.Cartridge;
using Serilog;

namespace NESCore.Mappers;
/// <summary>
/// UxROM/UNROM
/// Mapper 002
/// Ducktales, Contra, Metal Gear, Mega Man, Castlevania
/// https://www.nesdev.org/wiki/UxROM
/// 2 banks, lower is switchable
///  0x8000-0xBFFF: 16k switchable
///  0xC000-0xFFFF: 16k fixed to the last bank
///
/// Bank select register: 0x8000-0xFFFF write low 3 or 4 bits
/// </summary>
public class UnRom : IMapper
{
    public int Id => 2;
    public SlicedRom Chr { get; }
    public RomBase PrgRam { get; }
    private readonly ILogger _logger;
    private readonly ICartridgeHeader _cartridgeHeader;

    private readonly SlicedRom _prgRoms;

    public UnRom(SlicedRom prgRoms, SlicedRom chr, ILogger logger,  ICartridgeHeader cartridgeHeader)
    {
        _cartridgeHeader = cartridgeHeader;
        _logger = logger;
        _prgRoms = prgRoms;
        
        Chr = chr;
        PrgRam = new Ram(8); // According to wiki this should be 0, but keep it safe

        prgRoms.Map(0, 0);
        prgRoms.Map(1, prgRoms.LastBankId); // "Upper" bank is initialized to the last bank
    }
    
    
    public bool TryRead(ushort address, out byte value)
    {
        switch (address)
        {
            case >= 0x8000:
                value = _prgRoms.Read((ushort)(address - 0x8000));
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
            _logger.Warning("Write to UnROM ram. UnRom shouldn't have RAM isn't it? ({Address:X4} 0x{Value})", address, value);
            PrgRam.Write((ushort)(address - 0x6000), value);
        }
        else if (address >= 0x8000)
        {
            // bank switch
            // NesDev wiki: Emulator implementations of iNES mapper 2 treat this as a full 8-bit bank select register,
            // without bus conflicts. This allows the mapper to be used for similar boards that are compatible.
            var selectedBank = (byte)(value % _prgRoms.NumberOfBanks);
            _prgRoms.Map(0, selectedBank);
        }
        else
        {
            _logger.Warning("Unsupported write to UNROM ROM area 0x{Address:X4} 0x{Value:X}", address, value);
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