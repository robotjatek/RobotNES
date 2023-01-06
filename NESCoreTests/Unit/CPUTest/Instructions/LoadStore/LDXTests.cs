using Moq;
using NESCore.CPU;
using NESCore;
using FluentAssertions;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class LDXTests : InstructionsetTests
    {
        [Fact]
        public void LDX_IMM()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x10);

            var ldx_imm = _instructions[Opcodes.LDX_IMM];
            var cycles = ldx_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDX_IMM_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

            var ldx_imm = _instructions[Opcodes.LDX_IMM];
            var cycles = ldx_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.X = 0);
            registers.Verify(r => r.SetZeroFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDX_IMM_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-1));

            var ldx_imm = _instructions[Opcodes.LDX_IMM];
            var cycles = ldx_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.X = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDX_ABS()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x10);

            var ldx_abs = _instructions[Opcodes.LDX_ABS];
            var cycles = ldx_abs(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead));
            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDX_ABS_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 10;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x10);

            var ldx_abs = _instructions[Opcodes.LDX_ABS_Y];
            var cycles = ldx_abs(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead + 10));
            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDX_ABS_Y_penalty_on_page_boundary()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xff).Returns(0x00).Returns(0x10);

            var ldx_abs = _instructions[Opcodes.LDX_ABS_Y];
            var cycles = ldx_abs(bus.Object, registers.Object);
            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0x00ff + 15));
            cycles.Should().Be(5);
        }

        [Fact]
        public void LDX_ABS_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x0);

            var ldx_abs = _instructions[Opcodes.LDX_ABS];
            var cycles = ldx_abs(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead));
            registers.VerifySet(r => r.X = 0);
            registers.Verify(r => r.SetZeroFlag(true));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDX_ABS_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(unchecked((byte)-1));

            var ldx_abs = _instructions[Opcodes.LDX_ABS];
            var cycles = ldx_abs(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead));
            registers.VerifySet(r => r.X = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDX_ZERO()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0x10);

            var ldx_zero = _instructions[Opcodes.LDX_ZERO];
            var cycles = ldx_zero(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad));
            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            cycles.Should().Be(3);
        }

        [Fact]
        public void LDX_ZERO_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0x10);

            var ldx_zero_y = _instructions[Opcodes.LDX_ZERO_Y];
            var cycles = ldx_zero_y(bus.Object, registers.Object);
            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0xad + 15));
            cycles.Should().Be(4);
        }
    }
}
