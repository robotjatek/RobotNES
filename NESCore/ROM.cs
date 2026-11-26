namespace NESCore
{
    public class InvalidSizeException : Exception
    {
    }

    public abstract class RomBase
    {
        public int RomSize { get; private set; }

        protected readonly byte[] _data;

        public RomBase(int size, byte[] rawBytes)
        {
            RomSize = size;
            if (rawBytes.Length != RomSize * 1024)
            {
                throw new InvalidSizeException();
            }

            _data = rawBytes;
        }

        public virtual byte Read(ushort address)
        {
            return _data[address];
        }

        public abstract void Write(ushort address, byte value);
    }

    public class Rom(byte[] rawBytes) : RomBase(16, rawBytes)
    {
        public override void Write(ushort address, byte value)
        {
            // can be ignored
        }
    }

    public class RomSlice(int size, byte[] rawBytes, bool isReadonly)
    {
        public int RomSize => size;
        
        public byte Read(ushort address)
        {
            return rawBytes[address];
        }

        public void Write(ushort address, byte value)
        {
            if (isReadonly)
                return;
            
            rawBytes[address] = value;
        }
    }

    /// <summary>
    /// Pageable ROM chip that can be sliced into any arbitrary size
    /// </summary>
    public class SlicedRom
    {
        private readonly List<RomSlice> _roms = [];
        private readonly Dictionary<int, int> _mappings = new();
        private readonly int _sliceSize;
        private readonly bool _isReadonly;
        public int NumberOfBanks => _roms.Count;

        public SlicedRom(int sliceSizeKb, byte[] rawBytes, bool isReadonly)
        {
            _isReadonly = isReadonly;
            _sliceSize = sliceSizeKb * 1024;
            if (rawBytes.Length % _sliceSize != 0)
            {
                throw new InvalidSizeException();
            }
            
            for (var i = 0; i < rawBytes.Length; i+= _sliceSize)
            {
                _roms.Add(new RomSlice(
                    sliceSizeKb,
                    [.. rawBytes.AsSpan(i, sliceSizeKb * 1024)],
                    isReadonly));
                _mappings.Add(i / _sliceSize, i / _sliceSize); // default mappings in the order of the banks
            }
        }

        public byte Read(int address)
        {
            var mappingId = address / _sliceSize;
            var offset = (ushort)(address % _sliceSize);
            var romId = _mappings[mappingId];
            return _roms[romId].Read(offset);
        }
        
        public void Write(ushort address, byte value)
        {
            if (_isReadonly) return;
            
            var mappingId = address / _sliceSize;
            var offset = (ushort)(address % _sliceSize);
            var romId = _mappings[mappingId];
            _roms[romId].Write(offset, value);
        }

        public void Map(byte sliceId, byte romId)
        {
            _mappings[sliceId] = romId;
        }
    }

    public abstract class ChrBase(int sizeKb, byte[] rawBytes) : RomBase(sizeKb, rawBytes);

    public class ChrRom(byte[] rawBytes) : ChrBase(8, rawBytes)
    {
        public override void Write(ushort address, byte value)
        {
            // can be ignored
        }
    }

    public class ChrRam() : ChrBase(8, new byte[8 * 1024])
    {
        public override void Write(ushort address, byte value)
        {
            _data[address] = value;
        }
    }

    public class Ram(int sizeKb) : RomBase(sizeKb, new byte[sizeKb * 1024])
    {
        public override void Write(ushort address, byte value)
        {
            _data[address] = value;
        }
    }

    // TODO: create a generic paged Rom for both chr and prg to enable smaller slices like 1kb chr for MMC3
    public class PagedChr : RomBase
    {
        private readonly List<ChrBase> _banks;
        private int _lowerBankIndex;
        private int _upperBankIndex;
        
        public PagedChr(List<ChrBase> banks) : base(8, new byte[8*1024])
        {
            _banks = banks.Count == 0 ? [new ChrRam()] : banks;
            Set8KBank(0);
        }

        public void SetLower4KBank(int bank)
        {
            var totalBanks = _banks.Count * 2;
            _lowerBankIndex = bank % (totalBanks);
        }

        public void SetUpper4KBank(int bank)
        {
            var totalBanks = _banks.Count * 2;
            _upperBankIndex = bank % (totalBanks);
        }

        public void Set8KBank(int bank)
        {
            var baseBank = bank & 0b11110;
            SetLower4KBank(baseBank);
            SetUpper4KBank(baseBank + 1);
        }

        public override void Write(ushort address, byte value)
        {
            var bank4K = address < 0x1000 ? _lowerBankIndex : _upperBankIndex;
            var offset = (ushort)((address & 0x0FFF) + bank4K % 2 * 0x1000);
            _banks[bank4K / 2].Write(offset, value);
        }

        public override byte Read(ushort address)
        {
            var bank4K = address < 0x1000 ? _lowerBankIndex :  _upperBankIndex;
            var offset = (ushort)((address & 0x0FFF) + ((bank4K % 2) * 0x1000));
            return _banks[bank4K / 2].Read(offset);
        }
    }
}