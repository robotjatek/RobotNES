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
            public int ID => throw new NotImplementedException();

            public byte PRGRomNumber => throw new NotImplementedException();

            public int PRGRomSize => throw new NotImplementedException();

            public ROM LowerBank => throw new NotImplementedException();

            public ROM UpperBank => throw new NotImplementedException();

            public VROM CHRROM => throw new NotImplementedException();

            public byte CHRROMNumber => throw new NotImplementedException();

            public int CHRROMSize => throw new NotImplementedException();

            public byte Read(ushort address)
            {
                throw new NotImplementedException();
            }

            public void Write(ushort address, byte value)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void HeaderBeginsWithMagicString()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            
            sut.LoadCartridge(NESTEST_ROM_PATH).Header.MAGIC.Should().BeEquivalentTo("NES\u001A"); //NES^Z
        }

        [Fact]
        public void HeaderEncodesTheNumberOf16kROMBanks()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(NESTEST_ROM_PATH).Header.NumberOf16kROMBanks.Should().Be(1);
        }

        [Fact]
        public void HeaderEncodesSuperMario16kROMBanksCorrectly()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(SUPER_MARIO_ROM_PATH).Header.NumberOf16kROMBanks.Should().Be(2);
        }

        [Fact]
        public void HeaderEncodesTheNumberOf8kVROMBanksCorrectly()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(SUPER_MARIO_ROM_PATH).Header.NumberOf8kVROMBanks.Should().Be(1);
        }

        [Fact]
        public void HeaderFlagsEncodesHorizontalMirroring()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(NESTEST_ROM_PATH).Header.FLAGS.Mirroring.Should().Be(MIRRORING.HORIZONTAL);
        }

        [Fact]
        public void HeaderFlagsEncodesVerticalMirroring()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(SUPER_MARIO_ROM_PATH).Header.FLAGS.Mirroring.Should().Be(MIRRORING.VERTICAL);
        }

        [Fact]
        public void HeaderFlagsEncodesBatteryBackedRamAsZeroWhenNotPresent()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(SUPER_MARIO_ROM_PATH).Header.FLAGS.BatteryRAM.Should().Be(BATTERY_RAM.NOT_PRESENT);
        }

        [Fact]
        public void HeaderFlagsEncodesBatteryBackedRamAsOneWhenPresent()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(new StubMapper());

            var sut = new CartridgeParser(mf.Object);
            sut.LoadCartridge(DRAGON_WARRIOR_ROM_PATH).Header.FLAGS.BatteryRAM.Should().Be(BATTERY_RAM.PRESENT);
        }

        [Fact]
        public void TrainerFieldIsZero()
        {
            var sut = new CartridgeHeader(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            sut.FLAGS.Trainer.Should().Be(TRAINER.NO);
        }

        [Fact]
        public void ThrowsExceptionWhenTrainerIsPresent()
        {
            var act = () => new CartridgeHeader(new byte[] { 0, 0, 0, 0, 0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            act.Should().Throw<TrainersNotSupportedException>();
        }

        [Fact]
        public void HeaderFlagsEncodeFourScreenMirroringWhenNotAvailable()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(new StubMapper());

            var sut = new CartridgeParser(mf.Object);
            sut.LoadCartridge(MARIO_BROS_ROM_PATH).Header.FLAGS.FourScreenMirroring.Should().Be(FOUR_SCREEN_MIRRORING.DISABLED);
        }

        [Fact]
        public void HeaderFlagsEncodeFourScreenMirroringWhenAvailable()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(new StubMapper());

            var sut = new CartridgeParser(mf.Object);
            sut.LoadCartridge(RAD_RACER_II_ROM_PATH).Header.FLAGS.FourScreenMirroring.Should().Be(FOUR_SCREEN_MIRRORING.ENABLED);
        }

        [Fact]
        public void HeaderFlagsEncodeMapperLowerNibble0()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(MARIO_BROS_ROM_PATH).Header.FLAGS.MapperLowerNibble.Should().Be(0);
        }

        [Fact]
        public void HeaderFlagsEncodeMapperUpperNibble0()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(MARIO_BROS_ROM_PATH).Header.FLAGS.MapperUpperNibble.Should().Be(0);
        }

        [Fact]
        public void ParsesMapperTypeCorrectly1()
        {
            var mf = new MapperFactory(_logger);
            var sut = new CartridgeParser(mf);
            sut.LoadCartridge(MARIO_BROS_ROM_PATH).Header.MapperID.Should().Be(0);
        }

        [Fact]
        public void ParsesMapperTypeCorrectly2()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(new StubMapper());

            var sut = new CartridgeParser(mf.Object);
            sut.LoadCartridge(DEATHBOTS_ROM_PATH).Header.MapperID.Should().Be(79);
        }

        [Fact]
        public void MapperFactoryThrowsOnUnknownMapperID()
        {
            var sut = new MapperFactory(_logger);
            var header = new Mock<ICartridgeHeader>();
            header.Setup(h => h.MapperID).Returns(666);
            var a = () => sut.CreateMapper(header.Object, new List<ROM>(), new List<VROM>());
            a.Should().Throw<UnknownMapperException>();

        }

        [Fact]
        public void Parses8kPRGRamToOneWhenItsZeroInTheROM() // The iNES wiki states bit 0 should be considered 1 for compatibility reasons. Also states that this field is rarely used
        {
            var sut = new CartridgeHeader(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            sut.NumberOf8kRAMBanks.Should().Be(1);
        }

        [Fact]
        public void Parses8kPRGRam()
        {
            var sut = new CartridgeHeader(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 50, 0, 0, 0, 0, 0, 0, 0 });
            sut.NumberOf8kRAMBanks.Should().Be(50);
        }

        [Fact]
        public void HeaderFlags9ByteBit0EncodesPalSystem()
        {
            var sut = new CartridgeHeader(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 });
            sut.IsPAL.Should().BeTrue();
        }

        [Fact]
        public void HeaderFlags9ByteBit0EncodesNTSCSystem()
        {
            var sut = new CartridgeHeader(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            sut.IsPAL.Should().BeFalse();
        }

        [Fact]
        public void CreatesMapperWithTheCorrectNumberOfPrgROMBanksWithARealCartridge1()
        {
            var sut = new CartridgeParser(new MapperFactory(_logger));
            var cartridge = sut.LoadCartridge(MARIO_BROS_ROM_PATH);
            cartridge.Mapper.PRGRomNumber.Should().Be(1);
            cartridge.Mapper.PRGRomSize.Should().Be(1 * 16 * 1024); // 16k
        }

        [Fact]
        public void CreatesMapperWithTheCorrectNumberOfPrgROMBanksWithARealCartridge2()
        {
            var sut = new CartridgeParser(new MapperFactory(_logger));
            var cartridge = sut.LoadCartridge(SUPER_MARIO_ROM_PATH);
            cartridge.Mapper.PRGRomNumber.Should().Be(2);
            cartridge.Mapper.PRGRomSize.Should().Be(2 * 16 * 1024); // 32k
        }

        [Fact]
        public void CreatesMapperWithTheCorrectNumberofCHRRomBanksWithARealCartridge()
        {
            var sut = new CartridgeParser(new MapperFactory(_logger));
            var cartridge = sut.LoadCartridge(DONKEY_KONG_ROM_PATH);
            cartridge.Mapper.CHRROMNumber.Should().Be(1);
            cartridge.Mapper.CHRROMSize.Should().Be(1 * 8 * 1024);
        }

        [Fact]
        public void CreatesMapperWithTheCorrectNumberofCHRRomBanksWithARealCartridge2()
        {
            var mf = new Mock<IMapperFactory>();
            var sut = new CartridgeParser(mf.Object);
            sut.LoadCartridge(DRAGON_WARRIOR_ROM_PATH);
            mf.Verify(f => f.CreateMapper(It.IsAny<ICartridgeHeader>(), It.Is<List<ROM>>(a => a.Count == 4), It.Is<List<VROM>>(a => a.Count == 2)));
        }

        [Fact]
        public void CreatesMapperWithTheCorrectNumberofCHRRomBanksWithARealCartridge3()
        {
            var mf = new Mock<IMapperFactory>();
            var sut = new CartridgeParser(mf.Object);
            sut.LoadCartridge(DUCKTALESROM_PATH);
            mf.Verify(f => f.CreateMapper(It.IsAny<ICartridgeHeader>(), It.Is<List<ROM>>(a => a.Count == 8), It.Is<List<VROM>>(a => a.Count == 0)));
        }

        [Fact]
        public void ROMShouldThrowWhenSizeIsNotEqualTo16k_1()
        {
            var createRom = () => new ROM(Array.Empty<byte>());
            createRom.Should().Throw<InvalidROMSizeException>();
        }

        [Fact]
        public void ROMShouldThrowWhenSizeIsNotEqualTo16k_2()
        {
            var createRom = () => new ROM(new byte[] { 0 });
            createRom.Should().Throw<InvalidROMSizeException>();
        }

        [Fact]
        public void ROMShouldThrowWhenSizeIsLargerThan16k()
        {
            var bytes = new byte[16 * 1024 + 1];
            var createRom = () => new ROM(bytes);
            createRom.Should().Throw<InvalidROMSizeException>();
        }

        [Fact]
        public void ROMShouldNotThrowWhenSizeIs16k()
        {
            var bytes = new byte[16 * 1024];
            var createRom = () => new ROM(bytes);
            createRom.Should().NotThrow();
        }
    }
}
