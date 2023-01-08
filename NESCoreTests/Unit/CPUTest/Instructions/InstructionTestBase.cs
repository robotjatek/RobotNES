using NESCore.CPU;
using NESCore;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    [Collection(nameof(InstructionsFixture))]
    public abstract class InstructionTestBase
    {
        protected IReadOnlyList<Func<IBUS, IRegisters, byte>> _instructions;

        public InstructionTestBase(InstructionsFixture fixture)
        {
            _instructions = fixture.Instructions;
        }
    }
}
