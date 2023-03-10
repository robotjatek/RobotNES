namespace NESCore.CPU
{
    public class Opcodes
    {
        #region Load/Store
        public const int LDA_IMM = 0xA9;
        public const int LDA_ABS = 0xAD;
        public const int LDA_ABS_X = 0xBD;
        public const int LDA_ABS_Y = 0xB9;
        public const int LDA_ZERO = 0xA5;
        public const int LDA_ZERO_X = 0xB5;
        public const int LDA_IND_X = 0xA1;
        public const int LDA_IND_Y = 0xB1;
        
        public const int LDX_IMM = 0xA2;
        public const int LDX_ABS = 0xAE;
        public const int LDX_ABS_Y = 0xBE;
        public const int LDX_ZERO = 0xA6;
        public const int LDX_ZERO_Y = 0xB6;
        
        public const int LDY_IMM = 0xA0;
        public const int LDY_ZERO = 0xA4;
        public const int LDY_ABS = 0xAC;
        public const int LDY_ABS_X = 0xBC;
        public const int LDY_ZERO_X = 0xB4;

        public const int STA_ABS = 0x8D;
        public const int STA_ABS_X = 0x9D;
        public const int STA_ABS_Y = 0x99;
        public const int STA_ZERO = 0x85;
        public const int STA_ZERO_X = 0x95;
        public const int STA_IND_X = 0x81;
        public const int STA_IND_Y = 0x91;
        
        public const int STX_ZERO = 0x86;
        public const int STX_ZERO_Y = 0x96;
        public const int STX_ABS = 0x8E;
        
        public const int STY_ZERO = 0x84;
        public const int STY_ZERO_X = 0x94;
        public const int STY_ABS = 0x8C;

        public const int LAX_IND_X = 0xA3;
        public const int LAX_ZERO = 0xA7;
        public const int LAX_ZERO_Y = 0xB7;
        public const int LAX_ABS = 0xAF;
        public const int LAX_ABS_Y = 0xBF;
        public const int LAX_IND_Y = 0xB3;

        public const int SAX_IND_X = 0x83;
        public const int SAX_ZERO = 0x87;
        public const int SAX_ZERO_Y = 0x97;
        public const int SAX_ABS = 0x8F;
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
        public const int ASL_A = 0x0A;
        public const int ASL_ABS = 0x0e;
        public const int ASL_ABS_X = 0x1e;
        public const int ASL_ZERO = 0x06;
        public const int ASL_ZERO_X = 0x16;

        public const int LSR_A = 0x4A;
        public const int LSR_ABS = 0x4E;
        public const int LSR_ABS_X = 0x5E;
        public const int LSR_ZERO = 0x46;
        public const int LSR_ZERO_X = 0x56;

        public const int ROL_A = 0x2A;
        public const int ROL_ABS = 0x2E;
        public const int ROL_ABS_X = 0x3E;
        public const int ROL_ZERO = 0x26;
        public const int ROL_ZERO_X = 0x36;

        public const int ROR_A = 0x6A;
        public const int ROR_ABS = 0x6E;
        public const int ROR_ABS_X = 0x7E;
        public const int ROR_ZERO = 0x66;
        public const int ROR_ZERO_X = 0x76;
        #endregion

        #region Logic
        public const int AND_IMM = 0x29;
        public const int AND_ABS = 0x2D;
        public const int AND_ABS_X = 0x3D;
        public const int AND_ABS_Y = 0x39;
        public const int AND_ZERO = 0x25;
        public const int AND_ZERO_X = 0x35;
        public const int AND_IND_X = 0x21;
        public const int AND_IND_Y = 0x31;
        
        public const int BIT_ZERO = 0x24;
        public const int BIT_ABS = 0x2C;

        public const int EOR_IMM = 0x49;
        public const int EOR_ABS = 0x4D;
        public const int EOR_ABS_X = 0x5D;
        public const int EOR_ABS_Y = 0x59;
        public const int EOR_ZERO = 0x45;
        public const int EOR_ZERO_X = 0x55;
        public const int EOR_IND_X = 0x41;
        public const int EOR_IND_Y = 0x51;
        
        public const int ORA_IMM = 0x09;
        public const int ORA_ABS = 0x0D;
        public const int ORA_ABS_X = 0x1D;
        public const int ORA_ABS_Y = 0x19;
        public const int ORA_ZERO = 0x05;
        public const int ORA_ZERO_X = 0x15;
        public const int ORA_IND_X = 0x01;
        public const int ORA_IND_Y = 0x11;
        #endregion

        #region Aritm
        public const int ADC_IMM = 0x69;
        public const int ADC_ABS = 0x6D;
        public const int ADC_ABS_X = 0x7D;
        public const int ADC_ABS_Y = 0x79;
        public const int ADC_ZERO = 0x65;
        public const int ADC_ZERO_X = 0x75;
        public const int ADC_IND_X = 0x61;
        public const int ADC_IND_Y = 0x71;
        
        public const int CMP_IMM = 0xC9;
        public const int CMP_ABS = 0xCD;
        public const int CMP_ABS_X = 0xDD;
        public const int CMP_ABS_Y = 0xD9;
        public const int CMP_ZERO = 0xC5;
        public const int CMP_ZERO_X = 0xD5;
        public const int CMP_IND_X = 0xC1;
        public const int CMP_IND_Y = 0xD1;
        
        public const int CPX_IMM = 0xE0;
        public const int CPX_ZERO = 0xE4;
        public const int CPX_ABS = 0xEC;

        public const int CPY_IMM = 0xC0;
        public const int CPY_ABS = 0xCC;
        public const int CPY_ZERO = 0xC4;

        public const int SBC_IMM = 0xE9;
        public const int SBC_ABS = 0xED;
        public const int SBC_ABS_X = 0xFD;
        public const int SBC_ABS_Y = 0xF9;
        public const int SBC_ZERO = 0xE5;
        public const int SBC_ZERO_X = 0xF5;
        public const int SBC_IND_X = 0xE1;
        public const int SBC_IND_Y = 0xF1;
        public const int SBC_IMM_EB = 0xEB;

        public const int DCP_IND_X = 0xC3;
        public const int DCP_ZERO = 0xC7;
        public const int DCP_ABS = 0xCF;
        public const int DCP_IND_Y = 0xD3;
        public const int DCP_ZERO_X = 0xD7;
        public const int DCP_ABS_Y = 0xDB;
        public const int DCP_ABS_X = 0xDF;

        public const int ISB_IND_X = 0xE3;
        public const int ISB_ZERO = 0xE7;
        public const int ISB_ABS = 0xEF;
        public const int ISB_IND_Y = 0xF3;
        public const int ISB_ZERO_X = 0xF7;
        public const int ISB_ABS_Y = 0xFB;
        public const int ISB_ABS_X = 0xFF;

        public const int SLO_IND_X = 0x03;
        public const int SLO_ZERO = 0x07;
        public const int SLO_ABS = 0x0F;
        public const int SLO_IND_Y = 0x13;
        public const int SLO_ZERO_X = 0x17;
        public const int SLO_ABS_Y = 0x1B;
        public const int SLO_ABS_X = 0x1F;

        public const int RLA_IND_X = 0x23;
        public const int RLA_ZERO = 0x27;
        public const int RLA_ABS = 0x2F;
        public const int RLA_IND_Y = 0x33;
        public const int RLA_ZERO_X = 0x37;
        public const int RLA_ABS_Y = 0x3B;
        public const int RLA_ABS_X = 0x3F;

        public const int SRE_IND_X = 0x43;
        public const int SRE_ZERO = 0x47;
        public const int SRE_ABS = 0x4F;
        public const int SRE_IND_Y = 0x53;
        public const int SRE_ZERO_X = 0x57;
        public const int SRE_ABS_Y = 0x5B;
        public const int SRE_ABS_X = 0x5F;

        public const int RRA_IND_X = 0x63;
        public const int RRA_ZERO = 0x67;
        public const int RRA_ABS = 0x6F;
        public const int RRA_IND_Y = 0x73;
        public const int RRA_ZERO_X = 0x77;
        public const int RRA_ABS_Y = 0x7B;
        public const int RRA_ABS_X = 0x7F;
        #endregion

        #region Inc
        public const int DEC_ABS = 0xCE;
        public const int DEC_ABS_X = 0xDE;
        public const int DEC_ZERO = 0xC6;
        public const int DEC_ZERO_X = 0xD6;

        public const int DEX = 0xCA;
        public const int DEY = 0x88;

        public const int INC_ABS = 0xEE;
        public const int INC_ABS_X = 0xFE;
        public const int INC_ZERO = 0xE6;
        public const int INC_ZERO_X = 0xF6;

        public const int INX = 0xE8;
        public const int INY = 0xC8;

        #endregion

        #region Control
        public const int BRK = 0x00;
        public const int JMP_ABS = 0x4C;
        public const int JSR_ABS = 0x20;
        public const int RTI = 0x40;
        public const int RTS = 0x60;
        public const int JMP_INDIRECT = 0x6C;
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
        public const int CLI = 0x58;
        public const int CLV = 0xB8;
        public const int SEC = 0x38;
        public const int SED = 0xF8;
        public const int SEI = 0x78;
        #endregion

        #region NOP
        public const int NOP = 0xEA;
        public const int NOP_1A = 0x1A;
        public const int NOP_3A = 0x3A;
        public const int NOP_5A = 0x5A;
        public const int NOP_7A = 0x7A;
        public const int NOP_DA = 0xDA;
        public const int NOP_FA = 0xFA;
        public const int NOP_IMM_80 = 0x80;
        public const int NOP_ZERO_04 = 0x04;
        public const int NOP_ZERO_44 = 0x44;
        public const int NOP_ZERO_64 = 0x64;
        public const int NOP_ABS_0C = 0x0C;
        public const int NOP_ABS_X_1C = 0x1C;
        public const int NOP_ABS_X_3C = 0x3C;
        public const int NOP_ABS_X_5C = 0x5C;
        public const int NOP_ABS_X_7C = 0x7C;
        public const int NOP_ABS_X_DC = 0xDC;
        public const int NOP_ABS_X_FC = 0xFC;
        public const int NOP_ZERO_X_14 = 0x14;
        public const int NOP_ZERO_X_34 = 0x34;
        public const int NOP_ZERO_X_54 = 0x54;
        public const int NOP_ZERO_X_74 = 0x74;
        public const int NOP_ZERO_X_D4 = 0xD4;
        public const int NOP_ZERO_X_F4 = 0xF4;
        #endregion
    }
}
