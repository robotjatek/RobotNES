using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class LDATests : InstructionTestBase
    {
        [Fact]
        public void LDA_IMM()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x10);

            var lda_imm = _instructions[Opcodes.LDA_IMM];
            var cycles = lda_imm(bus.Object, registers.Object);
            registers.VerifySet(r => r.A = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            cycles.Should().Be(2);

        }

        [Fact]
        public void LDA_IMM_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

            var lda_imm = _instructions[Opcodes.LDA_IMM];
            var cycles = lda_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.A = 0);
            registers.Verify(r => r.SetZeroFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDA_IMM_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-1));

            var lda_imm = _instructions[Opcodes.LDA_IMM];
            var cycles = lda_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.A = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDA_ABS()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x10);

            var lda_abs = _instructions[Opcodes.LDA_ABS];
            var cycles = lda_abs(bus.Object, registers.Object);
            registers.VerifySet(r => r.A = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0xdead));
            cycles.Should().Be(4);

        }

        [Fact]
        public void LDA_ABS_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0);

            var lda_abs = _instructions[Opcodes.LDA_ABS];
            var cycles = lda_abs(bus.Object, registers.Object);

            registers.VerifySet(r => r.A = 0);
            registers.Verify(r => r.SetZeroFlag(true));
            bus.Verify(b => b.Read(0xdead));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDA_ABS_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(unchecked((byte)-1));

            var lda_abs = _instructions[Opcodes.LDA_ABS];
            var cycles = lda_abs(bus.Object, registers.Object);

            registers.VerifySet(r => r.A = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
            bus.Verify(b => b.Read(0xdead));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDA_ABS_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x10);

            var lda_abs_x = _instructions[Opcodes.LDA_ABS_X];
            var cycles = lda_abs_x(bus.Object, registers.Object);
            registers.VerifySet(r => r.A = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0xdead + 15));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDA_ABS_X_penalty_on_page_boundary()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xff).Returns(0x00).Returns(0x10);

            var lda_abs_x = _instructions[Opcodes.LDA_ABS_X];
            var cycles = lda_abs_x(bus.Object, registers.Object);
            registers.VerifySet(r => r.A = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0x00ff + 15));
            cycles.Should().Be(5);
        }

        [Fact]
        public void LDA_ABS_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x10);

            var lda_abs = _instructions[Opcodes.LDA_ABS_Y];
            var cycles = lda_abs(bus.Object, registers.Object);
            registers.VerifySet(r => r.A = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0xdead + 15));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDA_ABS_Y_penalty_on_page_boundary()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xff).Returns(0x00).Returns(0x10);

            var lda_abs = _instructions[Opcodes.LDA_ABS_Y];
            var cycles = lda_abs(bus.Object, registers.Object);
            registers.VerifySet(r => r.A = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0x00ff + 15));
            cycles.Should().Be(5);
        }

        [Fact]
        public void LDA_zero_loads_A_with_value_from_on_zero_page_address()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0) // address
                .Returns(0xde); // value

            var lda = _instructions[Opcodes.LDA_ZERO];
            var cycles = lda(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xde);

            cycles.Should().Be(3);
        }

        [Fact]
        public void LDA_zero_loads_A_with_value_from_on_zero_page_address_2()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0x20) // address
                .Returns(0xde); // value

            var lda = _instructions[Opcodes.LDA_ZERO];
            var cycles = lda(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xde);
            bus.Verify(b => b.Read(0x20));

            cycles.Should().Be(3);
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
        public void LDA_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address
                .Returns(0xde) // high address
                .Returns(0xaa); // value at location final location

            var lda = _instructions[Opcodes.LDA_IND_Y];
            var cycles = lda(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void LDA_indirect_Y_page_cross_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0xff) //low address
                .Returns(0x00) // high address
                .Returns(0xaa); 

            var lda = _instructions[Opcodes.LDA_IND_Y];
            var cycles = lda(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0x00ff + 5), Times.Once());

            cycles.Should().Be(6);
        }

        [Fact]
        public void LDA_ZERO_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0x10);

            var lda_zero_x = _instructions[Opcodes.LDA_ZERO_X];
            var cycles = lda_zero_x(bus.Object, registers.Object);
            registers.VerifySet(r => r.A = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0xad + 15));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDA_ZERO_X_wraps()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xff).Returns(0x10);

            var lda_zero_x = _instructions[Opcodes.LDA_ZERO_X];
            var cycles = lda_zero_x(bus.Object, registers.Object);
            registers.VerifySet(r => r.A = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(4));
            cycles.Should().Be(4);
        }
    }
}
