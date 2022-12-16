namespace NESCore
{
    public class Memory : IMemory
    {
        private readonly byte[] _data = new byte[0x800];

        public byte Read(ushort address)
        {
            return _data[address];
        }

        public void Write(ushort address, byte value)
        {
            _data[address] = value;
        }
    }
}
