using Moq;

using NESCore;
using NESCore.CPU;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class NOPTests : InstructionTestBase
    {
        [Fact]
        public void NOP_ZERO_0x04()
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
        public void NOP_ZERO_0x44()
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
        public void NOP_ZERO_0x64()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0xde);

            var registers = new Mock<IRegisters>();

            var nop = _instructions[Opcodes.NOP_ZERO_64];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xde), Times.Once());

            cycles.Should().Be(3);
        }

        [Fact]
        public void NOP_ZERO_X_0x14()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ZERO_X_14];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xad + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void NOP_ZERO_X_0x34()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ZERO_X_34];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xad + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void NOP_ZERO_X_0x54()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ZERO_X_54];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xad + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void NOP_ZERO_X_0x74()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ZERO_X_74];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xad + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void NOP_ZERO_X_0xD4()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ZERO_X_D4];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xad + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void NOP_ZERO_X_0xF4()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ZERO_X_F4];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xad + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void NOP_ABS_0x0C()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();

            var nop = _instructions[Opcodes.NOP_ABS_0C];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead), Times.Once());

            cycles.Should().Be(4);
        }
    }
}
