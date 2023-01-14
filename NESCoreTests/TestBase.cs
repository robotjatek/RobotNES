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
        protected const string NESTEST_ROM_PATH = "TestROMs/nestest.nes";
        protected const string MARIO_BROS_ROM_PATH = "TestROMs/mario.nes";
        protected const string SUPER_MARIO_ROM_PATH = "TestROMs/smario.nes";
        protected const string DONKEY_KONG_ROM_PATH = "TestROMs/dk.nes";
        protected const string DRAGON_WARRIOR_ROM_PATH = "TestROMs/dragon_warrior.nes";
        protected const string RAD_RACER_II_ROM_PATH = "TestROMs/rad_racer_2.nes";
        protected const string DEATHBOTS_ROM_PATH = "TestROMs/deathbots.nes";
        protected const string DUCKTALESROM_PATH = "TestROMs/ducktales.nes";

        protected ILogger _logger = Mock.Of<ILogger>();
        protected Mock<ICartridge> _mockCartridge = new();
        protected IPPU _mockPPU = Mock.Of<IPPU>();
        protected Func<IBUS, IRegisters, byte>[] _instructions = new Func<IBUS, IRegisters, byte>[255];
        protected Mock<IRegisters> _registers = new();
        protected Mock<IPPURegisters> _ppuRegisters = new();
        protected Mock<IPPUMemory> _ppuMemory = new();

        protected static byte[] RandomBytePattern(int sizeInKilobytes)
        {
            var rnd = new Random();
            var array = new byte[sizeInKilobytes * 1024];
            rnd.NextBytes(array);

            return array;
        }
    }
}
