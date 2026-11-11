using Moq;
using NESCore;
using NESCore.CPU;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class ArithmeticTests(InstructionsFixture fixture) : InstructionTestBase(fixture)
    {
        [Fact]
        public void ADC_IMM_adds_two_numbers()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var adc = Instructions[Opcodes.ADC_IMM];
            adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(30);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Once());
        }

        [Fact]
        public void ADC_IMM_adds_carry_to_the_result()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var adc = Instructions[Opcodes.ADC_IMM];
            adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(31);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Once());
        }

        [Fact]
        public void ADC_IMM_sets_carry_bit_to_true()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(6);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 253;
            registers.Setup(r => r.GetCarryFlag()).Returns(false);

            var adc = Instructions[Opcodes.ADC_IMM];
            adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(3);
            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
        }

        [Fact]
        public void ADC_IMM_sets_zero_bit_to_true()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;
            registers.Setup(r => r.GetCarryFlag()).Returns(false);

            var adc = Instructions[Opcodes.ADC_IMM];
            adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once());
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Once());
        }


        [Fact]
        public void ADC_IMM_sets_negative_bit_to_true()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(253);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 2;
            registers.Setup(r => r.GetCarryFlag()).Returns(false);

            var adc = Instructions[Opcodes.ADC_IMM];
            adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(255);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
        }

        [Fact]
        public void ADC_IMM_resets_negative_bit_sets_carry()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(6);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 253;
            registers.Setup(r => r.GetNegativeFlag()).Returns(false);

            var adc = Instructions[Opcodes.ADC_IMM];
            adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(3);
            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
        }

        [Fact]
        public void ADC_IMM_sets_overflow_flag2()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(3);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 125;

            var adc = Instructions[Opcodes.ADC_IMM];
            adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(128);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());
        }

        [Fact]
        public void ADC_IMM_sets_overflow_flag3()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x7F);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 1;

            var adc = Instructions[Opcodes.ADC_IMM];
            adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x80);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());
        }

        [Fact]
        public void ADC_IMM_sets_overflow_flag4()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x7F);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x7f;

            var adc = Instructions[Opcodes.ADC_IMM];
            adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(unchecked((byte)-2));
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());
        }

        [Fact]
        public void ADC_bug_0x80_plus_0x7f_should_set_the_overflow_flag()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x80);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x7f;
            registers.Setup(r => r.GetNegativeFlag()).Returns(false);
            registers.Setup(r => r.GetOverflowFlag()).Returns(true);
            registers.Setup(r => r.GetDecimalFlag()).Returns(true);
            registers.Setup(r => r.GetZeroFlag()).Returns(false);
            registers.Setup(r => r.GetCarryFlag()).Returns(false);

            var adc = Instructions[Opcodes.ADC_IMM];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xff);

            registers.Verify(r => r.SetNegativeFlag(true));
            registers.Verify(r => r.SetOverflowFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(false));
        }

        [Fact]
        public void ADC_bug_adc_does_not_set_zero_flag_correctly()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x80);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x7f;
            registers.Setup(r => r.GetNegativeFlag()).Returns(false);
            registers.Setup(r => r.GetOverflowFlag()).Returns(false);
            registers.Setup(r => r.GetDecimalFlag()).Returns(true);
            registers.Setup(r => r.GetZeroFlag()).Returns(false);
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var adc = Instructions[Opcodes.ADC_IMM];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x0);

            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetOverflowFlag(false));
            registers.Verify(r => r.SetZeroFlag(true));
            registers.Verify(r => r.SetCarryFlag(true));
        }

        [Fact]
        public void ADC_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 20;

            var adc = Instructions[Opcodes.ADC_IND_X];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
        }

        [Fact]
        public void ADC_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0x10)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var adc = Instructions[Opcodes.ADC_ZERO];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0x10), Times.Once());
        }

        [Fact]
        public void ADC_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0x10)
                .Returns(0xee)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 5;

            var adc = Instructions[Opcodes.ADC_ZERO_X];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0x10 + 5), Times.Once());
        }

        [Fact]
        public void ADC_ZERO_X_wraps()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0xee)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 5;

            var adc = Instructions[Opcodes.ADC_ZERO_X];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(4), Times.Once());
        }

        [Fact]
        public void ADC_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var adc = Instructions[Opcodes.ADC_ABS];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0xdead), Times.Once());
        }

        [Fact]
        public void ADC_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 10;

            var adc = Instructions[Opcodes.ADC_ABS_X];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void ADC_ABS_X_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xee) // dummy read on page cross
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 10;

            var adc = Instructions[Opcodes.ADC_ABS_X];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());
        }

        [Fact]
        public void ADC_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var adc = Instructions[Opcodes.ADC_ABS_Y];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void ADC_ABS_Y_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xee) // dummy read on page cross
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var adc = Instructions[Opcodes.ADC_ABS_Y];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());
        }

        [Fact]
        public void LDA_indirect_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xee) // dummy read
                .Returns(0x0ad) //low address
                .Returns(0xde) // high address
                .Returns(0xaa); // value at location 0xdead

            var lda = Instructions[Opcodes.LDA_IND_X];
            lda(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());
        }

        [Fact]
        public void ADC_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 1;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address
                .Returns(0xde) // high address
                .Returns(10); // value at location final location

            var adc = Instructions[Opcodes.ADC_IND_Y];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(11);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());
        }

        [Fact]
        public void ADC_indirect_Y_page_cross_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 1;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xff) //low address
                .Returns(0x00) // high address
                .Returns(0xee) // dummy read on page cross
                .Returns(10);

            var adc = Instructions[Opcodes.ADC_IND_Y];
            adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(11);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0x00ff + 5), Times.Once());
        }

        [Fact]
        public void CMP_IMM_sets_zero_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = Instructions[Opcodes.CMP_IMM];
            cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
        }

        [Fact]
        public void CMP_IMM_sets_carry_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = Instructions[Opcodes.CMP_IMM];
            cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
        }

        [Fact]
        public void CMP_IMM_sets_carry_flag_when_a_is_larger_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 27;

            var cmp = Instructions[Opcodes.CMP_IMM];
            cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
        }

        [Fact]
        public void CMP_IMM_sets_carry_flag_to_false_when_a_is_less_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = Instructions[Opcodes.CMP_IMM];
            cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(false), Times.Once());
        }

        [Fact]
        public void CMP_IMM_sets_zero_flag_to_false_when_the_two_numbers_are_not_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = Instructions[Opcodes.CMP_IMM];
            cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
        }

        [Fact]
        public void CMP_IMM_sets_negative_flag_when_operand_is_larger_than_a()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = Instructions[Opcodes.CMP_IMM];
            cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
        }

        [Fact]
        public void CMP_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 20;

            var cmp = Instructions[Opcodes.CMP_IND_X];
            cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
        }

        [Fact]
        public void CMP_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 20;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address
                .Returns(0xde) // high address
                .Returns(10); // value at location final location

            var cmp = Instructions[Opcodes.CMP_IND_Y];
            cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());
        }

        [Fact]
        public void CMP_indirect_Y_page_cross_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 20;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xff) //low address
                .Returns(0x00) // high address
                .Returns(10);

            var cmp = Instructions[Opcodes.CMP_IND_Y];
            cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0x00ff + 5), Times.Once());
        }

        [Fact]
        public void CMP_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var cmp = Instructions[Opcodes.CMP_ZERO];
            cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(10), Times.Once());
        }

        [Fact]
        public void CMP_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 5;

            var cmp = Instructions[Opcodes.CMP_ZERO_X];
            cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(10 + 5), Times.Once());
        }


        [Fact]
        public void CMP_ZERO_X_wraps()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 5;

            var cmp = Instructions[Opcodes.CMP_ZERO_X];
            cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(4), Times.Once());
        }

        [Fact]
        public void CMP_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var cmp = Instructions[Opcodes.CMP_ABS];
            cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(0xdead), Times.Once());
        }

        [Fact]
        public void CMP_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 10;

            var cmp = Instructions[Opcodes.CMP_ABS_X];
            cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void CMP_ABS_X_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 10;

            var cmp = Instructions[Opcodes.CMP_ABS_X];
            cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());
        }

        [Fact]
        public void CMP_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var cmp = Instructions[Opcodes.CMP_ABS_Y];
            cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void CMP_ABS_Y_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var cmp = Instructions[Opcodes.CMP_ABS_Y];
            cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());
        }

        [Fact]
        public void CPX_IMM_sets_zero_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 26;

            var cpx = Instructions[Opcodes.CPX_IMM];
            cpx(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
        }

        [Fact]
        public void CPX_IMM_sets_carry_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 26;

            var cpx = Instructions[Opcodes.CPX_IMM];
            cpx(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
        }

        [Fact]
        public void CPX_IMM_sets_carry_flag_when_a_is_larger_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 27;

            var cpx = Instructions[Opcodes.CPX_IMM];
            cpx(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
        }

        [Fact]
        public void CPX_IMM_sets_carry_flag_to_false_when_a_is_less_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 26;

            var cpx = Instructions[Opcodes.CPX_IMM];
            cpx(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(false), Times.Once());
        }

        [Fact]
        public void CPX_IMM_sets_zero_flag_to_false_when_the_two_numbers_are_not_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 26;

            var cpx = Instructions[Opcodes.CPX_IMM];
            cpx(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
        }

        [Fact]
        public void CPX_IMM_sets_negative_flag_when_operand_is_larger_than_a()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 26;

            var cpx = Instructions[Opcodes.CPX_IMM];
            cpx(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
        }

        [Fact]
        public void CPX_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0x10)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.X = 20;

            var cpx = Instructions[Opcodes.CPX_ZERO];
            cpx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(20);
            bus.Verify(b => b.Read(0x10), Times.Once());
        }

        [Fact]
        public void CPX_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.X = 20;

            var cpx = Instructions[Opcodes.CPX_ABS];
            cpx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(20);
            bus.Verify(b => b.Read(0xdead), Times.Once());
        }

        [Fact]
        public void CPY_IMM_sets_zero_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 26;

            var cpy = Instructions[Opcodes.CPY_IMM];
            cpy(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
        }

        [Fact]
        public void CPY_IMM_sets_carry_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 26;

            var cpy = Instructions[Opcodes.CPY_IMM];
            cpy(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
        }

        [Fact]
        public void CPY_IMM_sets_carry_flag_when_a_is_larger_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 27;

            var cpy = Instructions[Opcodes.CPY_IMM];
            cpy(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
        }

        [Fact]
        public void CPY_IMM_sets_carry_flag_to_false_when_a_is_less_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 26;

            var cpy = Instructions[Opcodes.CPY_IMM];
            cpy(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(false), Times.Once());
        }

        [Fact]
        public void CPY_IMM_sets_zero_flag_to_false_when_the_two_numbers_are_not_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 26;

            var cpy = Instructions[Opcodes.CPY_IMM];
            cpy(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
        }

        [Fact]
        public void CPY_IMM_sets_negative_flag_when_operand_is_larger_than_a()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 26;

            var cpy = Instructions[Opcodes.CPY_IMM];
            cpy(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
        }

        [Fact]
        public void CPY_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0x10)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.Y = 20;

            var cpy = Instructions[Opcodes.CPY_ZERO];
            cpy(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(20);
            bus.Verify(b => b.Read(0x10), Times.Once());
        }

        [Fact]
        public void CPY_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.Y = 20;

            var cpy = Instructions[Opcodes.CPY_ABS];
            cpy(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(20);
            bus.Verify(b => b.Read(0xdead), Times.Once());
        }

        [Fact]
        public void SBC_IMM_subtracts_two_numbers()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(5);
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 10;

            var sub = Instructions[Opcodes.SBC_IMM];
            sub(bus.Object, registers.Object);

            registers.Object.A.Should().Be(5);
        }

        [Fact]
        public void SBC_IMM_EB()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(5);
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 10;

            var sub = Instructions[Opcodes.SBC_IMM_EB];
            sub(bus.Object, registers.Object);

            registers.Object.A.Should().Be(5);
        }

        [Fact]
        public void SBC_IMM_subtracts_carry_from_result()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(5);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 10;
            registers.Setup(r => r.GetCarryFlag()).Returns(false);

            var sub = Instructions[Opcodes.SBC_IMM];
            sub(bus.Object, registers.Object);

            registers.Object.A.Should().Be(4);
        }

        [Fact]
        public void SBC_IMM_sets_carry_bit_to_true()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(5);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 10;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var sub = Instructions[Opcodes.SBC_IMM];
            sub(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
            registers.Object.A.Should().Be(5);
        }

        [Fact]
        public void SBC_IMM_sets_carry_and_overflow_bit_to_true()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x80;
            registers.Setup(r => r.GetCarryFlag()).Returns(false);
            registers.Setup(r => r.GetOverflowFlag()).Returns(false);

            var sub = Instructions[Opcodes.SBC_IMM];
            sub(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());
            registers.Object.A.Should().Be(0x7F);
        }

        [Fact]
        public void SBC_IMM_sets_zero_bits_to_true()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 10;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var sub = Instructions[Opcodes.SBC_IMM];
            sub(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
        }

        [Fact]
        public void SBC_IMM_sets_negative_bit_to_true()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 5;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var sub = Instructions[Opcodes.SBC_IMM];
            sub(bus.Object, registers.Object);

            registers.Object.A.Should().Be(unchecked((byte)-5));
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
        }

        [Fact]
        public void SBC_IMM_sets_overflow()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x80);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x7F;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var sbc = Instructions[Opcodes.SBC_IMM];
            sbc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0xff);
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());
        }

        [Fact]
        public void SBC_IMM_sets_overflow2()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(112);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xd0;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var sbc = Instructions[Opcodes.SBC_IMM];
            sbc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(96);
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());
        }

        [Fact]
        public void SBC_IMM_sets_overflow3()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x80);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x7F;
            registers.Setup(r => r.GetCarryFlag()).Returns(false);

            var sbc = Instructions[Opcodes.SBC_IMM];
            sbc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0xfe);
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());
        }

        [Fact]
        public void SBC_CARRY_BUG()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xec);

            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(false);
            registers.SetupAllProperties();
            registers.Object.A = 64;

            var sbc = Instructions[Opcodes.SBC_IMM];
            sbc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x53);
            registers.Verify(r => r.SetCarryFlag(false));
        }

        [Fact]
        public void SBC_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0xad)
                .Returns(0xde)
                .Returns(5);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 20;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var sbc = Instructions[Opcodes.SBC_IND_X];
            sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(15);
            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
        }

        [Fact]
        public void SBC_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0x10)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;

            var sbc = Instructions[Opcodes.SBC_ZERO];
            sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0x10), Times.Once());
        }

        [Fact]
        public void SBC_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0x10)
                .Returns(0xee) // dummy read
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;
            registers.Object.X = 5;

            var sbc = Instructions[Opcodes.SBC_ZERO_X];
            sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0x10 + 5), Times.Once());
        }

        [Fact]
        public void SBC_ZERO_X_wraps()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0xee) // dummy read on page cross
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;
            registers.Object.X = 5;

            var sbc = Instructions[Opcodes.SBC_ZERO_X];
            sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(4), Times.Once());
        }

        [Fact]
        public void SBC_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;

            var sbc = Instructions[Opcodes.SBC_ABS];
            sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0xdead), Times.Once());
        }

        [Fact]
        public void SBC_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;
            registers.Object.X = 10;

            var sbc = Instructions[Opcodes.SBC_ABS_X];
            sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void SBC_ABS_X_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xee) // dummy read on page cross
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;
            registers.Object.X = 10;

            var sbc = Instructions[Opcodes.SBC_ABS_X];
            sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());
        }

        [Fact]
        public void SBC_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var sbc = Instructions[Opcodes.SBC_ABS_Y];
            sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void SBC_ABS_Y_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xee) // dummy read on page cross
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var sbc = Instructions[Opcodes.SBC_ABS_Y];
            sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());
        }

        [Fact]
        public void SBC_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 20;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address
                .Returns(0xde) // high address
                .Returns(10); // value at location final location

            var sbc = Instructions[Opcodes.SBC_IND_Y];
            sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());
        }

        [Fact]
        public void SBC_indirect_Y_page_cross_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 20;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xff) //low address
                .Returns(0x00) // high address
                .Returns(0xee) // dummy read on page cross
                .Returns(10);

            var sbc = Instructions[Opcodes.SBC_IND_Y];
            sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0x00ff + 5), Times.Once());
        }

        [Fact]
        public void DCP_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 20;

            var dcp = Instructions[Opcodes.DCP_IND_X];
            dcp(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());

            bus.Verify(b => b.Write(0xdead, 14));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void DCP_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(15); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var dcp = Instructions[Opcodes.DCP_ZERO];
            dcp(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad), Times.Once());
            bus.Verify(b => b.Write(0xad, 14));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void DCP_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xee) // dummy read
                .Returns(15); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 5;

            var dcp = Instructions[Opcodes.DCP_ZERO_X];
            dcp(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad + 5), Times.Once());
            bus.Verify(b => b.Write(0xad + 5, 14));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void DCP_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var dcp = Instructions[Opcodes.DCP_ABS];
            dcp(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Write(0xdead, 14));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void DCP_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 10;

            var dcp = Instructions[Opcodes.DCP_ABS_X];
            dcp(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead + 10), Times.Exactly(2));
            bus.Verify(b => b.Write(0xdead + 10, 14));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void DCP_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var dcp = Instructions[Opcodes.DCP_ABS_Y];
            dcp(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead + 10), Times.Exactly(2));
            bus.Verify(b => b.Write(0xdead + 10, 14));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void DCP_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 20;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xad) //low address
                .Returns(0xde) // high address
                .Returns(15); // value at location final location

            var dcp = Instructions[Opcodes.DCP_IND_Y];
            dcp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Exactly(2));

            bus.Verify(b => b.Write(0xdead + 5, 14));
        }

        [Fact]
        public void ISB_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.X = 5;
            registers.Object.A = 20;

            var isb = Instructions[Opcodes.ISB_IND_X];
            isb(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());

            bus.Verify(b => b.Write(0xdead, 16));
            registers.Object.A.Should().Be(4);

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void ISB_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(15); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;

            var isb = Instructions[Opcodes.ISB_ZERO];
            isb(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad), Times.Once());
            bus.Verify(b => b.Write(0xad, 16));
            registers.Object.A.Should().Be(4);

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void ISB_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;

            var isb = Instructions[Opcodes.ISB_ABS];
            isb(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Write(0xdead, 16));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void ISB_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 20;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xad) //low address
                .Returns(0xde) // high address
                .Returns(15); // value at location final location

            var isb = Instructions[Opcodes.ISB_IND_Y];
            isb(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Exactly(2));

            bus.Verify(b => b.Write(0xdead + 5, 16));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void ISB_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xee) // dummy read
                .Returns(15); // value at 0xad

            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 5;

            var isb = Instructions[Opcodes.ISB_ZERO_X];
            isb(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(0xad + 5), Times.Once());
            bus.Verify(b => b.Write(0xad + 5, 16));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void ISB_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var isb = Instructions[Opcodes.ISB_ABS_Y];
            isb(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(0xdead + 10), Times.Exactly(2));
            bus.Verify(b => b.Write(0xdead + 10, 16));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void ISB_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 10;

            var isb = Instructions[Opcodes.ISB_ABS_X];
            isb(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(0xdead + 10), Times.Exactly(2));
            bus.Verify(b => b.Write(0xdead + 10, 16));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));
        }

        [Fact]
        public void SLO_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0;

            var slo = Instructions[Opcodes.SLO_IND_X];
            slo(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());

            bus.Verify(b => b.Write(0xdead, 16));
            registers.Object.A.Should().Be(16);
        }

        [Fact]
        public void SLO_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(8); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;

            var slo = Instructions[Opcodes.SLO_ZERO];
            slo(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad), Times.Once());
            bus.Verify(b => b.Write(0xad, 16));
            registers.Object.A.Should().Be(16);
        }

        [Fact]
        public void SLO_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;

            var slo = Instructions[Opcodes.SLO_ABS];
            slo(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Write(0xdead, 16));
        }

        [Fact]
        public void SLO_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 0;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xad) //low address
                .Returns(0xde) // high address
                .Returns(8); // value at location final location

            var slo = Instructions[Opcodes.SLO_IND_Y];
            slo(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Exactly(2));

            bus.Verify(b => b.Write(0xdead + 5, 16));
        }

        [Fact]
        public void SLO_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xee) // dummy read
                .Returns(8); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;
            registers.Object.X = 5;

            var slo = Instructions[Opcodes.SLO_ZERO_X];
            slo(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(0xad + 5), Times.Once());
            bus.Verify(b => b.Write(0xad + 5, 16));
        }

        [Fact]
        public void SLO_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;
            registers.Object.Y = 10;

            var slo = Instructions[Opcodes.SLO_ABS_Y];
            slo(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(0xdead + 10), Times.Exactly(2));
            bus.Verify(b => b.Write(0xdead + 10, 16));
        }

        [Fact]
        public void SLO_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;
            registers.Object.X = 10;

            var slo = Instructions[Opcodes.SLO_ABS_X];
            slo(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(0xdead + 10), Times.Exactly(2));
            bus.Verify(b => b.Write(0xdead + 10, 16));
        }

        [Fact]
        public void RLA_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0xff;

            var rla = Instructions[Opcodes.RLA_IND_X];
            rla(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());

            bus.Verify(b => b.Write(0xdead, 16));
            registers.Object.A.Should().Be(16);
        }

        [Fact]
        public void RLA_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(8); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;

            var rla = Instructions[Opcodes.RLA_ZERO];
            rla(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad), Times.Once());
            bus.Verify(b => b.Write(0xad, 16));
            registers.Object.A.Should().Be(16);
        }

        [Fact]
        public void RLA_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;

            var rla = Instructions[Opcodes.RLA_ABS];
            rla(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Write(0xdead, 16));
        }

        [Fact]
        public void RLA_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 0xff;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xad) //low address
                .Returns(0xde) // high address
                .Returns(8); // value at location final location

            var rla = Instructions[Opcodes.RLA_IND_Y];
            rla(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Exactly(2));

            bus.Verify(b => b.Write(0xdead + 5, 16));
        }

        [Fact]
        public void RLA_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xee) // dummy read
                .Returns(8); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            registers.Object.X = 5;

            var rla = Instructions[Opcodes.RLA_ZERO_X];
            rla(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(0xad + 5), Times.Once());
            bus.Verify(b => b.Write(0xad + 5, 16));
        }

        [Fact]
        public void RLA_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            registers.Object.Y = 10;

            var rla = Instructions[Opcodes.RLA_ABS_Y];
            rla(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(0xdead + 10), Times.Exactly(2));
            bus.Verify(b => b.Write(0xdead + 10, 16));
        }

        [Fact]
        public void RLA_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            registers.Object.X = 10;

            var rla = Instructions[Opcodes.RLA_ABS_X];
            rla(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(0xdead + 10), Times.Exactly(2));
            bus.Verify(b => b.Write(0xdead + 10, 16));
        }

        [Fact]
        public void SRE_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0xff;

            var sre = Instructions[Opcodes.SRE_IND_X];
            sre(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());

            bus.Verify(b => b.Write(0xdead, 4));
            registers.Object.A.Should().Be(0xfb);
        }

        [Fact]
        public void SRE_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(8); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;

            var sre = Instructions[Opcodes.SRE_ZERO];
            sre(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad), Times.Once());
            bus.Verify(b => b.Write(0xad, 4));
            registers.Object.A.Should().Be(0xfb);
        }

        [Fact]
        public void SRE_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;

            var sre = Instructions[Opcodes.SRE_ABS];
            sre(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xfb);

            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Write(0xdead, 4));
        }

        [Fact]
        public void SRE_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 0xff;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xad) //low address
                .Returns(0xde) // high address
                .Returns(8); // value at location final location

            var sre = Instructions[Opcodes.SRE_IND_Y];
            sre(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xfb);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Exactly(2));

            bus.Verify(b => b.Write(0xdead + 5, 4));
        }

        [Fact]
        public void SRE_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xee) // dummy read
                .Returns(8); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            registers.Object.X = 5;

            var sre = Instructions[Opcodes.SRE_ZERO_X];
            sre(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xfb);

            bus.Verify(b => b.Read(0xad + 5), Times.Once());
            bus.Verify(b => b.Write(0xad + 5, 4));
        }

        [Fact]
        public void SRE_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            registers.Object.Y = 10;

            var sre = Instructions[Opcodes.SRE_ABS_Y];
            sre(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xfb);

            bus.Verify(b => b.Read(0xdead + 10), Times.Exactly(2));
            bus.Verify(b => b.Write(0xdead + 10, 4));
        }

        [Fact]
        public void SRE_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            registers.Object.X = 10;

            var sre = Instructions[Opcodes.SRE_ABS_X];
            sre(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xfb);

            bus.Verify(b => b.Read(0xdead + 10), Times.Exactly(2));
            bus.Verify(b => b.Write(0xdead + 10, 4));
        }

        [Fact]
        public void RRA_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0;

            var rra = Instructions[Opcodes.RRA_IND_X];
            rra(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());

            bus.Verify(b => b.Write(0xdead, 4));
            registers.Object.A.Should().Be(4);
        }

        [Fact]
        public void RRA_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(8); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;

            var rra = Instructions[Opcodes.RRA_ZERO];
            rra(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad), Times.Once());
            bus.Verify(b => b.Write(0xad, 4));
            registers.Object.A.Should().Be(4);
        }

        [Fact]
        public void RRA_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;

            var rra = Instructions[Opcodes.RRA_ABS];
            rra(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Write(0xdead, 4));
        }

        [Fact]
        public void RRA_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 0;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xad) //low address
                .Returns(0xde) // high address
                .Returns(8); // value at location final location

            var rra = Instructions[Opcodes.RRA_IND_Y];
            rra(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Exactly(2));

            bus.Verify(b => b.Write(0xdead + 5, 4));
        }

        [Fact]
        public void RRA_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xee) // dummy read
                .Returns(8); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;
            registers.Object.X = 5;

            var rra = Instructions[Opcodes.RRA_ZERO_X];
            rra(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(0xad + 5), Times.Once());
            bus.Verify(b => b.Write(0xad + 5, 4));
        }

        [Fact]
        public void RRA_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;
            registers.Object.Y = 10;

            var rra = Instructions[Opcodes.RRA_ABS_Y];
            rra(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(0xdead + 10), Times.Exactly(2));
            bus.Verify(b => b.Write(0xdead + 10, 4));
        }

        [Fact]
        public void RRA_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;
            registers.Object.X = 10;

            var rra = Instructions[Opcodes.RRA_ABS_X];
            rra(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(0xdead + 10), Times.Exactly(2));
            bus.Verify(b => b.Write(0xdead + 10, 4));
        }
    }
}
