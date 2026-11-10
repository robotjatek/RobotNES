using Moq;

using NESCore;
using NESCore.CPU;

namespace NESCoreTests.Unit.CPUTest
{
    public class CpuTests : TestBase
    {
        [Fact]
        public void CpuLoadsResetVectorFromBus()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.Is<ushort>(a => a == 0xfffd))).Returns(0xde);
            bus.Setup(b => b.Read(It.Is<ushort>(a => a == 0xfffc))).Returns(0xad);

            _ = new Cpu(bus.Object, _registers.Object, _instructions, _logger);
            bus.Verify(b => b.Read(0xfffd));
            bus.Verify(b => b.Read(0xfffc));
        }

        [Fact]
        public void CpuFetchesInstructionFromTheCorrectResetVector()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.Is<ushort>(a => a == 0xfffd))).Returns(0xde);
            bus.Setup(b => b.Read(It.Is<ushort>(a => a == 0xfffc))).Returns(0xad);

            var registers = new Mock<IRegisters>();
            registers.SetupAllProperties();

            var sut = new Cpu(bus.Object, registers.Object, _instructions, _logger);
            try
            {
                sut.RunInstruction();
            }
            catch { 
                // intentionally ignored exception
            }

            bus.Verify(b => b.Read(It.Is<ushort>(a => a == 0xdead)));
        }

        [Fact]
        public void CpuHandlesNmi()
        {
            var writeArgs = new List<byte>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Write(It.IsAny<ushort>(), Capture.In(writeArgs)));
            //NMI vector:
            bus.Setup(b => b.Read(0xfffb)).Returns(0xbe);
            bus.Setup(b => b.Read(0xfffa)).Returns(0xef);
            //setup PC on startup:
            bus.Setup(b => b.Read(0xfffd)).Returns(0xde);
            bus.Setup(b => b.Read(0xfffc)).Returns(0xad);

            var registers = new Registers
            {
                STATUS = 0xee
            };

            var cpu = new Cpu(bus.Object, registers, _instructions, _logger);
            
            cpu.HandleNmi();
            var cycles = cpu.RunInstruction();

            writeArgs.Should().ContainInConsecutiveOrder(0xde, 0xad, 0xee);
            registers.PC.Should().Be(0xbeef);
            registers.GetInterruptDisableFlag().Should().BeTrue();
            cycles.Should().Be(7);
        }

        [Fact]
        public void CpuHandlesDma()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xaa);

            var cpu = new Cpu(bus.Object, _registers.Object, _instructions, _logger);

            cpu.HandleDma(0);
            cpu.RunInstruction();
            for (byte i = 0; i < 0xff; i++)
            {
                var i1 = i;
                bus.Verify(b => b.Read((ushort)(0x0 + i1)));
            }
            bus.Verify(b => b.PPUOamDMA(0xaa), Times.Exactly(256));
        }

        [Fact]
        public void CpuHandlesDma2()
        {
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xaa);

            var cpu = new Cpu(bus.Object, _registers.Object, _instructions, _logger);

            cpu.HandleDma(0x10);
            cpu.RunInstruction();
            for(byte i = 0; i < 0xff; i++)
            {
                var i1 = i;
                bus.Verify(b => b.Read((ushort)((0x10 << 8) + i1)));
            }
            bus.Verify(b => b.PPUOamDMA(0xaa), Times.Exactly(256));
        }

        [Fact]
        public void DmaReportsTheFull513Cycles()
        {
            // DoDMA returned the cycle count as a byte, so 513 was
            // truncated to 1. The PPU then advanced 3 cycles instead of 1539 for
            // every OAM DMA, leaving it ~4.5 scanlines behind the CPU - which made
            // sprite-0-hit scroll splits land several lines too low.
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(0xaa);

            var cpu = new Cpu(bus.Object, _registers.Object, _instructions, _logger);

            cpu.HandleDma(0);
            var cycles = cpu.RunInstruction();

            cycles.Should().Be(513);
        }

        [Theory]
        // NOP = 2 cycles (even) -> no penalty; JMP abs = 3 cycles (odd) -> +1 penalty.
        [InlineData((byte)Opcodes.NOP, 2, 513)]
        [InlineData((byte)Opcodes.JMP_ABS, 3, 514)]
        public void DmaAddsTheAlignmentPenaltyForOddCycleInstructions(byte opcode, int instructionCycles, int expectedDmaCycles)
        {
            var bus = new Mock<IBUS>();
            // Reset vector $C000; the opcode fetch at $C000 returns the instruction.
            bus.Setup(b => b.Read(0xfffd)).Returns(0xc0);
            bus.Setup(b => b.Read(0xfffc)).Returns(0x00);
            bus.Setup(b => b.Read(It.IsAny<ushort>())).Returns(opcode);

            var registers = new Registers();
            var cpu = new Cpu(bus.Object, registers, new NESCore.CPU.Instructions.CPUInstructions().InstructionSet, _logger);

            cpu.RunInstruction().Should().Be((uint)instructionCycles);

            cpu.HandleDma(0);
            var cycles = cpu.RunInstruction();

            cycles.Should().Be((uint)expectedDmaCycles);
        }
    }
}
