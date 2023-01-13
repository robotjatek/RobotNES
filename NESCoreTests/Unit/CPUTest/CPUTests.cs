using Moq;

using NESCore;
using NESCore.CPU;

namespace NESCoreTests.Unit.CPUTest
{
    public class CPUTests : TestBase
    {
        [Fact]
        public void CpuLoadsResetVectorFromBus()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.Is<ushort>(a => a == 0xfffd))).Returns(0xde);
            bus.Setup(b => b.Read(It.Is<ushort>(a => a == 0xfffc))).Returns(0xad);

            var sut = new CPU(bus.Object, _registers.Object, _instructions, _logger);
            bus.Verify(b => b.Read(0xfffd));
            bus.Verify(b => b.Read(0xfffc));
        }

        [Fact]
        public void CpuFetchesInstructionFromTheCorrectResetVector()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.Is<ushort>(a => a == 0xfffd))).Returns(0xde);
            bus.Setup(b => b.Read(It.Is<ushort>(a => a == 0xfffc))).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var sut = new CPU(bus.Object, registers.Object, _instructions, _logger);
            try
            {
                sut.RunInstruction();
            }
            catch { 
                // intentionally ignored exception
            }

            bus.Verify(b => b.Read(It.Is<ushort>(a => a == 0xdead)));
        }

        [Fact]
        public void CpuHandlesNMI()
        {
            var writeArgs = new List<byte>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Write(It.IsAny<UInt16>(), Capture.In(writeArgs)));
            //NMI vector:
            bus.Setup(b => b.Read(0xfffb)).Returns(0xbe);
            bus.Setup(b => b.Read(0xfffa)).Returns(0xef);
            //setup PC on startup:
            bus.Setup(b => b.Read(0xfffd)).Returns(0xde);
            bus.Setup(b => b.Read(0xfffc)).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.STATUS = 0xee;

            var cpu = new CPU(bus.Object, registers.Object, _instructions, _logger);
            
            cpu.HandleNMI();
            var cycles = cpu.RunInstruction();

            writeArgs.Should().ContainInConsecutiveOrder(new byte[] {0xde, 0xad, 0xee});
            registers.Object.PC.Should().Be(0xbeef);
            cycles.Should().Be(7);
        }
    }
}
