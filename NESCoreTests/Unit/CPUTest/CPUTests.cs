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
            //NMI vector: 0xbeef
            bus.Setup(b => b.Read(0xfffb)).Returns(0xbe);
            bus.Setup(b => b.Read(0xfffa)).Returns(0xef);
            //setup PC on startup: 0xdead
            bus.Setup(b => b.Read(0xfffd)).Returns(0xde);
            bus.Setup(b => b.Read(0xfffc)).Returns(0xad);

            var registers = new Registers
            {
                STATUS = 0xee
            };

            var cpu = new Cpu(bus.Object, registers, _instructions, _logger);
            
            cpu.HandleNmi();
            cpu.RunInstruction();

            writeArgs.Should().ContainInConsecutiveOrder(0xde, 0xad, 0xee);
            registers.PC.Should().Be(0xbeef);
            registers.GetInterruptDisableFlag().Should().BeTrue();

            // An NMI costs 7 cycles and all 7 of them are bus accesses:
            // two dummy reads at the paused PC, the three stack pushes, then the NMI address pair.
            bus.Verify(b => b.Read(0xdead), Times.Exactly(2), "the NMI pauses on two dummy reads at the current PC");
            bus.Verify(b => b.Write(0x1fd, 0xde), Times.Once, "PCH has to be pushed first");
            bus.Verify(b => b.Write(0x1fc, 0xad), Times.Once, "PCL has to be pushed second");
            bus.Verify(b => b.Write(0x1fb, 0xee), Times.Once, "STATUS has to be pushed last");
            bus.Verify(b => b.Read(0xfffa), Times.Once, "NMI vector");
            bus.Verify(b => b.Read(0xfffb), Times.Once, "NMI vector");

            // The 2 extra reads are from the constructor
            bus.Verify(b => b.Read(It.IsAny<ushort>()), Times.Exactly(6));
            bus.Verify(b => b.Write(It.IsAny<ushort>(), It.IsAny<byte>()), Times.Exactly(3));
        }

        [Fact]
        public void CpuHandlesIrq()
        {
            var writeArgs = new List<byte>();
            var bus = new Mock<IBUS>();
            bus.Setup(b => b.Write(It.IsAny<ushort>(), Capture.In(writeArgs)));
            bus.SetupAllProperties();
            // NMI vector:
            bus.Setup(b => b.Read(0xffff)).Returns(0xbe);
            bus.Setup(b => b.Read(0xfffe)).Returns(0xef);
            //setup PC on startup: 0xdead
            bus.Setup(b => b.Read(0xfffd)).Returns(0xde);
            bus.Setup(b => b.Read(0xfffc)).Returns(0xad);
            bus.Setup(b => b.Read(0xbeef)).Returns(Opcodes.NOP);
            bus.Setup(b => b.Irq).Returns(true);
            
            var registers = new Registers
            {
                STATUS = 0x80 // Negative flag set (1), Interrupt disable is unset (0)
            };
            registers.GetInterruptDisableFlag().Should().BeFalse();
            
            var cpu = new Cpu(bus.Object, registers, _instructions, _logger);
            cpu.RunInstruction();
            
            writeArgs.Should().ContainInConsecutiveOrder(0xde, 0xad, 0xA0); // should save pc and status to stack
            registers.PC.Should().Be(0xbeef);
            registers.GetInterruptDisableFlag().Should().BeTrue();
            
            bus.Verify(b => b.Read(0xdead), Times.Exactly(2), "the IRQ pauses on two dummy reads at the current PC");
            bus.Verify(b => b.Write(0x1fd, 0xde), Times.Once, "PCH has to be pushed first");
            bus.Verify(b => b.Write(0x1fc, 0xad), Times.Once, "PCL has to be pushed second");
            bus.Verify(b => b.Write(0x1fb, 0xA0), Times.Once, "STATUS has to be pushed last");
            bus.Verify(b => b.Read(0xfffe), Times.Once, "IRQ vector");
            bus.Verify(b => b.Read(0xffff), Times.Once, "IRQ vector");
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
            bus.Verify(b => b.PpuOamDma(0xaa), Times.Exactly(256));
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
            bus.Verify(b => b.PpuOamDma(0xaa), Times.Exactly(256));
        }

        [Theory]
        // NOP leaves the CPU on an even cycle and JMP on an odd one. Only the odd case pays the
        // alignment penalty, and the 6502 spends it on one extra dummy read where it paused.
        [InlineData((byte)Opcodes.NOP, 1)]
        [InlineData((byte)Opcodes.JMP_ABS, 2)]
        public void DmaAddsTheAlignmentPenaltyForOddCycleInstructions(byte opcode, int dummyReadsWhereItPaused)
        {
            uint cycles = 0;
            var bus = new Mock<IBUS>();
            bus.SetupGet(b => b.Cycles).Returns(() => cycles);
            bus.Setup(b => b.Read(It.IsAny<ushort>()))
                .Returns((ushort address) => address switch
                {
                    // Reset vector $C000; the opcode fetch there returns the instruction.
                    0xfffd => 0xc0,
                    0xfffc => 0x00,
                    _ => opcode
                })
                .Callback<ushort>(_ => cycles++);

            var registers = new Registers();
            var cpu = new Cpu(bus.Object, registers, new NESCore.CPU.Instructions.CpuInstructions().InstructionSet, _logger);

            cpu.RunInstruction();

            cpu.HandleDma(0);
            cpu.RunInstruction();

            bus.Verify(b => b.Read(registers.PC), Times.Exactly(dummyReadsWhereItPaused));
        }
    }
}
