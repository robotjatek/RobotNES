namespace NESCore.Cartridge
{
    public interface ICartridgeHeader
    {
        FLAGS FLAGS { get; }
        bool IsPAL { get; }
        string MAGIC { get; }
        int MapperID { get; }
        byte NumberOf16kROMBanks { get; }
        byte NumberOf8kRAMBanks { get; }
        byte NumberOf8kVROMBanks { get; }
    }
}