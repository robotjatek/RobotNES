using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class ArtitmeticTests : InstructionTestBase
    {
        [Fact]
        public void ADC_IMM_adds_two_numbers()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var adc = _instructions[Opcodes.ADC_IMM];
            var cycles = adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(30);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Once());

            cycles.Should().Be(2);
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

            var adc = _instructions[Opcodes.ADC_IMM];
            var cycles = adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(31);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Once());

            cycles.Should().Be(2);
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

            var adc = _instructions[Opcodes.ADC_IMM];
            var cycles = adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(3);
            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
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

            var adc = _instructions[Opcodes.ADC_IMM];
            var cycles = adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once());
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Once());

            cycles.Should().Be(2);
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

            var adc = _instructions[Opcodes.ADC_IMM];
            var cycles = adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(255);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
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

            var adc = _instructions[Opcodes.ADC_IMM];
            var cycles = adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(3);
            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void ADC_IMM_sets_overflow_flag2()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(3);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 125;

            var adc = _instructions[Opcodes.ADC_IMM];
            var cycles = adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(128);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void ADC_IMM_sets_overflow_flag3()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x7F);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 1;

            var adc = _instructions[Opcodes.ADC_IMM];
            var cycles = adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x80);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once());
            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void ADC_IMM_sets_overflow_flag4()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x7F);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x7f;

            var adc = _instructions[Opcodes.ADC_IMM];
            var cycles = adc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(unchecked((byte)-2));
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void ADC_bug_0x80_plus_0x7f_should_set_the_overflow_flag()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0x80);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x7f;
            registers.Setup(r => r.GetNegativeFlag()).Returns(false);
            registers.Setup(r => r.GetOverflowFlag()).Returns(true);
            registers.Setup(r => r.GetDecimalFlag()).Returns(true);
            registers.Setup(r => r.GetZeroFlag()).Returns(false);
            registers.Setup(r => r.GetCarryFlag()).Returns(false);

            var adc = _instructions[Opcodes.ADC_IMM];
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
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0x80);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x7f;
            registers.Setup(r => r.GetNegativeFlag()).Returns(false);
            registers.Setup(r => r.GetOverflowFlag()).Returns(false);
            registers.Setup(r => r.GetDecimalFlag()).Returns(true);
            registers.Setup(r => r.GetZeroFlag()).Returns(false);
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var adc = _instructions[Opcodes.ADC_IMM];
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 20;

            var adc = _instructions[Opcodes.ADC_IND_X];
            var cycles = adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());

            cycles.Should().Be(6);
        }

        [Fact]
        public void ADC_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var adc = _instructions[Opcodes.ADC_ZERO];
            var cycles = adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0x10), Times.Once());

            cycles.Should().Be(3);
        }

        [Fact]
        public void ADC_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 5;

            var adc = _instructions[Opcodes.ADC_ZERO_X];
            var cycles = adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0x10 + 5), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void ADC_ZERO_X_wraps()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xff)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 5;

            var adc = _instructions[Opcodes.ADC_ZERO_X];
            var cycles = adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(4), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void ADC_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var adc = _instructions[Opcodes.ADC_ABS];
            var cycles = adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0xdead), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void ADC_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 10;

            var adc = _instructions[Opcodes.ADC_ABS_X];
            var cycles = adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void ADC_ABS_X_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 10;

            var adc = _instructions[Opcodes.ADC_ABS_X];
            var cycles = adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void ADC_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var adc = _instructions[Opcodes.ADC_ABS_Y];
            var cycles = adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void ADC_ABS_Y_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var adc = _instructions[Opcodes.ADC_ABS_Y];
            var cycles = adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(30);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void LDA_indirect_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address
                .Returns(0xde) // high address
                .Returns(0xaa); // value at location 0xdead

            var lda = _instructions[Opcodes.LDA_IND_X];
            var cycles = lda(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());

            cycles.Should().Be(6);
        }

        [Fact]
        public void ADC_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 1;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address
                .Returns(0xde) // high address
                .Returns(10); // value at location final location

            var adc = _instructions[Opcodes.ADC_IND_Y];
            var cycles = adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(11);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void ADC_indirect_Y_page_cross_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 1;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0xff) //low address
                .Returns(0x00) // high address
                .Returns(10);

            var adc = _instructions[Opcodes.ADC_IND_Y];
            var cycles = adc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(11);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0x00ff + 5), Times.Once());

            cycles.Should().Be(6);
        }

        [Fact]
        public void CMP_IMM_sets_zero_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = _instructions[Opcodes.CMP_IMM];
            var cycles = cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CMP_IMM_sets_carry_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = _instructions[Opcodes.CMP_IMM];
            var cycles = cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CMP_IMM_sets_carry_flag_when_a_is_larger_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 27;

            var cmp = _instructions[Opcodes.CMP_IMM];
            var cycles = cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CMP_IMM_sets_carry_flag_to_false_when_a_is_less_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = _instructions[Opcodes.CMP_IMM];
            var cycles = cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CMP_IMM_sets_zero_flag_to_false_when_the_two_numbers_are_not_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = _instructions[Opcodes.CMP_IMM];
            var cycles = cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CMP_IMM_sets_negative_flag_when_operand_is_larger_than_a()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 26;

            var cmp = _instructions[Opcodes.CMP_IMM];
            var cycles = cmp(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CMP_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 20;

            var cmp = _instructions[Opcodes.CMP_IND_X];
            var cycles = cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());

            cycles.Should().Be(6);
        }

        [Fact]
        public void CMP_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 20;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address
                .Returns(0xde) // high address
                .Returns(10); // value at location final location

            var cmp = _instructions[Opcodes.CMP_IND_Y];
            var cycles = cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void CMP_indirect_Y_page_cross_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 20;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0xff) //low address
                .Returns(0x00) // high address
                .Returns(10);

            var cmp = _instructions[Opcodes.CMP_IND_Y];
            var cycles = cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0x00ff + 5), Times.Once());

            cycles.Should().Be(6);
        }

        [Fact]
        public void CMP_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var cmp = _instructions[Opcodes.CMP_ZERO];
            var cycles = cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(10), Times.Once());

            cycles.Should().Be(3);
        }

        [Fact]
        public void CMP_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 5;

            var cmp = _instructions[Opcodes.CMP_ZERO_X];
            var cycles = cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(10 + 5), Times.Once());

            cycles.Should().Be(4);
        }


        [Fact]
        public void CMP_ZERO_X_wraps()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xff)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 5;

            var cmp = _instructions[Opcodes.CMP_ZERO_X];
            var cycles = cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(4), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void CMP_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var cmp = _instructions[Opcodes.CMP_ABS];
            var cycles = cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(0xdead), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void CMP_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 10;

            var cmp = _instructions[Opcodes.CMP_ABS_X];
            var cycles = cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void CMP_ABS_X_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 10;

            var cmp = _instructions[Opcodes.CMP_ABS_X];
            var cycles = cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void CMP_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var cmp = _instructions[Opcodes.CMP_ABS_Y];
            var cycles = cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void CMP_ABS_Y_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var cmp = _instructions[Opcodes.CMP_ABS_Y];
            var cycles = cmp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void CPX_IMM_sets_zero_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 26;

            var cpx = _instructions[Opcodes.CPX_IMM];
            var cycles = cpx(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CPX_IMM_sets_carry_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 26;

            var cpx = _instructions[Opcodes.CPX_IMM];
            var cycles = cpx(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CPX_IMM_sets_carry_flag_when_a_is_larger_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 27;

            var cpx = _instructions[Opcodes.CPX_IMM];
            var cycles = cpx(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CPX_IMM_sets_carry_flag_to_false_when_a_is_less_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 26;

            var cpx = _instructions[Opcodes.CPX_IMM];
            var cycles = cpx(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CPX_IMM_sets_zero_flag_to_false_when_the_two_numbers_are_not_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 26;

            var cpx = _instructions[Opcodes.CPX_IMM];
            var cycles = cpx(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CPX_IMM_sets_negative_flag_when_operand_is_larger_than_a()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 26;

            var cpx = _instructions[Opcodes.CPX_IMM];
            var cycles = cpx(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CPX_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.X = 20;

            var cpx = _instructions[Opcodes.CPX_ZERO];
            var cycles = cpx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(20);
            bus.Verify(b => b.Read(0x10), Times.Once());

            cycles.Should().Be(3);
        }

        [Fact]
        public void CPX_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.X = 20;

            var cpx = _instructions[Opcodes.CPX_ABS];
            var cycles = cpx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(20);
            bus.Verify(b => b.Read(0xdead), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void CPY_IMM_sets_zero_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 26;

            var cpy = _instructions[Opcodes.CPY_IMM];
            var cycles = cpy(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(true), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CPY_IMM_sets_carry_flag_when_the_two_numbers_are_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 26;

            var cpy = _instructions[Opcodes.CPY_IMM];
            var cycles = cpy(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CPY_IMM_sets_carry_flag_when_a_is_larger_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(26);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 27;

            var cpy = _instructions[Opcodes.CPY_IMM];
            var cycles = cpy(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CPY_IMM_sets_carry_flag_to_false_when_a_is_less_than_memory()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 26;

            var cpy = _instructions[Opcodes.CPY_IMM];
            var cycles = cpy(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CPY_IMM_sets_zero_flag_to_false_when_the_two_numbers_are_not_equal()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 26;

            var cpy = _instructions[Opcodes.CPY_IMM];
            var cycles = cpy(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CPY_IMM_sets_negative_flag_when_operand_is_larger_than_a()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(27);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 26;

            var cpy = _instructions[Opcodes.CPY_IMM];
            var cycles = cpy(bus.Object, registers.Object);

            registers.Verify(r => r.SetZeroFlag(false), Times.Once());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void CPY_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.Y = 20;

            var cpy = _instructions[Opcodes.CPY_ZERO];
            var cycles = cpy(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(20);
            bus.Verify(b => b.Read(0x10), Times.Once());

            cycles.Should().Be(3);
        }

        [Fact]
        public void CPY_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.Y = 20;

            var cpy = _instructions[Opcodes.CPY_ABS];
            var cycles = cpy(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(20);
            bus.Verify(b => b.Read(0xdead), Times.Once());

            cycles.Should().Be(4);
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

            var sub = _instructions[Opcodes.SBC_IMM];
            var cycles = sub(bus.Object, registers.Object);

            registers.Object.A.Should().Be(5);

            cycles.Should().Be(2);
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

            var sub = _instructions[Opcodes.SBC_IMM_EB];
            var cycles = sub(bus.Object, registers.Object);

            registers.Object.A.Should().Be(5);

            cycles.Should().Be(2);
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

            var sub = _instructions[Opcodes.SBC_IMM];
            var cycles = sub(bus.Object, registers.Object);

            registers.Object.A.Should().Be(4);

            cycles.Should().Be(2);
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

            var sub = _instructions[Opcodes.SBC_IMM];
            var cycles = sub(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
            registers.Object.A.Should().Be(5);

            cycles.Should().Be(2);
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

            var sub = _instructions[Opcodes.SBC_IMM];
            var cycles = sub(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());
            registers.Object.A.Should().Be(0x7F);

            cycles.Should().Be(2);
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

            var sub = _instructions[Opcodes.SBC_IMM];
            var cycles = sub(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());

            cycles.Should().Be(2);
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

            var sub = _instructions[Opcodes.SBC_IMM];
            var cycles = sub(bus.Object, registers.Object);

            registers.Object.A.Should().Be(unchecked((byte)-5));
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
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

            var sbc = _instructions[Opcodes.SBC_IMM];
            var cycles = sbc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0xff);
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());

            cycles.Should().Be(2);
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

            var sbc = _instructions[Opcodes.SBC_IMM];
            var cycles = sbc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(96);
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());

            cycles.Should().Be(2);
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

            var sbc = _instructions[Opcodes.SBC_IMM];
            var cycles = sbc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0xfe);
            registers.Verify(r => r.SetOverflowFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void SBC_CARRY_BUG()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<UInt16>())).Returns(0xec);

            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(false);
            registers.SetupAllProperties();
            registers.Object.A = 64;

            var sbc = _instructions[Opcodes.SBC_IMM];
            sbc(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x53);
            registers.Verify(r => r.SetCarryFlag(false));
        }

        [Fact]
        public void SBC_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)
                .Returns(0xad)
                .Returns(0xde)
                .Returns(5);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 20;
            registers.Setup(r => r.GetCarryFlag()).Returns(true);

            var sbc = _instructions[Opcodes.SBC_IND_X];
            var cycles = sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(15);
            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());

            cycles.Should().Be(6);
        }

        [Fact]
        public void SBC_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;

            var sbc = _instructions[Opcodes.SBC_ZERO];
            var cycles = sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0x10), Times.Once());

            cycles.Should().Be(3);
        }

        [Fact]
        public void SBC_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x10)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;
            registers.Object.X = 5;

            var sbc = _instructions[Opcodes.SBC_ZERO_X];
            var cycles = sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0x10 + 5), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void SBC_ZERO_X_wraps()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xff)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;
            registers.Object.X = 5;

            var sbc = _instructions[Opcodes.SBC_ZERO_X];
            var cycles = sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(4), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void SBC_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;

            var sbc = _instructions[Opcodes.SBC_ABS];
            var cycles = sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0xdead), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void SBC_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;
            registers.Object.X = 10;

            var sbc = _instructions[Opcodes.SBC_ABS_X];
            var cycles = sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void SBC_ABS_X_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;
            registers.Object.X = 10;

            var sbc = _instructions[Opcodes.SBC_ABS_X];
            var cycles = sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void SBC_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var sbc = _instructions[Opcodes.SBC_ABS_Y];
            var cycles = sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void SBC_ABS_Y_page_cross_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(10);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var sbc = _instructions[Opcodes.SBC_ABS_Y];
            var cycles = sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);
            bus.Verify(b => b.Read(0x00ff + 10), Times.Once());

            cycles.Should().Be(5);
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address
                .Returns(0xde) // high address
                .Returns(10); // value at location final location

            var sbc = _instructions[Opcodes.SBC_IND_Y];
            var cycles = sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());

            cycles.Should().Be(5);
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0xff) //low address
                .Returns(0x00) // high address
                .Returns(10);

            var sbc = _instructions[Opcodes.SBC_IND_Y];
            var cycles = sbc(bus.Object, registers.Object);
            registers.Object.A.Should().Be(10);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0x00ff + 5), Times.Once());

            cycles.Should().Be(6);
        }

        [Fact]
        public void DCP_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 20;

            var dcp = _instructions[Opcodes.DCP_IND_X];
            var cycles = dcp(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());

            bus.Verify(b => b.Write(0xdead, 14));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(8);
        }

        [Fact]
        public void DCP_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(15); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var dcp = _instructions[Opcodes.DCP_ZERO];
            var cycles = dcp(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad), Times.Once());
            bus.Verify(b => b.Write(0xad, 14));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(5);
        }

        [Fact]
        public void DCP_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(15); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 5;

            var dcp = _instructions[Opcodes.DCP_ZERO_X];
            var cycles = dcp(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad + 5), Times.Once());
            bus.Verify(b => b.Write(0xad + 5, 14));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(6);
        }

        [Fact]
        public void DCP_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;

            var dcp = _instructions[Opcodes.DCP_ABS];
            var cycles = dcp(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Write(0xdead, 14));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(6);
        }

        [Fact]
        public void DCP_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 10;

            var dcp = _instructions[Opcodes.DCP_ABS_X];
            var cycles = dcp(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
            bus.Verify(b => b.Write(0xdead + 10, 14));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(7);
        }

        [Fact]
        public void DCP_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var dcp = _instructions[Opcodes.DCP_ABS_Y];
            var cycles = dcp(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
            bus.Verify(b => b.Write(0xdead + 10, 14));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(7);
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0xad) //low address
                .Returns(0xde) // high address
                .Returns(15); // value at location final location

            var dcp = _instructions[Opcodes.DCP_IND_Y];
            var cycles = dcp(bus.Object, registers.Object);
            registers.Object.A.Should().Be(20);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());

            bus.Verify(b => b.Write(0xdead + 5, 14));

            cycles.Should().Be(8);
        }

        [Fact]
        public void ISB_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.X = 5;
            registers.Object.A = 20;

            var isb = _instructions[Opcodes.ISB_IND_X];
            var cycles = isb(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());

            bus.Verify(b => b.Write(0xdead, 16));
            registers.Object.A.Should().Be(4);

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(8);
        }

        [Fact]
        public void ISB_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(15); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;

            var isb = _instructions[Opcodes.ISB_ZERO];
            var cycles = isb(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad), Times.Once());
            bus.Verify(b => b.Write(0xad, 16));
            registers.Object.A.Should().Be(4);

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(5);
        }

        [Fact]
        public void ISB_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.Object.A = 20;

            var isb = _instructions[Opcodes.ISB_ABS];
            var cycles = isb(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Write(0xdead, 16));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(6);
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
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0xad) //low address
                .Returns(0xde) // high address
                .Returns(15); // value at location final location

            var isb = _instructions[Opcodes.ISB_IND_Y];
            var cycles = isb(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());

            bus.Verify(b => b.Write(0xdead + 5, 16));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(8);
        }

        [Fact]
        public void ISB_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(15); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 5;

            var isb = _instructions[Opcodes.ISB_ZERO_X];
            var cycles = isb(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(0xad + 5), Times.Once());
            bus.Verify(b => b.Write(0xad + 5, 16));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(6);
        }

        [Fact]
        public void ISB_ABS_Y()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.Y = 10;

            var isb = _instructions[Opcodes.ISB_ABS_Y];
            var cycles = isb(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
            bus.Verify(b => b.Write(0xdead + 10, 16));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(7);
        }

        [Fact]
        public void ISB_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(15); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 20;
            registers.Object.X = 10;

            var isb = _instructions[Opcodes.ISB_ABS_X];
            var cycles = isb(bus.Object, registers.Object);
            registers.Object.A.Should().Be(4);

            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
            bus.Verify(b => b.Write(0xdead + 10, 16));

            registers.Verify(r => r.SetZeroFlag(false));
            registers.Verify(r => r.SetCarryFlag(true));
            registers.Verify(r => r.SetNegativeFlag(false));

            cycles.Should().Be(7);
        }

        [Fact]
        public void SLO_IND_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0;

            var slo = _instructions[Opcodes.SLO_IND_X];
            var cycles = slo(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());

            bus.Verify(b => b.Write(0xdead, 16));
            registers.Object.A.Should().Be(16);

            cycles.Should().Be(8);
        }

        [Fact]
        public void SLO_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(8); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;

            var slo = _instructions[Opcodes.SLO_ZERO];
            var cycles = slo(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad), Times.Once());
            bus.Verify(b => b.Write(0xad, 16));
            registers.Object.A.Should().Be(16);

            cycles.Should().Be(5);
        }

        [Fact]
        public void SLO_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(8); //value at 0xdead

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;

            var slo = _instructions[Opcodes.SLO_ABS];
            var cycles = slo(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(0xdead), Times.Once());
            bus.Verify(b => b.Write(0xdead, 16));


            cycles.Should().Be(6);
        }

        [Fact]
        public void SLO_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 0;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0xad) //low address
                .Returns(0xde) // high address
                .Returns(8); // value at location final location

            var slo = _instructions[Opcodes.SLO_IND_Y];
            var cycles = slo(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());

            bus.Verify(b => b.Write(0xdead + 5, 16));

            cycles.Should().Be(8);
        }

        [Fact]
        public void SLO_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(8); //value at 0xad

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0;
            registers.Object.X = 5;

            var slo = _instructions[Opcodes.SLO_ZERO_X];
            var cycles = slo(bus.Object, registers.Object);
            registers.Object.A.Should().Be(16);

            bus.Verify(b => b.Read(0xad + 5), Times.Once());
            bus.Verify(b => b.Write(0xad + 5, 16));

            cycles.Should().Be(6);
        }
    }
}
