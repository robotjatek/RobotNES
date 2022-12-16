namespace NESCore
{
    public class InvalidROMSizeException : Exception { }

    public abstract class ROMBase
    {
        public int ROMSize { get; private set; }

        private readonly byte[] _data;

        public ROMBase(int size, byte[] rawBytes)
        {
            ROMSize = size;
            if (rawBytes.Length != ROMSize * 1024)
            {
                throw new InvalidROMSizeException();
            }

            _data = rawBytes;
        }

        public byte Read(UInt16 address)
        {
            return _data[address];
        }
    }

    public class ROM : ROMBase
    {
        public ROM(byte[] rawBytes) : base(16, rawBytes)
        {
        }
    }

    public class VROM : ROMBase
    {
        public VROM(byte[] rawBytes) : base(8, rawBytes)
        { 
        }
    }
}
