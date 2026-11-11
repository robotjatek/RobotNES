using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class LaxTests : InstructionTestBase
    {
        
        public LaxTests(InstructionsFixture fixture) : base(fixture) {}
        
        [Fact]
        public void LAX_zero()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)  // param
                .Returns(0xaa);

            var lax = Instructions[Opcodes.LAX_ZERO];
            lax(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(0xad), Times.Once());
        }

        [Fact]
        public void LAX_zero_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 10;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)  // param
                .Returns(0xee) // dummy read
                .Returns(0xaa);

            var lax = Instructions[Opcodes.LAX_ZERO_Y];
            lax(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(0xad + 10), Times.Once());
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

            var lax = Instructions[Opcodes.LAX_ABS];
            lax(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(0xdead), Times.Once());
        }

        [Fact]
        public void LAX_indirect_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xee) // dummy read
                .Returns(0x0ad) //low address (at 10 + 5)
                .Returns(0xde) // high address (at 10 + 5 + 1)
                .Returns(0xaa); // value at location 0xdead

            var lax = Instructions[Opcodes.LAX_IND_X];
            lax(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead), Times.Once());
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

            var lda = Instructions[Opcodes.LAX_IND_Y];
            lda(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0xdead + 5), Times.Once());
        }

        [Fact]
        public void LAX_indirect_Y_page_cross_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xff) //low address
                .Returns(0x00) // high address
                .Returns(0xee) // Dummy read on page cross
                .Returns(0xaa);

            var lda = Instructions[Opcodes.LAX_IND_Y];
            lda(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Read(0x00ff + 5), Times.Once());
        }

        [Fact]
        public void LAX_ABS_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x10);

            var laxAbs = Instructions[Opcodes.LAX_ABS_Y];
            laxAbs(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0xdead + 15));
        }

        [Fact]
        public void LAX_ABS_Y_penalty_on_page_boundary()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xee) // Dummy read on page cross
                .Returns(0x10);

            var laxAbs = Instructions[Opcodes.LAX_ABS_Y];
            laxAbs(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0x00ff + 15));
        }
    }
}
