using Moq;
using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class FlagInstructionTests(InstructionsFixture fixture) : InstructionTestBase(fixture)
    {
        [Fact]
        public void CLC()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            var clc = Instructions[Opcodes.CLC];
            clc(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(false));

            //Everything else never happens
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
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
        public void CLD()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            var cld = Instructions[Opcodes.CLD];
            cld(bus.Object, registers.Object);

            registers.Verify(r => r.SetDecimalFlag(false));

            //Everything else never happens
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never()); ;
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }

        [Fact]
        public void CLV()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            var clv = Instructions[Opcodes.CLV];
            clv(bus.Object, registers.Object);

            registers.Verify(r => r.SetOverflowFlag(false), Times.Once());

            //Everything else never happens
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
        }

        [Fact]
        public void CLI()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            var cli = Instructions[Opcodes.CLI];
            cli(bus.Object, registers.Object);

            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Once());

            //Everything else never happens
            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetCarryFlag(false), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
        }

        [Fact]
        public void SEC()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var sec = Instructions[Opcodes.SEC];
            sec(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Once);
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
        public void SEI()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var sei = Instructions[Opcodes.SEI];
            sei(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Once());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }

        [Fact]
        public void SED()
        {
            var bus = new Mock<IBUS>();
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            var sed = Instructions[Opcodes.SED];
            sed(bus.Object, registers.Object);

            registers.Verify(r => r.SetCarryFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(true), Times.Never());
            registers.Verify(r => r.SetZeroFlag(false), Times.Never());
            registers.Verify(r => r.SetDecimalFlag(true), Times.Once());
            registers.Verify(r => r.SetDecimalFlag(false), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(true), Times.Never());
            registers.Verify(r => r.SetInterruptDisableFlag(false), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(true), Times.Never());
            registers.Verify(r => r.SetNegativeFlag(false), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(true), Times.Never());
            registers.Verify(r => r.SetOverflowFlag(false), Times.Never());
        }
    }
}
