// ported from longmynd - https://github.com/myorangedragon/longmynd - Heather Lomond

using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{

    class stv0910
    {
        public const byte KHZ22ON = 0;
        public const byte KHZ22OFF = 1;
        public const byte DEMOD_HUNTING = 0;
        public const byte DEMOD_FOUND_HEADER = 1;
        public const byte DEMOD_S2 = 2;
        public const byte DEMOD_S = 3;

        const byte STV0910_PLL_LOCK_TIMEOUT = 100;

        const byte STV0910_SCAN_BLIND_BEST_GUESS = 0x15;
        const byte STV0910_DEMOD_STOP = 0x1C;

        public const byte STV0910_DEMOD_TOP = 1;
        public const byte STV0910_DEMOD_BOTTOM = 2;

        const byte STV0910_PUNCTURE_1_2 = 0x0d;
        const byte STV0910_PUNCTURE_2_3 = 0x12;
        const byte STV0910_PUNCTURE_3_4 = 0x15;
        const byte STV0910_PUNCTURE_5_6 = 0x18;
        const byte STV0910_PUNCTURE_6_7 = 0x19;
        const byte STV0910_PUNCTURE_7_8 = 0x1a;

        nim nim_device;

        byte[] stv0910_shadow_regs = new byte[stv0910_regs.STV0910_END_ADDR - stv0910_regs.STV0910_START_ADDR + 1];

        private bool _enableSerialTS = false;

        private byte stv0910_init_regs(bool EnableSerialTS)
        {
            _enableSerialTS = EnableSerialTS;

            byte val1 = 0;
            byte val2 = 0;
            byte err = 0;
            ushort i = 0;

            Log.Information("Flow: STV0910 init regs");
            
            err = nim_device.nim_read_demod(0xf100, ref val1);

            if (err == 0) err = nim_device.nim_read_demod(0xf101, ref val2);

            Log.Information("      Status: STV0910 MID = {0}, DID = {1}", val1.ToString("X2"), val2.ToString("X2"));

            if ( (val1 != 0x51) || (val2 != 0x20 ))
            {
                return Errors.ERROR_DEMOD_INIT;
            }

            // init all registers in the list
            do
            {
                if (err == 0) err = stv0910_write_reg(stv0910_regs_init.STV0910DefVal[i].reg, stv0910_regs_init.STV0910DefVal[i].val);
            } while (stv0910_regs_init.STV0910DefVal[i++].reg != stv0910_regs.RSTV0910_TSTTSRS);

            // serial ts for pico tuner
            if (_enableSerialTS)
            {
                if (err == 0) stv0910_write_reg(stv0910_regs.RSTV0910_P1_TSCFGH, 0x40);
                if (err == 0) stv0910_write_reg(stv0910_regs.RSTV0910_P2_TSCFGH, 0x40);
            }


            // reset lpdc decoder
            if (err == 0) err = stv0910_write_reg(stv0910_regs.RSTV0910_TSTRES0, 0x80);
            if (err == 0) err = stv0910_write_reg(stv0910_regs.RSTV0910_TSTRES0, 0x00);

            return err;
        }

        private byte stv0910_setup_equalisers( byte demod )
        {
            Log.Information("Flow: Setup equializers: {0}", demod);
            return 0;
        }

        private byte stv0910_setup_carrier_loop(byte demod, UInt32 halfscan_sr)
        {
            byte err = 0;
            Int64 temp;


            Log.Information("Flow: Setup carrier loop: {0}" , demod);

            err = stv0910_write_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_CFRINIT0 : stv0910_regs.RSTV0910_P1_CFRINIT0, 0);

            if (err == 0)
            {
                err = stv0910_write_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_CFRINIT0 : stv0910_regs.RSTV0910_P1_CFRINIT0, 0);
            }

            // 0.6 * SR seems to give +/- 0.5 SR lock
            temp = halfscan_sr * 65536 / 135000;

            // Upper Limit
            if (err == 0)
            {
                err = stv0910_write_reg((demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_CFRUP0 : stv0910_regs.RSTV0910_P1_CFRUP0), (byte)(temp & 0xff));
                err = stv0910_write_reg((demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_CFRUP1 : stv0910_regs.RSTV0910_P1_CFRUP1), (byte)((temp >> 8) & 0xff));
            }
            // the lower value is the negative of the upper value
            temp = -temp;

            if (err == 0)
            {
                err = stv0910_write_reg((demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_CFRLOW0 : stv0910_regs.RSTV0910_P1_CFRLOW0), (byte)(temp & 0xff));
                err = stv0910_write_reg((demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_CFRLOW1 : stv0910_regs.RSTV0910_P1_CFRLOW1), (byte)((temp >> 8) & 0xff));
            }

            return err;

        }

        public byte stv0910_read_ts_status(byte demod, ref UInt32 info)  
        {
            byte err;
            byte temp0 = 0;
            byte temp1 = 0;

            err = 0;

            err |= stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_TSSTATUS : stv0910_regs.RSTV0910_P1_TSSTATUS, ref temp0);
            err |= stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_TSSTATUS2 : stv0910_regs.RSTV0910_P1_TSSTATUS2, ref temp1);

            info = (UInt32)(temp0 | (temp1 << 8));

            if (err != 0) Log.Error("ERROR: STV0910 read multistream0\r\n");

            return (err);
        }

        byte stv0910_setup_timing_loop(byte demod, UInt32 sr)
        {
            byte err = 0;
            ushort sr_reg = 0;

            Log.Information("Flow: Setup timing loop {0}", demod);


            sr_reg = (ushort)((((UInt32)sr) << 16) / 135 / 1000);

            if (err == 0)
            {
                    err = stv0910_write_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_SFRINIT1 : stv0910_regs.RSTV0910_P1_SFRINIT1, (byte)(sr_reg >> 8));
            }

            if (err == 0)
            {
                    err = stv0910_write_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_SFRINIT0 : stv0910_regs.RSTV0910_P1_SFRINIT0, (byte)(sr_reg & 0xFF));
            }


            return err;
        }

        private byte stv0910_setup_clocks()
        {
            byte err = 0;

            UInt32 ndiv;
            byte odf;
            byte idf;
            UInt32 f_phi;
            UInt32 f_xtal;
            byte cp;
            byte _lock = 0;
            ushort timeout = 16;

            Log.Information("Flow: STV0910 set MCLK");

            odf = 4;
            idf = 1;

            if (err == 0) err = stv0910_write_reg_field(stv0910_regs.FSTV0910_ODF, odf);
            if (err == 0) err = stv0910_write_reg_field(stv0910_regs.FSTV0910_IDF, idf);

            f_xtal = nim.NIM_TUNER_XTAL / 1000; /* in MHz */

            f_phi = 135000000 / 1000000;
            ndiv = (f_phi * odf * idf) / f_xtal;

            if (err == 0) err = stv0910_write_reg_field(stv0910_regs.FSTV0910_N_DIV, (byte)ndiv);

            /* Set CP according to NDIV */
            cp = 7;
            if (err == 0) err = stv0910_write_reg_field(stv0910_regs.FSTV0910_CP, cp);

            /* turn on all the clocks */
            if (err == 0) err = stv0910_write_reg_field(stv0910_regs.FSTV0910_STANDBY, 0);

            /* derive clocks from PLL */
            if (err == 0) err = stv0910_write_reg_field(stv0910_regs.FSTV0910_BYPASSPLLCORE, 0);

            /* wait for PLL to lock */
            do
            {
                timeout++;
                if (timeout == STV0910_PLL_LOCK_TIMEOUT)
                {
                    err = Errors.ERROR_DEMOD_PLL_TIMEOUT;
                    //printf("ERROR: STV0910 pll lock timeout\n");
                }
                if (err == 0) stv0910_read_reg_field(stv0910_regs.FSTV0910_PLLLOCK, ref _lock) ;
            } while ((err == 0) && (_lock== 0));

            //if (err != ERROR_NONE) printf("ERROR: STV0910 set MCLK\n");

            return err;
        }

        public stv0910(nim _nim_device)
        {
            nim_device = _nim_device;
        }


        public byte stv0910_init(bool EnableSerialTS)
        {
            byte err = 0;

            Log.Information("Flow: STV0910 init");

            // stop demodulators
            if (err == 0) err = stv0910_write_reg(stv0910_regs.RSTV0910_P1_DMDISTATE, 0x1c);
            if (err == 0) err = stv0910_write_reg(stv0910_regs.RSTV0910_P2_DMDISTATE, 0x1c);

            // non demodulator specific
            if (err == 0) err = stv0910_init_regs(EnableSerialTS);
            if (err == 0) err = stv0910_setup_clocks();

            return err;
        }

        // setup receive of the demodulator
        public byte stv0910_setup_receive(byte demod, UInt32 sr)
        {
            byte err = 0;

            if (err == 0) err = stv0910_setup_equalisers(demod);
            if (err == 0) err = stv0910_setup_carrier_loop(demod, Convert.ToUInt32(sr * 1.5));
            if (err == 0) err = stv0910_setup_timing_loop(demod, sr);

            return err;
        }

        public byte stv0910_switch_22Khz_p1(bool switch_flag)
        {
            byte err = 0;

            if (switch_flag)
            {
                err = stv0910_write_reg(stv0910_regs.RSTV0910_P1_DISTXCFG, KHZ22ON);
            }
            else
            {
                err = stv0910_write_reg(stv0910_regs.RSTV0910_P1_DISTXCFG, KHZ22OFF);
            }

            if (err != 0)
            {
                Log.Error("Error switching 22khz - P1");
            }

            if (err == 0)
            {
                if (switch_flag)
                {
                    err = stv0910_write_reg(stv0910_regs.RSTV0910_P2_DISTXCFG, KHZ22ON);
                }
                else
                {
                    err = stv0910_write_reg(stv0910_regs.RSTV0910_P2_DISTXCFG, KHZ22OFF);
                }

                if (err != 0)
                {
                    Log.Error("Error switching 22khz - P2");
                }

            }

            return err;
        }

        private byte stv0910_write_reg_field(UInt32 field, byte field_val)
        {
            byte err = 0;
            ushort reg = 0;
            byte val;

            reg = (ushort)(field >> 16);

            val = (byte)((stv0910_shadow_regs[reg - stv0910_regs.STV0910_START_ADDR] & ~(byte)(field & 0xff)) | (field_val << (byte)(((field >> 12) & 0x0f))));

            if (err == 0) err = nim_device.nim_write_demod(reg, val);
            if (err == 0) stv0910_shadow_regs[reg - stv0910_regs.STV0910_START_ADDR] = val;

            return err;
        }


        private byte stv0910_read_reg_field( UInt32 field, ref byte field_val)
        {
            byte err = 0;
            byte val = 0;

            if (err == 0) err = nim_device.nim_read_demod((ushort)(field >> 16), ref val);

            //Log.Information("Read REG Field {0} {1}", field.ToString(), val.ToString());

            UInt32 t1 = ((val) & (field & 0xff));
            Int32 t2 = (((Int32)field >> 12) & 0x0f);

            UInt32 t3 = t1 >> t2;

            field_val = (byte)t3;

            //Log.Information("-- Read REG Field {0} {1}", field.ToString(), field_val.ToString());

            return err;
        }

        byte stv0910_read_reg(ushort reg, ref byte val)
        {
            return nim_device.nim_read_demod(reg, ref val);
        }


        byte stv0910_write_reg(ushort reg, byte val)
        {
            stv0910_shadow_regs[reg - stv0910_regs.STV0910_START_ADDR] = val;
            return nim_device.nim_write_demod(reg, val);
        }

        public byte stv0910_read_matype(byte demod, ref UInt32 matype1, ref UInt32 matype2)
        {
            byte err;
            byte regval = 0;

            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? Convert.ToUInt16(stv0910_regs.RSTV0910_P2_MATSTR0 - 1) : Convert.ToUInt16(stv0910_regs.RSTV0910_P1_MATSTR0 - 1), ref regval);
            matype1 = regval;

            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_MATSTR0 : stv0910_regs.RSTV0910_P1_MATSTR0, ref regval);
            matype2 = regval;

            if (err != 0) Log.Error("ERROR: STV0910 read MATYPE");

            return err;
        }

        public byte stv0910_read_mer(byte demod, ref Int32 mer)
        {
            byte err = 0;
            byte high = 0;
            byte low = 0;

            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_NOSRAMPOS : stv0910_regs.RSTV0910_P1_NOSRAMPOS, ref high);
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_NOSRAMVAL : stv0910_regs.RSTV0910_P1_NOSRAMVAL, ref low);

            if (((high >> 2) & 0x01) == 1)
            {
                /* Px_NOSRAM_CNRVAL is valid */
                if (((high >> 1) & 0x01) == 1)
                {
                    mer = (int)(((high & 0x01) << 8) | low) - 512;
                }
                else
                {
                    mer = (int)(((high & 0x01) << 8) | low); 
                }
            }
            else
            {
                mer = 0;
                if (err == 0) err = stv0910_write_reg_field(demod == STV0910_DEMOD_TOP ? stv0910_regs.FSTV0910_P2_NOSRAM_ACTIVATION : stv0910_regs.FSTV0910_P1_NOSRAM_ACTIVATION, 0x02);
            }

            if (err != 0) Log.Error("ERROR: STV0910 read DVBS2 MER\n");

            return err;

        }

        public byte stv0910_read_errors_ldpc_count(byte demod, ref UInt32 errors_ldpc_count)
        {
            /* -------------------------------------------------------------------------------------------------- */
            /*               demod: STV0910_DEMOD_TOP | STV0910_DEMOD_BOTTOM: which demodulator is being read      */
            /*   errors_ldpc_count: place to store the result                                                      */
            /*              return: error state                                                                    */
            /* -------------------------------------------------------------------------------------------------- */
            byte err;
            byte high = 0, low = 0;

            /* This parameter appears to be total, not for an individual demodulator */
            //(void)demod;

            err = stv0910_read_reg_field(stv0910_regs.FSTV0910_LDPC_ERRORS1, ref high);
            if (err == 0) err = stv0910_read_reg_field(stv0910_regs.FSTV0910_LDPC_ERRORS0, ref low);

            errors_ldpc_count = (UInt32)high << 8 | (UInt32)low;

            if (err != 0) Log.Error("ERROR: STV0910 read LDPC Errors Count\n");

            return err;
        }

        public byte  stv0910_read_modcod_and_type(byte demod, ref UInt32 modcod, ref bool short_frame, ref bool pilots, ref byte rolloff)
        {
            /* -------------------------------------------------------------------------------------------------- */
            /*   Note that MODCODs are different in DVBS and DVBS2. Also short_frame and pilots only valid for S2 */
            /*    demod: STV0910_DEMOD_TOP | STV0910_DEMOD_BOTTOM: which demodulator is being read                */
            /*   modcod: place to store the result                                                                */
            /*   return: error state                                                                              */
            /* -------------------------------------------------------------------------------------------------- */
            byte err;
            byte regval = 0;

            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_DMDMODCOD : stv0910_regs.RSTV0910_P1_DMDMODCOD, ref regval);

            modcod = (UInt32)((regval & 0x7c) >> 2);
            if (((regval & 0x02) >> 1) == 1)
                short_frame = true;
            else
                short_frame = false;

            if ((regval & 0x01) == 1)
                pilots = true;
            else
                pilots = false;

            if (err != 0) Log.Error("ERROR: STV0910 read MODCOD\n");

            err = stv0910_read_reg_field(demod == STV0910_DEMOD_TOP ? stv0910_regs.FSTV0910_P2_ROLLOFF_STATUS : stv0910_regs.FSTV0910_P1_ROLLOFF_STATUS, ref regval);

            rolloff = regval;

            return err;
        }

        public byte stv0910_read_errors_bch_count(byte demod, ref UInt32 errors_bch_count)
        {
            byte err = 0;
            byte result = 0;

            /* This parameter appears to be total, not for an individual demodulator */
            //(void)demod;

            err = stv0910_read_reg_field(stv0910_regs.FSTV0910_BCH_ERRORS_COUNTER, ref result);

            errors_bch_count = (UInt32)result;

            if (err != 0) Log.Error("ERROR: STV0910 read BCH Errors Count\n");
            return err;
        }

        public byte stv0910_read_errors_bch_uncorrected(byte demod, ref bool errors_bch_uncorrected)
        {
            byte err = 0;
            byte result = 0;

            /* This parameter appears to be total, not for an individual demodulator */
            //(void)demod;

            err = stv0910_read_reg_field(stv0910_regs.FSTV0910_ERRORFLAG, ref result);

            if (result == 0)
            {
                errors_bch_uncorrected = true;
            }
            else
            {
                errors_bch_uncorrected = false;
            }

            if (err != 0) Log.Error("ERROR: STV0910 read BCH Errors Uncorrected\n");

            return err;
        }

        public byte stv0910_read_ber(byte demod, ref UInt32 ber)
        {
            byte err = 0;

            byte high = 0, mid_u = 0, mid_m = 0, mid_l = 0, low = 0;
            double cpt = 0;
            double errs = 0;

            /* first we trigger a buffer transfer and read the byte counter 40 bits */
            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_FBERCPT4 : stv0910_regs.RSTV0910_P1_FBERCPT4, ref high);
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_FBERCPT3 : stv0910_regs.RSTV0910_P1_FBERCPT3, ref mid_u);
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_FBERCPT2 : stv0910_regs.RSTV0910_P1_FBERCPT2, ref mid_m);
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_FBERCPT1 : stv0910_regs.RSTV0910_P1_FBERCPT1, ref mid_l);
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_FBERCPT0 : stv0910_regs.RSTV0910_P1_FBERCPT0, ref low);
            cpt = (double)high * 256.0 * 256.0 * 256.0 * 256.0 + (double)mid_u * 256.0 * 256.0 * 256.0 + (double)mid_m * 256.0 * 256.0 +
                (double)mid_l * 256.0 + (double)low;

            /* we have already triggered the register buffer transfer, so now we we read the bit error from them */
            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_FBERERR2 : stv0910_regs.RSTV0910_P1_FBERERR2, ref high);
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_FBERERR1 : stv0910_regs.RSTV0910_P1_FBERERR1, ref mid_m);
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_FBERERR0 : stv0910_regs.RSTV0910_P1_FBERERR0, ref low);
            errs = (double)high * 256.0 * 256.0 + (double)mid_m * 256.0 + (double)low;

            ber = (UInt32)(10000.0 * errs / (cpt * 8.0));

            if (err != 0) Log.Error("ERROR: STV0910 read BER\n");

            return err;
        }

        public byte stv0910_read_err_rate(byte demod, ref UInt32 vit_errs)
        {
            byte err = 0;
            byte val = 0;

            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_VERROR : stv0910_regs.RSTV0910_P1_VERROR, ref val);
            /* 0=perfect, 0xff=6.23 %errors (errs/4096) */
            /* note there is a problem in the datasheet here as it says 255/2048=6.23% */
            /* to report an integer we will report in 100 * the percentage, so 623=6.23% */
            /* also want to round up to the nearest integer just to be pedantic */
            vit_errs = ((((UInt32)val) * 100000 / 4096) + 5) / 10;

            if (err != 0) Log.Error("ERROR: STV0910 read viterbi error rate\n");

            return err;
        }

        public byte stv0910_read_sr(byte demod, ref UInt32 found_sr)
        {
            byte err = 0;

            double sr;
            byte val_h = 0, val_mu = 0, val_ml = 0, val_l = 0;

            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_SFR3 : stv0910_regs.RSTV0910_P1_SFR3, ref val_h);  /* high byte */
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_SFR2 : stv0910_regs.RSTV0910_P1_SFR2, ref val_mu); /* mid upper */
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_SFR1 : stv0910_regs.RSTV0910_P1_SFR1, ref val_ml); /* mid lower */
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_SFR0 : stv0910_regs.RSTV0910_P1_SFR0, ref val_l);  /* low byte */

            sr = ((UInt32)val_h << 24) +
               ((UInt32)val_mu << 16) +
               ((UInt32)val_ml << 8) +
               ((UInt32)val_l);

            /* sr (MHz) = ckadc (MHz) * SFR/2^32. So in Symbols per Second we need */
            sr = 135000000 * sr / 256.0 / 256.0 / 256.0 / 256.0;
            found_sr = (UInt32)sr;

            // read the symbol rate detection offset

            int temp = 0;
            byte tempc = 0;
            double tempf = 0;
            if (err == 0)
            {
                err |= stv0910_read_reg
                (
                    demod == STV0910_DEMOD_TOP
                    ? stv0910_regs.RSTV0910_P2_TMGREG2 : stv0910_regs.RSTV0910_P1_TMGREG2,            /* byte2 */
                    ref tempc
                );
                temp |= tempc << 24;

                err |= stv0910_read_reg
                (
                    demod == STV0910_DEMOD_TOP
                    ? stv0910_regs.RSTV0910_P2_TMGREG1 : stv0910_regs.RSTV0910_P1_TMGREG1,            /* byte1 */
                    ref tempc
                );
                temp |= tempc << 16;

                err |= stv0910_read_reg
                (
                    demod == STV0910_DEMOD_TOP
                    ? stv0910_regs.RSTV0910_P2_TMGREG0 : stv0910_regs.RSTV0910_P1_TMGREG0,            /* byte0 */
                    ref tempc
                );
                temp |= tempc << 8;

                temp = temp / 256;                                          // move to the bottom 24 bits 
                                                                            // and extend the sign
            }

            tempf = temp;                                                   // convert to double
            tempf = tempf * 1000 / (1 << 29);                               // calculate offset in symbols
            tempf = tempf * sr / 1000;                                      // multiply by nominal symbol rate
            found_sr = (uint)(sr + tempf);							// update the value

            if (err != 0) Log.Error("ERROR: STV0910 read symbol rate\n");

            return err;
        }

        public byte stv0910_read_car_freq(byte demod, ref Int32 cf)
        {
            /* -------------------------------------------------------------------------------------------------- */
            /* reads the current carrier frequency and return it (in Hz)                                          */
            /*    demod: STV0910_DEMOD_TOP | STV0910_DEMOD_BOTTOM: which demodulator is being read                */
            /* car_freq: signed place to store the answer                                                         */
            /*   return: error state                                                                              */
            /* -------------------------------------------------------------------------------------------------- */
            byte err;
            byte val_h = 0, val_m = 0, val_l = 0;
            double car_offset_freq;

            /* first off we read in the carrier offset as a signed number */
            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ?
                             stv0910_regs.RSTV0910_P2_CFR2 : stv0910_regs.RSTV0910_P1_CFR2, ref val_h); /* high byte*/
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ?
                                                      stv0910_regs.RSTV0910_P2_CFR1 : stv0910_regs.RSTV0910_P1_CFR1, ref val_m); /* mid */
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ?
                                                      stv0910_regs.RSTV0910_P2_CFR0 : stv0910_regs.RSTV0910_P1_CFR0, ref val_l); /* low */
            /* since this is a 24 bit signed value, we need to build it as a 24 bit value, shift it up to the top
               to get a 32 bit signed value, then convert it to a double */
            car_offset_freq = (double)(Int32)((((UInt32)val_h << 16) + ((UInt32)val_m << 8) + ((UInt32)val_l)) << 8);
            /* carrier offset freq (MHz)= mclk (MHz) * CFR/2^24. But we have the extra 256 in there from the sign shift */
            /* so in Hz we need: */
            car_offset_freq = 135000000 * car_offset_freq / 256.0 / 256.0 / 256.0 / 256.0;

            cf = (Int32)car_offset_freq;

            if (err != 0) Log.Error("ERROR: STV0910 read carrier frequency\n");

            return err;
        }

        public byte stv0910_read_puncture_rate(byte demod, ref byte rate)
        {

            /* -------------------------------------------------------------------------------------------------- */
            /* reads teh detected viterbi punctuation rate                                                        */
            /*   demod: STV0910_DEMOD_TOP | STV0910_DEMOD_BOTTOM: which demodulator is being read                 */
            /*   rate: place to store the result                                                                   */
            /*         The single byta, n, represents a rate=n/n+1                                                 */
            /* return: error code                                                                                 */
            /* -------------------------------------------------------------------------------------------------- */
            byte err;
            byte val = 0;

            err = stv0910_read_reg_field(demod == STV0910_DEMOD_TOP ? stv0910_regs.FSTV0910_P2_VIT_CURPUN : stv0910_regs.FSTV0910_P1_VIT_CURPUN, ref val);
            switch (val)
            {
                case STV0910_PUNCTURE_1_2: rate = 1; break;
                case STV0910_PUNCTURE_2_3: rate = 2; break;
                case STV0910_PUNCTURE_3_4: rate = 3; break;
                case STV0910_PUNCTURE_5_6: rate = 5; break;
                case STV0910_PUNCTURE_6_7: rate = 6; break;
                case STV0910_PUNCTURE_7_8: rate = 7; break;
                default: err = Errors.ERROR_VITERBI_PUNCTURE_RATE; break;
            }

            if (err != 0) Log.Error("ERROR: STV0910 read puncture rate");

            return err;

        }

        public byte stv0910_read_constellation(byte demod, ref byte i, ref byte q)
        {
            byte err = 0;

            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_ISYMB : stv0910_regs.RSTV0910_P1_ISYMB, ref i);
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_QSYMB : stv0910_regs.RSTV0910_P1_QSYMB, ref q);

            if (err != 0) Log.Error("ERROR: STV0910 read constellation");

            return err;
        }

        /* -------------------------------------------------------------------------------------------------- */
        /* reads the AGC1 Gain registers in the Demodulator and returns the results                           */
        /* demod: STV0910_DEMOD_TOP | STV0910_DEMOD_BOTTOM: which demodulator is being read                  */
        /* agc: place to store the results                                                                    */
        /* return: error state                                                                                */
        /* -------------------------------------------------------------------------------------------------- */

        public byte stv0910_read_agc1_gain(byte demod, ref ushort agc)
        {

            byte err = 0;
            byte agc_low = 0, agc_high = 0;

            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_AGCIQIN0 : stv0910_regs.RSTV0910_P1_AGCIQIN0, ref agc_low);
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_AGCIQIN1 : stv0910_regs.RSTV0910_P1_AGCIQIN1, ref agc_high);
            if (err == 0) agc = (ushort)((ushort)agc_high << 8 | (ushort)agc_low);

            if (err != 0) Log.Error("ERROR: STV0910 read agc1 gain\n");

            return err;
        }

        /* -------------------------------------------------------------------------------------------------- */
        public byte stv0910_read_agc2_gain(byte demod, ref ushort agc)
        {
            /* -------------------------------------------------------------------------------------------------- */
            /* reads the AGC2 Gain registers in the Demodulator and returns the results                           */
            /*  demod: STV0910_DEMOD_TOP | STV0910_DEMOD_BOTTOM: which demodulator is being read                  */
            /* agc: place to store the results                                                                    */
            /* return: error state                                                                                */
            /* -------------------------------------------------------------------------------------------------- */
            byte err;
            byte agc_low = 0, agc_high = 0;

            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_AGC2I0 : stv0910_regs.RSTV0910_P1_AGC2I0, ref agc_low);
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_AGC2I1 : stv0910_regs.RSTV0910_P1_AGC2I1, ref agc_high);
            if (err == 0) agc = (ushort)((ushort)agc_high << 8 | (ushort)agc_low);

            if (err != 0) Log.Error("ERROR: STV0910 read agc2 gain\n");

            return err;
        }

        public byte stv0910_read_power(byte demod, ref byte power_i, ref byte power_q)
        {
            byte err = 0;

            err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_POWERI : stv0910_regs.RSTV0910_P1_POWERI, ref power_i);
            if (err == 0) err = stv0910_read_reg(demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_POWERQ : stv0910_regs.RSTV0910_P1_POWERQ, ref power_q);

            if (err != 0) Log.Error("ERROR: STV0910 read power");

            return err;
        }

        public byte stv0910_start_scan(byte demod)
        {
            byte err = 0;

            Log.Information("Flow: STV0910 start scan");

            if (err == 0) err = stv0910_write_reg((demod == STV0910_DEMOD_TOP ? stv0910_regs.RSTV0910_P2_DMDISTATE : stv0910_regs.RSTV0910_P1_DMDISTATE),
                                                                                             STV0910_SCAN_BLIND_BEST_GUESS);

            if (err != 0) Log.Error("ERROR: STV0910 start scan");

            return err;
        }

        public byte stv0910_read_scan_state(byte demod, ref byte state)
        {
            byte err = 0;

            if (err == 0) err = stv0910_read_reg_field((demod == STV0910_DEMOD_TOP ?
                                            stv0910_regs.FSTV0910_P2_HEADER_MODE : stv0910_regs.FSTV0910_P1_HEADER_MODE), ref state);

            return err;
        }

    }
}
