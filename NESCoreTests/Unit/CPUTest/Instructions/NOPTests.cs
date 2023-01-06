using Moq;

using NESCore;
using NESCore.CPU;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class NOPTests : InstructionTestBase
    {
        [Fact]
        public void NOP_Zero_0x04()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0xde);

            var registers = new Mock<IRegisters>();

            var nop = _instructions[Opcodes.NOP_ZERO_04];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xde), Times.Once());

            cycles.Should().Be(3);
        }

        [Fact]
        public void NOP_Zero_0x44()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0xde);

            var registers = new Mock<IRegisters>();

            var nop = _instructions[Opcodes.NOP_ZERO_44];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xde), Times.Once());

            cycles.Should().Be(3);
        }

        [Fact]
        public void NOP_Zero_0x64()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0xde);

            var registers = new Mock<IRegisters>();

            var nop = _instructions[Opcodes.NOP_ZERO_64];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xde), Times.Once());

            cycles.Should().Be(3);
        }
    }
}
