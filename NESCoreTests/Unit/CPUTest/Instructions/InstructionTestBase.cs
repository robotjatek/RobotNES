using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    [Collection(nameof(InstructionsFixture))]
    public abstract class InstructionTestBase(InstructionsFixture fixture)
    {
        protected readonly IReadOnlyList<Action<IBUS, IRegisters>> Instructions = fixture.Instructions;
    }
}
