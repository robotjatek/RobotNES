using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions.Shift
{
    public class RolTests(InstructionsFixture fixture) : InstructionTestBase(fixture)
    {
        [Fact]
        public void ROL_A_shifts_value_left_by_one()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 4;

            var rol = Instructions[Opcodes.ROL_A];
            rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(8);
        }

        [Fact]
        public void ROL_A_sets_carry_flag_to_true()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x80;

            var rol = Instructions[Opcodes.ROL_A];
            rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0);
            registers.Verify(r => r.SetCarryFlag(true), Times.Once);
        }

        [Fact]
        public void ROL_A_sets_carry_flag_to_false()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(false);
            registers.SetupAllProperties();
            registers.Object.A = 0x7F;

            var rol = Instructions[Opcodes.ROL_A];
            rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0xFE);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);
        }

        [Fact]
        public void ROL_A_sets_bit0_to_0_when_carry_flag_is_false()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(false);
            registers.SetupAllProperties();
            registers.Object.A = 2;

            var rol = Instructions[Opcodes.ROL_A];
            rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(4);
            (registers.Object.A & 0x1).Should().Be(0);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);
        }

        [Fact]
        public void ROL_A_sets_bit0_to_1_when_carry_flag_is_true()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 2;

            var rol = Instructions[Opcodes.ROL_A];
            rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(5);
            (registers.Object.A & 0x1).Should().Be(1);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);
        }

        [Fact]
        public void ROL_A_rotates_correctly()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 0x6e;

            var rol = Instructions[Opcodes.ROL_A];
            rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0xdd);
            (registers.Object.A & 0x1).Should().NotBe(0);
            registers.Verify(r => r.SetCarryFlag(false), Times.Once);
        }

        [Fact]
        public void ROL_A_sets_zero_flag_to_false()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x4;

            var rol = Instructions[Opcodes.ROL_A];
            rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x8);
            registers.Verify(r => r.SetZeroFlag(false), Times.Once);
        }

        [Fact]
        public void ROL_A_sets_zero_flag_to_true()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x80;

            var rol = Instructions[Opcodes.ROL_A];
            rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once);
        }

        [Fact]
        public void ROL_A_sets_negative_flag_to_false_when_the_input_bit6_is_zero()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x20;

            var rol = Instructions[Opcodes.ROL_A];
            rol(bus.Object, registers.Object);

            registers.Object.A.Should().Be(0x40);
            registers.Verify(r => r.SetNegativeFlag(false), Times.Once);
        }

        [Fact]
        public void ROL_A_sets_negative_flag_to_true_when_the_input_bit6_is_one()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.GetCarryFlag()).Returns(true);
            registers.SetupAllProperties();
            registers.Object.A = 0x40;

            var rol = Instructions[Opcodes.ROL_A];
            rol(bus.Object, registers.Object);

            (registers.Object.A & 0x80).Should().NotBe(0);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once);
        }

        [Fact]
        public void ROL_ZERO()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0x10).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var rol = Instructions[Opcodes.ROL_ZERO];
            rol(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x10, 8));
        }

        [Fact]
        public void ROL_ZERO_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0x10) // zero-page address 10 (+5 from the X register)
                .Returns(0xee) // dummy read
                .Returns(4); // original value
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var addresses = new List<ushort>();
            var writes = new List<byte>();
            bus.Setup(b => b.Write(Capture.In(addresses), Capture.In(writes)));
            
            var rol = Instructions[Opcodes.ROL_ZERO_X];
            rol(bus.Object, registers.Object);
            addresses.Should().Equal(0x15, 0x15); // Two writes to the same address (one is a dummy write)
            writes.Should().Equal(4, 8); // the first write is the original value
        }

        [Fact]
        public void ROL_ZERO_X_wraps()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff) // end of the zero page
                .Returns(0xee) // Dummy read
                .Returns(4); // original value
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var addresses = new List<ushort>();
            var writes = new List<byte>();
            bus.Setup(b => b.Write(Capture.In(addresses), Capture.In(writes)));
            
            var rol = Instructions[Opcodes.ROL_ZERO_X];
            rol(bus.Object, registers.Object);
            addresses.Should().Equal(0x4, 0x4); // writes wrapped around
            writes.Should().Equal(4, 8); // first write is the original value, the second is the rotated
        }

        [Fact]
        public void ROL_ABS()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var rol = Instructions[Opcodes.ROL_ABS];
            rol(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead, 8));
        }

        [Fact]
        public void ROL_ABS_X()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde).Returns(4);
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var rol = Instructions[Opcodes.ROL_ABS_X];
            rol(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead + 10, 8));
        }
    }
}
