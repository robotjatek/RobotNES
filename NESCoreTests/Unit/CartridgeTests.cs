using Moq;
using NESCore.Cartridge;
using NESCore.Mappers;

namespace NESCoreTests.Unit
{
    public class CartridgeTests
    {
             
        [Fact]
        public void WriteToCartridgeDispatchedToMapper()
        {
            var mapperMock = new Mock<IMapper>();
            var cartridge = new Cartridge(Mock.Of<ICartridgeHeader>(), mapperMock.Object);
            cartridge.Write(0x10, 0xff);
            mapperMock.Verify(m => m.Write(It.Is<UInt16>(a => a == 0x10), It.Is<byte>(v => v == 0xff)));
        }

        [Fact]
        public void ReadFromCartridgeDispatchedToMapper()
        {
            var mapperMock = new Mock<IMapper>();
            var cartridge = new Cartridge(Mock.Of<ICartridgeHeader>(), mapperMock.Object);
            cartridge.TryRead(0x10, out _);
            byte mapperValue;
            mapperMock.Verify(m => m.TryRead(It.Is<UInt16>(a => a == 0x10), out mapperValue));
        }

        [Fact]
        public void UndecodedReadIsReportedToTheCaller()
        {
            var mapperMock = new Mock<IMapper>();
            var cartridge = new Cartridge(Mock.Of<ICartridgeHeader>(), mapperMock.Object);

            cartridge.TryRead(0x4100, out var data).Should().BeFalse();
            data.Should().Be(0);
        }
    }
}