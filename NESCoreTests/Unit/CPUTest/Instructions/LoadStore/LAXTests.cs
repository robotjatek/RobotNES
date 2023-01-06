using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class LAXTests : InstructionTestBase
    {
        [Fact]
        public void LAX_zero()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)  // param
                .Returns(0xaa);

            var lax = _instructions[Opcodes.LAX_ZERO];
            var cycles = lax(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(0xad), Times.Once());

            cycles.Should().Be(3);
        }

        [Fact]
        public void LAX_zero_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 10;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)  // param
                .Returns(0xaa);

            var lax = _instructions[Opcodes.LAX_ZERO_Y];
            var cycles = lax(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(0xad + 10), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void LAX_abs()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xad)
                .Returns(0xde)
                .Returns(0xaa); // value at address 10

            var lax = _instructions[Opcodes.LAX_ABS];
            var cycles = lax(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(0xdead), Times.Once());

            cycles.Should().Be(4);
        }

        [Fact]
        public void LAX_indirect_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address (at 10 + 5)
                .Returns(0xde) // high address (at 10 + 5 + 1)
                .Returns(0xaa); // value at location 0xdead

            var lax = _instructions[Opcodes.LAX_IND_X];
            var cycles = lax(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());

            cycles.Should().Be(6);
        }

        [Fact]
        public void LAX_indirect_Y()
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

            var lda = _instructions[Opcodes.LAX_IND_Y];
            var cycles = lda(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());

            cycles.Should().Be(5);
        }

        [Fact]
        public void LAX_indirect_Y_page_cross_penalty()
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

            var lda = _instructions[Opcodes.LAX_IND_Y];
            var cycles = lda(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0x00ff + 5), Times.Once());

            cycles.Should().Be(6);
        }

        [Fact]
        public void LAX_ABS_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x10);

            var lax_abs = _instructions[Opcodes.LAX_ABS_Y];
            var cycles = lax_abs(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0xdead + 15));
            cycles.Should().Be(4);
        }

        [Fact]
        public void LAX_ABS_Y_penalty_on_page_boundary()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xff).Returns(0x00).Returns(0x10);

            var lax_abs = _instructions[Opcodes.LAX_ABS_Y];
            var cycles = lax_abs(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0x00ff + 15));
            cycles.Should().Be(5);
        }
    }
}
