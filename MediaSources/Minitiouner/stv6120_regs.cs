// ported from longmynd - https://github.com/myorangedragon/longmynd - Heather Lomond

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    public static class stv6120_regs
    {
        public static byte STV6120_CTRL1 = 0x00;
        public static byte STV6120_CTRL1_K_SHIFT = 3;
        public static byte STV6120_CTRL1_K_MASK = 0xf8;
        public static byte STV6120_CTRL1_RDIV_SHIFT = 2;
        public static byte STV6120_CTRL1_OSHAPE_SHIFT = 1;
        public static byte STV6120_CTRL1_OSHAPE_SINE = 0;
        public static byte STV6120_CTRL1_OSHAPE_SQUARE = 1;
        public static byte STV6120_CTRL1_MCLKDIV_SHIFT = 0;
        public static byte STV6120_CTRL1_MCLKDIV_2 = 0;
        public static byte STV6120_CTRL1_MCLKDIV_4 = 1;
        public static byte STV6120_CTRL2 = 0x01;
        public static byte STV6120_CTRL2_DCLOOPOFF_SHIFT = 7;
        public static byte STV6120_CTRL2_DCLOOPOFF_ENABLE = 0;
        public static byte STV6120_CTRL2_DCLOOPOFF_DISABLE = 1;
        public static byte STV6120_CTRL2_SDOFF_SHIFT = 6;
        public static byte STV6120_CTRL2_SDOFF_OFF = 0;
        public static byte STV6120_CTRL2_SDOFF_ON = 1;
        public static byte STV6120_CTRL2_SYN_SHIFT = 5;
        public static byte STV6120_CTRL2_SYN_OFF = 0;
        public static byte STV6120_CTRL2_SYN_ON = 1;
        public static byte STV6120_CTRL2_REFOUTSEL_SHIFT = 4;
        public static byte STV6120_CTRL2_REFOUTSEL_VCC_DIV_2 = 0;
        public static byte STV6120_CTRL2_REFOUTSEL_1_25V = 1;
        public static byte STV6120_CTRL2_BBGAIN_SHIFT = 0;
        public static byte STV6120_CTRL2_BBGAIN_0DB = 0x0;
        public static byte STV6120_CTRL2_BBGAIN_2DB = 0x1;
        public static byte STV6120_CTRL2_BBGAIN_4DB = 0x2;
        public static byte STV6120_CTRL2_BBGAIN_6DB = 0x3;
        public static byte STV6120_CTRL2_BBGAIN_8DB = 0x4;
        public static byte STV6120_CTRL2_BBGAIN_10DB = 0x5;
        public static byte STV6120_CTRL2_BBGAIN_12DB = 0x6;
        public static byte STV6120_CTRL2_BBGAIN_14DB = 0x7;
        public static byte STV6120_CTRL2_BBGAIN_16DB = 0x8;

        public static byte STV6120_CTRL3 = 0x02;

        public static byte STV6120_CTRL4 = 0x03;
        public static byte STV6120_CTRL4_F_6_0_SHIFT = 1;
        public static byte STV6120_CTRL4_F_6_0_MASK = 0xfe;
        public static byte STV6120_CTRL4_N_9_SHIFT = 0;

        public static byte STV6120_CTRL5 = 0x04;

        public static byte STV6120_CTRL6 = 0x05;
        public static byte STV6120_CTRL6_ICP_SHIFT = 4;
        public static byte STV6120_CTRL6_ICP_MASK = 0x70;
        public static byte STV6120_CTRL6_ICP_300UA = 0;
        public static byte STV6120_CTRL6_ICP_325UA = 1;
        public static byte STV6120_CTRL6_ICP_360UA = 2;
        public static byte STV6120_CTRL6_ICP_400UA = 3;
        
        public static byte STV6120_CTRL6_ICP_400UA_2 = 4;
        public static byte STV6120_CTRL6_ICP_450UA = 5;
        public static byte STV6120_CTRL6_ICP_525UA = 6;
        public static byte STV6120_CTRL6_ICP_600UA = 7;
        public static byte STV6120_CTRL6_F_17_15_SHIFT =  0;
        public static byte STV6120_CTRL6_F_17_15_MASK = 0x07;
        public static byte STV6120_CTRL6_RESERVED = 0x08;

        public static byte STV6120_CTRL7 = 0x06;
        public static byte STV6120_CTRL7_RCCLKOFF_SHIFT = 7;
        public static byte STV6120_CTRL7_RCCLKOFF_ENABLE = 0;
        public static byte STV6120_CTRL7_RCCLKOFF_DISABLE= 1;
        public static byte STV6120_CTRL7_PDIV_SHIFT = 5;
        public static byte STV6120_CTRL7_CF_SHIFT = 0;
        public static byte STV6120_CTRL7_CF_5MHZ = 0x00;

        public static byte STV6120_CTRL8 = 0x07;
        public static byte STV6120_CTRL8_TCAL_SHIFT = 6;
        public static byte STV6120_CTRL8_TCAL_MASK = 0xc0;
        public static byte STV6120_CTRL8_TCAL_DIV_1 = 0;
        public static byte STV6120_CTRL8_TCAL_DIV_2 = 1;
        public static byte STV6120_CTRL8_TCAL_DIV_4 = 2;
        public static byte STV6120_CTRL8_TCAL_DIV_8 = 3;
        public static byte STV6120_CTRL8_CALTIME_SHIFT =  5;
        public static byte STV6120_CTRL8_CALTIME_500US = 0;
        public static byte STV6120_CTRL8_CALTIME_1MS = 1;
        public static byte STV6120_CTRL8_CFHF_SHIFT = 0;
        public static byte STV6120_CTRL8_CFHF_MASK = 0x1f;

        public static byte STV6120_STAT1 = 0x08;
        public static byte STV6120_STAT1_CALVCOSTRT_SHIFT = 2;
        public static byte STV6120_STAT1_CALVCOSTRT_FINISHED = 0;
        public static byte STV6120_STAT1_CALVCOSTRT_START = 1;
        public static byte STV6120_STAT1_CALRCSTRT_SHIFT = 1;
        public static byte STV6120_STAT1_CALRCSTRT_FINISHED = 0;
        public static byte STV6120_STAT1_CALRCSTRT_START = 1;
        public static byte STV6120_STAT1_LOCK_SHIFT = 0;
        public static byte STV6120_STAT1_LOCK_NOT_IN_LOCK = 0;
        public static byte STV6120_STAT1_LOCK_LOCKED = 1;
        public static byte STV6120_STAT1_RESERVED = 0x08;

        public static byte STV6120_CTRL9 = 0x09;
        public static byte STV6120_CTRL9_RFSEL_2_SHIFT = 2;
        public static byte STV6120_CTRL9_RFSEL_2_MASK = 0x0c;
        public static byte STV6120_CTRL9_RFSEL_RFA_IN = 0;
        public static byte STV6120_CTRL9_RFSEL_RFB_IN = 1;
        public static byte STV6120_CTRL9_RFSEL_RFC_IN = 2;
        public static byte STV6120_CTRL9_RFSEL_RFD_IN = 3;
        public static byte STV6120_CTRL9_RFSEL_1_SHIFT =  0;
        public static byte STV6120_CTRL9_RFSEL_1_MASK = 0x03;
        public static byte STV6120_CTRL9_RESERVED = 0xf0;

        public static byte STV6120_CTRL10 = 0x0a;
        public static byte STV6120_CTRL10_DEFAULT = 0xbf;
        public static byte STV6120_CTRL10_ID_SHIFT = 6;
        public static byte STV6120_CTRL10_ID_MASK = 0xc0;
        public static byte STV6120_CTRL10_LNADON_SHIFT = 5;
        public static byte STV6120_CTRL10_LNACON_SHIFT = 4;
        public static byte STV6120_CTRL10_LNA_OFF = 0;
        public static byte STV6120_CTRL10_LNA_ON = 1;
        public static byte STV6120_CTRL10_LNABON_SHIFT = 3;
        public static byte STV6120_CTRL10_LNAAON_SHIFT = 2;
        public static byte STV6120_CTRL10_PATHON_2_SHIFT = 1;
        public static byte STV6120_CTRL10_PATH_OFF = 0;
        public static byte STV6120_CTRL10_PATH_ON = 1;
        public static byte STV6120_CTRL10_PATHON_1_SHIFT = 0;

        public static byte STV6120_CTRL11 = 0x0b;
        public static byte STV6120_CTRL12 = 0x0c;
        public static byte STV6120_CTRL13 = 0x0d;
        public static byte STV6120_CTRL14 = 0x0e;
        public static byte STV6120_CTRL15 = 0x0f;
        public static byte STV6120_CTRL16 = 0x10;
        public static byte STV6120_CTRL17 = 0x11;
        public static byte STV6120_STAT2 = 0x12;

        public static byte STV6120_CTRL18 = 0x13;
        public static byte STV6120_CTRL18_DEFAULT = 0x00;
        public static byte STV6120_CTRL19 = 0x14;
        public static byte STV6120_CTRL19_DEFAULT = 0x00;

        public static byte STV6120_CTRL20 = 0x15;
        public static byte STV6120_CTRL20_VCOAMP_SHIFT = 6;
        public static byte STV6120_CTRL20_VCOAMP_MASK = 0xc0;
        public static byte STV6120_CTRL20_VCOAMP_AUTO = 0;
        public static byte STV6120_CTRL20_VCOAMP_LOW = 1;
        public static byte STV6120_CTRL20_VCOAMP_NORMAL = 2;
        public static byte STV6120_CTRL20_VCOAMP_VERY_LOW = 3;
        public static byte STV6120_CTRL20_RESERVED = 0x0c;

        public static byte STV6120_CTRL21 = 0x16;
        public static byte STV6120_CTRL21_DEFAULT = 0x00;
        public static byte STV6120_CTRL22 = 0x17;
        public static byte STV6120_CTRL22_DEFAULT = 0x00;

        public static byte STV6120_CTRL23 = 0x18;

        public static byte STV6120_CTRL10_RESET = 0xbf;		// register reset value
        public static byte STV6120_CTRL8_CALTIME_MASK = 0x20;
        //public static byte STV6120_CTRL7_CF_5MHZ = 0x00;
        public static byte STV6120_CTRL7_CF_23MHZ = 0x12;
        //public static byte STV6120_CTRL1_OSHAPE_SHIFT = 1;
        //public static byte STV6120_CTRL10_LNADON_SHIFT = 5;
        //public statuc byte STV6120_CTRL10_LNACON_SHIFT = 4;

    }
}
