using Moq;
using NESCore.CPU.Instructions;
using NESCore.CPU;
using NESCore;
using FluentAssertions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class ControlInstructionTests : InstructionTestBase
    {
        [Fact]
        public void JMP_ABS()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);

            var jmp_abs = _instructions[Opcodes.JMP_ABS];
            var cycles = jmp_abs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 0xdead);
            cycles.Should().Be(3);
        }

        [Fact]
        public void JSR_ABS()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.PC = 0xd000;
            registers.Object.STATUS = 0;
            registers.Object.SP = 0xfe;

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<ushort>())).Returns(0xad).Returns(0xde);

            var jsr_abs = _instructions[Opcodes.JSR_ABS];
            var cycles = jsr_abs(bus.Object, registers.Object);

            bus.Verify(b => b.Write(0x100 | 0xfe, 0xd0));
            bus.Verify(b => b.Write(0x100 | 0xfd, 0x01));

            registers.Object.PC.Should().Be(0xdead);
            registers.Object.STATUS.Should().Be(0);
            cycles.Should().Be(6);
        }

        [Fact]
        public void RTS()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();
            registers.Object.SP = 0xFF - 2;

            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(0x100 | 0xff)).Returns(0xde);
            bus.Setup(b => b.Read(0x100 | 0xff - 1)).Returns(0xad);

            var sut = _instructions[Opcodes.RTS];
            var cycles = sut(bus.Object, registers.Object);

            registers.Object.PC.Should().Be(0xdead + 1);
            registers.Object.SP.Should().Be(0xFF);

            cycles.Should().Be(6);
        }

        [Fact]
        public void RTI()
        {
            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>()))
                .Returns(0xaa) //content that goes to the STATUS register
                .Returns(0xad) //low address byte
                .Returns(0xde); //high address byte

            var rti = _instructions[Opcodes.RTI];
            var cycles = rti(bus.Object, registers.Object);
            registers.Object.PC.Should().Be(0xdead);
            registers.Object.STATUS.Should().Be(0xAA);
            registers.Object.SP.Should().Be(3);

            cycles.Should().Be(6);
        }
    }
}
