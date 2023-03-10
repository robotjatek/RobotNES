using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class LDYTests : InstructionTestBase
    {
        public LDYTests(InstructionsFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void LDY_IMM()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x10);

            var ldy_imm = _instructions[Opcodes.LDY_IMM];
            var cycles = ldy_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.Y = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDY_IMM_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

            var ldy_imm = _instructions[Opcodes.LDY_IMM];
            var cycles = ldy_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.Y = 0);
            registers.Verify(r => r.SetZeroFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDY_IMM_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-1));

            var ldy_imm = _instructions[Opcodes.LDY_IMM];
            var cycles = ldy_imm(bus.Object, registers.Object);

            registers.VerifySet(r => r.Y = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
            cycles.Should().Be(2);
        }

        [Fact]
        public void LDY_zero_loads_A_with_value_from_on_zero_page_address()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0) // address
                .Returns(0xde); // value

            var ldy = _instructions[Opcodes.LDY_ZERO];
            var cycles = ldy(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0xde);

            cycles.Should().Be(3);
        }

        [Fact]
        public void LDY_zero_loads_A_with_value_from_on_zero_page_address2()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xff) // address
                .Returns(0xde); // value

            var ldy = _instructions[Opcodes.LDY_ZERO];
            var cycles = ldy(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0xde);
            bus.Verify(b => b.Read(0x00ff));

            cycles.Should().Be(3);
        }

        [Fact]
        public void LDY_ABS()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xaa);

            var ldy = _instructions[Opcodes.LDY_ABS];
            var cycles = ldy(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0xaa);

            cycles.Should().Be(4);
        }

        [Fact]
        public void LDY_ZERO_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0x10);

            var ldy_zero_x = _instructions[Opcodes.LDY_ZERO_X];
            var cycles = ldy_zero_x(bus.Object, registers.Object);
            registers.VerifySet(r => r.Y = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0xad + 15));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDY_ABS_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x10);

            var ldy_abs_x = _instructions[Opcodes.LDY_ABS_X];
            var cycles = ldy_abs_x(bus.Object, registers.Object);
            registers.VerifySet(r => r.Y = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0xdead + 15));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LDY_ABS_X_penalty_on_page_boundary()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xff).Returns(0x00).Returns(0x10);

            var ldy_abs_x = _instructions[Opcodes.LDY_ABS_X];
            var cycles = ldy_abs_x(bus.Object, registers.Object);
            registers.VerifySet(r => r.Y = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0x00ff + 15));
            cycles.Should().Be(5);
        }
    }
}
