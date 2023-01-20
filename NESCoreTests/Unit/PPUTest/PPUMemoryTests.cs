using Moq;

using NESCore;
using NESCore.Cartridge;
using NESCore.Mappers;
using NESCore.PPU;

namespace NESCoreTests.Unit.PPUTest
{
    public class PPUMemoryTests : TestBase
    {
        [Fact]
        public void ReadsPatternTables()
        {
            var chr = new VROM(RandomBytePattern(8));
            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.CHRROM).Returns(chr);
            
            var cartridge = new Mock<ICartridge>();
            cartridge.Setup(c => c.Mapper).Returns(mapper.Object);

            var memory = new PPUMemory(cartridge.Object, _logger);
            for (UInt16 i = 0; i < 0x2000; i++)
            {
                memory.Read(i).Should().Be(chr.Read(i));
            }
        }

        [Fact]
        public void WritesToNametablesCanBeReadBack1()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            var mapper = Mock.Of<IMapper>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(mapper);

            var cartridge = new CartridgeParser(mf.Object).LoadCartridge(NESTEST_ROM_PATH); //Cart with horizontal mirroring

            var memory = new PPUMemory(cartridge, _logger);
            for(UInt16 i = 0x2000; i < 0x3000; i++)
            {
                memory.Write(i, 0xaa);
                memory.Read(i).Should().Be(0xaa);
            }
        }

        [Fact]
        public void WritesToNametable0CanBeReadBackFromNameTable1HorizontalMirroring()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            var mapper = Mock.Of<IMapper>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(mapper);

            var cartridge = new CartridgeParser(mf.Object).LoadCartridge(NESTEST_ROM_PATH); //Cart with horizontal mirroring

            var memory = new PPUMemory(cartridge, _logger);
            for (UInt16 i = 0x2000; i < 0x2400; i++)
            {
                memory.Write(i, 0xaa);
                memory.Read((UInt16)(i + 0x400)).Should().Be(0xaa);
            }
        }

        [Fact]
        public void WritesToNametable1CanBeReadBackFromNameTable0HorizontalMirroring()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            var mapper = Mock.Of<IMapper>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(mapper);

            var cartridge = new CartridgeParser(mf.Object).LoadCartridge(NESTEST_ROM_PATH); //Cart with horizontal mirroring

            var memory = new PPUMemory(cartridge, _logger);
            for (UInt16 i = 0x2400; i < 0x2800; i++)
            {
                memory.Write(i, 0xaa);
                memory.Read((UInt16)(i - 0x400)).Should().Be(0xaa);
            }
        }

        [Fact]
        public void WritesToNametable2CanBeReadBackFromNameTable3HorizontalMirroring()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            var mapper = Mock.Of<IMapper>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(mapper);

            var cartridge = new CartridgeParser(mf.Object).LoadCartridge(NESTEST_ROM_PATH); //Cart with horizontal mirroring

            var memory = new PPUMemory(cartridge, _logger);
            for (UInt16 i = 0x2800; i < 0x2C00; i++)
            {
                memory.Write(i, 0xaa);
                memory.Read((UInt16)(i + 0x400)).Should().Be(0xaa);
            }
        }

        [Fact]
        public void WritesToNametable3CanBeReadBackFromNameTable2HorizontalMirroring()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            var mapper = Mock.Of<IMapper>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(mapper);

            var cartridge = new CartridgeParser(mf.Object).LoadCartridge(NESTEST_ROM_PATH); //Cart with horizontal mirroring

            var memory = new PPUMemory(cartridge, _logger);
            for (UInt16 i = 0x2C00; i < 0x2FFF; i++)
            {
                memory.Write(i, 0xaa);
                memory.Read((UInt16)(i - 0x400)).Should().Be(0xaa);
            }
        }

        [Fact]
        public void WritesToNametablesCanBeReadBack2()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            var mapper = Mock.Of<IMapper>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(mapper);

            var cartridge = new CartridgeParser(mf.Object).LoadCartridge(SUPER_MARIO_ROM_PATH); //Cart with vertical mirroring

            var memory = new PPUMemory(cartridge, _logger);
            for (UInt16 i = 0x2000; i < 0x3000; i++)
            {
                memory.Write(i, 0xaa);
                memory.Read(i).Should().Be(0xaa);
            }
        }

        [Fact]
        public void WritesToNametable0CanBeReadBackFromNameTable2VerticalMirroring()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            var mapper = Mock.Of<IMapper>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(mapper);

            var cartridge = new CartridgeParser(mf.Object).LoadCartridge(SUPER_MARIO_ROM_PATH); //Cart with vertical mirroring

            var memory = new PPUMemory(cartridge, _logger);
            for (UInt16 i = 0x2000; i < 0x2400; i++)
            {
                memory.Write(i, 0xaa);
                memory.Read((UInt16)(i + 0x800)).Should().Be(0xaa);
            }
        }

        [Fact]
        public void WritesToNametabl1CanBeReadBackFromNameTable3VerticalMirroring()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            var mapper = Mock.Of<IMapper>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(mapper);

            var cartridge = new CartridgeParser(mf.Object).LoadCartridge(SUPER_MARIO_ROM_PATH); //Cart with vertical mirroring

            var memory = new PPUMemory(cartridge, _logger);
            for (UInt16 i = 0x2400; i < 0x2800; i++)
            {
                memory.Write(i, 0xaa);
                memory.Read((UInt16)(i + 0x800)).Should().Be(0xaa);
            }
        }

        [Fact]
        public void WritesToNametable2CanBeReadBackFromNameTable0VerticalMirroring()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            var mapper = Mock.Of<IMapper>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(mapper);

            var cartridge = new CartridgeParser(mf.Object).LoadCartridge(SUPER_MARIO_ROM_PATH); //Cart with vertical mirroring

            var memory = new PPUMemory(cartridge, _logger);
            for (UInt16 i = 0x2800; i < 0x2C00; i++)
            {
                memory.Write(i, 0xaa);
                memory.Read((UInt16)(i + 0x800)).Should().Be(0xaa);
            }
        }

        [Fact]
        public void WritesToNametable3CanBeReadBackFromNameTable1HorizontalMirroring()
        {
            var mf = new Mock<IMapperFactory>();
            var header = Mock.Of<ICartridgeHeader>();
            var mapper = Mock.Of<IMapper>();
            mf.Setup(f => f.CreateMapper(header, new List<ROM>(), new List<VROM>())).Returns(mapper);

            var cartridge = new CartridgeParser(mf.Object).LoadCartridge(SUPER_MARIO_ROM_PATH); //Cart with vertical mirroring

            var memory = new PPUMemory(cartridge, _logger);
            for (UInt16 i = 0x2C00; i < 0x2FFF; i++)
            {
                memory.Write(i, 0xaa);
                memory.Read((UInt16)(i - 0x800)).Should().Be(0xaa);
            }
        }

        [Fact]
        public void WritesToPaletteMemoryCanBeReadBack()
        {
            var memory = new PPUMemory(_mockCartridge.Object, _logger);
            for(UInt16 i = 0x3f00; i < 0x3f20; i++)
            {
                memory.Write(i, 0xaa);
                memory.Read(i).Should().Be(0xaa);
            }
        }

        [Fact]
        public void WritesToPaletteMemoryCanBeReadBackFromMirroredLocations()
        {
            var memory = new PPUMemory(_mockCartridge.Object, _logger);
            for (UInt16 i = 0x3f00; i < 0x3f20; i++)
            {
                memory.Write(i, 0xaa);
            }

            for(UInt16 i = 0x3f20; i < 0x4000; i++)
            {
                memory.Read(i).Should().Be(0xaa);
            }
        }

        [Fact]
        public void WritesOamCanBeReadBack()
        {
            var memory = new PPUMemory(_mockCartridge.Object, _logger);
            for (byte i = 0; i < 0xff; i++)
            {
                memory.OamWrite(i, 0xaa);
            }

            for (byte i = 0; i < 0xff; i++)
            {
                memory.OamRead(i).Should().Be(0xaa);
            }
        }
    }
}
