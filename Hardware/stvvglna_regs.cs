// ported from longmynd - https://github.com/myorangedragon/longmynd - Heather Lomond

namespace opentuner
{
    static class stvvglna_regs
    {
        public const byte STVVGLNA_I2C_ADDR0 = 0xc8;
        public const byte STVVGLNA_I2C_ADDR1 = 0xca;
        public const byte STVVGLNA_I2C_ADDR2 = 0xcc;
        public const byte STVVGLNA_I2C_ADDR3 = 0xce;
        public const byte STVVGLNA_REG0 = 0x00;
        public const byte STVVGLNA_REG0_IDENT_SHIFT = 4;
        public const byte STVVGLNA_REG0_IDENT_MASK = 0xf0;
        public const byte STVVGLNA_REG0_IDENT_DEFAULT = 0x20;
        public const byte STVVGLNA_REG0_AGC_TUPD_SHIFT = 3;
        public const byte STVVGLNA_REG0_AGC_TUPD_FAST = 0;
        public const byte STVVGLNA_REG0_AGC_TUPD_SLOW = 1;
        public const byte STVVGLNA_REG0_AGC_TLOCK_SHIFT = 2;
        public const byte STVVGLNA_REG0_AGC_TLOCK_SLOW = 0;
        public const byte STVVGLNA_REG0_AGC_TLOCK_FAST = 1;
        public const byte STVVGLNA_REG0_RFAGC_HIGH_SHIFT = 1;
        public const byte STVVGLNA_REG0_RFAGC_HIGH_NOT_HIGH = 0;
        public const byte STVVGLNA_REG0_RFAGC_HIGH_IS_HIGH = 1;
        public const byte STVVGLNA_REG0_RFAGC_LOW_SHIFT = 0;
        public const byte STVVGLNA_REG0_RFAGC_LOW_NOT_LOW = 0;
        public const byte STVVGLNA_REG0_RFAGC_LOW_IS_LOW = 1;
        public const byte STVVGLNA_REG1 = 0x01;
        public const byte STVVGLNA_REG1_LNAGC_PWD_SHIFT =  7;
        public const byte STVVGLNA_REG1_LNAGC_PWD_POWER_ON = 0;
        public const byte STVVGLNA_REG1_LNAGC_PWD_POWER_OFF =   1;
        public const byte STVVGLNA_REG1_GETOFF_SHIFT = 6;
        public const byte STVVGLNA_REG1_GETOFF_ACQUISITION_MODE =  0;
        public const byte STVVGLNA_REG1_GETOFF_VGO_4_0 = 1;
        public const byte STVVGLNA_REG1_GETAGC_SHIFT = 5;
        public const byte STVVGLNA_REG1_GETAGC_FORCED = 0;
        public const byte STVVGLNA_REG1_GETAGC_START = 1;
        public const byte STVVGLNA_REG1_VGO_SHIFT = 0;
        public const byte STVVGLNA_REG1_VGO_MASK = 0x1f;
        public const byte STVVGLNA_REG2 = 0x02;
        public const byte STVVGLNA_REG2_PATH2OFF_SHIFT = 7;
        public const byte STVVGLNA_REG2_PATH_ACTIVE = 0;
        public const byte STVVGLNA_REG2_PATH_OFF = 1;
        public const byte STVVGLNA_REG2_RFAGC_PREF_SHIFT =  4;
        public const byte STVVGLNA_REG2_RFAGC_PREF_MASK = 0x70;
        public const byte STVVGLNA_REG2_RFAGC_PREF_N25DBM = 0x0;
        public const byte STVVGLNA_REG2_RFAGC_PREF_N24DBM = 0x1;
        public const byte STVVGLNA_REG2_RFAGC_PREF_N23DBM = 0x2;
        public const byte STVVGLNA_REG2_RFAGC_PREF_N22DBM = 0x3;
        public const byte STVVGLNA_REG2_RFAGC_PREF_N21DBM = 0x4;
        public const byte STVVGLNA_REG2_RFAGC_PREF_N20DBM = 0x5;
        public const byte STVVGLNA_REG2_RFAGC_PREF_N19DBM = 0x6;
        public const byte STVVGLNA_REG2_RFAGC_PREF_N18DBM = 0x7;
        public const byte STVVGLNA_REG2_PATH1OFF_SHIFT = 3;
        public const byte STVVGLNA_REG2_RFAGC_MODE_SHIFT =  0;
        public const byte STVVGLNA_REG2_RFAGC_MODE_MASK = 0x07;
        public const byte STVVGLNA_REG2_RFAGC_MODE_AUTO_TRACK = 0x0;
        public const byte STVVGLNA_REG2_RFAGC_MODE_AUTO_REQUEST = 0x1;
        public const byte STVVGLNA_REG2_RFAGC_MODE_MINIMAL_GAIN_INTERNAL = 0x2;
        public const byte STVVGLNA_REG2_RFAGC_MODE_MAXIMAL_GAIN_INTERNAL = 0x3;
        public const byte STVVGLNA_REG2_RFAGC_MODE_EXTERNAL_AGC_EXTERNAL = 0x4;
        public const byte STVVGLNA_REG2_RFAGC_MODE_AGC_LOOP_EXTERNAL = 0x5;
        public const byte STVVGLNA_REG2_RFAGC_MODE_MINIMAL_AGC_EXTERNAL = 0x6;
        public const byte STVVGLNA_REG2_RFAGC_MODE_MAXIMAL_AGC_EXTERNAL = 0x7;
        public const byte STVVGLNA_REG3 = 0x03;
        public const byte STVVGLNA_REG3_LCAL_SHIFT = 4;
        public const byte STVVGLNA_REG3_LCAL_MASK           = 0x70 ;
        public const byte STVVGLNA_REG3_LCAL_68KHZ = 0x0;
        public const byte STVVGLNA_REG3_LCAL_34KHZ = 0x1;
        public const byte STVVGLNA_REG3_LCAL_17KHZ = 0x2;
        public const byte STVVGLNA_REG3_LCAL_8_5KHZ = 0x3;
        public const byte STVVGLNA_REG3_LCAL_4_2KHZ = 0x4;
        public const byte STVVGLNA_REG3_LCAL_2_1KHZ = 0x5;
        public const byte STVVGLNA_REG3_LCAL_1_0KHZ = 0x6;
        public const byte STVVGLNA_REG3_LCAL_0_5KHZ = 0x7;
        public const byte STVVGLNA_REG3_RFAGC_UPDATE_SHIFT = 3;
        public const byte STVVGLNA_REG3_RFAGC_UPDATE_FORCED = 0;
        public const byte STVVGLNA_REG3_RFAGC_UPDATE_START = 1;
        public const byte STVVGLNA_REG3_RFAGC_CALSTART_SHIFT =  2;
        public const byte STVVGLNA_REG3_RFAGC_CALSTART_FORCED = 0;
        public const byte STVVGLNA_REG3_RFAGC_CALSTART_START = 1;
        public const byte STVVGLNA_REG3_SWLNAGAIN_SHIFT = 0;
        public const byte STVVGLNA_REG3_SWLNAGAIN_MASK = 0x03;
        public const byte STVVGLNA_REG3_SWLNAGAIN_LOWEST = 0x0;
        public const byte STVVGLNA_REG3_SWLNAGAIN_INTERMEDIATE_LOW = 0x1;
        public const byte STVVGLNA_REG3_SWLNAGAIN_INTERMEDIATE_HIGH = 0x2;
        public const byte STVVGLNA_REG3_SWLNAGAIN_HIGHEST = 0x3;
    }
}
