using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class LogicTests(InstructionsFixture fixture) : InstructionTestBase(fixture)
    {
        [Fact]
        public void AND_IMM_does_not_change_A_when_bitmask_is_FF()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x2A;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var and = Instructions[Opcodes.AND_IMM];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x2A);
        }

        [Fact]
        public void AND_IMM_zeroes_out_A_when_bitmask_is_0()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xFF;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

            var and = Instructions[Opcodes.AND_IMM]; 
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);
        }

        [Fact]
        public void AND_IMM_sets_zero_flag_when_bitmask_is_0()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xFF;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

            var and = Instructions[Opcodes.AND_IMM];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }

        [Fact]
        public void AND_IMM_sets_zero_flag_when_acc_is_0()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var and = Instructions[Opcodes.AND_IMM];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }

        [Fact]
        public void AND_IMM_sets_negative_flag_when_result_is_negative()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var and = Instructions[Opcodes.AND_IMM];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }

        [Fact]
        public void AND_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xF0);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0xAA;

            var and = Instructions[Opcodes.AND_IND_X];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xa0);
            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
        }

        [Fact]
        public void AND_ZERO()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0xf0);
            var and = Instructions[Opcodes.AND_ZERO];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xa0);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetNegativeFlag(true));
        }

        [Fact]
        public void AND_ZERO_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            registers.Object.X = 5;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee)
                .Returns(0xf0);
            var and = Instructions[Opcodes.AND_ZERO_X];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xa0);

            bus.Verify(b => b.Read(10+5));
            registers.Verify(r => r.SetNegativeFlag(true));
        }

        [Fact]
        public void AND_ZERO_X_wraps()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            registers.Object.X = 5;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0xee) // dummy read
                .Returns(0xf0);
            var and = Instructions[Opcodes.AND_ZERO_X];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xa0);

            bus.Verify(b => b.Read(4));
            registers.Verify(r => r.SetNegativeFlag(true));
        }

        [Fact]
        public void AND_ABS()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0xf0);
            var and = Instructions[Opcodes.AND_ABS];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xa0);

            bus.Verify(b => b.Read(0xdead));
        }

        [Fact]
        public void AND_ABS_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            registers.Object.X = 10;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0xf0);
            var and = Instructions[Opcodes.AND_ABS_X];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xa0);

            bus.Verify(b => b.Read(0xdead + 10));
        }

        [Fact]
        public void AND_ABS_X_page_boundary_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            registers.Object.X = 10;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xee) // dummy read
                .Returns(0xf0);
            var and = Instructions[Opcodes.AND_ABS_X];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xa0);

            bus.Verify(b => b.Read(0x00ff + 10));
        }

        [Fact]
        public void AND_ABS_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            registers.Object.Y = 10;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0xf0);
            var and = Instructions[Opcodes.AND_ABS_Y];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xa0);

            bus.Verify(b => b.Read(0xdead + 10));
        }

        [Fact]
        public void AND_ABS_Y_page_boundary_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            registers.Object.Y = 10;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xee) // dummy read
                .Returns(0xf0);
            var and = Instructions[Opcodes.AND_ABS_Y];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xa0);

            bus.Verify(b => b.Read(0x00ff + 10));
        }

        [Fact]
        public void AND_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 0xaa;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address
                .Returns(0xde) // high address
                .Returns(0xf0); // value at location final location

            var and = Instructions[Opcodes.AND_IND_Y];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xa0);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());
        }

        [Fact]
        public void AND_indirect_Y_page_cross_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 0xaa;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xff) //low address
                .Returns(0x00) // high address
                .Returns(0xee) // dummy read
                .Returns(0xf0);

            var and = Instructions[Opcodes.AND_IND_Y];
            and(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xa0);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0x00ff + 5), Times.Once());
        }

        [Fact]
        public void BIT_ZERO_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0x80); //0x0010 address on zero page, then the value
            var sut = Instructions[Opcodes.BIT_ZERO];
            sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetNegativeFlag(true));
        }

        [Fact]
        public void BIT_ZERO_sets_overflow_flag()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0x40); //0x0010 address on zero page, then the value
            var sut = Instructions[Opcodes.BIT_ZERO];
            sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetOverflowFlag(true));
        }

        [Fact]
        public void BIT_ZERO_sets_both_negative_and_overflow_flag()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0xC0); //0x0010 address on zero page, then the value
            var sut = Instructions[Opcodes.BIT_ZERO];
            sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetNegativeFlag(true));
            registers.Verify(r => r.SetOverflowFlag(true));
        }

        [Fact]
        public void BIT_ZERO_sets_neither_negative_and_overflow_flag()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0x0); //0x0010 address on zero page, then the value
            var sut = Instructions[Opcodes.BIT_ZERO];
            sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetOverflowFlag(false));
        }

        [Fact]
        public void BIT_ZERO_sets_zero_flag_when_A_is_zero()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.A).Returns(0);

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0xFF); //0x0010 address on zero page, then the value
            var sut = Instructions[Opcodes.BIT_ZERO];
            sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetNegativeFlag(true));
            registers.Verify(r => r.SetOverflowFlag(true));
            registers.Verify(r => r.SetZeroFlag(true));
        }

        [Fact]
        public void BIT_ZERO_sets_zero_flag_when_value_is_zero()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.A).Returns(0xFF);

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0); //0x0010 address on zero page, then the value
            var sut = Instructions[Opcodes.BIT_ZERO];
            sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetZeroFlag(true));
        }

        [Fact]
        public void BIT_ZERO_sets_zero_flag_to_false_when_A_is_not_zero()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.A).Returns(0xFF);

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0xFF); //0x0010 address on zero page, then the value
            var sut = Instructions[Opcodes.BIT_ZERO];
            sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetZeroFlag(false));
        }

        [Fact]
        public void BIT_ZERO_sets_zero_flag_to_true_when_bit_patterns_do_not_match()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.A).Returns(0xAA);

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0x55); //0x0010 address on zero page, then the value
            var sut = Instructions[Opcodes.BIT_ZERO];
            sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetZeroFlag(true));
        }

        [Fact]
        public void BIT_ZERO_sets_zero_flag_to_false_when_bit_patterns_match()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.A).Returns(0xAA);

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0xAA); //0x0010 address on zero page, then the value
            var sut = Instructions[Opcodes.BIT_ZERO];
            sut(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetZeroFlag(false));
        }

        [Fact]
        public void BIT_ABS()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0);

            var bit = Instructions[Opcodes.BIT_ABS];
            bit(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff); // does not touch register
            bus.Verify(b => b.Read(0xdead), Times.Once());
            registers.Verify(r => r.SetZeroFlag(true));
        }

        [Fact]
        public void EOR_IMM_does_not_change_A_when_bitmask_is_00()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x2A;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x00);

            var eor = Instructions[Opcodes.EOR_IMM];
            registers.Object.A.Should().Be(0x2A);
            eor(bus.Object, registers.Object);
        }

        [Fact]
        public void EOR_IMM_zeroes_out_A_when_bitmask_is_the_same()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xde;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xde);

            var eor = Instructions[Opcodes.EOR_IMM];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);
        }

        [Fact]
        public void EOR_IMM_sets_zero_flag_when_bitmask_is_the_same()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xFF;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xFF);

            var eor = Instructions[Opcodes.EOR_IMM];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }

        [Fact]
        public void EOR_IMM_sets_negative_flag_when_result_is_negative()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x00;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var eor = Instructions[Opcodes.EOR_IMM];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }

        [Fact]
        public void EOR_IMM_sets_the_correct_value()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x55);

            var eor = Instructions[Opcodes.EOR_IMM];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }

        [Fact]
        public void EOR_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xFF);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0xAA;

            var eor = Instructions[Opcodes.EOR_IND_X];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x55);
            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
        }

        [Fact]
        public void EOR_Zero()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0x10)
                .Returns(0xFF);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;

            var eor = Instructions[Opcodes.EOR_ZERO];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x55);
            bus.Verify(b => b.Read(0x10), Times.Once());
        }

        [Fact]
        public void EOR_Zero_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0x10)
                .Returns(0xee) // dummy read
                .Returns(0xFF);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            registers.Object.X = 5;

            var eor = Instructions[Opcodes.EOR_ZERO_X];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x55);
            bus.Verify(b => b.Read(0x10 + 5), Times.Once());
        }

        [Fact]
        public void EOR_Zero_X_wraps()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0xee) // dummy read
                .Returns(0xFF);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            registers.Object.X = 5;

            var eor = Instructions[Opcodes.EOR_ZERO_X];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x55);
            bus.Verify(b => b.Read(4), Times.Once());
        }

        [Fact]
        public void EOR_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xFF);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;

            var eor = Instructions[Opcodes.EOR_ABS];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x55);
            bus.Verify(b => b.Read(0xdead), Times.Once());
        }

        [Fact]
        public void EOR_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xFF);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            registers.Object.X = 10;

            var eor = Instructions[Opcodes.EOR_ABS_X];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x55);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void EOR_ABS_X_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xee) // dummy read on page cross
                .Returns(0xFF);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            registers.Object.X = 10;

            var eor = Instructions[Opcodes.EOR_ABS_X];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x55);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());
        }

        [Fact]
        public void EOR_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xFF);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            registers.Object.Y = 10;

            var eor = Instructions[Opcodes.EOR_ABS_Y];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x55);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void EOR_ABS_Y_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xEE) // dummy read on page cross
                .Returns(0xFF);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xAA;
            registers.Object.Y = 10;

            var eor = Instructions[Opcodes.EOR_ABS_Y];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x55);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());
        }

        [Fact]
        public void EOR_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 0xaa;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address
                .Returns(0xde) // high address
                .Returns(0xff); // value at location final location

            var eor = Instructions[Opcodes.EOR_IND_Y];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x55);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());
        }

        [Fact]
        public void EOR_indirect_Y_page_cross_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 0xaa;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xff) //low address
                .Returns(0x00) // high address
                .Returns(0xee) // dummy read on page cross
                .Returns(0xff);

            var eor = Instructions[Opcodes.EOR_IND_Y];
            eor(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x55);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0x00ff + 5), Times.Once());
        }

        [Fact]
        public void ORA_IMM_does_not_change_A_when_bitmask_is_00()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x2A;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x00);

            var ora = Instructions[Opcodes.ORA_IMM];
            registers.Object.A.Should().Be(0x2A);
            ora(bus.Object, registers.Object);
        }

        [Fact]
        public void ORA_IMM_fills_A_with_ones_when_bitmask_is_FF()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var ora = Instructions[Opcodes.ORA_IMM];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);
        }

        [Fact]
        public void ORA_IMM_sets_zero_flag_when_result_is_0()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

            var ora = Instructions[Opcodes.ORA_IMM];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }

        [Fact]
        public void ORA_IMM_sets_negative_flag_when_result_is_negative()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var ora = Instructions[Opcodes.ORA_IMM];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());

            //Doesnt touch other flags
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }

        [Fact]
        public void ORA_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read on every call
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xaa);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0;

            var ora = Instructions[Opcodes.ORA_IND_X];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
        }

        [Fact]
        public void ORA_ZERO()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x80;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(10).Returns(0x7f);
            var ora = Instructions[Opcodes.ORA_ZERO];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            bus.Verify(b => b.Read(10));
            registers.Verify(r => r.SetNegativeFlag(true));
        }

        [Fact]
        public void ORA_ZERO_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x80;
            registers.Object.X = 10;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0x7f);
            var ora = Instructions[Opcodes.ORA_ZERO_X];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            bus.Verify(b => b.Read(10 + 10));
            registers.Verify(r => r.SetNegativeFlag(true));
        }

        [Fact]
        public void ORA_ZERO_X_wraps()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x80;
            registers.Object.X = 10;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0xee) // dummy read
                .Returns(0x7f);
            var ora = Instructions[Opcodes.ORA_ZERO_X];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            bus.Verify(b => b.Read(9));
            registers.Verify(r => r.SetNegativeFlag(true));
        }

        [Fact]
        public void ORA_ABS()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0xaa);
            var ora = Instructions[Opcodes.ORA_ABS];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);

            bus.Verify(b => b.Read(0xdead));
        }

        [Fact]
        public void ORA_ABS_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            registers.Object.X = 10;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xaa);
            var ora = Instructions[Opcodes.ORA_ABS_X];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);

            bus.Verify(b => b.Read(0xdead + 10));
        }

        [Fact]
        public void ORA_ABS_X_page_boundary_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            registers.Object.X = 10;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xee) // dummy read on page cross
                .Returns(0xaa);
            var ora = Instructions[Opcodes.ORA_ABS_X];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);

            bus.Verify(b => b.Read(0x00ff + 10));
        }

        [Fact]
        public void ORA_ABS_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            registers.Object.Y = 10;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0xaa);
            var ora = Instructions[Opcodes.ORA_ABS_Y];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);

            bus.Verify(b => b.Read(0xdead + 10));
        }

        [Fact]
        public void ORA_ABS_Y_page_boundary_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            registers.Object.Y = 10;
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xee) // dummy read on page cross
                .Returns(0xaa);
            var ora = Instructions[Opcodes.ORA_ABS_Y];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);

            bus.Verify(b => b.Read(0x00ff + 10));
        }

        [Fact]
        public void ORA_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 0x80;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address
                .Returns(0xde) // high address
                .Returns(0x7f); // value at location final location

            var ora = Instructions[Opcodes.ORA_IND_Y];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());
        }

        [Fact]
        public void ORA_indirect_Y_page_cross_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xff) //low address
                .Returns(0x00) // high address
                .Returns(0xee) // dummy read on page cross
                .Returns(0xaa);

            var ora = Instructions[Opcodes.ORA_IND_Y];
            ora(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0x00ff + 5), Times.Once());
        }
    }
}
