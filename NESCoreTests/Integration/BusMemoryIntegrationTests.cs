using FluentAssertions;

using NESCore;

namespace NESCoreTests.Integration
{
    public class BusMemoryIntegrationTests : TestBase
    {
        private const UInt16 ramSize = 0x800;

        private readonly Random _random = new();
        private readonly byte[] _testData = new byte[ramSize];

        public BusMemoryIntegrationTests()
        {
            _random.NextBytes(_testData);
        }

        [Fact]
        public void WritesRandomDataToMemoryThroughTheBusAndReadsItBackCorectly()
        {
            var memory = new Memory();
            var bus = new Bus(_mockCartridge.Object, memory, _logger);
            for(UInt16 i = 0; i < ramSize; i++)
            {
                bus.Write(i, _testData[i]);
            }

            for (UInt16 i = 0; i < ramSize; i++)
            {
                bus.Read(i).Should().Be(_testData[i]);
            }
        }

        [Fact]
        public void RandomDataShouldBeAvailableOnMirroredRegions()
        {
            var memory = new Memory();
            var bus = new Bus(_mockCartridge.Object, memory, _logger);
            for (UInt16 i = 0; i < ramSize; i++)
            {
                bus.Write(i, _testData[i]);
            }

            //First mirror
            for (UInt16 i = 0; i < ramSize; i++)
            {
                ushort address = (ushort)(i + 0x1000);
                bus.Read(address).Should().Be(_testData[i]);
            }

            //Second Mirror
            for (UInt16 i = 0; i < ramSize; i++)
            {
                ushort address = (ushort)(i + 0x1800);
                bus.Read(address).Should().Be(_testData[i]);
            }
        }

        [Fact]
        public void WritesToFirstMirrorShouldBeAvailableInOtherMirroredRegions()
        {
            var memory = new Memory();
            var bus = new Bus(_mockCartridge.Object, memory, _logger);
            for (UInt16 i = 0; i < ramSize; i++)
            {
                bus.Write((ushort)(i + 0x800), _testData[i]);
            }

           //0x0-0x800 range
            for (UInt16 i = 0; i < ramSize; i++)
            {
                ushort address = (ushort)(i);
                bus.Read(address).Should().Be(_testData[i]);
            }

            //First mirror
            for (UInt16 i = 0; i < ramSize; i++)
            {
                ushort address = (ushort)(i + 0x1000);
                bus.Read(address).Should().Be(_testData[i]);
            }

            //Second Mirror
            for (UInt16 i = 0; i < ramSize; i++)
            {
                ushort address = (ushort)(i + 0x1800);
                bus.Read(address).Should().Be(_testData[i]);
            }            
        }

        [Fact]
        public void WritesToSecendMirrorShouldBeAvailableInOtherMirroredRegions()
        {
            var memory = new Memory();
            var bus = new Bus(_mockCartridge.Object, memory, _logger);
            for (UInt16 i = 0; i < ramSize; i++)
            {
                bus.Write((ushort)(i + 0x1800), _testData[i]);
            }

            //0x0-0x800 range
            for (UInt16 i = 0; i < ramSize; i++)
            {
                ushort address = (ushort)(i);
                bus.Read(address).Should().Be(_testData[i]);
            }

            //First mirror
            for (UInt16 i = 0; i < ramSize; i++)
            {
                ushort address = (ushort)(i + 0x1000);
                bus.Read(address).Should().Be(_testData[i]);
            }

            //Second Mirror
            for (UInt16 i = 0; i < ramSize; i++)
            {
                ushort address = (ushort)(i + 0x1800);
                bus.Read(address).Should().Be(_testData[i]);
            }
        }
    }
}
