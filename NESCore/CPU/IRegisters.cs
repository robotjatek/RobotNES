namespace NESCore.CPU
{
    public interface IRegisters
    {
        ushort PC { get; set; }
        byte A { get; set; }
        byte X { get; set; }
        byte Y { get; set; }

        /*
         * Bit 0 Carry
         * Bit 1 Zero
         * Bit 2 Interrupt disable
         * Bit 3 Decimal
         * Bit 4 --
         * Bit 5 --
         * Bit 6 Overflow
         * Bit 7 Negative
         */
        byte STATUS { get; set; }
        byte SP { get; set; }

        void SetCarryFlag(bool flag);
        void SetZeroFlag(bool flag);
        void SetInterruptDisableFlag(bool flag);
        void SetDecimalFlag(bool flag);
        void SetOverflowFlag(bool flag);
        void SetNegativeFlag(bool flag);

        bool GetCarryFlag();
        bool GetZeroFlag();
        bool GetInterruptDisableFlag();
        bool GetDecimalFlag();
        bool GetOverflowFlag();
        bool GetNegativeFlag();
    }
}
