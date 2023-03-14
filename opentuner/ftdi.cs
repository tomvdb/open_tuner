using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTD2XX_NET;
using System.Threading;

namespace opentuner
{
    public class ftdi
    {
        // ###### I2C Library defines ######
        const byte I2C_Dir_SDAin_SCLin = 0x00;
        const byte I2C_Dir_SDAin_SCLout = 0x01;
        const byte I2C_Dir_SDAout_SCLout = 0x03;
        const byte I2C_Dir_SDAout_SCLin = 0x02;
        const byte I2C_Data_SDAhi_SCLhi = 0x03;
        const byte I2C_Data_SDAlo_SCLhi = 0x01;
        const byte I2C_Data_SDAlo_SCLlo = 0x00;
        const byte I2C_Data_SDAhi_SCLlo = 0x02;

        // MPSSE clocking commands
        const byte MSB_FALLING_EDGE_CLOCK_BYTE_IN = 0x24;
        const byte MSB_RISING_EDGE_CLOCK_BYTE_IN = 0x20;
        const byte MSB_FALLING_EDGE_CLOCK_BYTE_OUT = 0x11;
        const byte MSB_DOWN_EDGE_CLOCK_BIT_IN = 0x26;
        const byte MSB_UP_EDGE_CLOCK_BYTE_IN = 0x20;
        const byte MSB_UP_EDGE_CLOCK_BYTE_OUT = 0x10;
        const byte MSB_RISING_EDGE_CLOCK_BIT_IN = 0x22;
        const byte MSB_FALLING_EDGE_CLOCK_BIT_OUT = 0x13;

        // Clock
        const uint ClockDivisor = 0x0095;

        // Sending and receiving
        static uint NumBytesToSend = 0;
        static uint NumBytesToRead = 0;
        uint NumBytesSent = 0;
        static uint NumBytesRead = 0;
        static byte[] MPSSEbuffer = new byte[500];
        static byte[] InputBuffer = new byte[500];
        static byte[] InputBuffer2 = new byte[500];
        static uint BytesAvailable = 0;
        static bool I2C_Ack = false;
        static byte AppStatus = 0;
        static byte I2C_Status = 0;
        public bool Running = true;
        static bool DeviceOpen = false;
        // GPIO
        static byte GPIO_Low_Dat = 0;
        static byte GPIO_Low_Dir = 0;
        static byte ADbusReadVal = 0;
        static byte ACbusReadVal = 0;

        FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
        FTDI ftdiDevice_i2c = new FTDI();
        FTDI ftdiDevice_ts = new FTDI();

        /* Default GPIO value 0x6f = 0b01101111 = LNB Bias Off, LNB Voltage 12V, NIM not reset */
        byte ftdi_gpio_value = 0x6f;

        /* Default GPIO direction 0xf1 = 0b11110001 = LNB pins, NIM Reset are outputs, TS2SYNC is input (0 for in and 1 for out) */
        byte ftdi_gpio_direction = 0xf1;


        private byte Receive_Data_i2c(uint BytesToRead)
        {
            uint NumBytesInQueue = 0;
            uint QueueTimeOut = 0;
            uint Buffer1Index = 0;
            uint Buffer2Index = 0;
            uint TotalBytesRead = 0;
            bool QueueTimeoutFlag = false;
            uint NumBytesRxd = 0;

            // Keep looping until all requested bytes are received or we've tried 5000 times (value can be chosen as required)
            while ((TotalBytesRead < BytesToRead) && (QueueTimeoutFlag == false))
            {
                ftStatus = ftdiDevice_i2c.GetRxBytesAvailable(ref NumBytesInQueue);       // Check bytes available

                if ((NumBytesInQueue > 0) && (ftStatus == FTDI.FT_STATUS.FT_OK))
                {
                    ftStatus = ftdiDevice_i2c.Read(InputBuffer, NumBytesInQueue, ref NumBytesRxd);  // if any available read them

                    if ((NumBytesInQueue == NumBytesRxd) && (ftStatus == FTDI.FT_STATUS.FT_OK))
                    {
                        Buffer1Index = 0;

                        while (Buffer1Index < NumBytesRxd)
                        {
                            InputBuffer2[Buffer2Index] = InputBuffer[Buffer1Index];     // copy into main overall application buffer
                            Buffer1Index++;
                            Buffer2Index++;
                        }
                        TotalBytesRead = TotalBytesRead + NumBytesRxd;                  // Keep track of total
                    }
                    else
                        return 1;

                    QueueTimeOut++;
                    if (QueueTimeOut == 5000)
                        QueueTimeoutFlag = true;
                    else
                        Thread.Sleep(0);                                                // Avoids running Queue status checks back to back
                }
            }
            // returning globals NumBytesRead and the buffer InputBuffer2
            NumBytesRead = TotalBytesRead;

            if (QueueTimeoutFlag == true)
                return 1;
            else
                return 0;
        }


        //###################################################################################################################################
        // Write a buffer of data and check that it got sent without error

        private byte Send_Data_i2c(uint BytesToSend)
        {

            NumBytesToSend = BytesToSend;

            // Send data. This will return once all sent or if times out
            ftStatus = ftdiDevice_i2c.Write(MPSSEbuffer, NumBytesToSend, ref NumBytesSent);

            // Ensure that call completed OK and that all bytes sent as requested
            if ((NumBytesSent != NumBytesToSend) || (ftStatus != FTDI.FT_STATUS.FT_OK))
                return 1;   // error   calling function can check NumBytesSent to see how many got sent
            else
                return 0;   // success
        }


        private byte FlushBuffer(FTDI ftdi)
        {
            ftStatus = ftdi.GetRxBytesAvailable(ref BytesAvailable);	 // Get the number of bytes in the receive buffer
            if (ftStatus != FTDI.FT_STATUS.FT_OK)
                return 1;

            if (BytesAvailable > 0)
            {
                ftStatus = ftdi.Read(InputBuffer, BytesAvailable, ref NumBytesRead);  	//Read out the data from receive buffer
                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                    return 1;       // error
                else
                    return 0;       // all bytes successfully read
            }
            else
            {
                return 0;           // there were no bytes to read
            }
        }


        // ***********************************************
        private byte ftdi_set_mpsse_mode(FTDI ftdi)
        {

            Console.WriteLine("Flow: FTDI set mpsse mode");

            NumBytesToSend = 0;

            /***** Initial device configuration *****/

            ftStatus = FTDI.FT_STATUS.FT_OK;
            ftStatus |= ftdi.SetTimeouts(5000, 5000);
            ftStatus |= ftdi.SetLatency(16);
            //ftStatus |= ftdi.SetFlowControl(FTDI.FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0x00, 0x00);
            ftStatus |= ftdi.SetBitMode(0x00, 0x00);
            ftStatus |= ftdi.SetBitMode(0x00, 0x02);         // MPSSE mode        

            Thread.Sleep(10);

            if (ftStatus != FTDI.FT_STATUS.FT_OK)
                return 1; // error();


            return 0;
        }

        private byte ftdi_set_ftdi_io(FTDI ftdi)
        {
            byte ADbusVal = 0;
            byte ADbusDir = 0;

            /***** Flush the buffer *****/
            I2C_Status = FlushBuffer(ftdi);

            /***** Synchronize the MPSSE interface by sending bad command 0xAA *****/
            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0xAA;
            I2C_Status = Send_Data_i2c(NumBytesToSend);
            if (I2C_Status != 0) return 1; // error();
            NumBytesToRead = 2;
            I2C_Status = Receive_Data_i2c(2);
            if (I2C_Status != 0) return 1; //error();

            if ((InputBuffer2[0] == 0xFA) && (InputBuffer2[1] == 0xAA))
            {
                //MessageBox.Show("MPSSE Synced");
            }
            else
            {
                return 1;            //error();
            }

            /***** Synchronize the MPSSE interface by sending bad command 0xAB *****/
            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0xAB;
            I2C_Status = Send_Data_i2c(NumBytesToSend);
            if (I2C_Status != 0) return 1; // error();
            NumBytesToRead = 2;
            I2C_Status = Receive_Data_i2c(2);
            if (I2C_Status != 0) return 1; //error();

            if ((InputBuffer2[0] == 0xFA) && (InputBuffer2[1] == 0xAB))
            {
                //MessageBox.Show("MPSSE Synced");
            }
            else
            {
                return 1;            //error();
            }

            NumBytesToSend = 0;

            MPSSEbuffer[NumBytesToSend++] = 0x8A; 	// Disable clock divide by 5 for 60Mhz master clock
            MPSSEbuffer[NumBytesToSend++] = 0x97;	// Turn off adaptive clocking
            MPSSEbuffer[NumBytesToSend++] = 0x8D;

            I2C_Status = Send_Data_i2c(NumBytesToSend);
            if (I2C_Status != 0)
            {
                return 1;            //error();
            }

            NumBytesToSend = 0;

            MPSSEbuffer[NumBytesToSend++] = 0x80; // set ouput, low byte
            MPSSEbuffer[NumBytesToSend++] = 0x13; // value
            MPSSEbuffer[NumBytesToSend++] = 0x13; // direction

            MPSSEbuffer[NumBytesToSend++] = 0x82; // set output, high byte
            MPSSEbuffer[NumBytesToSend++] = 0x6F; // value
            MPSSEbuffer[NumBytesToSend++] = 0xF1; // direction

            MPSSEbuffer[NumBytesToSend++] = 0x86; 	//Command to set clock divisor
            MPSSEbuffer[NumBytesToSend++] = (byte)(ClockDivisor & 0x00FF);	//Set 0xValueL of clock divisor
            MPSSEbuffer[NumBytesToSend++] = (byte)((ClockDivisor >> 8) & 0x00FF);	//Set 0xValueH of clock divisor

            I2C_Status = Send_Data_i2c(NumBytesToSend);
            if (I2C_Status != 0)
            {
                return 1;            //error();
            }

            Thread.Sleep(30);

            NumBytesToSend = 0;

            MPSSEbuffer[NumBytesToSend++] = 0x85; 			// loopback off
            I2C_Status = Send_Data_i2c(NumBytesToSend);
            NumBytesToSend = 0;

            if (I2C_Status != 0)
            {
                return 1;            //error();
            }

            Thread.Sleep(30);

            return 0;
        }

        byte ftdi_nim_reset()
        {
            // byte err = I2C_SetGPIOValuesHigh(0, 0);
            byte err = ftdi_gpio_write(0, false);

            Thread.Sleep(10);

            if (err != 0)
                return 1;
            
            ftdi_gpio_write(0, true);
            //err = I2C_SetGPIOValuesHigh(1, 1);
            Thread.Sleep(10);

            return err;
        }

        byte ftdi_i2c_set_start()
        {
            int count;

            // FTDI_STOP_START_REPEATS = 4

            for (count = 0; count < 4; count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;
                MPSSEbuffer[NumBytesToSend++] = 0x03;
                MPSSEbuffer[NumBytesToSend++] = 0x13;
            }

            for (count = 0; count < 4; count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;
                MPSSEbuffer[NumBytesToSend++] = 0x01;
                MPSSEbuffer[NumBytesToSend++] = 0x13;
            }

            return 0;
        }

        byte ftdi_i2c_set_stop()
        {
            int count;

            // FTDI_STOP_START_REPEATS = 4

            for (count = 0; count < 4; count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;
                MPSSEbuffer[NumBytesToSend++] = 0x01;
                MPSSEbuffer[NumBytesToSend++] = 0x13;
            }

            for (count = 0; count < 4; count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;
                MPSSEbuffer[NumBytesToSend++] = 0x03;
                MPSSEbuffer[NumBytesToSend++] = 0x13;
            }

            MPSSEbuffer[NumBytesToSend++] = 0x80;
            MPSSEbuffer[NumBytesToSend++] = 0x03;
            MPSSEbuffer[NumBytesToSend++] = 0x10;

            return 0;
        }


        public byte ftdi_i2c_send_byte_check_ack(byte b)
        {
            byte err;

            MPSSEbuffer[NumBytesToSend++] = 0x80;
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x13;
            MPSSEbuffer[NumBytesToSend++] = 0x11;
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = b;
            MPSSEbuffer[NumBytesToSend++] = 0x80;
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x11;
            MPSSEbuffer[NumBytesToSend++] = 0x27;
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x87;

            err = Send_Data_i2c(NumBytesToSend);
            NumBytesToSend = 0;

            if (err == 0)
                err = Receive_Data_i2c(1);

            uint i = NumBytesRead;
            int x = 0;

            if (err == 0 && (InputBuffer2[0] & 0x01) != 0)
            {
                err = 18;
                //MessageBox.Show("No I2C Ack");
            }


            return err;
        }

        public byte ftdi_i2c_read_byte_send_nak(ref byte b)
        {
            byte err;

            MPSSEbuffer[NumBytesToSend++] = 0x80;
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x13;
            MPSSEbuffer[NumBytesToSend++] = 0x80;
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x11;
            MPSSEbuffer[NumBytesToSend++] = 0x25;
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x87;


            err = Send_Data_i2c(NumBytesToSend);
            NumBytesToSend = 0;

            if (err == 0)
                err = Receive_Data_i2c(1);

            b = InputBuffer2[0];

            return err;

        }


        byte ftdi_i2c_output()
        {
            byte err;

            err = Send_Data_i2c(NumBytesToSend);
            NumBytesToSend = 0;

            return err;
        }

        public byte ftdi_i2c_read_reg8(byte addr, byte reg, ref byte val)
        {
            byte err = 0;
            int i = 0;
            int timeout = 0;

            do
            {
                for (i = 0; i < 10; i++)
                {
                    err = ftdi_i2c_set_start();
                    err |= ftdi_i2c_send_byte_check_ack(addr);
                    err |= ftdi_i2c_send_byte_check_ack(reg);
                    err |= ftdi_i2c_set_stop();
                    err |= ftdi_i2c_output();
                    if (err == 0) break;
                }

                if (err == 0)
                {
                    for (i = 0; i < 10; i++)
                    {
                        err = ftdi_i2c_set_start();
                        err |= ftdi_i2c_send_byte_check_ack((byte)(addr | 0x01));
                        err |= ftdi_i2c_read_byte_send_nak(ref val);
                        err |= ftdi_i2c_set_stop();
                        err |= ftdi_i2c_output();
                        if (err == 0) break;
                    }
                }


            } while (err != 0 && (timeout != 100));

            return err;
        }

        public byte ftdi_i2c_write_reg8(byte addr, byte reg, byte val)
        {
            byte err = 0;
            int i;
            int timeout = 0;

            do
            {
                for ( i = 0; i < 10; i++ )
                {
                    err = ftdi_i2c_set_start();
                    err|= ftdi_i2c_send_byte_check_ack(addr);
                    err |= ftdi_i2c_send_byte_check_ack(reg);
                    err |= ftdi_i2c_send_byte_check_ack(val);
                    err |= ftdi_i2c_set_stop();
                    err |= ftdi_i2c_output();

                    if (err == 0) break;
                }

            } while ((err != 0) && timeout != 100);

            return err;
        }

        public byte ftdi_i2c_write_reg16(byte addr, ushort reg, byte val)
        {
            byte err = 0;
            short i;
            short timeout = 0;

            do
            {
                for (i = 0; i < 10; i++)
                {
                    err = ftdi_i2c_set_start();
                    err |= ftdi_i2c_send_byte_check_ack(addr);
                    err |= ftdi_i2c_send_byte_check_ack((byte)(reg >> 8));
                    err |= ftdi_i2c_send_byte_check_ack((byte)(reg & 0xFF));
                    err |= ftdi_i2c_send_byte_check_ack(val);
                    err |= ftdi_i2c_set_stop();
                    err |= ftdi_i2c_output();

                    if (err == 0)
                        break;
                }

                timeout += 1;
            } while (err != 0 && timeout != 100);

            if (err != 0)
            {
                //MessageBox.Show("Error Write Reg 16");
            }

            return err;
        }

        public byte ftdi_i2c_read_reg16(byte addr, ushort reg, ref byte val)
        {
            byte err = 0;
            int i = 0;
            int timeout = 0;

            do
            {
                for (i = 0; i < 10; i++)
                {
                    err = ftdi_i2c_set_start();
                    err |= ftdi_i2c_send_byte_check_ack(addr);
                    err |= ftdi_i2c_send_byte_check_ack((byte)(reg >> 8));
                    err |= ftdi_i2c_send_byte_check_ack((byte)(reg & 0xff));
                    if (err == 0)
                        break;
                }

                if (err == 0)
                {
                    for (i = 0; i < 10; i++)
                    {
                        err = ftdi_i2c_set_start();
                        err |= ftdi_i2c_send_byte_check_ack((byte)(addr | 0x01));
                        err |= ftdi_i2c_read_byte_send_nak(ref val);
                        err |= ftdi_i2c_set_stop();
                        err |= ftdi_i2c_output();
                        if (err == 0)
                            break;
                    }
                }

                timeout += 1;

            } while (err != 0 && timeout != 100);

            if (err != 0)
            {
                //MessageBox.Show("Error Read Reg 16");
            }

            return err;

        }

        public byte ftdi_init()
        {
            byte err = 0;
            uint devcount = 0;

            // get devices
            try
            {
                ftStatus = ftdiDevice_i2c.GetNumberOfDevices(ref devcount);
            }
            catch (Exception Ex)
            {
                return 1;
            }

            ftStatus = ftdiDevice_i2c.OpenByIndex(0);

            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                return 1;
            }

            ftStatus = ftdiDevice_ts.OpenByIndex(1);

            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                return 1;
            }

            err = ftdi_set_mpsse_mode(ftdiDevice_i2c);
            //if (err == 0) err = ftdi_set_mpsse_mode(ftdiDevice_ts);
            if (err == 0) err = ftdi_set_ftdi_io(ftdiDevice_i2c);
            if (err == 0) err = ftdi_nim_reset();


            return err;            
        }

        byte ftdi_gpio_write(byte pin_id, bool pin_value)
        {
            Console.WriteLine("Flow: FTDI GPIO Write: pin {0} -> value {1}", pin_id, pin_value);

            Console.WriteLine("ftdi_gpio_value: before: " + Convert.ToString(ftdi_gpio_value, 2));

            if (pin_value)
            {
                ftdi_gpio_value |= (byte)(1 << pin_id);
            }
            else
            {
                ftdi_gpio_value &= (byte)(~(1 << pin_id));
            }

            Console.WriteLine("ftdi_gpio_value: after: " + Convert.ToString(ftdi_gpio_value, 2));


            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0x82; /* aka. MPSSE_CMD_SET_DATA_BITS_HIGHBYTE */
            MPSSEbuffer[NumBytesToSend++] = ftdi_gpio_value;
            MPSSEbuffer[NumBytesToSend++] = ftdi_gpio_direction;

            I2C_Status = Send_Data_i2c(NumBytesToSend);

            NumBytesToSend = 0;

            return I2C_Status;
        }

        public byte ftdi_ts_read(ref byte[] data, ref uint bytesRead)
        {
            byte err = 0;

            FTDI.FT_STATUS ts_ftdi_status = FTDI.FT_STATUS.FT_OK;

            ts_ftdi_status = ftdiDevice_ts.Read(data, Convert.ToUInt32(data.Length), ref bytesRead);

            //Console.WriteLine(ts_ftdi_status.ToString());

            if (ts_ftdi_status != FTDI.FT_STATUS.FT_OK)
                err = 1;

            return err;
        }

        public byte ftdi_ts_available(ref uint bytes_available)
        {
            byte err = 0;

            FTDI.FT_STATUS ts_ftdi_status = FTDI.FT_STATUS.FT_OK;

            ts_ftdi_status = ftdiDevice_ts.GetRxBytesAvailable(ref bytes_available);

            
            byte[] data = new byte[20*512];
            uint dataRead = 0;

            ts_ftdi_status = ftdiDevice_ts.Read(data, 20*512, ref dataRead);

            Console.WriteLine(ts_ftdi_status.ToString());
            Console.WriteLine(dataRead.ToString());

            // todo check result
            //Console.WriteLine(ts_ftdi_status.ToString());
            

            return err;
        }
    }
}
