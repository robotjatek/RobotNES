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

            var sut = new CPU(bus.Object, _instructions, _logger);
            bus.Verify(b => b.Read(0xfffd));
            bus.Verify(b => b.Read(0xfffc));
        }

        [Fact]
        public void CpuFetchesInstructionFromTheCorrectResetVector()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.Is<ushort>(a => a == 0xfffd))).Returns(0xde);
            bus.Setup(b => b.Read(It.Is<ushort>(a => a == 0xfffc))).Returns(0xad);

            var sut = new CPU(bus.Object, _instructions, _logger);
            try
            {
                sut.RunInstruction();
            }
            catch { 
                // intentionally ignored exception
            }

            bus.Verify(b => b.Read(It.Is<ushort>(a => a == 0xdead)));
        }
    }
}
