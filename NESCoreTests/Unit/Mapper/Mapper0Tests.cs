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
            var lowerBank = new Rom(RandomBytePattern(16));
            var upperBank = new Rom(RandomBytePattern(16));
            var loggerMock = new Mock<ILogger>();

            var sut = new Mapper0(new List<Rom>() { lowerBank, upperBank }, [new ChrRom(RandomBytePattern(8))],
                loggerMock.Object);
            for (int address = 0x8000; address <= 0xBFFF; address++)
            {
                var data = sut.Read((ushort)address);
                data.Should().Be(lowerBank.Read((ushort)(address - 0x8000)));
            }
        }

        [Fact]
        public void ReadFromUpperBank()
        {
            var lowerBank = new Rom(RandomBytePattern(16));
            var upperBank = new Rom(RandomBytePattern(16));
            var loggerMock = new Mock<ILogger>();

            var sut = new Mapper0(new List<Rom>() { lowerBank, upperBank }, [new ChrRom(RandomBytePattern(8))],
                loggerMock.Object);
            for (int address = 0xC000; address <= 0xFFFF; address++)
            {
                var data = sut.Read((ushort)address);
                data.Should().Be(upperBank.Read((ushort)(address - 0xC000)));
            }
        }

        [Fact]
        public void WritesAreIgnoredToRomArea()
        {
            var lowerBank = new Rom(RandomBytePattern(16));
            var upperBank = new Rom(RandomBytePattern(16));
            var loggerMock = new Mock<ILogger>();

            var sut = new Mapper0(new List<Rom>() { lowerBank, upperBank }, [new ChrRom(RandomBytePattern(8))],
                loggerMock.Object);

            for (int address = 0x8000; address <= 0xFFFF; address++)
            {
                sut.Write((ushort)address, 0xda);
            }

            loggerMock.Verify(l => l.Warning(It.IsAny<string>()), Times.Exactly(32 * 1024));
        }

        [Fact]
        public void PrgRamCanBeWrittenAndRead()
        {
            var bank = new Rom(RandomBytePattern(16));
            var sut = new Mapper0([bank], [new ChrRom(RandomBytePattern(8))], new Mock<ILogger>().Object);

            sut.Write(0x6000, 0xa5);
            sut.Read(0x6000).Should().Be(0xa5);
            sut.Write(0x7fff, 0x5a);
            sut.Read(0x7fff).Should().Be(0x5a);
        }

        [Fact]
        public void MapperCreationIsSuccessfulWithOnlyOneROMBank()
        {
            var loggerMock = new Mock<ILogger>();
            var bank = new Rom(RandomBytePattern(16));
            var sut = () =>
                new Mapper0(new List<Rom>() { bank }, [new ChrRom(RandomBytePattern(8))], loggerMock.Object);
            sut.Should().NotThrow();
        }

        [Fact]
        public void MapperCreationIsSuccessfulWithTwoROMBanks()
        {
            var loggerMock = new Mock<ILogger>();
            var bank1 = new Rom(RandomBytePattern(16));
            var bank2 = new Rom(RandomBytePattern(16));
            var sut = () =>
                new Mapper0(new List<Rom> { bank1, bank2 }, [new ChrRom(RandomBytePattern(8))], loggerMock.Object);
            ;
            sut.Should().NotThrow();
        }

        [Fact]
        public void Mapper0ShouldThrowWhenThereAreMoreROMsThanTwo()
        {
            var loggerMock = new Mock<ILogger>();
            var bank1 = new Rom(RandomBytePattern(16));
            var bank2 = new Rom(RandomBytePattern(16));
            var bank3 = new Rom(RandomBytePattern(16));

            var sut = () => new Mapper0(new List<Rom> { bank1, bank2, bank3 }, [], loggerMock.Object);
            ;
            sut.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ShouldThrowWhenThereAreNoROMS()
        {
            var sut = () => new Mapper0(new List<Rom>(), [], new Mock<ILogger>().Object);
            sut.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MapperIDShouldBe0()
        {
            var bank1 = new Rom(RandomBytePattern(16));
            var sut = new Mapper0(new List<Rom>() { bank1 }, [new ChrRom(RandomBytePattern(8))],
                new Mock<ILogger>().Object);
            sut.ID.Should().Be(0);
        }
    }
}