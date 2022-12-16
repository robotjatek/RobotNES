
using FluentAssertions;

using Moq;

using NESCore;

namespace NESCoreTests.Unit.CPUTest
{
    public class InstructionsetTests
    {
        [Fact]
        public void JMP_ABS()
        {
            var registers = new Mock<IRegisters>();
            var bus = new Mock<IBUS>();
            bus.SetupSequence(b => b.Read(It.IsAny<UInt16>())).Returns(0xad).Returns(0xde);

            var jmp_abs = new CPUInstructions().InstructionSet[Opcodes.JMP_ABS];
            var cycles = jmp_abs(bus.Object, registers.Object);

            registers.VerifySet(r => r.PC = 0xdead);
            cycles.Should().Be(3);
        }
    }
}
