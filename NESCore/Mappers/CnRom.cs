using Serilog;

namespace NESCore.Mappers;

public class CnRom : IMapper
{
    public int Id => 3;
    public byte PrgRomNumber { get; }
    public int PrgRomSize { get; }
    public byte ChrRomNumber { get; }
    public int ChrRomSize { get; }
    public RomBase Chr { get; private set; } // 8kb Switchable, up to 32kb
    public RomBase PrgRam { get; }
    private readonly ILogger _logger;
    private readonly List<ChrBase> _chrRoms;
    private readonly Rom _lowerBank;
    private readonly Rom _upperBank;
    
    public CnRom(List<Rom> prgRoms, List<ChrBase> chr, ILogger logger)
    {
        _logger = logger;
        _chrRoms = chr;
        PrgRomNumber = (byte)prgRoms.Count;
        PrgRomSize = PrgRomNumber * 1024 * 8;
        ChrRomNumber = (byte)chr.Count;
        ChrRomSize = ChrRomNumber * 1024 * 8;
        
        Chr = chr[0];
        PrgRam = new Ram(2); // CPU $6000-$7FFF: 2 KiB of PRG-RAM, mirrored three times (Hayauchi Super Igo only)
        _lowerBank = prgRoms[0];
        _upperBank = prgRoms[^1];
    }
    
    public bool TryRead(ushort address, out byte value)
    {
        switch (address)
        {
            case >= 0x8000 and <= 0xBFFF:
                value = _lowerBank.Read((ushort)(address - 0x8000));
                return true;
            case >= 0xC000:
                value = _upperBank.Read((ushort)(address - 0xC000));
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
            var bank = (value & 0b11) % _chrRoms.Count; // TODO: bus conflicts
            Chr = _chrRoms[bank];
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
}