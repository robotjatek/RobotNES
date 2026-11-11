using Moq;

using NESCore;
using NESCore.CPU;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class NopTests(InstructionsFixture fixture) : InstructionTestBase(fixture)
    {
        [Fact]
        public void NOP()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            var nop = Instructions[Opcodes.NOP];
            nop(bus.Object, registers.Object);

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
        public void NOP_1A()
        {
            var bus = new Mock<IBUS>();

            var registers = new Mock<IRegisters>();

            var nop = Instructions[Opcodes.NOP_1A];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<ushort>()), Times.Once());
        }

        [Fact]
        public void NOP_3A()
        {
            var bus = new Mock<IBUS>();

            var registers = new Mock<IRegisters>();

            var nop = Instructions[Opcodes.NOP_3A];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<ushort>()), Times.Once());
        }

        [Fact]
        public void NOP_5A()
        {
            var bus = new Mock<IBUS>();

            var registers = new Mock<IRegisters>();

            var nop = Instructions[Opcodes.NOP_5A];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<ushort>()), Times.Once());
        }

        [Fact]
        public void NOP_7A()
        {
            var bus = new Mock<IBUS>();

            var registers = new Mock<IRegisters>();

            var nop = Instructions[Opcodes.NOP_7A];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<ushort>()), Times.Once());
        }

        [Fact]
        public void NOP_DA()
        {
            var bus = new Mock<IBUS>();

            var registers = new Mock<IRegisters>();

            var nop = Instructions[Opcodes.NOP_DA];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<ushort>()), Times.Once());
        }

        [Fact]
        public void NOP_FA()
        {
            var bus = new Mock<IBUS>();

            var registers = new Mock<IRegisters>();

            var nop = Instructions[Opcodes.NOP_FA];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<ushort>()), Times.Once());
        }

        [Fact]
        public void NOP_IMM_80()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(30);

            var registers = new Mock<IRegisters>();

            var nop = Instructions[Opcodes.NOP_IMM_80];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<ushort>()), Times.Once());
        }

        [Fact]
        public void NOP_IMM_82()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(30);
            var registers = new Mock<IRegisters>();
            var nop = Instructions[Opcodes.NOP_IMM_82];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<ushort>()), Times.Once());
        }
        
        [Fact]
        public void NOP_IMM_89()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(30);
            var registers = new Mock<IRegisters>();
            var nop = Instructions[Opcodes.NOP_IMM_89];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<ushort>()), Times.Once());
        }
        
        [Fact]
        public void NOP_IMM_C2()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(30);
            var registers = new Mock<IRegisters>();
            var nop = Instructions[Opcodes.NOP_IMM_C2];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<ushort>()), Times.Once());
        }
        [Fact]
        public void NOP_IMM_E2()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(30);
            var registers = new Mock<IRegisters>();
            var nop = Instructions[Opcodes.NOP_IMM_E2];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(It.IsAny<ushort>()), Times.Once());
        }
        

        [Fact]
        public void NOP_ZERO_0x04()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xde);

            var registers = new Mock<IRegisters>();

            var nop = Instructions[Opcodes.NOP_ZERO_04];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xde), Times.Once());
        }

        [Fact]
        public void NOP_ZERO_0x44()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xde);

            var registers = new Mock<IRegisters>();

            var nop = Instructions[Opcodes.NOP_ZERO_44];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xde), Times.Once());
        }

        [Fact]
        public void NOP_ZERO_0x64()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xde);

            var registers = new Mock<IRegisters>();

            var nop = Instructions[Opcodes.NOP_ZERO_64];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xde), Times.Once());
        }

        [Fact]
        public void NOP_ZERO_X_0x14()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ZERO_X_14];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xad + 10), Times.Once());
        }

        [Fact]
        public void NOP_ZERO_X_0x34()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ZERO_X_34];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xad + 10), Times.Once());
        }

        [Fact]
        public void NOP_ZERO_X_0x54()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ZERO_X_54];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xad + 10), Times.Once());
        }

        [Fact]
        public void NOP_ZERO_X_0x74()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ZERO_X_74];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xad + 10), Times.Once());
        }

        [Fact]
        public void NOP_ZERO_X_0xD4()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ZERO_X_D4];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xad + 10), Times.Once());
        }

        [Fact]
        public void NOP_ZERO_X_0xF4()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ZERO_X_F4];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xad + 10), Times.Once());
        }

        [Fact]
        public void NOP_ABS_0x0C()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();

            var nop = Instructions[Opcodes.NOP_ABS_0C];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead), Times.Once());
        }

        [Fact]
        public void NOP_ABS_X_0x1C()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ABS_X_1C];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void NOP_ABS_X_0x1C_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xff).Returns(0x00);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ABS_X_1C];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xff + 10), Times.Once());
        }

        [Fact]
        public void NOP_ABS_X_0x3C()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ABS_X_3C];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void NOP_ABS_X_0x3C_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xff).Returns(0x00);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ABS_X_3C];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xff + 10), Times.Once());
        }

        [Fact]
        public void NOP_ABS_X_0x5C()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ABS_X_5C];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void NOP_ABS_X_0x5C_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xff).Returns(0x00);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ABS_X_5C];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xff + 10), Times.Once());
        }

        [Fact]
        public void NOP_ABS_X_0x7C()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ABS_X_7C];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void NOP_ABS_X_0x7C_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xff).Returns(0x00);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ABS_X_7C];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xff + 10), Times.Once());
        }

        [Fact]
        public void NOP_ABS_X_0xDC()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ABS_X_DC];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void NOP_ABS_X_0xDC_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xff).Returns(0x00);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ABS_X_DC];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xff + 10), Times.Once());
        }

        [Fact]
        public void NOP_ABS_X_0xFC()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ABS_X_FC];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xdead + 10), Times.Once());
        }

        [Fact]
        public void NOP_ABS_X_0xFC_penalty()
        {
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xff).Returns(0x00);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.X = 10;

            var nop = Instructions[Opcodes.NOP_ABS_X_FC];
            nop(bus.Object, registers.Object);
            bus.Verify(b => b.Read(0xff + 10), Times.Once());
        }
    }
}
