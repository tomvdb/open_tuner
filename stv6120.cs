// ported from longmynd - https://github.com/myorangedragon/longmynd - Heather Lomond

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    class stv6120
    {
        nim nim_device;

        const byte TUNER_LOCKED = 0;
        const byte TUNER_NOT_LOCKED = 1;

        const byte TUNER_1 = 1;
        const byte TUNER_2 = 2;

        const UInt32 STV6120_RDIV_THRESHOLD = 27000;
        const UInt32 STV6120_P_THRESHOLD_1 = 299000;
        const UInt32 STV6120_P_THRESHOLD_2 = 596000;
        const UInt32 STV6120_P_THRESHOLD_3 = 1191000;

        const byte STV6120_CAL_TIMEOUT = 200;

        byte rdiv;
        byte ctrl7;
        byte ctrl8;
        byte ctrl16;
        byte ctrl17;


        UInt32[,] stv6120_icp_lookup = new UInt32[,] {
     /* low     high  icp */
    {2380000, 2472000, 0},
    {2473000, 2700000, 1},  
    {2701000, 3021000, 2},  
    {3022000, 3387000, 3},
    {3388000, 3845000, 5},
    {3846000, 4394000, 6},
    {4395000, 4760000, 7} 
};

        ushort[] stv6120_cfhf = new ushort[] {6796, 5828, 4778, 4118, 3513, 3136, 2794, 2562,
                            2331, 2169, 2006, 1890, 1771, 1680, 1586, 1514,
                            1433, 1374, 1310, 1262, 1208, 1167, 1122, 1087,
                            1049, 1018,  983,  956,  926,  902,  875,  854 };


        public stv6120(nim _nim_device)
        {
            nim_device = _nim_device;
        }

        private byte stv6120_write_reg(byte reg, byte val)
        {
            return nim_device.nim_write_tuner(reg, val);
        }

        private byte stv6120_read_reg(byte reg, ref byte val)
        {
            return nim_device.nim_read_tuner(reg, ref val);
        }

        public byte stv6120_read_rf_sel(ref byte rfsel)
        {
            byte err = 0;
            byte temp = 0;

            err = stv6120_read_reg(stv6120_regs.STV6120_CTRL9, ref temp);

            rfsel = temp;
            return err;
        }

        public byte stv6120_init(byte nimtuner, UInt32 freq_tuner, UInt32 antenna, UInt32 symbolrate)
        {
            byte err = 0;
            byte k;
            byte temp = 0;
            byte temp9 = 0;
            byte temp10 = 0;

            Console.WriteLine("Flow: Tuner " + nimtuner.ToString() + " init " + freq_tuner.ToString() + "  antenna " + antenna.ToString() + " \r\n");

            // Note: the tuners are set up separately as required, so the other one must not be disturbed

            if (true) 
            {                                                   // LNA-A and LNA-D will be turned off lower down		

                // we calculate K from: F_xtal / (K+16) = 1MHz, as specified in the datasheet

                k = nim.NIM_TUNER_XTAL / 1000 - 16;

                // set up the common clock for both tuners (note rdiv is a global) 

                if (nim.NIM_TUNER_XTAL >= STV6120_RDIV_THRESHOLD)
                {
                    rdiv = 1;       // from the data sheet
                }
                else
                {
                    rdiv = 0;
                }

                if (err == 0)
                {
                    err = stv6120_write_reg
                    (
                        stv6120_regs.STV6120_CTRL1,
                        (byte)((k << stv6120_regs.STV6120_CTRL1_K_SHIFT) |
                        (rdiv << stv6120_regs.STV6120_CTRL1_RDIV_SHIFT) |
                        (stv6120_regs.STV6120_CTRL1_OSHAPE_SINE << stv6120_regs.STV6120_CTRL1_OSHAPE_SHIFT) |
                        (stv6120_regs.STV6120_CTRL1_MCLKDIV_4 << stv6120_regs.STV6120_CTRL1_MCLKDIV_SHIFT))
                    );
                    err = stv6120_write_reg
                    (
                        stv6120_regs.STV6120_CTRL1,
                        (byte)((k << stv6120_regs.STV6120_CTRL1_K_SHIFT) |
                        (rdiv << stv6120_regs.STV6120_CTRL1_RDIV_SHIFT) |
                        (stv6120_regs.STV6120_CTRL1_OSHAPE_SINE << stv6120_regs.STV6120_CTRL1_OSHAPE_SHIFT) |
                        (stv6120_regs.STV6120_CTRL1_MCLKDIV_4 << stv6120_regs.STV6120_CTRL1_MCLKDIV_SHIFT))
                    );
                }

                // set up the CAL registers

                if (err == 0)
                {
                    err = stv6120_read_reg(stv6120_regs.STV6120_CTRL8, ref temp);
                    temp &= (byte)(~(stv6120_regs.STV6120_CTRL8_TCAL_MASK | stv6120_regs.STV6120_CTRL8_CALTIME_MASK));
                    temp |= (byte)((stv6120_regs.STV6120_CTRL8_TCAL_DIV_2 << stv6120_regs.STV6120_CTRL8_TCAL_SHIFT) |
                              (stv6120_regs.STV6120_CTRL8_CALTIME_500US << stv6120_regs.STV6120_CTRL8_CALTIME_SHIFT));     // changed from TCAL_SHIFT

                    err = stv6120_write_reg(stv6120_regs.STV6120_CTRL8, temp);
                }

                if (err == 0)
                {
                    err = stv6120_read_reg(stv6120_regs.STV6120_CTRL17, ref temp);
                    temp &= (byte)(~stv6120_regs.STV6120_CTRL8_CALTIME_MASK);
                    temp |= (byte)(stv6120_regs.STV6120_CTRL8_CALTIME_500US << stv6120_regs.STV6120_CTRL8_CALTIME_SHIFT);     // changed from TCAL_SHIFT   
                    err = stv6120_write_reg(stv6120_regs.STV6120_CTRL17, temp);
                }
            }


            //	setup the LNAs and path registers
            //	LNA-A and LNA-D are not used 
            //	LNA-B is fed from the TOP NIM input, LNB-C from the BOTTOM 

            if (nimtuner == TUNER_1)
            {

                if (err == 0)
                {
                    err = stv6120_write_reg
                    (
                        stv6120_regs.STV6120_CTRL2,
                        (byte)((stv6120_regs.STV6120_CTRL2_DCLOOPOFF_ENABLE << stv6120_regs.STV6120_CTRL2_DCLOOPOFF_SHIFT) |
                        (stv6120_regs.STV6120_CTRL2_SDOFF_OFF << stv6120_regs.STV6120_CTRL2_SDOFF_SHIFT) |
                        (stv6120_regs.STV6120_CTRL2_SYN_ON << stv6120_regs.STV6120_CTRL2_SYN_SHIFT) |
                        (stv6120_regs.STV6120_CTRL2_REFOUTSEL_1_25V << stv6120_regs.STV6120_CTRL2_REFOUTSEL_SHIFT) |
                        (stv6120_regs.STV6120_CTRL2_BBGAIN_6DB << stv6120_regs.STV6120_CTRL2_BBGAIN_SHIFT))
                    );
                }

                if (symbolrate == 27500 || symbolrate == 22000)
                {
                    temp = stv6120_regs.STV6120_CTRL7_CF_23MHZ;
                }
                else
                {
                    temp = stv6120_regs.STV6120_CTRL7_CF_5MHZ;
                }
                if (err == 0)
                {
                    ctrl7 = (byte)((stv6120_regs.STV6120_CTRL7_RCCLKOFF_DISABLE << stv6120_regs.STV6120_CTRL7_RCCLKOFF_SHIFT) |
                              (temp << stv6120_regs.STV6120_CTRL7_CF_SHIFT));                 // needs changing for higher SR

                    err = stv6120_write_reg(stv6120_regs.STV6120_CTRL7, ctrl7);
                }

                temp10 = 0;
                err |= stv6120_read_reg(stv6120_regs.STV6120_CTRL10, ref temp10);       // get LNA / path selection register

                if (freq_tuner > 0)
                {
                    temp10 |= (byte)(1 << stv6120_regs.STV6120_CTRL10_PATHON_1_SHIFT);           // enable path 1 (nimtuner 1)
                }
                else
                {
                    temp10 &= (byte)(~(1 << stv6120_regs.STV6120_CTRL10_PATHON_1_SHIFT));        // disable path 1 (nimtuner 1)
                }

                temp9 = 0;

                err |= stv6120_read_reg(stv6120_regs.STV6120_CTRL9, ref temp9);         // get the antenna selection register
                temp9 &= (byte)(~(3 << stv6120_regs.STV6120_CTRL9_RFSEL_1_SHIFT));               // clear the LNA entry for tuner 1
                temp10 &= (byte)(~(1 << stv6120_regs.STV6120_CTRL10_LNAAON_SHIFT));              // LNA-A is not connected, so turn it off
                temp10 &= (byte)(~(1 << stv6120_regs.STV6120_CTRL10_LNADON_SHIFT));              // LNA-D is not connected

                if (antenna == nim.NIM_INPUT_TOP)
                {
                    temp9 |= (byte)(stv6120_regs.STV6120_CTRL9_RFSEL_RFB_IN << stv6120_regs.STV6120_CTRL9_RFSEL_1_SHIFT);
                    temp10 |= (byte)(1 << stv6120_regs.STV6120_CTRL10_LNABON_SHIFT);         // LNA-B is on
                }
                else if (antenna == nim.NIM_INPUT_TOP)
                {
                    temp9 |= (byte)(stv6120_regs.STV6120_CTRL9_RFSEL_RFC_IN << stv6120_regs.STV6120_CTRL9_RFSEL_1_SHIFT);
                    temp10 |= (byte)(1 << stv6120_regs.STV6120_CTRL10_LNACON_SHIFT);         // LNA-C is on
                }
                err |= stv6120_write_reg(stv6120_regs.STV6120_CTRL9, temp9);         // update the registers
                err |= stv6120_write_reg(stv6120_regs.STV6120_CTRL10, temp10);
            }
            else if (nimtuner == TUNER_2)
            {
                if (err == 0)
                {
                    err = stv6120_write_reg
                    (
                        stv6120_regs.STV6120_CTRL11,(byte)(
                        (stv6120_regs.STV6120_CTRL2_DCLOOPOFF_ENABLE << stv6120_regs.STV6120_CTRL2_DCLOOPOFF_SHIFT) |
                        (stv6120_regs.STV6120_CTRL2_SDOFF_OFF << stv6120_regs.STV6120_CTRL2_SDOFF_SHIFT) |
                        (stv6120_regs.STV6120_CTRL2_SYN_ON << stv6120_regs.STV6120_CTRL2_SYN_SHIFT) |
                        (stv6120_regs.STV6120_CTRL2_REFOUTSEL_1_25V << stv6120_regs.STV6120_CTRL2_REFOUTSEL_SHIFT) |
                        (stv6120_regs.STV6120_CTRL2_BBGAIN_6DB << stv6120_regs.STV6120_CTRL2_BBGAIN_SHIFT))
                    );
                }

                if (symbolrate == 27500 || symbolrate == 22000)
                {
                    temp = stv6120_regs.STV6120_CTRL7_CF_23MHZ;
                }
                else
                {
                    temp = stv6120_regs.STV6120_CTRL7_CF_5MHZ;
                }
                if (err == 0)
                {
                    ctrl16 = (byte)((stv6120_regs.STV6120_CTRL7_RCCLKOFF_DISABLE << stv6120_regs.STV6120_CTRL7_RCCLKOFF_SHIFT) |
                              (temp << stv6120_regs.STV6120_CTRL7_CF_SHIFT));             // needs changing for higher SR

                    err = stv6120_write_reg(stv6120_regs.STV6120_CTRL16, ctrl16);
                }

                temp10 = 0;
                err |= stv6120_read_reg(stv6120_regs.STV6120_CTRL10, ref temp10);       // get LNA / path selection register

                if (freq_tuner > 0)
                {
                    temp10 |= (byte)(1 << stv6120_regs.STV6120_CTRL10_PATHON_2_SHIFT);           // enable path 1 (nimtuner 1)
                }
                else
                {
                    temp10 &= (byte)(~(1 << stv6120_regs.STV6120_CTRL10_PATHON_2_SHIFT));        // disable path 1 (nimtuner 1)
                }

                temp9 = 0;

                err |= stv6120_read_reg(stv6120_regs.STV6120_CTRL9, ref temp9);         // get the antenna selection register
                temp9 &= (byte)(~(3 << stv6120_regs.STV6120_CTRL9_RFSEL_2_SHIFT));               // clear the LNA entry for tuner 2
                temp10 &= (byte)(~(1 << stv6120_regs.STV6120_CTRL10_LNAAON_SHIFT));              // LNA-A is not connected, so turn it off
                temp10 &= (byte)(~(1 << stv6120_regs.STV6120_CTRL10_LNADON_SHIFT));              // LNA-D is not connected

                if (antenna == nim.NIM_INPUT_TOP)
                {
                    temp9 |= (byte)(stv6120_regs.STV6120_CTRL9_RFSEL_RFB_IN << stv6120_regs.STV6120_CTRL9_RFSEL_2_SHIFT);
                    temp10 |= (byte)(1 << stv6120_regs.STV6120_CTRL10_LNABON_SHIFT);         // LNA-B is on
                }
                else if (antenna == nim.NIM_INPUT_BOTTOM)
                {
                    temp9 |= (byte)(stv6120_regs.STV6120_CTRL9_RFSEL_RFC_IN << stv6120_regs.STV6120_CTRL9_RFSEL_2_SHIFT);
                    temp10 |= (byte)(1 << stv6120_regs.STV6120_CTRL10_LNACON_SHIFT);         // LNA-C is on
                }
                err |= stv6120_write_reg(stv6120_regs.STV6120_CTRL9, temp9);         // update the registers
                err |= stv6120_write_reg(stv6120_regs.STV6120_CTRL10, temp10);
            }

            ///	err |= stvvglna_init (antenna,STVVGLNA_ON,0) ;					// turn on the top or bottom LNA	



            /* no need to touch the STAT2 status register for now */

            /* CTRL18, CTRL19 are test regs so just write in the default */

            if (err == 0)
            {
                err = stv6120_write_reg(stv6120_regs.STV6120_CTRL18, stv6120_regs.STV6120_CTRL18_DEFAULT);
            }
            if (err == 0)
            {
                err = stv6120_write_reg(stv6120_regs.STV6120_CTRL19, stv6120_regs.STV6120_CTRL19_DEFAULT);
            }
            if (err == 0)
            {
                err = stv6120_write_reg
                (
                    stv6120_regs.STV6120_CTRL20, (byte)((stv6120_regs.STV6120_CTRL20_VCOAMP_NORMAL << stv6120_regs.STV6120_CTRL20_VCOAMP_SHIFT) | stv6120_regs.STV6120_CTRL20_RESERVED)
                );
            }

            /* CTRL21, CTRL22 are test regs so leave alone */

            if (err == 0)
            {
                err = stv6120_write_reg(stv6120_regs.STV6120_CTRL21, stv6120_regs.STV6120_CTRL21_DEFAULT);
            }

            if (err == 0)
            {
                err = stv6120_write_reg(stv6120_regs.STV6120_CTRL22, stv6120_regs.STV6120_CTRL22_DEFAULT);
            }

            if (err == 0)
            {
                err = stv6120_write_reg
                (
                    stv6120_regs.STV6120_CTRL23, (byte)((stv6120_regs.STV6120_CTRL20_VCOAMP_NORMAL << stv6120_regs.STV6120_CTRL20_VCOAMP_SHIFT) | stv6120_regs.STV6120_CTRL20_RESERVED)
                );
            }


            /* now we can calibrate the lowpass filters and setup the PLLs for each tuner required */

            if ((err == 0) && (freq_tuner > 0))
            {
                err = stv6120_cal_lowpass(nimtuner);
                if (err == 0)
                {
                    err = stv6120_set_freq(nimtuner, freq_tuner);
                }
            }

            if (err != 0)
            {
                Console.WriteLine("ERROR: Failed to init Tuner%i, %i\r\n", nimtuner, freq_tuner);
            }

            return (err);
        }


        /* -------------------------------------------------------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------- */

        byte stv6120_cal_lowpass(byte nimtuner)
        {
            byte err = 0;
            byte val;
            ushort timeout;

            Console.WriteLine("Flow: Tuner cal lowpass\n");

            /* turn on the clock for the low pass filter. This is in ctrl7/16 so we have a shadow for it */

            if (nimtuner == TUNER_1)
            {
                err = stv6120_write_reg(stv6120_regs.STV6120_CTRL7, (byte)(ctrl7 & ~(1 << stv6120_regs.STV6120_CTRL7_RCCLKOFF_SHIFT)));
            }
            else
            {
                err = stv6120_write_reg(stv6120_regs.STV6120_CTRL16, (byte)(ctrl16 & ~(1 << stv6120_regs.STV6120_CTRL7_RCCLKOFF_SHIFT)));
            }

            /* now we can do a low pass filter calibration, by setting the CALRCSTRT bit. NOte it is safe to just write to it */

            if (err == 0)
            {
                err = stv6120_write_reg
                (
                    nimtuner == TUNER_1 ? stv6120_regs.STV6120_STAT1 : stv6120_regs.STV6120_STAT2, (byte)(stv6120_regs.STV6120_STAT1_CALRCSTRT_START << stv6120_regs.STV6120_STAT1_CALRCSTRT_SHIFT)
                );
            }

            /* wait for the bit to be cleared  to say cal has finished */

            if (err == 0)
            {
                timeout = 0;
                do
                {
                    val = 0;
                    err = stv6120_read_reg(stv6120_regs.STV6120_STAT1, ref val);
                    timeout++;
                    if (timeout == STV6120_CAL_TIMEOUT)
                    {
                        err = errors.ERROR_TUNER_CAL_LOWPASS_TIMEOUT;
                        Console.WriteLine("ERROR: tuner wait on CAL_lowpass timed out\n");
                    }
                } while ((err == 0) && ((val & (1 << stv6120_regs.STV6120_STAT1_CALRCSTRT_SHIFT)) == (1 << stv6120_regs.STV6120_STAT1_CALRCSTRT_SHIFT)));
            }

            /* turn off the low pass filter clock (=1) */

            if (err == 0)
            {
                err = stv6120_write_reg(nimtuner == TUNER_1 ? stv6120_regs.STV6120_CTRL7 : stv6120_regs.STV6120_CTRL16, (nimtuner == TUNER_1 ? ctrl7 : ctrl16));
            }

            if (err != 0)
            {
                Console.WriteLine("ERROR: Failed to cal lowpass filter\n");
            }

            return (err);
        }

        /* -------------------------------------------------------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------- */
        /* Sets one of the tuners to the given frequency                                                      */
        /*                                                                                                    */
        /*   nimtuner: 	TUNER_1  |  TUNER_2 : which tuner we are going to work on                             */
        /*    	 freq: 	the frequency to set the tuner to in KHz                                              */
        /*     return: 	error code                                                                            */
        /*                                                                                                    */
        /* when locked,  F.lo = F.vco/P = (F.xtal/R) * (N+F/2^18)/P                                           */
        /* -------------------------------------------------------------------------------------------------- */

        byte stv6120_set_freq(byte nimtuner, UInt32 freq)
        {
            byte err = 0;
            byte val;
            byte pos;
            UInt32 f;
            ushort n;
            byte p;
            byte icp;
            UInt32 f_vco;
            ushort timeout;
            byte cfhf;

            Console.WriteLine("Flow: Tuner set freq\n");

            /* the global rdiv has already been set up in the init routines */

            /* p is defined from the datasheet (note, this is reg value, not P) */

            if (freq <= STV6120_P_THRESHOLD_1)
            {
                p = 3;                             /* P = 16 */
            }
            else if (freq <= STV6120_P_THRESHOLD_2)
            {
                p = 2;                             /* P = 8 */
            }
            else if (freq <= STV6120_P_THRESHOLD_3)
            {
                p = 1;                             /* P = 4 */
            }
            else
            {
                p = 0;                             /* P = 2 */
            }

            /* we have to be careful of the size of the typesi in the following  */
            /* F.vco=F.rf*P where F.rf=F.lo    all in KHz */
            /* f_vco is uint32_t, so p_max is 3 (i.e P_max is 16), freq_max is 2500000KHz, results is 0x02625a00 ... OK */

            f_vco = freq << (p + 1);

            /* n=integer(f_vco/f_xtal*R)  note: f_xtal and f_vco both in KHz */
            /* we do the *R first (a shift by rdiv), and max is 0x04c4b400, then the divide and we are OK */

            n = (ushort)((f_vco << rdiv) / nim.NIM_TUNER_XTAL);

            /* f = fraction(f_vco/f_xtal*R).2^18 */
            /* as for n, we do the shift first (which we know is safe), then modulus to get the fraction */
            /* then we have to go to 64 bits to do the shift and divide, and then back to uint32_t for the result */

            f = (UInt32)((((UInt64)((f_vco << rdiv) % nim.NIM_TUNER_XTAL)) << 18) / nim.NIM_TUNER_XTAL);

            /* lookup the ICP value in the lookup table as per datasheet */

            pos = 0;
            while (f_vco > stv6120_icp_lookup[pos++,1]) ;
            icp = (byte)stv6120_icp_lookup[pos - 1,2];

            /* lookup the high freq filter cutoff setting as per datasheet */

            cfhf = 0;
            while ((3 * freq / 1000) <= stv6120_cfhf[cfhf])
            {
                cfhf++;
            }
            cfhf--;    /* we are sure it isn't greater then the first array element so this is safe */
            

            /* now we fill in the PLL and ICP values */

            if (err == 0)
            {
                err = stv6120_write_reg
                (
                    nimtuner == TUNER_1 ? stv6120_regs.STV6120_CTRL3 : stv6120_regs.STV6120_CTRL12, (byte)(n & 0x00ff)       /* set N[7:0] */
                );
            }
            if (err == 0) /* set F[6:0] */
            {
                err = stv6120_write_reg
                (
                    nimtuner == TUNER_1 ? stv6120_regs.STV6120_CTRL4 : stv6120_regs.STV6120_CTRL13, (byte)((byte)((f & 0x0000007f) << 1) | ((n & 0x0100) >> 8))     /* N[8] */
                );
            }
            if (err == 0)
            {
                err = stv6120_write_reg
                (
                    nimtuner == TUNER_1 ? stv6120_regs.STV6120_CTRL5 : stv6120_regs.STV6120_CTRL14, (byte)((f & 0x00007f80) >> 7)            /* set F[14:7] */
                );
            }
            if (err == 0)
            {
                err = stv6120_write_reg
                (
                    nimtuner == TUNER_1 ? stv6120_regs.STV6120_CTRL6 : stv6120_regs.STV6120_CTRL15,
                    (byte)(
                        ((f & 0x00038000) >> 15) |                                          /* set f[17:15] */
                        (icp << stv6120_regs.STV6120_CTRL6_ICP_SHIFT) |                                  /* ICP[2:0] */
                        stv6120_regs.STV6120_CTRL6_RESERVED                                              /* reserved bit */
                    )
                );
            }
            if (nimtuner == TUNER_1)
            {
                if (err == 0)
                {
                    err = stv6120_write_reg(stv6120_regs.STV6120_CTRL7, (byte)((p << stv6120_regs.STV6120_CTRL7_PDIV_SHIFT) | ctrl7)); /* put back in RCCLKOFF_1 as well */
                }

                if (err == 0)
                {
                    err = stv6120_write_reg(stv6120_regs.STV6120_CTRL8, (byte)((cfhf << stv6120_regs.STV6120_CTRL8_CFHF_SHIFT) | ctrl8));
                }

            }
            else    /* tuner = TUNER_2 */
            {
                if (err == 0)
                {
                    err = stv6120_write_reg(stv6120_regs.STV6120_CTRL16, (byte)((p << stv6120_regs.STV6120_CTRL7_PDIV_SHIFT) | ctrl16)); /* put back in RCCLKOFF_2 as well */
                }
                if (err == 0)
                {
                    err = stv6120_write_reg(stv6120_regs.STV6120_CTRL17, (byte)((cfhf << stv6120_regs.STV6120_CTRL8_CFHF_SHIFT) | ctrl17));
                }
            }

            /* if we change the filter re-cal it, and if we change VCO we have to re-cal it, so here goes */

            if (err == 0)
            {
                err = stv6120_write_reg
                (
                    nimtuner == TUNER_1 ? stv6120_regs.STV6120_STAT1 : stv6120_regs.STV6120_STAT2,                    /* start CALVCOSTRT */
                    (byte)((stv6120_regs.STV6120_STAT1_CALVCOSTRT_START << stv6120_regs.STV6120_STAT1_CALVCOSTRT_SHIFT) |
                    stv6120_regs.STV6120_STAT1_RESERVED)
                );
            }

            /* wait for CALVCOSTRT bit to go low to say VCO cal is finished */

            if (err == 0)
            {
                timeout = 0;
                do
                {
                    val = 0;
                    err = stv6120_read_reg(nimtuner == TUNER_1 ? stv6120_regs.STV6120_STAT1 : stv6120_regs.STV6120_STAT2, ref val);
                    timeout++;
                }
                while
                (
                    (err == 0) &&
                    (timeout < STV6120_CAL_TIMEOUT) &&
                    ((val & (1 << stv6120_regs.STV6120_STAT1_CALVCOSTRT_SHIFT)) != (stv6120_regs.STV6120_STAT1_CALVCOSTRT_FINISHED << stv6120_regs.STV6120_STAT1_CALVCOSTRT_SHIFT))
                );

                if ((err == 0) && (timeout == STV6120_CAL_TIMEOUT))
                {
                    Console.WriteLine("ERROR: tuner wait on CAL timed out\n");
                    err = errors.ERROR_TUNER_CAL_TIMEOUT;
                }
            }

            /* wait for LOCK bit to go high to say PLL is locked */

            if (err == 0)
            {
                timeout = 0;
                do
                {
                    val = 0;
                    err = stv6120_read_reg(nimtuner == TUNER_1 ? stv6120_regs.STV6120_STAT1 : stv6120_regs.STV6120_STAT2, ref val);
                    timeout++;
                }
                while
                (
                    (err == 0) &&
                    (timeout < STV6120_CAL_TIMEOUT) &&
                    ((val & (1 << stv6120_regs.STV6120_STAT1_LOCK_SHIFT)) != (stv6120_regs.STV6120_STAT1_LOCK_LOCKED << stv6120_regs.STV6120_STAT1_LOCK_SHIFT))
                );

                if ((err == 0) && (timeout == STV6120_CAL_TIMEOUT))
                {
                    Console.WriteLine("ERROR: tuner wait on lock timed out\n");
                    err = errors.ERROR_TUNER_LOCK_TIMEOUT;
                }
            }

            if (err != 0)
            {
                Console.WriteLine("ERROR: Tuner set freq %i\n", freq);
            }

            return (err);
        }

        

    }
}
