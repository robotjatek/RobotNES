using NESCore.Cartridge;
using Serilog;

namespace NESCore.Mappers;

/// <summary>
/// Banks
/// CPU $6000-$7FFF: 8 KiB PRG RAM bank (optional)
/// 
/// CPU $8000-$9FFF (or $C000-$DFFF): 8 KiB switchable PRG ROM bank
/// CPU $A000-$BFFF: 8 KiB switchable PRG ROM bank
/// CPU $C000-$DFFF (or $8000-$9FFF): 8 KiB PRG ROM bank, fixed to the second-last bank
/// CPU $E000-$FFFF: 8 KiB PRG ROM bank, fixed to the last bank
/// 
/// PPU $0000-$07FF (or $1000-$17FF): 2 KiB switchable CHR bank //2 KiB banks can only select even numbered CHR banks. (The lowest bit is ignored.) 
/// PPU $0800-$0FFF (or $1800-$1FFF): 2 KiB switchable CHR bank
/// PPU $1000-$13FF (or $0000-$03FF): 1 KiB switchable CHR bank
/// PPU $1400-$17FF (or $0400-$07FF): 1 KiB switchable CHR bank
/// PPU $1800-$1BFF (or $0800-$0BFF): 1 KiB switchable CHR bank
/// PPU $1C00-$1FFF (or $0C00-$0FFF): 1 KiB switchable CHR bank
///
/// CHR window 	2Kx2 + 1Kx4
/// PRG ROM window 	8K + 8K + 16K fixed
///
/// Registers:
/// Bank select ($8000-$9FFE, even) - Selects the target internal register (R0–R7) and banking modes
/// Bank Data	$8000–$9FFF	Odd ($8001)	Writes the bank number into the register selected by $8000
/// Mirroring	$A000–$BFFF	Even ($A000)	Sets nametable mirroring (Vertical or Horizontal)
/// 
/// PRG RAM Protect	$A000–$BFFF	Odd ($A001)	Enables/disables WRAM ($6000–$7FFF) and controls write protection
/// 
/// IRQ Latch	$C000–$DFFF	Even ($C000)	Sets the scanline counter reload value
/// IRQ Reload	$C000–$DFFF	Odd ($C001)	Clears the current IRQ counter so it reloads on the next clock
/// IRQ Disable	$E000–$FFFF	Even ($E000)	Disables IRQ generation and acknowledges pending interrupts
/// IRQ Enable	$E000–$FFFF	Odd ($E001)	Enables scanline IRQ generation
/// </summary>
public class Mmc3 : IMapper
{
    public int Id => 4;
    public SlicedRom Chr { get; }
    public Ram PrgRam { get; } = new(8);
    private readonly SlicedRom _prg; // 8K + 8K + 16K fixed
    private readonly ILogger _logger;
    private readonly ICartridgeHeader _cartridgeHeader;
    
    private readonly byte[] _bankRegisters = new byte[8];
    private byte _registerId;
    
    /// <summary>
    /// 0: $8000-$9FFF swappable, $C000-$DFFF fixed to second-last bank;
    /// 1: $C000-$DFFF swappable,$8000-$9FFF fixed to second-last bank
    /// </summary>
    private bool _prgBankMode;
    /// <summary>
    /// 0: two 2 KiB banks at $0000-$0FFF, four 1 KiB banks at $1000-$1FFF;
    /// 1: two 2 KiB banks at $1000-$1FFF, four 1 KiB banks at $0000-$0FFF
    /// </summary>
    private bool _chrInversion;
    private IMapper.MirroringMode _mirroringMode;
    private bool _prgRamWriteProtect; // 0: allow writes, 1: deny writes
    private bool _prgRamEnable = true; // 0: disable, 1: enable
    
    
    // Irq is just a basic counter: it counts down every end of a scanline
    // (on the real hardware the ppu address bus is snooped to determine the exact end of a scanline)
    // When the counter reaches zero an NMI is fired
    // The value of the counter can be set via the IRQ latch register, the counter immediately reloads from the latch
    public bool IrqPending { get; private set; }
    private byte _irqLatch; // When the IRQ counter is zero (or a reload is requested through $C001), this value will be copied to the IRQ counter at the NEXT rising edge of the PPU address, presumably at PPU cycle 260 of the current scanline.
    private bool _irqEnabled;
    private byte _irqCounter;
    private bool _irqReload;

    public Mmc3(SlicedRom prg, SlicedRom chr, ILogger logger, ICartridgeHeader cartridgeHeader)
    {
        _logger = logger;
        _cartridgeHeader = cartridgeHeader;
        _prg = prg; // switchable at 8KiB slices, 4 visible
        Chr = chr; // switchable at 1KiB slices
        UpdateBanks();
    }
    
    public bool TryRead(ushort address, out byte value)
    {
        switch (address)
        {
            case >= 0x6000 and < 0x8000:
                if (!_prgRamEnable)
                {
                    value = 0;
                    return false;
                }
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

    public void OnScanline()
    {
        if (_irqCounter == 0 || _irqReload)
        {
            _irqCounter = _irqLatch;
            _irqReload = false;
        }
        else
        {
            _irqCounter--;
        }

        if (_irqCounter == 0 && _irqEnabled)
        {
            IrqPending = true;
        }
    }

    public void Write(ushort address, byte value)
    {
        switch (address)
        {
            case >= 0x6000 and <= 0x7FFF:
            {
                if (_prgRamEnable && !_prgRamWriteProtect)
                {
                    PrgRam.Write((ushort)(address - 0x6000), value);
                }

                break;
            }
            case >= 0x8000 and <= 0x9fff:
            {
                // bank select and bank data
                if (address % 2 == 0)
                {
                    // Bank select
                    // Selects the target internal register (R0–R7) and banking modes
                    _registerId = (byte)(value & 0b111);
                    _prgBankMode = (value & 0b0100_0000) > 0;
                    _chrInversion = (value & 0x80) > 0;
                }
                else
                {
                    // Bank Data
                    // $8000–$9FFF	Odd ($8001)	Writes the bank number into the register selected by $8000
                    _bankRegisters[_registerId] = value;
                }
                UpdateBanks();
                break;
            }
            case >= 0xa000 and <= 0xbfff:
            {
                if (address % 2 == 0) // Mirroring mode
                {
                    SetMirrorMode(value);
                }
                else
                {
                    // prg ram protect
                    _prgRamWriteProtect = (value & 0b0100_0000) > 0;
                    _prgRamEnable = (value & 0x80) > 0;
                }

                break;
            }
            case >= 0xc000 and <= 0xDFFF:
            {
                if (address % 2 == 0)
                {
                    _irqLatch = value;
                }
                else
                {
                    // irq reload
                    // Writing any value to this register clears the MMC3 IRQ counter immediately,
                    // and then reloads it at the NEXT rising edge of the PPU address, presumably at PPU cycle 260 of the current scanline.
                    _irqCounter = 0;
                    _irqReload = true;
                }
                break;
            }
            case >= 0xe000:
            {
                if (address % 2 == 0)
                {
                    // irq disable
                    // Writing any value to this register will disable MMC3 interrupts AND acknowledge any pending interrupts.
                    _irqEnabled = false;
                    IrqPending = false;
                }
                else
                {
                    // irq enable
                    // Writing any value to this register will enable MMC3 interrupts. 
                    _irqEnabled = true;
                    
                }
                break;
            }
            default:
                _logger.Warning("Unsupported write to cart: {Address:X}:{Value:X}", address, value);
                break;
        }
    }

    private void SetMirrorMode(byte value)
    {
        if (_cartridgeHeader.FLAGS.FourScreenMirroring == FOUR_SCREEN_MIRRORING.ENABLED)
        {
            _mirroringMode = IMapper.MirroringMode.FourScreen;
            return;
        }

        _mirroringMode = (value & 0b1) > 0 ? IMapper.MirroringMode.Horizontal : IMapper.MirroringMode.Vertical;
    }

    private void UpdateBanks()
    {
        UpdatePrg();
        UpdateChr();
    }

    // https://www.nesdev.org/wiki/MMC3#PRG_Banks
    private void UpdatePrg()
    {
        if (_prgBankMode)
        {
            _prg.Map(0, _prg.LastBankId - 1); // 8000-9fff
            _prg.Map(1, _bankRegisters[7] & 0x3f); // a000-bfff
            _prg.Map(2, _bankRegisters[6] & 0x3f); // c000-dfff
            _prg.Map(3, _prg.LastBankId); // e000-ffff
        }
        else
        {
            _prg.Map(0, _bankRegisters[6] & 0x3f); // 8000-9fff
            _prg.Map(1, _bankRegisters[7] & 0x3f); // a000-bfff
            _prg.Map(2, _prg.LastBankId - 1); // c000-dfff
            _prg.Map(3, _prg.LastBankId); // e000-ffff
        }
    }

    // https://www.nesdev.org/wiki/MMC3#CHR_Banks
    private void UpdateChr()
    {
        // 2 KiB banks can only select even numbered CHR banks. (The lowest bit is ignored.) 
        if (_chrInversion)
        {
            // 1k registers
            Chr.Map(0, _bankRegisters[2]); // 0000-03ff
            Chr.Map(1, _bankRegisters[3]); // 0400-07ff
            Chr.Map(2, _bankRegisters[4]); // 0800-0bff
            Chr.Map(3, _bankRegisters[5]); // 0c00-0fff
            // 2kb registers 
            Chr.Map(4, _bankRegisters[0] & 0xfe); // 1000-13ff
            Chr.Map(5, (_bankRegisters[0] & 0xfe) + 1); // 1400-17ff
            Chr.Map(6, _bankRegisters[1] & 0xfe); // 1800-1bff
            Chr.Map(7, (_bankRegisters[1] & 0xfe) + 1); // 1c00-1fff
        }
        else
        {
            // 2 kb registers
            Chr.Map(0, _bankRegisters[0] & 0xfe); // 0000-03ff
            Chr.Map(1, (_bankRegisters[0] & 0xfe) + 1); // 0400-07ff
            Chr.Map(2, _bankRegisters[1] & 0xfe); // 0800-0bff
            Chr.Map(3, (_bankRegisters[1] & 0xfe) + 1); // 0c00-0fff
            // 1k registers
            Chr.Map(4, _bankRegisters[2]); // 1000-13ff
            Chr.Map(5, _bankRegisters[3]); // 1400-17ff
            Chr.Map(6, _bankRegisters[4]); // 1800-1bff
            Chr.Map(7, _bankRegisters[5]); // 1c00-1fff
        }
    }

    public IMapper.MirroringMode GetMirroring()
    {
        return _mirroringMode;
    }
}