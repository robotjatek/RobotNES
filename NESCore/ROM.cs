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