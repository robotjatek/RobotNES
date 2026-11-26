using NESCore.Cartridge;
using Serilog;

namespace NESCore.Mappers;

public class CnRom : IMapper
{
    public int Id => 3;
    
    public SlicedRom Chr { get; } // 8kb Switchable, up to 32kb
    public RomBase PrgRam { get; }
    private readonly ILogger _logger;
    private readonly ICartridgeHeader _cartridgeHeader;
    private readonly SlicedRom _prg;
    
    public CnRom(SlicedRom prgRoms, SlicedRom chr, ILogger logger, ICartridgeHeader cartridgeHeader)
    {
        _cartridgeHeader = cartridgeHeader;
        _logger = logger;
        
        Chr = chr;
        PrgRam = new Ram(2); // CPU $6000-$7FFF: 2 KiB of PRG-RAM, mirrored three times (Hayauchi Super Igo only)
        _prg = prgRoms;
        prgRoms.Map(0, 0);
        prgRoms.Map(1, prgRoms.LastBankId);
    }
    
    public bool TryRead(ushort address, out byte value)
    {
        switch (address)
        {
            case >= 0x8000:
                value = _prg.Read((ushort)(address - 0x8000));
                return true;
            case >= 0x6000:
            {
                var mirroredAddress = (ushort)((address - 0x6000) & 0x07ff);
                value = PrgRam.Read(mirroredAddress);
                return true;
            }
            default:
                value = 0;
                return false;
        }
    }

    public void Write(ushort address, byte value)
    {
        if (address >= 0x8000)
        {
            // chr Bank Select: responds to writes to CPU $8000-$FFFF
            var bank = (value & 0b11) % Chr.NumberOfBanks; // TODO: bus conflicts
            Chr.Map(0, (byte)bank);
        }
        else if (address is >= 0x6000 and <= 0x7FFF)
        {
            var mirroredAddress = (ushort)((address - 0x6000) & 0x07ff);
            PrgRam.Write(mirroredAddress, value);
        }
        else
        {
            _logger.Warning("Unsupported write to CART: {Address:X}:{Value:X}", address, value);
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