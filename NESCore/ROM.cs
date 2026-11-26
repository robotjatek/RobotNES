namespace NESCore
{
    public class InvalidSizeException : Exception;

    public class RomSlice(byte[] rawBytes, bool isReadonly)
    {
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
        public bool IsReadonly => _isReadonly;

        public SlicedRom(int sliceSizeKb, byte[] rawBytes, bool isReadonly = true)
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

        public byte LastBankId => (byte)(_roms.Count - 1);
    }

    public class Ram(int sizeKb) : SlicedRom(sizeKb, new byte[sizeKb * 1024], false);
}