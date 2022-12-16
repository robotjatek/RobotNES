using FluentAssertions;

using NESCore;

namespace NESCoreTests.Unit
{
    public class MemoryTests
    {
        private const UInt16 memorySize = 0x800; // 2 kB
        private readonly Random _random = new();
        private readonly byte[] testData = new byte[memorySize];

        public MemoryTests()
        {
            _random.NextBytes(testData);
        }

        [Fact]
        public void WritesToMemory()
        {
            var sut = new Memory();
            sut.Write(0, 10);
            sut.Read(0).Should().Be(10);
        }

        [Fact]
        public void WritesRandomDataInMemoryAndReadsItBackCorrectly()
        {
            var sut = new Memory();
            for (UInt16 i = 0; i < 0x800; i++)
            {
                sut.Write(i, testData[i]);
            }

            for (UInt16 i = 0; i < 0x800; i++)
            {
                sut.Read(i).Should().Be(testData[i]);
            }

        }
    }
}
