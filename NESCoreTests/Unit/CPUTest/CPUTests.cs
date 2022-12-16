using Moq;

using NESCore;

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

            var registers = new Mock<IRegisters>();
            var sut = new CPU(bus.Object, registers.Object, _instructions);
            registers.VerifySet(r => r.PC = 0xdead);
        }

        [Fact]
        public void CpuFetchesInstructionFromTheCorrectResetVector()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.Is<ushort>(a => a == 0xfffd))).Returns(0xde);
            bus.Setup(b => b.Read(It.Is<ushort>(a => a == 0xfffc))).Returns(0xad);

            var registers = new Registers();
            var sut = new CPU(bus.Object, registers, _instructions);
            sut.RunInstruction();

            bus.Verify(b => b.Read(It.Is<ushort>(a => a == 0xdead)));
        }
    }
}
