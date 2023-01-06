using Moq;
using NESCore.CPU;
using NESCore;
using FluentAssertions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class IncrementInstructionTests : InstructionTestBase
    {
        [Fact]
        public void DEX_decrements_Y()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 2;

            var dex = _instructions[Opcodes.DEX];
            var cycles = dex(bus.Object, registers.Object);
            registers.Object.X.Should().Be(1);
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void DEX_sets_the_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 0x1;

            var dex = _instructions[Opcodes.DEX];
            var cycles = dex(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void DEX_sets_the_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 0x0;

            var dex = _instructions[Opcodes.DEX];
            var cycles = dex(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0xff);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void DEY_decrements_Y()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 2;

            var dey = _instructions[Opcodes.DEY];
            var cycles = dey(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(1);
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void DEY_sets_the_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 0x1;

            var dey = _instructions[Opcodes.DEY];
            var cycles = dey(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void DEY_sets_the_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 0x0;

            var dey = _instructions[Opcodes.DEY];
            var cycles = dey(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0xff);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void INX_increments_X()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 1;

            var inx = _instructions[Opcodes.INX];
            var cycles = inx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(2);
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void INX_sets_the_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 0xff;

            var inx = _instructions[Opcodes.INX];
            var cycles = inx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void INX_sets_the_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 0x7F;

            var inx = _instructions[Opcodes.INX];
            var cycles = inx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0x80);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void INY_increments_Y()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 1;

            var iny = _instructions[Opcodes.INY];
            var cycles = iny(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(2);
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void INY_sets_the_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 0xff;

            var iny = _instructions[Opcodes.INY];
            var cycles = iny(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void INY_sets_the_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 0x7F;

            var iny = _instructions[Opcodes.INY];
            var cycles = iny(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0x80);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void DEC_Zero_decrements_value_memory()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(5);

            var registers = new Mock<IRegisters>();

            var dec = _instructions[Opcodes.DEC_ZERO];
            var cycles = dec(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 4));

            cycles.Should().Be(5);
        }

        [Fact]
        public void DEC_Zero_sets_zero_flag_to_true()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(1);

            var registers = new Mock<IRegisters>();

            var dec = _instructions[Opcodes.DEC_ZERO];
            var cycles = dec(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 0));
            registers.Verify(r => r.SetZeroFlag(true));

            cycles.Should().Be(5);
        }

        [Fact]
        public void DEC_Zero_sets_zero_flag_to_false()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(0xfe);

            var registers = new Mock<IRegisters>();

            var dec = _instructions[Opcodes.DEC_ZERO];
            var cycles = dec(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 0xfd));
            registers.Verify(r => r.SetZeroFlag(false));

            cycles.Should().Be(5);
        }

        [Fact]
        public void DEC_Zero_sets_negative_flag_to_false()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(0x1);

            var registers = new Mock<IRegisters>();

            var dec = _instructions[Opcodes.DEC_ZERO];
            var cycles = dec(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 0));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(5);
        }

        [Fact]
        public void DEC_Zero_sets_negative_flag_to_true()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(0x0);

            var registers = new Mock<IRegisters>();

            var dec = _instructions[Opcodes.DEC_ZERO];
            var cycles = dec(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 0xff));
            registers.Verify(r => r.SetNegativeFlag(true));

            cycles.Should().Be(5);
        }

        [Fact]
        public void DEC_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde).Returns(10);

            var registers = new Mock<IRegisters>();

            var dec = _instructions[Opcodes.DEC_ABS];
            var cycles = dec(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead, 9));

            cycles.Should().Be(6);
        }

        [Fact]
        public void DEC_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde).Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var dec = _instructions[Opcodes.DEC_ABS_X];
            var cycles = dec(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead + 10, 9));

            cycles.Should().Be(7);
        }

        [Fact]
        public void DEC_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xde).Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var dec = _instructions[Opcodes.DEC_ZERO_X];
            var cycles = dec(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xde + 5, 9));

            cycles.Should().Be(6);
        }

        [Fact]
        public void DEC_ZERO_X_wraps()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xde).Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var dec = _instructions[Opcodes.DEC_ZERO_X];
            var cycles = dec(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xde + 5, 9));

            cycles.Should().Be(6);
        }

        [Fact]
        public void INC_Zero_increments_value_memory()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(5);

            var registers = new Mock<IRegisters>();

            var inc = _instructions[Opcodes.INC_ZERO];
            var cycles = inc(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 6));

            cycles.Should().Be(5);
        }

        [Fact]
        public void INC_Zero_sets_zero_flag_to_true()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(0xff);

            var registers = new Mock<IRegisters>();

            var inc = _instructions[Opcodes.INC_ZERO];
            var cycles = inc(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 0));
            registers.Verify(r => r.SetZeroFlag(true));

            cycles.Should().Be(5);
        }

        [Fact]
        public void INC_Zero_sets_zero_flag_to_false()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(0xfe);

            var registers = new Mock<IRegisters>();

            var inc = _instructions[Opcodes.INC_ZERO];
            var cycles = inc(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 0xff));
            registers.Verify(r => r.SetZeroFlag(false));

            cycles.Should().Be(5);
        }

        [Fact]
        public void INC_Zero_sets_negative_flag_to_false()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(0x1);

            var registers = new Mock<IRegisters>();

            var inc = _instructions[Opcodes.INC_ZERO];
            var cycles = inc(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 2));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(5);
        }

        [Fact]
        public void INC_Zero_sets_negative_flag_to_true()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(0x7f);

            var registers = new Mock<IRegisters>();

            var inc = _instructions[Opcodes.INC_ZERO];
            var cycles = inc(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 0x80));
            registers.Verify(r => r.SetNegativeFlag(true));

            cycles.Should().Be(5);
        }

        [Fact]
        public void INC_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde).Returns(10);

            var registers = new Mock<IRegisters>();

            var inc = _instructions[Opcodes.INC_ABS];
            var cycles = inc(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead, 11));

            cycles.Should().Be(6);
        }

        [Fact]
        public void INC_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde).Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var inc = _instructions[Opcodes.INC_ABS_X];
            var cycles = inc(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead + 10, 11));

            cycles.Should().Be(7);
        }

        [Fact]
        public void INC_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xde).Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var inc = _instructions[Opcodes.INC_ZERO_X];
            var cycles = inc(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xde + 5, 11));

            cycles.Should().Be(6);
        }

        [Fact]
        public void INC_ZERO_X_wraps()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xff).Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var inc = _instructions[Opcodes.INC_ZERO_X];
            var cycles = inc(bus.Object, registers.Object);
            bus.Verify(b => b.Write(4, 11));

            cycles.Should().Be(6);
        }
    }
}
