using Moq;

using NESCore;
using NESCore.CPU;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class NOPTests : InstructionTestBase
    {
        public NOPTests(InstructionsFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void NOP()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            var nop = _instructions[Opcodes.NOP];
            var cycles = nop(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void NOP_1A()
        {
            var bus = new Mock<IBUS>();

            var registers = new Mock<IRegisters>();

            var nop = _instructions[Opcodes.NOP_1A];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<UInt16>()), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void NOP_3A()
        {
            var bus = new Mock<IBUS>();

            var registers = new Mock<IRegisters>();

            var nop = _instructions[Opcodes.NOP_3A];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<UInt16>()), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void NOP_5A()
        {
            var bus = new Mock<IBUS>();

            var registers = new Mock<IRegisters>();

            var nop = _instructions[Opcodes.NOP_5A];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<UInt16>()), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void NOP_7A()
        {
            var bus = new Mock<IBUS>();

            var registers = new Mock<IRegisters>();

            var nop = _instructions[Opcodes.NOP_7A];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<UInt16>()), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void NOP_DA()
        {
            var bus = new Mock<IBUS>();

            var registers = new Mock<IRegisters>();

            var nop = _instructions[Opcodes.NOP_DA];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<UInt16>()), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void NOP_FA()
        {
            var bus = new Mock<IBUS>();

            var registers = new Mock<IRegisters>();

            var nop = _instructions[Opcodes.NOP_FA];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<UInt16>()), Times.Never());

            cycles.Should().Be(2);
        }

        [Fact]
        public void NOP_IMM_80()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(30);

            var registers = new Mock<IRegisters>();

            var nop = _instructions[Opcodes.NOP_IMM_80];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<UInt16>()), Times.Once());

            cycles.Should().Be(2);
        }

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

        [Fact]
        public void NOP_ABS_X_0x1C()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ABS_X_1C];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void NOP_ABS_X_0x1C_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xff).Returns(0x00);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ABS_X_1C];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xff + 10), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void NOP_ABS_X_0x3C()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ABS_X_3C];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void NOP_ABS_X_0x3C_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xff).Returns(0x00);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ABS_X_3C];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xff + 10), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void NOP_ABS_X_0x5C()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ABS_X_5C];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void NOP_ABS_X_0x5C_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xff).Returns(0x00);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ABS_X_5C];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xff + 10), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void NOP_ABS_X_0x7C()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ABS_X_7C];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void NOP_ABS_X_0x7C_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xff).Returns(0x00);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ABS_X_7C];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xff + 10), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void NOP_ABS_X_0xDC()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ABS_X_DC];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void NOP_ABS_X_0xDC_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xff).Returns(0x00);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ABS_X_DC];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xff + 10), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void NOP_ABS_X_0xFC()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ABS_X_FC];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void NOP_ABS_X_0xFC_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xff).Returns(0x00);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = _instructions[Opcodes.NOP_ABS_X_FC];
            var cycles = nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xff + 10), Times.Once());

            cycles.Should().Be(5);
        }
    }
}
