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

        const byte TUNER_1 = 0;
        const byte TUNER_2 = 1;

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


        byte stv6120_cal_lowpass(byte tuner)
        {
            byte err = 0;
            byte val = 0;
            ushort timeout = 0;

            Console.WriteLine("Flow: Tuner cal lowpass : {0}", tuner );

            /* turn on the clock for the low pass filter. This is in ctrl7/16 so we have a shadow for it */
            if (tuner == TUNER_1) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL7, (byte)(ctrl7 & ~(1 << stv6120_regs.STV6120_CTRL7_RCCLKOFF_SHIFT)));
            else err = stv6120_write_reg(stv6120_regs.STV6120_CTRL16, (byte)(ctrl16 & ~(1 << stv6120_regs.STV6120_CTRL7_RCCLKOFF_SHIFT)));
            /* now we can do a low pass filter calibration, by setting the CALRCSTRT bit. NOte it is safe to just write to it */
            if (err == 0) err = stv6120_write_reg(tuner == TUNER_1 ? stv6120_regs.STV6120_STAT1 : stv6120_regs.STV6120_STAT2,
                                                   (byte)((stv6120_regs.STV6120_STAT1_CALRCSTRT_START << stv6120_regs.STV6120_STAT1_CALRCSTRT_SHIFT)));
            /* wait for the bit to be cleared  to say cal has finished*/
            if (err == 0)
            {
                timeout = 0;
                do
                {
                    err = stv6120_read_reg(stv6120_regs.STV6120_STAT1, ref val);
                    timeout++;
                    if (timeout == STV6120_CAL_TIMEOUT)
                    {
                        err = errors.ERROR_TUNER_CAL_LOWPASS_TIMEOUT;                        
                    }
                } while ((err == 0) && ((val & (1 << stv6120_regs.STV6120_STAT1_CALRCSTRT_SHIFT)) == (1 << stv6120_regs.STV6120_STAT1_CALRCSTRT_SHIFT)));
            }
            /* turn off the low pass filter clock (=1) */
            if (err == 0) err = stv6120_write_reg(tuner == TUNER_1 ? stv6120_regs.STV6120_CTRL7 : stv6120_regs.STV6120_CTRL16,
                                                        (tuner == TUNER_1 ? ctrl7 : ctrl16));

            return err;
        }

        byte stv6120_set_freq(byte tuner, UInt32 freq)
        {
            byte err = 0;

            byte val = 0;
            byte pos;
            UInt32 f;
            ushort n;
            byte p;
            byte icp;
            UInt32 f_vco;
            ushort timeout;
            byte cfhf;

            Console.WriteLine("Flow: Tuner {0} set freq {1}", tuner, freq);

            /* the global rdiv has already been set up in the init routines */

            /* p is defined from the datasheet (note, this is reg value, not P) */
            if (freq <= STV6120_P_THRESHOLD_1) p = 3; /* P=16 */
            else if (freq <= STV6120_P_THRESHOLD_2) p = 2; /* P= 8 */
            else if (freq <= STV6120_P_THRESHOLD_3) p = 1; /* P= 4 */
            else p = 0; /* P= 2 */

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
            cfhf--; /* we are sure it isn't greater then the first array element so this is safe */

            //printf("      Status: tuner:%i, f_vco=0x%x, icp=0x%x, f=0x%x, n=0x%x,\n", tuner, f_vco, icp, f, n);
            //printf("              rdiv=0x%x, p=0x%x, freq=%i, cfhf=%i\n", rdiv, p, freq, stv6120_cfhf[cfhf]);

            Console.WriteLine("Status: tuner:{0}, f_vco={1}, icp={2}, f={3}, n={4}", tuner.ToString("X2"), f_vco.ToString("X2"), icp.ToString("X2"), f.ToString("X2"), n.ToString("X2"));
            Console.WriteLine("        rdiv={0}, p={1}, freq={2}, cfgf={3}", rdiv.ToString("X2"), p.ToString("X2"), freq, stv6120_cfhf[cfhf]);
            /* now we fill in the PLL and ICP values */
            if (err == 0) err = stv6120_write_reg(tuner == TUNER_1 ? stv6120_regs.STV6120_CTRL3 : stv6120_regs.STV6120_CTRL12,
                                                      (byte)(n & 0x00ff));      /* set N[7:0] */
            if (err == 0) err = stv6120_write_reg(tuner == TUNER_1 ? stv6120_regs.STV6120_CTRL4 : stv6120_regs.STV6120_CTRL13,
                                                      (byte)(((f & 0x0000007f) << 1) |       /* set F[6:0] */
                                                      ((n & 0x0100) >> 8)));      /* N[8] */
            if (err == 0) err = stv6120_write_reg(tuner == TUNER_1 ? stv6120_regs.STV6120_CTRL5 : stv6120_regs.STV6120_CTRL14,
                                                      (byte)(((f & 0x00007f80) >> 7)));      /* set F[14:7] */
            if (err == 0) err = stv6120_write_reg(tuner == TUNER_1 ? stv6120_regs.STV6120_CTRL6 : stv6120_regs.STV6120_CTRL15,
                                                      (byte)(((f & 0x00038000) >> 15) |       /* set f[17:15] */
                                                      (icp << stv6120_regs.STV6120_CTRL6_ICP_SHIFT) | /* ICP[2:0] */
                                                      stv6120_regs.STV6120_CTRL6_RESERVED));      /* reserved bit */

            if (tuner == TUNER_1)
            {
                if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL7,
                                                           (byte)((p << stv6120_regs.STV6120_CTRL7_PDIV_SHIFT) |
                                                           ctrl7)); /* put back in RCCLKOFF_1 as well */

                if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL8,
                                                           (byte)((cfhf << stv6120_regs.STV6120_CTRL8_CFHF_SHIFT) |
                                                           ctrl8));
            }
            else
            { /* tuner=TUNER_2 */
                if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL16,
                                                           (byte)((p << stv6120_regs.STV6120_CTRL7_PDIV_SHIFT) |
                                                           ctrl16)); /* put back in RCCLKOFF_2 as well */
                if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL17,
                                                           (byte)((cfhf << stv6120_regs.STV6120_CTRL8_CFHF_SHIFT) |
                                                           ctrl17));
            }

            /* if we change the filter re-cal it, and if we change VCO we have to re-cal it, so here goes */
            if (err == 0) err = stv6120_write_reg(tuner == TUNER_1 ? stv6120_regs.STV6120_STAT1 : stv6120_regs.STV6120_STAT2,
                                                      (byte)((stv6120_regs.STV6120_STAT1_CALVCOSTRT_START << stv6120_regs.STV6120_STAT1_CALVCOSTRT_SHIFT) | /* start CALVCOSTRT */
                                                      stv6120_regs.STV6120_STAT1_RESERVED));

            /* wait for CALVCOSTRT bit to go low to say VCO cal is finished */
            if (err == 0)
            {
                timeout = 0;
                do
                {
                    err = stv6120_read_reg(tuner == TUNER_1 ? stv6120_regs.STV6120_STAT1 : stv6120_regs.STV6120_STAT2, ref val);
                    timeout++;
                } while ((err == 0) &&
                         (timeout < STV6120_CAL_TIMEOUT) &&
                         ((val & (1 << stv6120_regs.STV6120_STAT1_CALVCOSTRT_SHIFT)) != (stv6120_regs.STV6120_STAT1_CALVCOSTRT_FINISHED << stv6120_regs.STV6120_STAT1_CALVCOSTRT_SHIFT)));
                if ((err == 0) && (timeout == STV6120_CAL_TIMEOUT))
                {
                    //printf("ERROR: tuner wait on CAL timed out\n");
                    err = errors.ERROR_TUNER_CAL_TIMEOUT;
                }
            }

            /* wait for LOCK bit to go high to say PLL is locked */
            if (err == 0)
            {
                timeout = 0;
                do
                {
                    err = stv6120_read_reg(tuner == TUNER_1 ? stv6120_regs.STV6120_STAT1 : stv6120_regs.STV6120_STAT2, ref val);
                    timeout++;
                } while ((err == 0) &&
                         (timeout < STV6120_CAL_TIMEOUT) &&
                         ((val & (1 << stv6120_regs.STV6120_STAT1_LOCK_SHIFT)) != (stv6120_regs.STV6120_STAT1_LOCK_LOCKED << stv6120_regs.STV6120_STAT1_LOCK_SHIFT)));
                if ((err == 0) && (timeout == STV6120_CAL_TIMEOUT))
                {
                    //printf("ERROR: tuner wait on lock timed out\n");
                    err = errors.ERROR_TUNER_LOCK_TIMEOUT;
                }
            }

            //if (err != ERROR_NONE) printf("ERROR: Tuner set freq %i\n", freq);

            return err;
        }


        public byte stv6120_init(UInt32 freq_tuner_1, UInt32 freq_tuner_2, bool swap)
        {
            byte err = 0;
            byte k = 0;

            Console.WriteLine("Flow: Tuner init");

            /* note, we always init the tuner from scratch so no need to check if we have already inited it before */
            /* also, the tuner doesn't have much of an ID so no point in checking it */

            /* we calculate K from F_xtal/(K+16)=1MHz as specified in the datasheet */
            k = nim.NIM_TUNER_XTAL / 1000 - 16;

            /* setup the clocks for both tuners (note rdiv is a global) */
            if (nim.NIM_TUNER_XTAL >= STV6120_RDIV_THRESHOLD)
            {
                rdiv = 1;
            }
            else
            {
                rdiv = 0;
            }

            if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL1,
                                   (byte)((k << stv6120_regs.STV6120_CTRL1_K_SHIFT) |
                                   (rdiv << stv6120_regs.STV6120_CTRL1_RDIV_SHIFT) |
                                   (stv6120_regs.STV6120_CTRL1_OSHAPE_SINE << stv6120_regs.STV6120_CTRL1_OSHAPE_SHIFT) |
                                   (stv6120_regs.STV6120_CTRL1_MCLKDIV_4 << stv6120_regs.STV6120_CTRL1_MCLKDIV_SHIFT)));

            /* Configure path 1 */
            if (freq_tuner_1 > 0)
            { /* we are go on tuner 1 so turn it on */
                if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL2,
                                       (byte)((stv6120_regs.STV6120_CTRL2_DCLOOPOFF_ENABLE << stv6120_regs.STV6120_CTRL2_DCLOOPOFF_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_SDOFF_OFF << stv6120_regs.STV6120_CTRL2_SDOFF_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_SYN_ON << stv6120_regs.STV6120_CTRL2_SYN_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_REFOUTSEL_1_25V << stv6120_regs.STV6120_CTRL2_REFOUTSEL_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_BBGAIN_0DB << stv6120_regs.STV6120_CTRL2_BBGAIN_SHIFT)));

                /* CTRL3,4,5,6 are all tuner 1 PLL regs we will set them later */

                /* turn off rcclk for now */
                if (err == 0)
                {
                    ctrl7 = (byte)((stv6120_regs.STV6120_CTRL7_RCCLKOFF_DISABLE << stv6120_regs.STV6120_CTRL7_RCCLKOFF_SHIFT) |
                            (stv6120_regs.STV6120_CTRL7_CF_5MHZ << stv6120_regs.STV6120_CTRL7_CF_SHIFT));
                    err = stv6120_write_reg(stv6120_regs.STV6120_CTRL7, ctrl7);
                }

            }
            else
            { /* we are not going to use path 1 so shut it down */

                if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL2,
                                       (byte)((stv6120_regs.STV6120_CTRL2_DCLOOPOFF_DISABLE << stv6120_regs.STV6120_CTRL2_DCLOOPOFF_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_SDOFF_ON << stv6120_regs.STV6120_CTRL2_SDOFF_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_SYN_OFF << stv6120_regs.STV6120_CTRL2_SYN_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_REFOUTSEL_1_25V << stv6120_regs.STV6120_CTRL2_REFOUTSEL_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_BBGAIN_0DB << stv6120_regs.STV6120_CTRL2_BBGAIN_SHIFT)));

                /* CTRL3,4,5,6 are all tuner 1 PLL regs we will set them later */

                if (err == 0)
                {
                    ctrl7 = (byte)((stv6120_regs.STV6120_CTRL7_RCCLKOFF_DISABLE << stv6120_regs.STV6120_CTRL7_RCCLKOFF_SHIFT) |
                            (stv6120_regs.STV6120_CTRL7_CF_5MHZ << stv6120_regs.STV6120_CTRL7_CF_SHIFT));
                    err = stv6120_write_reg(stv6120_regs.STV6120_CTRL7, ctrl7);
                }
            }

            /* we need to set tcal for both tuners, but we need to remember the state in case we are using tuner 1 later */
            if (err == 0)
            {
                ctrl8 = (byte)((stv6120_regs.STV6120_CTRL8_TCAL_DIV_2 << stv6120_regs.STV6120_CTRL8_TCAL_SHIFT) |
                        (stv6120_regs.STV6120_CTRL8_CALTIME_500US << stv6120_regs.STV6120_CTRL8_TCAL_SHIFT));
                err = stv6120_write_reg(stv6120_regs.STV6120_CTRL8, ctrl8);
            }

            /* no need to touch the STAT1 status register for now */

            /* setup the RF path registers. RFA and RFD are not used. RFB is fed from the TOP NIM input, RFC from the BOTTOM */
            /* if we are swapping these inputs over we need to enable the correct LNAs */
            if (swap)
            {
                if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL9,
                               (byte)((stv6120_regs.STV6120_CTRL9_RFSEL_RFC_IN << stv6120_regs.STV6120_CTRL9_RFSEL_1_SHIFT) |
                               (stv6120_regs.STV6120_CTRL9_RFSEL_RFB_IN << stv6120_regs.STV6120_CTRL9_RFSEL_2_SHIFT) |
                               stv6120_regs.STV6120_CTRL9_RESERVED));
                /* decide on which LNAs are we going to enable */
                if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL10,
                               (byte)(((stv6120_regs.STV6120_CTRL10_LNA_OFF) << stv6120_regs.STV6120_CTRL10_LNADON_SHIFT) |
                               ((freq_tuner_2 > 0 ? stv6120_regs.STV6120_CTRL10_LNA_ON : stv6120_regs.STV6120_CTRL10_LNA_OFF) << stv6120_regs.STV6120_CTRL10_LNABON_SHIFT) |
                               ((freq_tuner_1 > 0 ? stv6120_regs.STV6120_CTRL10_LNA_ON : stv6120_regs.STV6120_CTRL10_LNA_OFF) << stv6120_regs.STV6120_CTRL10_LNACON_SHIFT) |
                               ((stv6120_regs.STV6120_CTRL10_LNA_OFF) << stv6120_regs.STV6120_CTRL10_LNAAON_SHIFT) |
                               ((freq_tuner_2 > 0 ? stv6120_regs.STV6120_CTRL10_PATH_ON : stv6120_regs.STV6120_CTRL10_PATH_OFF) << stv6120_regs.STV6120_CTRL10_PATHON_2_SHIFT) |
                               ((freq_tuner_1 > 0 ? stv6120_regs.STV6120_CTRL10_PATH_ON : stv6120_regs.STV6120_CTRL10_PATH_OFF) << stv6120_regs.STV6120_CTRL10_PATHON_1_SHIFT)));

            }
            else
            {
                if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL9,
                               (byte)((stv6120_regs.STV6120_CTRL9_RFSEL_RFB_IN << stv6120_regs.STV6120_CTRL9_RFSEL_1_SHIFT) |
                               (stv6120_regs.STV6120_CTRL9_RFSEL_RFC_IN << stv6120_regs.STV6120_CTRL9_RFSEL_2_SHIFT) |
                               stv6120_regs.STV6120_CTRL9_RESERVED));
                /* decide on which LNAs are we going to enable */
                if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL10,
                               (byte)(((stv6120_regs.STV6120_CTRL10_LNA_OFF) << stv6120_regs.STV6120_CTRL10_LNADON_SHIFT) |
                               ((freq_tuner_2 > 0 ? stv6120_regs.STV6120_CTRL10_LNA_ON : stv6120_regs.STV6120_CTRL10_LNA_OFF) << stv6120_regs.STV6120_CTRL10_LNACON_SHIFT) |
                               ((freq_tuner_1 > 0 ? stv6120_regs.STV6120_CTRL10_LNA_ON : stv6120_regs.STV6120_CTRL10_LNA_OFF) << stv6120_regs.STV6120_CTRL10_LNABON_SHIFT) |
                               ((stv6120_regs.STV6120_CTRL10_LNA_OFF) << stv6120_regs.STV6120_CTRL10_LNAAON_SHIFT) |
                               ((freq_tuner_2 > 0 ? stv6120_regs.STV6120_CTRL10_PATH_ON : stv6120_regs.STV6120_CTRL10_PATH_OFF) << stv6120_regs.STV6120_CTRL10_PATHON_2_SHIFT) |
                               ((freq_tuner_1 > 0 ? stv6120_regs.STV6120_CTRL10_PATH_ON : stv6120_regs.STV6120_CTRL10_PATH_OFF) << stv6120_regs.STV6120_CTRL10_PATHON_1_SHIFT)));
            }

            /* Configure path 2 */
            if (freq_tuner_2 > 0)
            { /* we are go on tuner 2 so turn it on */
                if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL11,
                                       (byte)((stv6120_regs.STV6120_CTRL2_DCLOOPOFF_ENABLE << stv6120_regs.STV6120_CTRL2_DCLOOPOFF_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_SDOFF_OFF << stv6120_regs.STV6120_CTRL2_SDOFF_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_SYN_ON << stv6120_regs.STV6120_CTRL2_SYN_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_REFOUTSEL_1_25V << stv6120_regs.STV6120_CTRL2_REFOUTSEL_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_BBGAIN_6DB << stv6120_regs.STV6120_CTRL2_BBGAIN_SHIFT)));

                /* CTRL12, 13, 14, 15 are PLL for tuner 2 */

                if (err == 0)
                {
                    ctrl16 = (byte)((stv6120_regs.STV6120_CTRL7_RCCLKOFF_ENABLE << stv6120_regs.STV6120_CTRL7_RCCLKOFF_SHIFT) |
                             (stv6120_regs.STV6120_CTRL7_CF_5MHZ << stv6120_regs.STV6120_CTRL7_CF_SHIFT));
                    err = stv6120_write_reg(stv6120_regs.STV6120_CTRL16, ctrl16);
                }

            }
            else
            { /* we are not going to use path 1 so shut it down */

                if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL11,
                                       (byte)((stv6120_regs.STV6120_CTRL2_DCLOOPOFF_DISABLE << stv6120_regs.STV6120_CTRL2_DCLOOPOFF_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_SDOFF_ON << stv6120_regs.STV6120_CTRL2_SDOFF_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_SYN_OFF << stv6120_regs.STV6120_CTRL2_SYN_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_REFOUTSEL_1_25V << stv6120_regs.STV6120_CTRL2_REFOUTSEL_SHIFT) |
                                       (stv6120_regs.STV6120_CTRL2_BBGAIN_0DB << stv6120_regs.STV6120_CTRL2_BBGAIN_SHIFT)));

                /* CTRL12, 13, 14, 15 are PLL for tuner 2 */

                if (err == 0)
                {
                    ctrl16 = (byte)((stv6120_regs.STV6120_CTRL7_RCCLKOFF_DISABLE << stv6120_regs.STV6120_CTRL7_RCCLKOFF_SHIFT) |
                             (stv6120_regs.STV6120_CTRL7_CF_5MHZ << stv6120_regs.STV6120_CTRL7_CF_SHIFT));
                    err = stv6120_write_reg(stv6120_regs.STV6120_CTRL16, ctrl16);
                }
            }

            /* there is no tcal field in CTRL17 but we still need to remember the state in case we are using tuner 2 later */
            if (err == 0)
            {
                ctrl17 = (byte)(stv6120_regs.STV6120_CTRL8_CALTIME_500US << stv6120_regs.STV6120_CTRL8_TCAL_SHIFT);
                err = stv6120_write_reg(stv6120_regs.STV6120_CTRL17, ctrl8);
            }

            /* no need to touch the STAT2 status register for now */

            /* CTRL18, CTRL19 are test regs so just write in the default */
            if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL18, stv6120_regs.STV6120_CTRL18_DEFAULT);
            if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL19, stv6120_regs.STV6120_CTRL19_DEFAULT);

            if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL20,
                                   (byte)((stv6120_regs.STV6120_CTRL20_VCOAMP_NORMAL << stv6120_regs.STV6120_CTRL20_VCOAMP_SHIFT) |
                                   stv6120_regs.STV6120_CTRL20_RESERVED));

            /* CTRL21, CTRL22 are test regs so leave alone */
            if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL21, stv6120_regs.STV6120_CTRL21_DEFAULT);
            if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL22, stv6120_regs.STV6120_CTRL22_DEFAULT);

            if (err == 0) err = stv6120_write_reg(stv6120_regs.STV6120_CTRL23,
                                   (byte)((stv6120_regs.STV6120_CTRL20_VCOAMP_NORMAL << stv6120_regs.STV6120_CTRL20_VCOAMP_SHIFT) |
                                   stv6120_regs.STV6120_CTRL20_RESERVED));

            
            /* now we can calibrate the lowpass filters and setup the PLLs for each tuner required */
            
            if ((err == 0) && (freq_tuner_1 > 0))
            {
                err = stv6120_cal_lowpass(TUNER_1);
                if (err == 0) err = stv6120_set_freq(TUNER_1, freq_tuner_1);
            }
            if ((err == 0) && (freq_tuner_2 > 0))
            {
                err = stv6120_cal_lowpass(TUNER_2);
                if (err == 0) err = stv6120_set_freq(TUNER_2, freq_tuner_2);
            }
            

            return err;
        }
    }
}
