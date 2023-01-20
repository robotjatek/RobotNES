using Serilog;

namespace NESCore.APU
{
    public class APU : IAPU
    {
        private readonly ILogger _logger;
        public APU(ILogger logger)
        {
            _logger = logger;
        }

        public byte Read(ushort address)
        {
            _logger.Warning($"Read from APU address 0x{address:X4} is ignored");
            return 0;
        }

        public void Write(ushort address, byte data)
        {
            _logger.Warning($"Write to APU address 0x{address:X4} is ignored. Value: 0x{data:X2}");
        }
    }
}
