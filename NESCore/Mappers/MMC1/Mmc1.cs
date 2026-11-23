using Serilog;

namespace NESCore.Mappers.MMC1;

// Metroid
// Dr. Mario
// Final Fantasy
// Chip and Dale 1/2
// Megaman 2
// Tetris
// Ninja Gaiden 1
// Robocop 3
// Double dragon
public class Mmc1 : IMapper
{
    public int Id => 1;
    public byte PrgRomNumber { get; }
    public int PrgRomSize { get; }
    public byte ChrRomNumber { get; }
    public int ChrRomSize { get; }

    private readonly ILogger _logger;
    
    // PPU $0000-$0FFF: 4 KiB switchable CHR bank
    //PPU $1000-$1FFF: 4 KiB switchable CHR bank
    // 4k/8k mode
    public RomBase Chr { get; } // TODO: sliced switching
    
    public RomBase PrgRam { get; }

    private readonly ShiftRegister _shiftRegister;
    
    // TODO: read helpers for the fields
    private byte _controlRegister; // $8000-$9FFF
    private byte _chrBank0; // $A000-$BFFF
    private byte _chrBank1; // $C000-$DFFF
    private byte _prgBank; // $E000-$FFFF
    private bool _prgRamDisabled;
    
    
    private readonly List<ChrBase> _chrRoms;
    private readonly Rom _lowerBank; // CPU $8000-$BFFF: 16 KiB PRG-ROM bank, either switchable or fixed to the first bank
    private readonly Rom _upperBank; // CPU $C000-$FFFF: 16 KiB PRG-ROM bank, either fixed to the last bank or switchable

    public Mmc1(List<Rom> prgRoms, List<ChrBase> chr, ILogger logger)
    {
        _logger = logger;
        _chrRoms = chr;
        PrgRomNumber = (byte)prgRoms.Count;
        PrgRomSize = prgRoms.Count * 1024 * 8;
        ChrRomNumber = (byte)chr.Count;
        ChrRomSize = chr.Count * 1024 * 8;
        
        Chr = chr[0]; // TODO: sliced switching 4kb slices
        PrgRam = new Ram(8); // CPU $6000-$7FFF: 8 KiB PRG-RAM bank, (optional)
        _lowerBank = prgRoms[0];
        _upperBank = prgRoms[^1];
        
        _shiftRegister = new ShiftRegister((address, value) =>
        {
            switch (address)
            {
                // TODO: fire mapper action
                case >= 0x8000 and <= 0x9FFF:
                    _controlRegister = (byte)(value & 0x1f);
                    break;
                case >= 0xA000 and <= 0xBFFF:
                    _chrBank0 = (byte)(value & 0x1f); // Select 4 KiB or 8 KiB CHR bank at PPU $0000 (low bit ignored in 8 KiB mode)
                    // TODO: switch bank
                    break;
                case >= 0xC000 and <= 0xDFFF:
                    _chrBank1 = (byte)(value & 0x1f); // Select 4 KiB CHR bank at PPU $1000 (ignored in 8 KiB mode)
                    // TODO: switch bank
                    break;
                case >= 0xE000:
                    _prgBank = (byte)(value & 0xf);
                    // TODO: switch bank
                    _prgRamDisabled = (value & 0x10) > 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(address));
            }
        });
    }
    
    public bool TryRead(ushort address, out byte value)
    {
        switch (address)
        {
            case >= 0x8000 and <= 0xBFFF:
                value = _lowerBank.Read((ushort)(address - 0x8000));
                return true;
            case >= 0xC000:
                value = _upperBank.Read((ushort)(address -  0xC000));
                return true;
            case >= 0x6000:
                value = PrgRam.Read((ushort)(address - 0x6000));
                return true;
        }

        value = 0;
        return false;
    }

    public void Write(ushort address, byte value)
    {
        if (address >= 0x8000)
        {
            // 5-bit serial shift register: on the 5th write it fires an action that depends on the last address it was written from
            _shiftRegister.Write(value, address);
        }
        else if (address is >= 0x6000 and <= 0x7FFF)
        {
            PrgRam.Write((ushort)(address - 0x6000), value);
        }
        else
        {
            _logger.Warning("Unsupported write to cart: {Address:X}:{Value:X}", address, value);
        }
    }
}