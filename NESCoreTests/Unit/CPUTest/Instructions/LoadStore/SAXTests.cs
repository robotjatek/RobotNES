using Moq;

using NESCore;
using NESCore.CPU;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class SAXTests : InstructionTestBase
    {
        public SAXTests(InstructionsFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void SAX_IND_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0xff;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(10).Returns(0xad).Returns(0xde);

            var sax = _instructions[Opcodes.SAX_IND_X];
            var cycles = sax(bus.Object, registers.Object);
            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());

            bus.Verify(b => b.Write(0xdead, 5&0xff));

            cycles.Should().Be(6);
        }

        [Fact]
        public void SAX_ZERO()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0xff;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xde);

            var sax = _instructions[Opcodes.SAX_ZERO];
            var cycles = sax(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0xde, 5));

            cycles.Should().Be(3);
        }

        [Fact]
        public void SAX_ZERO_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.Y = 10;
            registers.Object.A = 0xff;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xde);

            var sax = _instructions[Opcodes.SAX_ZERO_Y];
            var cycles = sax(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0xde + 10, 5));

            cycles.Should().Be(4);
        }

        [Fact]
        public void SAX_ABS()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0xff;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde);

            var sax = _instructions[Opcodes.SAX_ABS];
            var cycles = sax(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0xdead, 5));

            cycles.Should().Be(4);
        }
    }
}
