using Moq;

using NESCore;
using NESCore.CPU;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class UnknownInstructionTests : InstructionTestBase
    {
        public UnknownInstructionTests(InstructionsFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void CpuThrowsOnUnknownInstruction()
        {
            var unknownOpcode = 0x02;
            var registers = Mock.Of<IRegisters>();
            var bus = Mock.Of<IBUS>();
            var instruction = _instructions[unknownOpcode];
            var sut = () => instruction(bus, registers);
            sut.Should().Throw<NotImplementedException>();
        }
    }
}
