using NESCore;
using NESCore.CPU;
using NESCore.CPU.Instructions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class InstructionsFixture
    {
        public IReadOnlyList<Action<IBUS, IRegisters>> Instructions { get; } = new CpuInstructions().InstructionSet;
    }

    [CollectionDefinition(nameof(InstructionsFixture))]
    public class InstructionsCollection : ICollectionFixture<InstructionsFixture>
    {
    }
}
