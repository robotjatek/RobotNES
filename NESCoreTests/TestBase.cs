using Moq;

using NESCore;
using NESCore.Cartridge;
using NESCore.CPU;
using NESCore.PPU;

using Serilog;

namespace NESCoreTests
{
    public abstract class TestBase
    {
        protected ILogger _logger;
        protected Mock<ICartridge> _mockCartridge;
        protected IPPU _mockPPU;
        protected Func<IBUS, IRegisters, byte>[] _instructions = new Func<IBUS, IRegisters, byte>[255];

        public TestBase()
        {
            _logger = Mock.Of<ILogger>();
            _mockCartridge = new Mock<ICartridge>();
            _mockPPU = Mock.Of<IPPU>();
        }

        protected static byte[] RandomBytePattern(int sizeInKilobytes)
        {
            var rnd = new Random();
            var array = new byte[sizeInKilobytes * 1024];
            rnd.NextBytes(array);

            return array;
        }
    }
}
