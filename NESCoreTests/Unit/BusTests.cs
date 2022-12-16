using Moq;

using NESCore;

namespace NESCoreTests.Unit
{
    public class BusTests : TestBase
    {
        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress1()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Write(0, 0xda);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0), It.Is<byte>(b => b == 0xDA)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress2()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Write(0x1, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x1), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress3()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Write(0x7ff, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x7ff), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress4()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Write(0xfff, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x7ff), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress5()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Write(0x800, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x0), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress6()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Write(0x1000, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x0), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress7()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Write(0x17ff, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x7ff), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress8()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Write(0x1800, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x0), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress9()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Write(0x17ff, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x7ff), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToCorrectAddressWhenWritingToMirroredAddressLocation()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Write(0x1001, 0xda);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x1), It.Is<byte>(b => b == 0xda)));
        }

        [Fact]
        public void BusWritesCorrectDataToCorrectAddressWhenWritingToMirroredAddressLocation2()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Write(0x1801, 0xda);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x1), It.Is<byte>(b => b == 0xda)));
        }

        //Reads
        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Read(0);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress2()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Read(0x1);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x1)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress3()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Read(0x7ff);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x7ff)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress4()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Read(0xfff);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x7ff)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress5()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Read(0x800);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x0)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress6()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Read(0x1000);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x0)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress7()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Read(0x17ff);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x7ff)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress8()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Read(0x1800);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x0)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress9()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Read(0x17ff);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x7ff)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddressWhenReadingFromMirroredAddressLocation()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Read(0x1001);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x1)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddressWhenReadingFromMirroredAddressLocation2()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Read(0x1801);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x1)));
        }

        [Fact]
        public void BusReadFromCartridgeArea1()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Read(0x4020);
            _mockCartridge.Verify(c => c.Read(It.Is<ushort>(a => a == 0x4020)));
        }

        [Fact]
        public void BusWriteToCartridgeArea1()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _logger);
            sut.Write(0x4020, 0xda);
            _mockCartridge.Verify(c => c.Write(It.Is<ushort>(a => a == 0x4020), It.Is<byte>(v => v == 0xda)));
        }
    }
}
