using NESCore;
using NESCore.CPU;
using NESCore.CPU.Instructions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class InstructionsFixture
    {
        public IReadOnlyList<Func<IBUS, IRegisters, byte>> Instructions { get; init; }

        public InstructionsFixture()
        {
            Instructions = new CPUInstructions().InstructionSet;
        }
    }

    [CollectionDefinition(nameof(InstructionsFixture))]
    public class InstructionsCollextion : ICollectionFixture<InstructionsFixture>
    {
    }
}
