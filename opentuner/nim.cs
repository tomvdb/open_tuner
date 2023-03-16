// mostly ported from longmynd - https://github.com/myorangedragon/longmynd - Heather Lomond

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace opentuner
{


    class nim
    {

        bool repeater_on = false;

        private ftdi ftdi_device;

        public const byte NIM_DEMOD_ADDR = 0xd2;
        public const byte NIM_TUNER_ADDR = 0xc0;

        public const byte NIM_LNA_0_ADDR = stvvglna_regs.STVVGLNA_I2C_ADDR3;
        public const byte NIM_LNA_1_ADDR = stvvglna_regs.STVVGLNA_I2C_ADDR0;

        public const ushort NIM_TUNER_XTAL = 30000; /* in KHz */
        public const UInt32 NIM_DEMOD_MCLK = 135000000; /* in Hz */

        public const byte NIM_INPUT_TOP = 1;
        public const byte NIM_INPUT_BOTTOM = 2;

        public nim(ftdi ftdidevice)
        {
            ftdi_device = ftdidevice;
        }

        public byte nim_read_lna(byte lna_addr, byte reg, ref byte val)
        {
            byte err = 0;

            if (!repeater_on)
            {
                err = nim_write_demod(0xf12a, 0xb8);
                repeater_on = true;
            }
            if (err == 0) err = ftdi_device.ftdi_i2c_read_reg8(lna_addr, reg, ref val);

            //Console.WriteLine("nim lna read: {0}, {1}", reg.ToString("X"), val.ToString("X"));

            return err;
        }


        public byte nim_write_lna(byte lna_addr, byte reg, byte val)
        {
            byte err = 0;

            if (!repeater_on)
            {
                err = nim_write_demod(0xf12a, 0xb8);
                repeater_on = true;
            }
            if (err == 0) err = ftdi_device.ftdi_i2c_write_reg8(lna_addr, reg, val);

            //Console.WriteLine("nim lna write: {0}, {1}", reg.ToString("X"), val.ToString("X"));


            return err;
        }

        public byte nim_write_tuner(byte reg, byte val)
        {
            byte err = 0;
            if (!repeater_on)
            {
                err = nim_write_demod(0xf12a, 0xb8);
                repeater_on = true;
            }

            if (err == 0) err = ftdi_device.ftdi_i2c_write_reg8(NIM_TUNER_ADDR, reg, val);

            //Console.WriteLine("nim tuner write: {0}, {1}", reg.ToString("X"), val.ToString("X"));

            return err;
        }

        public byte nim_read_tuner(byte reg, ref byte val)
        {
            byte err = 0;

            if (!repeater_on)
            {
                err = nim_write_demod(0xf12a, 0xb8);
                repeater_on = true;
            }

            if (err == 0) err = ftdi_device.ftdi_i2c_read_reg8(NIM_TUNER_ADDR, reg, ref val);

            //Console.WriteLine("nim tuner read: {0}, {1}", reg.ToString("X"), val.ToString("X"));

            return err;
        }

        public byte nim_write_demod(ushort reg, byte val)
        {
            //Console.WriteLine("nim demod write: {0}, {1}", reg.ToString("X"), val.ToString("X"));

            byte error = 0;

            if (repeater_on)
            {
                repeater_on = false;
                error = nim_write_demod(0xf12a, 0x38);
            }

            if (error == 0) error = ftdi_device.ftdi_i2c_write_reg16(0xd2, reg, val);

            if (error != 0)
            {
                Console.WriteLine("Error: demod write");
            }

            return error;
        }

        public byte nim_read_demod(ushort reg, ref byte val)
        {
            byte err = 0;


            if (repeater_on)
            {
                repeater_on = false;
                err = nim_write_demod(0xf12a, 0x38);
            }

            if (err == 0) err = ftdi_device.ftdi_i2c_read_reg16(0xd2, reg, ref val);

            if (err != 0)
            {
                Console.WriteLine("Error: demod read");
            }

            //Console.WriteLine("nim demod read: {0}, {1}", reg.ToString("X"), val.ToString("X"));


            return err;
        }

        public byte nim_init()
        {
            byte error = 0;
            byte val = 0;

            Console.WriteLine("Flow: NIM init");

            repeater_on = false;

            error = nim_write_demod(0xF536, 0xAA);
            error = nim_read_demod(0xF536, ref val);

            if (0xAA != val)
            {
                error = 12;
            }

            //turn off repeater
            repeater_on = false;

            if (error == 0)
            {
                error = nim_write_demod(0xf12a, 0x38);
            }

            return error;
        }
    }
}
