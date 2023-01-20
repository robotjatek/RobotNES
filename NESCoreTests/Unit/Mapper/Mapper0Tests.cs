using Moq;

using NESCore;
using NESCore.Mappers;

using Serilog;


namespace NESCoreTests.Unit.Mapper
{
    public class Mapper0Tests : TestBase
    {
        [Fact]
        public void ReadFromLowerBank()
        {
            var lowerBank = new ROM(RandomBytePattern(16));
            var upperBank = new ROM(RandomBytePattern(16));
            var loggerMock = new Mock<ILogger>();

            var sut = new Mapper0(new List<ROM>() { lowerBank, upperBank }, new List<VROM>() { new VROM(RandomBytePattern(8)) }, loggerMock.Object);
            for (int address = 0x8000; address <= 0xBFFF; address++)
            {
                var data = sut.Read((ushort)address);
                data.Should().Be(lowerBank.Read((ushort)(address - 0x8000)));
            }
        }

        [Fact]
        public void ReadFromUpperBank()
        {
            var lowerBank = new ROM(RandomBytePattern(16));
            var upperBank = new ROM(RandomBytePattern(16));
            var loggerMock = new Mock<ILogger>();

            var sut = new Mapper0(new List<ROM>() { lowerBank, upperBank }, new List<VROM>() { new VROM(RandomBytePattern(8)) }, loggerMock.Object);
            for (int address = 0xC000; address <= 0xFFFF; address++)
            {
                var data = sut.Read((ushort)address);
                data.Should().Be(upperBank.Read((ushort)(address - 0xC000)));
            }
        }

        [Fact]
        public void WritesAreIgnoredToRomArea()
        {
            var lowerBank = new ROM(RandomBytePattern(16));
            var upperBank = new ROM(RandomBytePattern(16));
            var loggerMock = new Mock<ILogger>();

            var sut = new Mapper0(new List<ROM>() { lowerBank, upperBank }, new List<VROM>() { new VROM(RandomBytePattern(8)) }, loggerMock.Object);

            for (int address = 0x8000; address <= 0xFFFF; address++)
            {
                sut.Write((ushort)address, 0xda);
            }

            loggerMock.Verify(l => l.Warning(It.IsAny<string>()), Times.Exactly(32 * 1024));

        }

        [Fact]
        public void MapperCreationIsSuccessfulWithOnlyOneROMBank()
        {
            var loggerMock = new Mock<ILogger>();
            var bank = new ROM(RandomBytePattern(16));
            var sut = () => new Mapper0(new List<ROM>() { bank }, new List<VROM>() { new VROM(RandomBytePattern(8)) }, loggerMock.Object);
            sut.Should().NotThrow();
        }

        [Fact]
        public void MapperCreationIsSuccessfulWithTwoROMBanks()
        {
            var loggerMock = new Mock<ILogger>();
            var bank1 = new ROM(RandomBytePattern(16));
            var bank2 = new ROM(RandomBytePattern(16));
            var sut = () => new Mapper0(new List<ROM> { bank1, bank2 }, new List<VROM>() { new VROM(RandomBytePattern(8)) }, loggerMock.Object); ;
            sut.Should().NotThrow();
        }

        [Fact]
        public void Mapper0ShouldThrowWhenThereAreMoreROMsThanTwo()
        {
            var loggerMock = new Mock<ILogger>();
            var bank1 = new ROM(RandomBytePattern(16));
            var bank2 = new ROM(RandomBytePattern(16));
            var bank3 = new ROM(RandomBytePattern(16));

            var sut = () => new Mapper0(new List<ROM> { bank1, bank2, bank3 }, new List<VROM>(), loggerMock.Object); ;
            sut.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ShouldThrowWhenThereAreNoROMS()
        {
            var sut = () => new Mapper0(new List<ROM>(), new List<VROM>(), new Mock<ILogger>().Object);
            sut.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MapperIDShouldBe0()
        {
            var bank1 = new ROM(RandomBytePattern(16));
            var sut = new Mapper0(new List<ROM>() { bank1 }, new List<VROM>() { new VROM(RandomBytePattern(8)) }, new Mock<ILogger>().Object);
            sut.ID.Should().Be(0);
        }
    }
}
