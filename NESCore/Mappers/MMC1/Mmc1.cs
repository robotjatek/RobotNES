using Serilog;

namespace NESCore.Mappers.MMC1;

// TODO: support save ram
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

    private readonly ILogger _logger;
    private readonly Func<IBUS> _getBus;
    
    // PPU $0000-$0FFF: 4 KiB switchable CHR bank
    //PPU $1000-$1FFF: 4 KiB switchable CHR bank
    // 4k/8k mode
    // 4kb slices
    public SlicedRom Chr { get; }
    
    public Ram PrgRam { get; }

    private readonly ShiftRegister _shiftRegister;
    
    private byte _controlRegister = 0x0C; // $8000-$9FFF
    private byte _prgBank; // $E000-$FFFF
    private bool _prgRamDisabled;
    
    // CPU $8000-$BFFF: 16 KiB PRG-ROM bank, either switchable or fixed to the first bank
    // CPU $C000-$FFFF: 16 KiB PRG-ROM bank, either fixed to the last bank or switchable
    private readonly SlicedRom _prgRoms;
    
    private byte _chrBank0;
    private byte _chrBank1;

    public Mmc1(SlicedRom prgRoms, SlicedRom chr, Func<IBUS> getBus, ILogger logger)
    {
        _logger = logger;
        _getBus = getBus;
        _prgRoms = prgRoms;
        
        //_pagedChr = new PagedChr(chr);
        Chr = chr;
        PrgRam = new Ram(8); // CPU $6000-$7FFF: 8 KiB PRG-RAM bank, (optional)
        _prgRoms.Map(0, 0);
        _prgRoms.Map(1, _prgRoms.LastBankId);
        
        _shiftRegister = new ShiftRegister((address, value) =>
        {
            // action after the shift register is filled is determined by the last address it was written from
            switch (address)
            {
                case >= 0x8000 and <= 0x9FFF:
                    _controlRegister = (byte)(value & 0x1f);
                    SwitchPrg(_prgBank);
                    SwitchChr0(_chrBank0);
                    SwitchChr1(_chrBank1);
                    break;
                case >= 0xA000 and <= 0xBFFF:
                    _chrBank0 = (byte)(value & 0x1f);
                    SwitchChr0(_chrBank0); // Select 4 KiB or 8 KiB CHR bank at PPU $0000 (low bit ignored in 8 KiB mode)
                    break;
                case >= 0xC000 and <= 0xDFFF:
                    _chrBank1 = (byte)(value & 0x1f); // Select 4 KiB CHR bank at PPU $1000 (ignored in 8 KiB mode)
                    SwitchChr1(_chrBank1);
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
            case >= 0x8000:
                value = _prgRoms.Read((ushort)(address - 0x8000));
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
                // Pass the bus cycle counter so the shift register can drop the second write of a read-modify-write (nesdev: "Consecutive-cycle writes").
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
        switch (mode)
        {
            case ChrSwitchMode._8Kib:
                value &= 0b11110; // low bit ignored in 8 KiB mode
                Chr.Map(0, value);
                Chr.Map(1, (byte)(value + 1));
                break;
            case ChrSwitchMode._4Kib:
                Chr.Map(0, value);
                break;
            default:
                throw new ArgumentOutOfRangeException();
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
        Chr.Map(1, value);
    }
    
    private void SwitchPrg(byte value)
    {
        var rawBank = value & 0b01111;
        var mode = (_controlRegister & 0b01100) >> 2;

        switch (mode)
        {
            case 0:
            case 1:
                // switch 32 KiB at $8000, ignoring low bit of bank number
                if (_prgRoms.NumberOfBanks == 1)
                {
                    // safeguard for cartridges with one bank only. unlikely but not impossible
                    _prgRoms.Map(0, 0);
                    _prgRoms.Map(1, 0);
                }
                else
                {
                    var bank = (rawBank & 0b11110) % _prgRoms.NumberOfBanks; // Select 16 KiB PRG-ROM bank (low bit ignored in 32 KiB mode)
                    bank &= 0b11110;
                    _prgRoms.Map(0, (byte)bank);
                    _prgRoms.Map(1, (byte)(bank + 1));
                }
                break;
            case 2:
                // fix first bank at $8000 and switch 16 KiB bank at $C000
                var upperBank = rawBank % _prgRoms.NumberOfBanks;
                _prgRoms.Map(0, 0);
                _prgRoms.Map(1, (byte)upperBank);
                break;
            case 3:
                //  fix last bank at $C000 and switch 16 KiB bank at $8000
                var lowerBank = rawBank % _prgRoms.NumberOfBanks;
                _prgRoms.Map(0, (byte)lowerBank);
                _prgRoms.Map(1, _prgRoms.LastBankId);
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