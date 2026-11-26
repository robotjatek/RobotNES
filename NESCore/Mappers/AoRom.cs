using Serilog;

namespace NESCore.Mappers;


/// <summary>
/// Notes:
/// One nametable:
/// Active nametable: M bit (bit 4) (0: first nametable at $2000-23ff, 1: second nametable at: $2400-27ff)
/// PRG ROM size: up to 256 KB
/// PRG ROM bank size: 32 KB
/// PRG RAM: None
/// CHR capacity: 8 KB RAM
/// CHR bank size: Not bankswitched
/// Nametable mirroring: Single-screen, mapper-selectable
/// Subject to bus conflicts: AMROM/AOROM only
///
/// CPU $8000-$FFFF: 32 KB switchable PRG ROM bank
/// </summary>
public class AoRom : IMapper
{
    public int Id => 7;
    public SlicedRom Chr { get; }
    public Ram PrgRam { get; } = new(8);
    private readonly SlicedRom _prg;

    private bool _mirrorMode;
    private readonly ILogger _logger;

    public AoRom(SlicedRom prgRoms, SlicedRom chr, ILogger logger)
    {
        _logger = logger;
        _prg = prgRoms;

        Chr = chr;
        SwitchBanks(_prg.LastBankId - 1);
    }
    
    public bool TryRead(ushort address, out byte value)
    {
        switch (address)
        {
            case >= 0x6000 and < 0x8000:
                _logger.Warning("PRG Ram read");
                value = PrgRam.Read((ushort)(address - 0x6000));
                return true;
            case >= 0x8000:
                value = _prg.Read((ushort)(address - 0x8000));
                return true;
            default:
                value = 0;
                return false;
        }
    }

    public void Write(ushort address, byte value)
    {
        if (address is >= 6000 and < 0x8000)
        {
            _logger.Warning("PRG Ram write");
            PrgRam.Write((ushort)(address - 0x6000), value);
        }
        else if (address >= 0x8000)
        {
            // bank select register
            _mirrorMode = (value & 0b10000) > 0;
            
            // switching in pairs (32k chunks)
            var unitCount = _prg.NumberOfBanks / 2;
            var bankId = ((value & 0b111) % unitCount) * 2;
            SwitchBanks(bankId);
        }
    }

    public IMapper.MirroringMode GetMirroring()
    {
        return _mirrorMode ? IMapper.MirroringMode.OneScreenUpper : IMapper.MirroringMode.OneScreenLower;
    }

    private void SwitchBanks(int bankId)
    {
        _prg.Map(0, (byte) bankId);
        _prg.Map(1, (byte)(bankId + 1));
    }
}