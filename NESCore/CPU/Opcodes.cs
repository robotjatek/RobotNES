namespace NESCore.CPU
{
    public class Opcodes
    {
        #region Load/Store
        public const int LDA_IMM = 0xA9;
        public const int LDA_ABS = 0xAD;
        //TODO: more LDA modes
        public const int LDX_IMM = 0xA2;
        public const int LDX_ABS = 0xAE;
        //TODO: more ldx modes
        public const int LDY_IMM = 0xA0;
        //TODO: more LDY modes
        public const int STA_ZERO = 0x85;
        //TODO: More STA modes
        public const int STX_ZERO = 0x86;
        public const int STX_ABS = 0x8E;
        //TODO: more stx modes
        //TODO: STY
        #endregion

        #region Transfer
        public const int TAX = 0xAA;
        public const int TAY = 0xA8;
        public const int TSX = 0xBA;
        public const int TXA = 0x8A;
        public const int TXS = 0x9A;
        public const int TYA = 0x98;
        #endregion

        #region Stack
        public const int PHA = 0x48;
        public const int PHP = 0x08;
        public const int PLA = 0x68;
        public const int PLP = 0x28;
        #endregion

        #region Shift
        //TODO: shift instructions
        #endregion

        #region Logic
        public const int AND_IMM = 0x29;
        //TODO: more AND modes
        public const int BIT_ZERO = 0x24;
        //TODO: BIT abs
        public const int EOR_IMM = 0x49;
        //TODO: more eor modes
        public const int ORA_IMM = 0x09;
        //TODO: more ora modes
        #endregion

        #region Arith
        public const int ADC_IMM = 0x69;
        //TODO: more adc modes
        public const int CMP_IMM = 0xC9;
        //TODO: more cmp modes
        public const int CPX_IMM = 0xE0;
        //TODO: cpx abs
        //TODO: cpx zero
        public const int CPY_IMM = 0xC0;
        //TODO: cpy abs
        //TODO: cpy zero
        public const int SBC_IMM = 0xE9;
        //TODO: more sbc modes
        #endregion

        #region Inc
        //TODO: dec abs
        //TODO: x abs dec
        //TODO: zero dec
        //TODO: x zero dec
        public const int DEX = 0xCA;
        public const int DEY = 0x88;
        //TODO: inc abs
        //TODO: x abs inc
        //TODO: zero inc
        //TODO: x zero inc
        public const int INX = 0xE8;
        public const int INY = 0xC8;

        #endregion

        #region Control
        public const int JMP_ABS = 0x4C;
        public const int JSR_ABS = 0x20;
        public const int RTI = 0x40;
        public const int RTS = 0x60;
        //TODO: JMP_ABS_INDIRECT
        #endregion

        #region Branch
        public const int BCC = 0x90;
        public const int BCS = 0xB0;
        public const int BEQ = 0xF0;
        public const int BMI = 0x30;
        public const int BNE = 0xD0;
        public const int BPL = 0x10;
        public const int BVC = 0x50;
        public const int BVS = 0x70;
        #endregion

        #region Flags
        public const int CLC = 0x18;
        public const int CLD = 0xD8;
        //TODO: CLI
        public const int CLV = 0xB8;
        public const int SEC = 0x38;
        public const int SED = 0xF8;
        public const int SEI = 0x78;
        #endregion

        public const int NOP = 0xEA;
    }
}
