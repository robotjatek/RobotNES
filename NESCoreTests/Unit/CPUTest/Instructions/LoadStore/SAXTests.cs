using Moq;

using NESCore;
using NESCore.CPU;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class SaxTests(InstructionsFixture fixture) : InstructionTestBase(fixture)
    {
        [Fact]
        public void SAX_IND_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0xff;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)
                .Returns(0xee) // dummy read
                .Returns(0xad)
                .Returns(0xde);

            var sax = Instructions[Opcodes.SAX_IND_X];
            sax(bus.Object, registers.Object);
            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());

            bus.Verify(b => b.Write(0xdead, 5&0xff));
        }

        [Fact]
        public void SAX_ZERO()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0xff;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xde);

            var sax = Instructions[Opcodes.SAX_ZERO];
            sax(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0xde, 5));
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
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xde);

            var sax = Instructions[Opcodes.SAX_ZERO_Y];
            sax(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0xde + 10, 5));
        }

        [Fact]
        public void SAX_ABS()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0xff;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);

            var sax = Instructions[Opcodes.SAX_ABS];
            sax(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0xdead, 5));
        }
    }
}
