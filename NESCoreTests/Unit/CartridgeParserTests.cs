using NESCore.Mappers;
using NESCore;
using Moq;
using NESCore.Cartridge;

namespace NESCoreTests.Unit
{
    public class CartridgeParserTests : TestBase
    {
        private class StubMapper : IMapper
        {
            public int Id => throw new NotImplementedException();

            public SlicedRom Chr => throw new NotImplementedException();

            public RomBase PrgRam => throw new NotImplementedException();
            
            public bool TryRead(ushort address, out byte value)
            {
                throw new NotImplementedException();
            }

            public void Write(ushort address, byte value)
            {
                throw new NotImplementedException();
            }

            public IMapper.MirroringMode GetMirroring()
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void HeaderBeginsWithMagicString()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);

            sut.LoadCartridge(NESTEST_ROM_PATH, _getBus).Header.MAGIC.Should().BeEquivalentTo("NES\u001A"); //NES^Z
        }

        [Fact]
        public void HeaderEncodesTheNumberOf16KRomBanks()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(NESTEST_ROM_PATH, _getBus).Header.NumberOf16kROMBanks.Should().Be(1);
        }

        [Fact]
        public void HeaderEncodesSuperMario16KRomBanksCorrectly()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(SUPER_MARIO_ROM_PATH, _getBus).Header.NumberOf16kROMBanks.Should().Be(2);
        }

        [Fact]
        public void HeaderEncodesTheNumberOf8KVromBanksCorrectly()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(SUPER_MARIO_ROM_PATH, _getBus).Header.NumberOf8kVROMBanks.Should().Be(1);
        }

        [Fact]
        public void HeaderFlagsEncodesHorizontalMirroring()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(NESTEST_ROM_PATH, _getBus).Header.FLAGS.Mirroring.Should().Be(MIRRORING.HORIZONTAL);
        }

        [Fact]
        public void HeaderFlagsEncodesVerticalMirroring()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(SUPER_MARIO_ROM_PATH, _getBus).Header.FLAGS.Mirroring.Should().Be(MIRRORING.VERTICAL);
        }

        [Fact]
        public void HeaderFlagsEncodesBatteryBackedRamAsZeroWhenNotPresent()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(SUPER_MARIO_ROM_PATH, _getBus).Header.FLAGS.BatteryRAM.Should().Be(BATTERY_RAM.NOT_PRESENT);
        }

        [Fact]
        public void HeaderFlagsEncodesBatteryBackedRamAsOneWhenPresent()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            mf.Setup(f => f.CreateMapper(header, RandomBytePattern(32), RandomBytePattern(8), _getBus)).Returns(new StubMapper());

            var sut = new CartridgeParser(mf.Object);
            sut.LoadCartridge(DRAGON_WARRIOR_ROM_PATH, _getBus).Header.FLAGS.BatteryRAM.Should().Be(BATTERY_RAM.PRESENT);
        }

        [Fact]
        public void TrainerFieldIsZero()
        {
            var sut = new CartridgeHeader([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
            sut.FLAGS.Trainer.Should().Be(TRAINER.NO);
        }

        [Fact]
        public void ThrowsExceptionWhenTrainerIsPresent()
        {
            var act = () => new CartridgeHeader([0, 0, 0, 0, 0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
            act.Should().Throw<TrainersNotSupportedException>();
        }

        [Fact]
        public void HeaderFlagsEncodeFourScreenMirroringWhenNotAvailable()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            mf.Setup(f => f.CreateMapper(header, RandomBytePattern(32), RandomBytePattern(8), _getBus)).Returns(new StubMapper());

            var sut = new CartridgeParser(mf.Object);
            sut.LoadCartridge(MARIO_BROS_ROM_PATH, _getBus).Header.FLAGS.FourScreenMirroring.Should()
                .Be(FOUR_SCREEN_MIRRORING.DISABLED);
        }

        [Fact]
        public void HeaderFlagsEncodeFourScreenMirroringWhenAvailable()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            mf.Setup(f => f.CreateMapper(header, RandomBytePattern(32), RandomBytePattern(8), _getBus)).Returns(new StubMapper());

            var sut = new CartridgeParser(mf.Object);
            sut.LoadCartridge(RAD_RACER_II_ROM_PATH, _getBus).Header.FLAGS.FourScreenMirroring.Should()
                .Be(FOUR_SCREEN_MIRRORING.ENABLED);
        }

        [Fact]
        public void HeaderFlagsEncodeMapperLowerNibble0()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(MARIO_BROS_ROM_PATH, _getBus).Header.FLAGS.MapperLowerNibble.Should().Be(0);
        }

        [Fact]
        public void HeaderFlagsEncodeMapperUpperNibble0()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(MARIO_BROS_ROM_PATH, _getBus).Header.FLAGS.MapperUpperNibble.Should().Be(0);
        }

        [Fact]
        public void ParsesMapperTypeCorrectly1()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(MARIO_BROS_ROM_PATH, _getBus).Header.MapperID.Should().Be(0);
        }

        [Fact]
        public void ParsesMapperTypeCorrectly2()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            mf.Setup(f => f.CreateMapper(header, RandomBytePattern(32), RandomBytePattern(8), _getBus)).Returns(new StubMapper());

            var sut = new CartridgeParser(mf.Object);
            sut.LoadCartridge(DEATHBOTS_ROM_PATH, _getBus).Header.MapperID.Should().Be(79);
        }

        [Fact]
        public void MapperFactoryThrowsOnUnknownMapperID()
        {
            var sut = new MapperFactory(_logger);
            var header = new Mock<ICartridgeHeader>();
            header.Setup(h => h.MapperID).Returns(666);
            var a = () => sut.CreateMapper(header.Object, [], [], _getBus);
            a.Should().Throw<UnknownMapperException>();
        }

        [Fact]
        public void MapperFactoryUsesChrRamWhenCartridgeHasNoChrRom()
        {
            var header = new Mock<ICartridgeHeader>();
            header.Setup(h => h.MapperID).Returns(0);
            header.Setup(h => h.NumberOf8kVROMBanks).Returns(0);

            var mapper = new MapperFactory(_logger).CreateMapper(
                header.Object,
                RandomBytePattern(16),
                [],
                _getBus);

            mapper.Chr.IsReadonly.Should().BeFalse();
        }

        [Fact]
        public void
            Parses8kPRGRamToOneWhenItsZeroInTheROM() // The iNES wiki states bit 0 should be considered 1 for compatibility reasons. Also states that this field is rarely used
        {
            var sut = new CartridgeHeader([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
            sut.NumberOf8kRAMBanks.Should().Be(1);
        }

        [Fact]
        public void Parses8KPrgRam()
        {
            var sut = new CartridgeHeader([0, 0, 0, 0, 0, 0, 0, 0, 50, 0, 0, 0, 0, 0, 0, 0]);
            sut.NumberOf8kRAMBanks.Should().Be(50);
        }

        [Fact]
        public void HeaderFlags9ByteBit0EncodesPalSystem()
        {
            var sut = new CartridgeHeader([0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0]);
            sut.IsPAL.Should().BeTrue();
        }

        [Fact]
        public void HeaderFlags9ByteBit0EncodesNtscSystem()
        {
            var sut = new CartridgeHeader([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]);
            sut.IsPAL.Should().BeFalse();
        }

        [Fact]
        public void ROMShouldThrowWhenSizeIsNotEqualTo16k_1()
        {
            var createRom = () => new Rom([]);
            createRom.Should().Throw<InvalidSizeException>();
        }

        [Fact]
        public void ROMShouldThrowWhenSizeIsNotEqualTo16k_2()
        {
            var createRom = () => new Rom([0]);
            createRom.Should().Throw<InvalidSizeException>();
        }

        [Fact]
        public void RomShouldThrowWhenSizeIsLargerThan16K()
        {
            var bytes = new byte[16 * 1024 + 1];
            var createRom = () => new Rom(bytes);
            createRom.Should().Throw<InvalidSizeException>();
        }

        [Fact]
        public void RomShouldNotThrowWhenSizeIs16K()
        {
            var bytes = new byte[16 * 1024];
            var createRom = () => new Rom(bytes);
            createRom.Should().NotThrow();
        }
    }
}