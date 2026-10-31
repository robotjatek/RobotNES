using NESCore;

namespace NESCoreTests.Unit
{
    public class ROMUnitTests : TestBase
    {
        [Fact]
        public void ReadROM()
        {
            var bytes = RandomBytePattern(16);
            var sut = new Rom(bytes);
            for (UInt16 i = 0; i < bytes.Length; i++)
            {
                sut.Read(i).Should().Be(bytes[i]);
            }
        }

        [Fact]
        public void ROMShouldThrowWhenDataIsNot16k1()
        {
            var bytes = RandomBytePattern(15);
            var sut = () => new Rom(bytes);
            sut.Should().Throw<InvalidSizeException>();
        }

        [Fact]
        public void ROMShouldThrowWhenDataIsNot16k2()
        {
            var bytes = RandomBytePattern(17);
            var sut = () => new Rom(bytes);
            sut.Should().Throw<InvalidSizeException>();
        }

        [Fact]
        public void ReadVROM()
        {
            var bytes = RandomBytePattern(8);
            var sut = new ChrRom(bytes);
            for (UInt16 i = 0; i < bytes.Length; i++)
            {
                sut.Read(i).Should().Be(bytes[i]);
            }
        }

        [Fact]
        public void VROMShouldThrowWhenDataIsNot8k1()
        {
            var bytes = RandomBytePattern(7);
            var sut = () => new ChrRom(bytes);
            sut.Should().Throw<InvalidSizeException>();
        }

        [Fact]
        public void VROMShouldThrowWhenDataIsNot8k2()
        {
            var bytes = RandomBytePattern(9);
            var sut = () => new ChrRom(bytes);
            sut.Should().Throw<InvalidSizeException>();
        }

        [Fact]
        public void PrgRamCanBeWrittenAndRead()
        {
            var ram = new Ram(8);
            ram.Write(0x1fff, 0xa5);
            ram.Read(0x1fff).Should().Be(0xa5);
        }

        [Fact]
        public void ChrRamCanBeWrittenAndRead()
        {
            var ram = new ChrRam();
            ram.Write(0x1fff, 0xa5);
            ram.Read(0x1fff).Should().Be(0xa5);
        }
    }
}