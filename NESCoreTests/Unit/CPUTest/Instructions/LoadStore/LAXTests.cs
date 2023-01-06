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
                .Returns(0xaa); // value at address 10

            var lax = _instructions[Opcodes.LAX_ZERO];
            var cycles = lax(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0xaa);
            registers.Object.X.Should().Be(0xaa);

            bus.Verify(b => b.Read(0xad), Times.Once());

            cycles.Should().Be(3);
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
    }
}
