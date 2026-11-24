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
    private readonly Func<IBUS> _getBus;
    
    // PPU $0000-$0FFF: 4 KiB switchable CHR bank
    //PPU $1000-$1FFF: 4 KiB switchable CHR bank
    // 4k/8k mode
    public RomBase Chr { get; }
    
    public RomBase PrgRam { get; }

    private readonly ShiftRegister _shiftRegister;
    
    private byte _controlRegister = 0x0C; // $8000-$9FFF
    private byte _prgBank; // $E000-$FFFF
    private bool _prgRamDisabled;
    private readonly PagedChr _pagedChr;
    private readonly List<Rom> _prgRoms;
    private Rom _lowerBank; // CPU $8000-$BFFF: 16 KiB PRG-ROM bank, either switchable or fixed to the first bank
    private Rom _upperBank; // CPU $C000-$FFFF: 16 KiB PRG-ROM bank, either fixed to the last bank or switchable

    public Mmc1(List<Rom> prgRoms, List<ChrBase> chr, Func<IBUS> getBus, ILogger logger)
    {
        _logger = logger;
        _getBus = getBus;
        _prgRoms = prgRoms;
        PrgRomNumber = (byte)prgRoms.Count;
        PrgRomSize = prgRoms.Count * 1024 * 16;
        ChrRomNumber = (byte)chr.Count;
        ChrRomSize = chr.Count * 1024 * 8;
        
        _pagedChr = new PagedChr(chr);
        Chr = _pagedChr;
        PrgRam = new Ram(8); // CPU $6000-$7FFF: 8 KiB PRG-RAM bank, (optional)
        _lowerBank = prgRoms[0];
        _upperBank = prgRoms[^1];
        
        _shiftRegister = new ShiftRegister((address, value) =>
        {
            // action after the shift register is filled is determined by the last address it was written from
            switch (address)
            {
                case >= 0x8000 and <= 0x9FFF:
                    _controlRegister = (byte)(value & 0x1f);
                    SwitchPrg(_prgBank);
                    break;
                case >= 0xA000 and <= 0xBFFF:
                    var chrBank0 = (byte)(value & 0x1f);
                    SwitchChr0(chrBank0); // Select 4 KiB or 8 KiB CHR bank at PPU $0000 (low bit ignored in 8 KiB mode)
                    break;
                case >= 0xC000 and <= 0xDFFF:
                    var chrBank1 = (byte)(value & 0x1f); // Select 4 KiB CHR bank at PPU $1000 (ignored in 8 KiB mode)
                    SwitchChr1(chrBank1);
                    break;
                case >= 0xE000:
                    _prgBank = (byte)(value & 0xf);
                    SwitchPrg(_prgBank);
                    _prgRamDisabled = (value & 0x10) > 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(address));
            }
        }, ResetControlRegister);
    }

    private void ResetControlRegister()
    {
        _controlRegister |= 0x0C;
        SwitchPrg(_prgBank);
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
        switch (address)
        {
            case >= 0x6000 and <= 0x7FFF:
            {
                if (_prgRamDisabled)
                {
                    _logger.Warning("Prg RAM was written to, but its supposed to be disabled");
                }
                PrgRam.Write((ushort)(address - 0x6000), value);
                break;
            }
            case >= 0x8000:
                // 5-bit serial shift register: on the 5th write it fires an action that depends on the last address it was written from
                _shiftRegister.Write(value, address, _getBus().Cycles);
                break;
            default:
                _logger.Warning("Unsupported write to cart: {Address:X}:{Value:X}", address, value);
                break;
        }
    }

    public IMapper.MirroringMode GetMirroring()
    {
        var mirrorBits = _controlRegister & 0b11;
        return mirrorBits switch
        {
            0 => IMapper.MirroringMode.OneScreenLower, // (CIRAM A10)	Physical VRAM Page Mapped to All 4 Nametables: Lower 1 KB bank (normally $2000)
            1 => IMapper.MirroringMode.OneScreenUpper, // and One-Screen B corresponds to the upper CIRAM bank (CIRAM A10 = 1, PPU $2400)
            2 => IMapper.MirroringMode.Vertical,
            3 => IMapper.MirroringMode.Horizontal,
            _ => throw new ArgumentOutOfRangeException(nameof(mirrorBits))
        };
    }

    /// <summary>
    /// Select 4 KiB or 8 KiB CHR bank at PPU $0000 (low bit ignored in 8 KiB mode)
    /// </summary>
    /// <param name="value"></param>
    private void SwitchChr0(byte value)
    {
        value &= 0x1f;
        var mode = GetChrMode();
        if (mode == ChrSwitchMode._8Kib)
        {
            value &= 0b11110; // low bit ignored in 8 KiB mode
            _pagedChr.Set8KBank(value);
        } 
        else if (mode == ChrSwitchMode._4Kib)
        {
            _pagedChr.SetLower4KBank(value);
        }
    }

    /// <summary>
    /// Select 4 KiB CHR bank at PPU $1000 (ignored in 8 KiB mode)
    /// </summary>
    /// <param name="value"></param>
    private void SwitchChr1(byte value)
    {
        if (GetChrMode() == ChrSwitchMode._8Kib) return;
        
        value &= 0x1f;
        _pagedChr.SetUpper4KBank(value);
    }

    private void SwitchPrg(byte value)
    {
        var bank = (value & 0b01111) % _prgRoms.Count; // Select 16 KiB PRG-ROM bank (low bit ignored in 32 KiB mode)
        var mode = (_controlRegister & 0b01100) >> 2;

        switch (mode)
        {
            case 0:
            case 1:
                // switch 32 KiB at $8000, ignoring low bit of bank number
                bank &= 0b11110;
                _lowerBank = _prgRoms[bank];
                _upperBank = _prgRoms[bank + 1];
                break;
            case 2:
                // fix first bank at $8000 and switch 16 KiB bank at $C000
                _lowerBank = _prgRoms[0];
                _upperBank = _prgRoms[bank];
                break;
            case 3:
                //  fix last bank at $C000 and switch 16 KiB bank at $8000
                _lowerBank = _prgRoms[bank];
                _upperBank = _prgRoms[^1];
                break;
        }
    }

    private ChrSwitchMode GetChrMode()
    {
        return (_controlRegister & 0x10) == 0 ? ChrSwitchMode._8Kib : ChrSwitchMode._4Kib;
    }

    private enum ChrSwitchMode
    {
        _8Kib,
        _4Kib
    }
}