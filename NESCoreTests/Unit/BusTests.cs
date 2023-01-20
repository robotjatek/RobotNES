using Moq;

using NESCore;
using NESCore.PPU;

namespace NESCoreTests.Unit
{
    public class BusTests : TestBase
    {
        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress1()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Write(0, 0xda);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0), It.Is<byte>(b => b == 0xDA)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress2()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Write(0x1, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x1), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress3()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Write(0x7ff, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x7ff), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress4()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Write(0xfff, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x7ff), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress5()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Write(0x800, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x0), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress6()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Write(0x1000, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x0), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress7()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Write(0x17ff, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x7ff), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress8()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Write(0x1800, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x0), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToTheCorrectAddress9()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Write(0x17ff, 0xaa);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x7ff), It.Is<byte>(b => b == 0xaa)));
        }

        [Fact]
        public void BusWritesCorrectDataToCorrectAddressWhenWritingToMirroredAddressLocation()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Write(0x1001, 0xda);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x1), It.Is<byte>(b => b == 0xda)));
        }

        [Fact]
        public void BusWritesCorrectDataToCorrectAddressWhenWritingToMirroredAddressLocation2()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Write(0x1801, 0xda);
            memory.Verify(m => m.Write(It.Is<ushort>(a => a == 0x1), It.Is<byte>(b => b == 0xda)));
        }

        //Reads
        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Read(0);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress2()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Read(0x1);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x1)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress3()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Read(0x7ff);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x7ff)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress4()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Read(0xfff);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x7ff)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress5()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Read(0x800);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x0)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress6()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Read(0x1000);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x0)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress7()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Read(0x17ff);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x7ff)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress8()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Read(0x1800);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x0)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddress9()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Read(0x17ff);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x7ff)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddressWhenReadingFromMirroredAddressLocation()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Read(0x1001);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x1)));
        }

        [Fact]
        public void BusReadFromCorrectInternalMemoryAddressWhenReadingFromMirroredAddressLocation2()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Read(0x1801);
            memory.Verify(m => m.Read(It.Is<ushort>(a => a == 0x1)));
        }

        [Fact]
        public void BusReadFromCartridgeArea1()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Read(0x4020);
            _mockCartridge.Verify(c => c.Read(It.Is<ushort>(a => a == 0x4020)));
        }

        [Fact]
        public void BusWriteToCartridgeArea1()
        {
            var memory = new Mock<IMemory>();
            var sut = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            sut.Write(0x4020, 0xda);
            _mockCartridge.Verify(c => c.Write(It.Is<ushort>(a => a == 0x4020), It.Is<byte>(v => v == 0xda)));
        }

        [Fact]
        public void BusReadFromPPUArea()
        {
            var ppu = new Mock<IPPU>();
            var memory = new Mock<IMemory>();
            var bus = new Bus(_mockCartridge.Object, memory.Object, ppu.Object, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            
            for(UInt16 address = 0x2000; address <= 0x2007; address++)
            {
                bus.Read(address);
            }

            for (UInt16 address = 0x2000; address <= 0x2007; address++)
            {
                ppu.Verify(p => p.Read(address), Times.Once());
            }
        }

        [Fact]
        public void BusReadFromPPUMirroredArea()
        {
            var ppu = new Mock<IPPU>();
            var memory = new Mock<IMemory>();
            var bus = new Bus(_mockCartridge.Object, memory.Object, ppu.Object, _controller1.Object, _controller2.Object, _apu.Object, _logger);

            for (UInt16 address = 0x2000; address <= 0x3FFF; address++)
            {
                bus.Read(address);
            }

            for (UInt16 address = 0x2000; address <= 0x2007; address++)
            {
                ppu.Verify(p => p.Read(address), Times.Exactly(1024));
            }
        }

        [Fact]
        public void BusWriteToPPUArea()
        {
            var ppu = new Mock<IPPU>();
            var memory = new Mock<IMemory>();
            var bus = new Bus(_mockCartridge.Object, memory.Object, ppu.Object, _controller1.Object, _controller2.Object, _apu.Object, _logger);

            for (UInt16 address = 0x2000; address <= 0x2007; address++)
            {
                bus.Write(address, 0xaa);
            }

            for (UInt16 address = 0x2000; address <= 0x2007; address++)
            {
                ppu.Verify(p => p.Write(address, 0xaa), Times.Once());
            }
        }

        [Fact]
        public void BusWriteToPPUMirroredArea()
        {
            var ppu = new Mock<IPPU>();
            var memory = new Mock<IMemory>();
            var bus = new Bus(_mockCartridge.Object, memory.Object, ppu.Object, _controller1.Object, _controller2.Object, _apu.Object, _logger);

            for (UInt16 address = 0x2000; address <= 0x3FFF; address++)
            {
                bus.Write(address, 0xaa);
            }

            for (UInt16 address = 0x2000; address <= 0x2007; address++)
            {
                ppu.Verify(p => p.Write(address, 0xaa), Times.Exactly(1024));
            }
        }

        [Fact]
        public void WriteToOAMDmaRegisterRaisesEvent()
        {
            var memory = new Mock<IMemory>();
            var bus = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            using var monitor = bus.Monitor<IBUS>();

            bus.Write(0x4014, 0x0);
            monitor.Should().Raise(nameof(bus.OAMDMAEvent)).WithArgs<byte>(a => a == 0);
        }

        [Fact]
        public void WriteToOAMDmaRegisterRaisesEvent2()
        {
            var memory = new Mock<IMemory>();
            var bus = new Bus(_mockCartridge.Object, memory.Object, _mockPPU, _controller1.Object, _controller2.Object, _apu.Object, _logger);
            using var monitor = bus.Monitor<IBUS>();

            bus.Write(0x4014, 0x10);
            monitor.Should().Raise(nameof(bus.OAMDMAEvent)).WithArgs<byte>(a => a == 0x10);
        }
    }
}
