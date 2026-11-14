using Moq;

using NESCore;
using NESCore.CPU;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class UnknownInstructionTests(InstructionsFixture fixture) : InstructionTestBase(fixture)
    {
        [Fact]
        public void CpuThrowsOnUnknownInstruction()
        {
            const int unknownOpcode = 0x02;
            var registers = Mock.Of<IRegisters>();
            var bus = Mock.Of<IBUS>();
            var instruction = Instructions[unknownOpcode];
            var sut = () => instruction(bus, registers);
            sut.Should().Throw<NotImplementedException>();
        }
    }
}
