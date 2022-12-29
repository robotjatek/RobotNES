using Moq;
using NESCore.CPU;
using NESCore;
using FluentAssertions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class TransferInstructionTests : InstructionTestBase
    {
        [Fact]
        public void TAX_transfers_a_to_x()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x50;
            registers.Object.X = 0xff;

            var tax = _instructions[Opcodes.TAX];
            var cycles = tax(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0x50);

            cycles.Should().Be(2);
        }

        [Fact]
        public void TAX_sets_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            registers.Object.X = 0xff;

            var tax = _instructions[Opcodes.TAX];
            var cycles = tax(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0x0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void TAX_sets_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x80;
            registers.Object.X = 0xff;

            var tax = _instructions[Opcodes.TAX];
            var cycles = tax(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0x80);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void TAY_transfers_a_to_y()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x50;
            registers.Object.Y = 0xff;

            var tay = _instructions[Opcodes.TAY];
            var cycles = tay(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0x50);

            cycles.Should().Be(2);
        }

        [Fact]
        public void TAY_sets_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x0;
            registers.Object.Y = 0xff;

            var tay = _instructions[Opcodes.TAY];
            var cycles = tay(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0x0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void TAY_sets_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0x80;
            registers.Object.Y = 0xff;

            var tay = _instructions[Opcodes.TAY];
            var cycles = tay(bus.Object, registers.Object);
            registers.Object.Y.Should().Be(0x80);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void TSX_transfers_s_to_x()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.SP = 0x50;
            registers.Object.X = 0xff;

            var tsx = _instructions[Opcodes.TSX];
            var cycles = tsx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0x50);

            cycles.Should().Be(2);
        }

        [Fact]
        public void TSX_sets_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.SP = 0x0;
            registers.Object.X = 0xff;

            var tsx = _instructions[Opcodes.TSX];
            var cycles = tsx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0x0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void TSX_sets_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.SP = 0x80;
            registers.Object.X = 0xff;

            var tsx = _instructions[Opcodes.TSX];
            var cycles = tsx(bus.Object, registers.Object);
            registers.Object.X.Should().Be(0x80);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void TXA_transfers_x_to_a()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            registers.Object.X = 0x50;

            var txa = _instructions[Opcodes.TXA];
            var cycles = txa(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x50);

            cycles.Should().Be(2);
        }

        [Fact]
        public void TXA_sets_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            registers.Object.X = 0x0;

            var txa = _instructions[Opcodes.TXA];
            var cycles = txa(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void TXA_sets_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            registers.Object.X = 0x80;

            var txa = _instructions[Opcodes.TXA];
            var cycles = txa(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x80);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void TXS_transfers_x_to_s()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.SP = 0xff;
            registers.Object.X = 0x50;

            var txs = _instructions[Opcodes.TXS];
            var cycles = txs(bus.Object, registers.Object);
            registers.Object.SP.Should().Be(0x50);

            cycles.Should().Be(2);
        }

        [Fact]
        public void TYA_sets_negative_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            registers.Object.Y = 0x80;

            var tya = _instructions[Opcodes.TYA];
            var cycles = tya(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x80);
            registers.Verify(r => r.SetNegativeFlag(true), Times.Once());

            cycles.Should().Be(2);
        }

        [Fact]
        public void TYA_transfers_y_to_a()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            registers.Object.Y = 0x50;

            var tya = _instructions[Opcodes.TYA];
            var cycles = tya(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x50);

            cycles.Should().Be(2);
        }

        [Fact]
        public void TYA_sets_zero_flag()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.A = 0xff;
            registers.Object.Y = 0x0;

            var tya = _instructions[Opcodes.TYA];
            var cycles = tya(bus.Object, registers.Object);
            registers.Object.A.Should().Be(0x0);
            registers.Verify(r => r.SetZeroFlag(true), Times.Once());

            cycles.Should().Be(2);
        }
    }
}
