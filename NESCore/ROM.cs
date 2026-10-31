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

        public byte Read(ushort address)
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
}