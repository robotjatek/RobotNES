using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions.LoadStore
{
    public class LdxTests(InstructionsFixture fixture) : InstructionTestBase(fixture)
    {
        [Fact]
        public void LDX_IMM()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0x10);

            var ldxImm = Instructions[Opcodes.LDX_IMM];
            ldxImm(bus.Object, registers.Object);

            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
        }

        [Fact]
        public void LDX_IMM_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0);

            var ldxImm = Instructions[Opcodes.LDX_IMM];
            ldxImm(bus.Object, registers.Object);

            registers.VerifySet(r => r.X = 0);
            registers.Verify(r => r.SetZeroFlag(true));
        }

        [Fact]
        public void LDX_IMM_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(unchecked((byte)-1));

            var ldxImm = Instructions[Opcodes.LDX_IMM];
            ldxImm(bus.Object, registers.Object);

            registers.VerifySet(r => r.X = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
        }

        [Fact]
        public void LDX_ABS()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x10);

            var ldxAbs = Instructions[Opcodes.LDX_ABS];
            ldxAbs(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead));
            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
        }

        [Fact]
        public void LDX_ABS_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 10;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x10);

            var ldxAbs = Instructions[Opcodes.LDX_ABS_Y];
            ldxAbs(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead + 10));
            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
        }

        [Fact]
        public void LDX_ABS_Y_penalty_on_page_boundary()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xff)
                .Returns(0x00)
                .Returns(0xEE) // this is a dummy read
                .Returns(0x10);

            var ldxAbs = Instructions[Opcodes.LDX_ABS_Y];
            ldxAbs(bus.Object, registers.Object);
            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0x00ff + 15));
        }

        [Fact]
        public void LDX_ABS_sets_zero_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(0x0);

            var ldxAbs = Instructions[Opcodes.LDX_ABS];
            ldxAbs(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead));
            registers.VerifySet(r => r.X = 0);
            registers.Verify(r => r.SetZeroFlag(true));
        }

        [Fact]
        public void LDX_ABS_sets_negative_flag()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde).Returns(unchecked((byte)-1));

            var ldxAbs = Instructions[Opcodes.LDX_ABS];
            ldxAbs(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xdead));
            registers.VerifySet(r => r.X = unchecked((byte)-1));
            registers.Verify(r => r.SetNegativeFlag(true));
        }

        [Fact]
        public void LDX_ZERO()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0x10);

            var ldxZero = Instructions[Opcodes.LDX_ZERO];
            ldxZero(bus.Object, registers.Object);

            bus.Verify(b => b.Read(0xad));
            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
        }

        [Fact]
        public void LDX_ZERO_Y()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.Y = 15;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>()))
                .Returns(0xad)
                .Returns(0xEE) // The second read is a dummy read
                .Returns(0x10);

            var ldxZeroY = Instructions[Opcodes.LDX_ZERO_Y];
            ldxZeroY(bus.Object, registers.Object);
            registers.VerifySet(r => r.X = 0x10);
            registers.Verify(r => r.SetNegativeFlag(false));
            registers.Verify(r => r.SetZeroFlag(false));
            bus.Verify(b => b.Read(0xad + 15));
        }
    }
}
