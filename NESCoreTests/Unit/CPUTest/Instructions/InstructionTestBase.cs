using NESCore;
using NESCore.CPU;
using NESCore.CPU.Instructions;

namespace NESCoreTests.Unit.CPUTest.Instructions
{
    public class InstructionTestBase
    {
        protected Func<IBUS, IRegisters, byte>[] _instructions = new CPUInstructions().InstructionSet;
    }
}
