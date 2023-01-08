using NESCore;

namespace NESCoreTests.Unit
{
    public class ROMUnitTests : TestBase
    {
        [Fact]
        public void ReadROM()
        {
            var bytes = RandomBytePattern(16);
            var sut = new ROM(bytes);
            for (UInt16 i = 0; i < bytes.Length; i++)
            {
                sut.Read(i).Should().Be(bytes[i]);
            }
        }

        [Fact]
        public void ROMShouldThrowWhenDataIsNot16k1()
        {
            var bytes = RandomBytePattern(15);
            var sut = () => new ROM(bytes);
            sut.Should().Throw<InvalidROMSizeException>();
        }

        [Fact]
        public void ROMShouldThrowWhenDataIsNot16k2()
        {
            var bytes = RandomBytePattern(17);
            var sut = () => new ROM(bytes);
            sut.Should().Throw<InvalidROMSizeException>();
        }

        [Fact]
        public void ReadVROM()
        {
            var bytes = RandomBytePattern(8);
            var sut = new VROM(bytes);
            for (UInt16 i = 0; i < bytes.Length; i++)
            {
                sut.Read(i).Should().Be(bytes[i]);
            }
        }

        [Fact]
        public void VROMShouldThrowWhenDataIsNot8k1()
        {
            var bytes = RandomBytePattern(7);
            var sut = () => new VROM(bytes);
            sut.Should().Throw<InvalidROMSizeException>();
        }

        [Fact]
        public void VROMShouldThrowWhenDataIsNot8k2()
        {
            var bytes = RandomBytePattern(9);
            var sut = () => new VROM(bytes);
            sut.Should().Throw<InvalidROMSizeException>();
        }
    }
}
