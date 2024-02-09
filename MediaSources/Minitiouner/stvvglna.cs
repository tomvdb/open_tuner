// ported from longmynd - https://github.com/myorangedragon/longmynd - Heather Lomond

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{
    class stvvglna
    {
        nim nim_device;

        const byte STVVGLNA_AGC_TIMEOUT = 20;
        public const byte STVVGLNA_OFF = 0;
        public const byte STVVGLNA_ON = 1;

        public stvvglna(nim _nim_device)
        {
            nim_device = _nim_device;
        }

        byte stvvglna_read_reg(byte lna_addr, byte reg, ref byte val)
        {
            return nim_device.nim_read_lna(lna_addr, reg, ref val);
        }

        byte stvvglna_write_reg(byte lna_addr, byte reg, byte val)
        {
            return nim_device.nim_write_lna(lna_addr, reg, val);
        }


        public byte stvvglna_read_agc(byte input, ref byte gain, ref byte vgo)
        {
            /* -------------------------------------------------------------------------------------------------- */
            /* once we are running, this routine will read the gain and AGC for the LNA specified                 */
            /*   input: NIM_INPUT_TOP | NIM_INPUT_BOTTOM: which LNA is being worked on                            */
            /*  return: error code                                                                                */
            /*   *gain: the gain read from the specified LNA                                                      */
            /*    *vgo: the vgo read from the specified LNA                                                       */
            /* -------------------------------------------------------------------------------------------------- */
            byte err = 0;
            byte lna_addr;
            ushort timeout = 0;
            byte status = 0;

            /* first we decide which LNA to use */
            if (input == nim.NIM_INPUT_TOP) lna_addr = nim.NIM_LNA_0_ADDR;
            else lna_addr = nim.NIM_LNA_1_ADDR;

            /* in fully auto, we can read the gain sn SWLNAGAIN and VGO[4:0]. First we get the LNA to measure the */
            /* variable part of the gain for us. Note, it is ok to write 0 to the other bitfields */
            if (err == 0) err = stvvglna_write_reg(lna_addr, stvvglna_regs.STVVGLNA_REG1,
                                        stvvglna_regs.STVVGLNA_REG1_GETAGC_START << stvvglna_regs.STVVGLNA_REG1_GETAGC_SHIFT);

            do
            {
                err = stvvglna_read_reg(lna_addr, stvvglna_regs.STVVGLNA_REG1, ref status);  /* read out the status */
                timeout++;
                if ((err == 0) && (timeout == STVVGLNA_AGC_TIMEOUT))
                {
                    err = Errors.ERROR_LNA_AGC_TIMEOUT;
                    Console.WriteLine("Error: read AGC timeout\n");
                }
            }
            while ((err == 0) && (((status >> stvvglna_regs.STVVGLNA_REG1_GETAGC_SHIFT) & 1) != stvvglna_regs.STVVGLNA_REG1_GETAGC_FORCED));
            stvvglna_read_reg(lna_addr, stvvglna_regs.STVVGLNA_REG0, ref status);  /* read out the RFAGC high and low bits */

            if (err == 0) err = stvvglna_read_reg(lna_addr, stvvglna_regs.STVVGLNA_REG3, ref gain);  /* read out the gain curves */
            gain = (byte)((gain & stvvglna_regs.STVVGLNA_REG3_SWLNAGAIN_MASK) >> stvvglna_regs.STVVGLNA_REG3_SWLNAGAIN_SHIFT);
            if (err == 0) err = stvvglna_read_reg(lna_addr, stvvglna_regs.STVVGLNA_REG1, ref vgo); /* read out the Vagc value */
            vgo = (byte)((vgo & stvvglna_regs.STVVGLNA_REG1_VGO_MASK) >> stvvglna_regs.STVVGLNA_REG1_VGO_SHIFT);

            if (err != 0) Console.WriteLine("ERROR: Failed LNA aquire AGC {0}\n", input);
            return err;

        }

        public byte stvvglna_init(byte input, byte state, ref bool lna_ok)
        {
            byte err = 0;

            byte val = 0;
            byte lna_addr;

            Console.WriteLine("Flow: LNA init {0}", input);

            /* first we decide which LNA to use */
            if (input == nim.NIM_INPUT_TOP) lna_addr = nim.NIM_LNA_0_ADDR;
            else lna_addr = nim.NIM_LNA_1_ADDR;

            /* now check to see if there is an LNA present */
            err = stvvglna_read_reg(lna_addr, stvvglna_regs.STVVGLNA_REG0, ref val);
            if (err != 0)
            {
                //printf("      Status: found an older NIM with no LNA\n");
                Console.WriteLine("      Status: found an older NIM with no LNA");

                lna_ok = false; /* tell caller that there is no LNA */
                err = 0; /* we do not throw an error, just exit init */
            }
            else
            {
                /* otherwise, lna is there so we go on to us it */
                Console.WriteLine("      Status: found new NIM with LNAs");
                lna_ok = true;

                /* now check it has a good ID */
                if ((val & stvvglna_regs.STVVGLNA_REG0_IDENT_MASK) != stvvglna_regs.STVVGLNA_REG0_IDENT_DEFAULT)
                {
                    Console.WriteLine("ERROR: failed to recognise LNA ID {0} {1}", input, val);
                    err = Errors.ERROR_LNA_ID;
                }

                if (state == STVVGLNA_ON)
                {
                    /* set up the defaults. We are going to use fully automatic mode */
                    if (err == 0) err = stvvglna_write_reg(lna_addr, stvvglna_regs.STVVGLNA_REG0,
                                               (stvvglna_regs.STVVGLNA_REG0_AGC_TUPD_FAST << stvvglna_regs.STVVGLNA_REG0_AGC_TUPD_SHIFT) |
                                               (stvvglna_regs.STVVGLNA_REG0_AGC_TLOCK_SLOW << stvvglna_regs.STVVGLNA_REG0_AGC_TLOCK_SHIFT));
                    if (err == 0) err = stvvglna_write_reg(lna_addr, stvvglna_regs.STVVGLNA_REG1,
                                               (stvvglna_regs.STVVGLNA_REG1_LNAGC_PWD_POWER_ON << stvvglna_regs.STVVGLNA_REG1_LNAGC_PWD_SHIFT) |
                                               (stvvglna_regs.STVVGLNA_REG1_GETOFF_ACQUISITION_MODE << stvvglna_regs.STVVGLNA_REG1_GETOFF_SHIFT) |
                                               (stvvglna_regs.STVVGLNA_REG1_GETAGC_FORCED << stvvglna_regs.STVVGLNA_REG1_GETAGC_SHIFT));
                    if (err == 0) err = stvvglna_write_reg(lna_addr, stvvglna_regs.STVVGLNA_REG2,
                                               (stvvglna_regs.STVVGLNA_REG2_PATH_ACTIVE << stvvglna_regs.STVVGLNA_REG2_PATH2OFF_SHIFT) |
                                               (stvvglna_regs.STVVGLNA_REG2_RFAGC_PREF_N20DBM << stvvglna_regs.STVVGLNA_REG2_RFAGC_PREF_SHIFT) |
                                               (stvvglna_regs.STVVGLNA_REG2_PATH_ACTIVE << stvvglna_regs.STVVGLNA_REG2_PATH1OFF_SHIFT) |
                                               (stvvglna_regs.STVVGLNA_REG2_RFAGC_MODE_AUTO_TRACK << stvvglna_regs.STVVGLNA_REG2_RFAGC_MODE_SHIFT));
                    /* note that in REG3 the gain curves (SWLNAGAIN) are read only in fully automatic mod, so no need to write to them */
                    if (err == 0) err = stvvglna_write_reg(lna_addr, stvvglna_regs.STVVGLNA_REG3,
                                               (stvvglna_regs.STVVGLNA_REG3_LCAL_17KHZ << stvvglna_regs.STVVGLNA_REG3_LCAL_SHIFT) |
                                               (stvvglna_regs.STVVGLNA_REG3_RFAGC_UPDATE_FORCED << stvvglna_regs.STVVGLNA_REG3_RFAGC_UPDATE_SHIFT) |
                                               (stvvglna_regs.STVVGLNA_REG3_RFAGC_CALSTART_FORCED << stvvglna_regs.STVVGLNA_REG3_RFAGC_CALSTART_SHIFT));
                }
                else
                { /* state==STVVGLAN_OFF so disable it as we are not turning it on */
                    if (err == 0) err = stvvglna_write_reg(lna_addr, stvvglna_regs.STVVGLNA_REG2,
                                               (stvvglna_regs.STVVGLNA_REG2_PATH_OFF << stvvglna_regs.STVVGLNA_REG2_PATH2OFF_SHIFT) |
                                               (stvvglna_regs.STVVGLNA_REG2_PATH_OFF << stvvglna_regs.STVVGLNA_REG2_PATH1OFF_SHIFT));
                }

            }

            return err;
        }
    }
}
