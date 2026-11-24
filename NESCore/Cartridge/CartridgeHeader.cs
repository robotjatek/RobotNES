namespace NESCore.Cartridge
{
    public class TrainersNotSupportedException : Exception { }

    public enum MIRRORING
    {
        HORIZONTAL = 0,
        VERTICAL = FLAG6FIELDS.MIRRORING,
    }

    public enum BATTERY_RAM
    {
        NOT_PRESENT = 0,
        PRESENT = FLAG6FIELDS.BATTERY_RAM,
    }

    public enum TRAINER
    {
        NO = 0,
        YES = FLAG6FIELDS.TRAINER,
    }

    public enum FOUR_SCREEN_MIRRORING
    {
        DISABLED = 0,
        ENABLED = FLAG6FIELDS.FOUR_SCREEN_MIRRORING,
    }

    public enum FLAG6FIELDS
    {
        MIRRORING = 0b00000001,
        BATTERY_RAM = 0b00000010,
        TRAINER = 0b00000100,
        FOUR_SCREEN_MIRRORING = 0b1000,
        MAPPER_LOWER_NIBBLE = 0b11110000,
    }

    public enum FLAG7FIELDS
    {
        MAPPER_UPPER_NIBBLE = 0b11110000,
        //Other bits are ignored
    }

    public class FLAGS
    {
        public MIRRORING Mirroring { get; private set; }

        public BATTERY_RAM BatteryRAM { get; private set; }

        public TRAINER Trainer { get; private set; }

        public FOUR_SCREEN_MIRRORING FourScreenMirroring { get; private set; }

        public byte MapperLowerNibble { get; private set; }
        public byte MapperUpperNibble { get; set; }

        public FLAGS(byte flags6, byte flags7)
        {
            Mirroring = (MIRRORING)DecodeField(flags6, (byte)FLAG6FIELDS.MIRRORING);
            BatteryRAM = (BATTERY_RAM)DecodeField(flags6, (byte)FLAG6FIELDS.BATTERY_RAM);

            Trainer = (TRAINER)DecodeField(flags6, (byte)FLAG6FIELDS.TRAINER);
            if (Trainer == TRAINER.YES)
            {
                throw new TrainersNotSupportedException();
            }

            FourScreenMirroring = (FOUR_SCREEN_MIRRORING)DecodeField(flags6, (byte)FLAG6FIELDS.FOUR_SCREEN_MIRRORING);

            MapperLowerNibble = DecodeField(flags6, (byte)FLAG6FIELDS.MAPPER_LOWER_NIBBLE);
            MapperUpperNibble = DecodeField(flags7, (byte)FLAG7FIELDS.MAPPER_UPPER_NIBBLE);
        }

        private static byte DecodeField(byte data, byte mask)
        {
            return (byte)(data & mask);
        }

    }

    public class CartridgeHeader : ICartridgeHeader
    {
        public string MAGIC { get; private set; }

        public byte NumberOf16kROMBanks { get; private set; }

        public byte NumberOf8kVROMBanks { get; private set; }

        public FLAGS FLAGS { get; private set; }

        public int MapperID { get; private set; }

        public byte NumberOf8kRAMBanks { get; private set; }
        public bool IsPAL { get; private set; }

        public CartridgeHeader(ReadOnlySpan<byte> rawBytes)
        {
            MAGIC = System.Text.Encoding.UTF8.GetString(rawBytes[0..4]);

            NumberOf16kROMBanks = rawBytes[4];
            NumberOf8kVROMBanks = rawBytes[5];

            FLAGS = new FLAGS(rawBytes[6], rawBytes[7]);
            MapperID = FLAGS.MapperUpperNibble | FLAGS.MapperLowerNibble >> 4;
            NumberOf8kRAMBanks = rawBytes[8] != 0 ? rawBytes[8] : (byte)1; // The iNES wiki states value 0 should be considered 1 for compatibility reasons. Also states that this field is rarely used
            IsPAL = (rawBytes[9] & 0x3) > 0;
        }
    }
}
