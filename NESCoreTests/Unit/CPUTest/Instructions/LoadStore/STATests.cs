using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class StaTests(InstructionsFixture fixture) : InstructionTestBase(fixture)
    {
        [Fact]
        public void STA_ZERO()
        {
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.A).Returns(10);
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xde);
            var sut = Instructions[Opcodes.STA_ZERO];
            sut(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0x00de, 10));

            //No flags affected
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }

        [Fact]
        public void STA_ZERO_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 5;
            registers.Object.X = 10;

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xaa);

            var sta = Instructions[Opcodes.STA_ZERO_X];
            sta(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0xaa + 10, 5));
        }

        [Fact]
        public void STA_ZERO_X_Wraps()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 5;
            registers.Object.X = 10;

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xff);

            var sta = Instructions[Opcodes.STA_ZERO_X];
            sta(bus.Object, registers.Object);

            bus.Verify(b => b.Write(9, 5));
        }

        [Fact]
        public void STA_ABS()
        {
            var registers = new Mock<IRegisters>();
            registers.Setup(r => r.A).Returns(10);
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);
            var sut = Instructions[Opcodes.STA_ABS];
            sut(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead, 10));

            //No flags affected
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }

        [Fact]
        public void STA_ABS_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 10;
            registers.Object.X = 10;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);
            var sut = Instructions[Opcodes.STA_ABS_X];
            sut(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead + 10, 10));
        }

        [Fact]
        public void STA_ABS_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 10;
            registers.Object.Y = 10;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);
            var sut = Instructions[Opcodes.STA_ABS_Y];
            sut(bus.Object, registers.Object);
            bus.Verify(b => b.Write(0xdead + 10, 10));
        }

        [Fact]
        public void STA_indirect_X()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 5;
            registers.Object.A = 0xaa;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0xee) // dummy address
                .Returns(0x0ad) //low address
                .Returns(0xde); // high address

            var sta = Instructions[Opcodes.STA_IND_X];
            sta(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10 + 5), Times.Once());
            bus.Verify(b => b.Read(10 + 5 + 1), Times.Once());
            bus.Verify(b => b.Write(0xdead, 0xaa), Times.Once());
        }

        [Fact]
        public void STA_indirect_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 0xaa;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(10)  // param
                .Returns(0x0ad) //low address
                .Returns(0xde); // high address

            var sta = Instructions[Opcodes.STA_IND_Y];
            sta(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Write(0xdead + 5, 0xaa));
        }

        [Fact]
        public void STA_indirect_Y_page_cross_penalty()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 5;
            registers.Object.A = 0xaa;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(10)  // param
                .Returns(0xff) //low address
                .Returns(0x00); // high address

            var sta = Instructions[Opcodes.STA_IND_Y];
            sta(bus.Object, registers.Object);

            bus.Verify(b => b.Read(10), Times.Once());
            bus.Verify(b => b.Read(10 + 1), Times.Once());
            bus.Verify(b => b.Write(0x00ff + 5, 0xaa));
        }
    }
}
