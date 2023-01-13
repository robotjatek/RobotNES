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
        protected ILogger _logger = Mock.Of<ILogger>();
        protected Mock<ICartridge> _mockCartridge = new();
        protected IPPU _mockPPU = Mock.Of<IPPU>();
        protected Func<IBUS, IRegisters, byte>[] _instructions = new Func<IBUS, IRegisters, byte>[255];
        protected Mock<IRegisters> _registers = new();

        protected static byte[] RandomBytePattern(int sizeInKilobytes)
        {
            var rnd = new Random();
            var array = new byte[sizeInKilobytes * 1024];
            rnd.NextBytes(array);

            return array;
        }
    }
}
