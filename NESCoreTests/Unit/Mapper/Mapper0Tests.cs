using Moq;
using NESCore;
using NESCore.Mappers;
using Serilog;
// ReSharper disable TemplateIsNotCompileTimeConstantProblem

namespace NESCoreTests.Unit.Mapper
{
    public class Mapper0Tests : TestBase
    {
        [Fact]
        public void ReadFromLowerBank()
        {
            var prg = new SlicedRom(16, RandomBytePattern(32), true);
            var chr = new SlicedRom(8, RandomBytePattern(8), true);
            var loggerMock = new Mock<ILogger>();

            var sut = new Mapper0(prg, chr, loggerMock.Object, _mockHeader.Object);
            for (var address = 0x8000; address <= 0xBFFF; address++)
            {
                sut.TryRead((ushort)address, out var data).Should().BeTrue();
                data.Should().Be(prg.Read((ushort)(address - 0x8000)));
            }
        }

        [Fact]
        public void ReadFromUpperBank()
        {
            var prg = new SlicedRom(16, RandomBytePattern(32), true);
            var chr = new SlicedRom(8, RandomBytePattern(8), true);
            var loggerMock = new Mock<ILogger>();

            var sut = new Mapper0(prg, chr, loggerMock.Object, _mockHeader.Object);
            for (var address = 0xC000; address <= 0xFFFF; address++)
            {
                sut.TryRead((ushort)address, out var data).Should().BeTrue();
                data.Should().Be(prg.Read((ushort)(address - 0x8000)));
            }
        }

        [Fact]
        public void WritesAreIgnoredToRomArea()
        {
            var prg = new SlicedRom(16, RandomBytePattern(32), true);
            var chr = new SlicedRom(8, RandomBytePattern(8), true);
            var loggerMock = new Mock<ILogger>();
            
            var sut = new Mapper0(prg, chr, loggerMock.Object, _mockHeader.Object);

            for (var address = 0x8000; address <= 0xFFFF; address++)
            {
                sut.Write((ushort)address, 0xda);
            }

            loggerMock.Verify(l => l.Warning(It.IsAny<string>()), Times.Exactly(32 * 1024));
        }

        [Fact]
        public void PrgRamCanBeWrittenAndRead()
        {
            var prg = new SlicedRom(16, RandomBytePattern(32), true);
            var chr = new SlicedRom(8, RandomBytePattern(8), false);
            var sut = new Mapper0(prg, chr, new Mock<ILogger>().Object, _mockHeader.Object);

            sut.Write(0x6000, 0xa5);
            sut.TryRead(0x6000, out var data).Should().BeTrue();
            data.Should().Be(0xa5);
            sut.Write(0x7fff, 0x5a);
            sut.TryRead(0x7fff, out data).Should().BeTrue();
            data.Should().Be(0x5a);
        }

        [Fact]
        public void MapperCreationIsSuccessfulWithOnlyOneROMBank()
        {
            var loggerMock = new Mock<ILogger>();
            var prg = new SlicedRom(16, RandomBytePattern(32), true);
            var chr = new SlicedRom(8, RandomBytePattern(8), true);
            var sut = () => new Mapper0(prg, chr, loggerMock.Object, _mockHeader.Object);
            sut.Should().NotThrow();
        }

        [Fact]
        public void MapperCreationIsSuccessfulWithTwoRomBanks()
        {
            var loggerMock = new Mock<ILogger>();
            var prg = new SlicedRom(16, RandomBytePattern(32), true);
            var chr = new SlicedRom(8, RandomBytePattern(8), true);
            var sut = () => new Mapper0(prg, chr, loggerMock.Object, _mockHeader.Object);
            
            sut.Should().NotThrow();
        }

        [Fact]
        public void Mapper0ShouldThrowWhenThereAreMoreRoMsThanTwo()
        {
            var loggerMock = new Mock<ILogger>();
            var prg = new SlicedRom(16, RandomBytePattern(48), true);
            var chr = new SlicedRom(8, RandomBytePattern(8), true);

            var sut = () => new Mapper0(prg, chr, loggerMock.Object, _mockHeader.Object);
            
            sut.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ShouldThrowWhenThereAreNoRoms()
        {
            var prg = new SlicedRom(16, RandomBytePattern(0), true);
            var chr = new SlicedRom(8, RandomBytePattern(8), true);
            var sut = () => new Mapper0(prg, chr, new Mock<ILogger>().Object, _mockHeader.Object);
            sut.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MapperIdShouldBe0()
        {
            var prg = new SlicedRom(16, RandomBytePattern(32), true);
            var chr = new SlicedRom(8, RandomBytePattern(8), true);
            var sut = new Mapper0(prg, chr, new Mock<ILogger>().Object, _mockHeader.Object);
            sut.Id.Should().Be(0);
        }

        [Theory]
        [InlineData(0x4020)]
        [InlineData(0x5fff)]
        public void ReadOutsideTheMappedAreasIsReportedAsUndecoded(int address)
        {
            var prg = new SlicedRom(16, RandomBytePattern(16), true);
            var chr = new SlicedRom(8, RandomBytePattern(8), true);
            var sut = new Mapper0(prg, chr, new Mock<ILogger>().Object, _mockHeader.Object);

            sut.TryRead((ushort)address, out var data).Should().BeFalse();
            data.Should().Be(0);
        }
    }
}