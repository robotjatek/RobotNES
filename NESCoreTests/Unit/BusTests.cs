using Moq;

using NESCore;
using NESCore.APU;
using NESCore.Cartridge;
using NESCore.Mappers;
using NESCore.PPU;
using Serilog;

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
            byte discarded;
            _mockCartridge.Verify(c => c.TryRead(It.Is<ushort>(a => a == 0x4020), out discarded));
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
        
        // $4100 is inside the cartridge area but outside every region Mapper0 decodes, so reads
        // there reach the real open bus path without any mocking.
        private static Bus CreateBusWithMapper0Cartridge(IMemory memory)
        {
            var prgBank = new Rom(RandomBytePattern(16));
            var mapper = new Mapper0([prgBank], [new ChrRom(RandomBytePattern(8))], Mock.Of<ILogger>());
            var cartridge = new Cartridge(Mock.Of<ICartridgeHeader>(), mapper);
            return new Bus(cartridge, memory, Mock.Of<IPPU>(), Mock.Of<IController>(), Mock.Of<IController>(),
                Mock.Of<IApu>(), Mock.Of<ILogger>());
        }

        [Fact]
        public void UndecodedReadReadsTheLastValidValue()
        {
            var bus = CreateBusWithMapper0Cartridge(new Memory());

            bus.Write(0x1000, 0x5a);
            bus.Read(0x4100).Should().Be(0x5a);
        }

        [Fact]
        public void UndecodedReadReturnsZeroBeforeAnyTransfer()
        {
            var bus = CreateBusWithMapper0Cartridge(new Memory());

            bus.Read(0x4100).Should().Be(0);
        }

        [Fact]
        public void WriteOverwritesTheValueSeenByAnUndecodedRead()
        {
            var bus = CreateBusWithMapper0Cartridge(new Memory());

            bus.Write(0x2, 0x11);
            bus.Read(0x4100).Should().Be(0x11);
            bus.Write(0x2, 0x22);
            bus.Read(0x4100).Should().Be(0x22);
        }

        [Fact]
        public void SuccessfulReadOverwritesTheValueSeenByAnUndecodedRead()
        {
            var memory = new Memory();
            var bus = CreateBusWithMapper0Cartridge(memory);

            memory.Write(0x10, 0x3c);
            bus.Read(0x10).Should().Be(0x3c);
            bus.Read(0x4100).Should().Be(0x3c);
        }

        [Fact]
        public void ControllerReadOnlyDrivesTheLowerFiveBits()
        {
            var controller = new Mock<IController>();
            controller.Setup(c => c.ReadNextButton()).Returns(1);
            var bus = new Bus(_mockCartridge.Object, new Memory(), _mockPPU, controller.Object,
                _controller2.Object, _apu.Object, _logger);

            bus.Write(0x6, 0x40);
            bus.Read(0x4016).Should().Be(0x41);
        }

        [Fact]
        public void ControllerReadRepeatsTheUpperBitsFromThePreviousTransfer()
        {
            var controller = new Mock<IController>();
            controller.Setup(c => c.ReadNextButton()).Returns(0);
            var bus = new Bus(_mockCartridge.Object, new Memory(), _mockPPU, controller.Object,
                _controller2.Object, _apu.Object, _logger);

            bus.Write(0x6, 0xb0);
            bus.Read(0x4016).Should().Be(0xb0);
        }
        
        [Theory]
        [InlineData(0x4000)]
        [InlineData(0x4001)]
        [InlineData(0x4002)]
        [InlineData(0x4003)]
        [InlineData(0x4004)]
        [InlineData(0x4005)]
        [InlineData(0x4006)]
        [InlineData(0x4007)]
        [InlineData(0x4008)]
        [InlineData(0x4009)]
        [InlineData(0x400A)]
        [InlineData(0x400B)]
        [InlineData(0x400C)]
        [InlineData(0x400D)]
        [InlineData(0x400E)]
        [InlineData(0x4010)]
        [InlineData(0x4011)]
        [InlineData(0x4012)]
        [InlineData(0x4013)]
        [InlineData(0x4014)]
        [InlineData(0x4018)]
        [InlineData(0x401F)]
        public void ApuReadsShouldReturnTheLastBusValue(ushort address)
        {
            var bus = CreateBusWithMapper0Cartridge(new Memory());
            bus.Write(0x40, 0x10);
            bus.Read(address).Should().Be(0x10);
        }

        [Fact]
        public void ApuReadShouldNotDriveTheBus()
        {
            var ppu = new Mock<IPPU>();
            var bus = new Bus(_mockCartridge.Object, new Memory(), ppu.Object, _controller1.Object,
                _controller2.Object, _apu.Object, _logger);
            ppu.Setup(p => p.Read(0x2000)).Returns(0x10);
            bus.Read(0x3f00).Should().Be(0x10);
            bus.Read(0x4000).Should().Be(0x10);
        }
    }
}
