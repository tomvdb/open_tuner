
// mostly ported from longmynd - https://github.com/myorangedragon/longmynd - Heather Lomond

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTD2XX_NET;
using System.Threading;
using FlyleafLib.MediaFramework.MediaDevice;

namespace opentuner
{
    public class ftdi
    {
        public const byte TS1 = 0;
        public const byte TS2 = 1;

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
        uint NumBytesSent = 0;
        static uint NumBytesRead = 0;
        static byte[] MPSSEbuffer = new byte[500];
        static byte[] InputBuffer = new byte[500];
        static byte[] InputBuffer2 = new byte[500];
        static uint BytesAvailable = 0;
        static byte I2C_Status = 0;
        public bool Running = true;

        FTDI.FT_STATUS ftStatus = FTDI.FT_STATUS.FT_OK;
        FTDI ftdiDevice_i2c = new FTDI();
        FTDI ftdiDevice_ts = new FTDI();
        FTDI ftdiDevice_ts2 = new FTDI();

        // high byte
        /* Default GPIO value 0x6f = 0b01101111 = LNB Bias Off, LNB Voltage 12V, NIM not reset */
        byte ftdi_gpio_highbyte_value = 0x6f;

        /* Default GPIO direction 0xf1 = 0b11110001 = LNB pins, NIM Reset are outputs, TS2SYNC is input (0 for in and 1 for out) */
        byte ftdi_gpio_highbyte_direction = 0xf1;

        // low byte
        byte ftdi_gpio_lowbyte_value = 0x00;
        byte ftdi_gpio_lowbyte_direction = 0xFF;

        // high byte pins
        const byte FTDI_GPIO_PINID_NIM_RESET = 0;
        const byte FTDI_GPIO_PINID_TS2SYNC = 1;
        const byte FTDI_GPIO_PINID_LNB_BIAS_ENABLE = 4;
        const byte FTDI_GPIO_PINID_LED1 = 5;
        const byte FTDI_GPIO_PINID_LED2 = 6;
        const byte FTDI_GPIO_PINID_LNB_BIAS_VSEL = 7;

        const byte FTDI_GPIO_PINID_LNB2_BIAS_ENABLE = 4;
        const byte FTDI_GPIO_PINID_LNB2_BIAS_VSEL = 5;

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
            /***** Flush the buffer *****/
            I2C_Status = FlushBuffer(ftdi);

            /***** Synchronize the MPSSE interface by sending bad command 0xAA *****/
            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0xAA;
            I2C_Status = Send_Data_i2c(NumBytesToSend);
            if (I2C_Status != 0) return 1; // error();
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
            MPSSEbuffer[NumBytesToSend++] = 0x97;	// Disable adaptive clocking
            MPSSEbuffer[NumBytesToSend++] = 0x8D;   // Disable 3 phase data clocking

            I2C_Status = Send_Data_i2c(NumBytesToSend);
            if (I2C_Status != 0)
            {
                return 1;            //error();
            }

            NumBytesToSend = 0;

            MPSSEbuffer[NumBytesToSend++] = 0x80; // set ouput, low byte
            MPSSEbuffer[NumBytesToSend++] = 0x00; // value
            MPSSEbuffer[NumBytesToSend++] = 0xFF; // direction

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
            byte err = ftdi_gpio_write_highbyte(0, false);

            Thread.Sleep(10);

            if (err != 0)
                return 1;

            ftdi_gpio_write_highbyte(0, true);
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

            MPSSEbuffer[NumBytesToSend++] = 0x80; // low byte
            MPSSEbuffer[NumBytesToSend++] = 0x00; // value
            MPSSEbuffer[NumBytesToSend++] = 0x13; // direction

            MPSSEbuffer[NumBytesToSend++] = 0x11; // clock data bytes out
            MPSSEbuffer[NumBytesToSend++] = 0x00; // length l
            MPSSEbuffer[NumBytesToSend++] = 0x00; // length h
            MPSSEbuffer[NumBytesToSend++] = b;    // byte

            MPSSEbuffer[NumBytesToSend++] = 0x80; // low byte
            MPSSEbuffer[NumBytesToSend++] = 0x00; // value
            MPSSEbuffer[NumBytesToSend++] = 0x11; // direction

            MPSSEbuffer[NumBytesToSend++] = 0x27; // ?
            MPSSEbuffer[NumBytesToSend++] = 0x00; // ?
            MPSSEbuffer[NumBytesToSend++] = 0x87; // ?

            err = Send_Data_i2c(NumBytesToSend);
            NumBytesToSend = 0;

            if (err == 0)
                err = Receive_Data_i2c(1);

            uint i = NumBytesRead;

            if (err == 0 && (InputBuffer2[0] & 0x01) != 0)
            {
                err = 18;
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

            MPSSEbuffer[NumBytesToSend++] = 0x25; // ?
            MPSSEbuffer[NumBytesToSend++] = 0x00; // ?
            MPSSEbuffer[NumBytesToSend++] = 0x00; // ?
            MPSSEbuffer[NumBytesToSend++] = 0x87; // ?


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

        // get a list of all detected ft2232 devices
        public List<FTDIDevice> detect_all_ftdi()
        {
            uint device_count = 0;
            List<FTDIDevice> ftdi_devices = new List<FTDIDevice>();

            try
            {
                ftStatus = ftdiDevice_i2c.GetNumberOfDevices(ref device_count);
                Console.WriteLine("Number of FTDI Devices: " + device_count.ToString());

                for (uint c = 0; c < device_count; c++)
                {
                    FTDI ftdi_device = new FTDI();
                    ftdi_device.OpenByIndex(c);

                    FTDI.FT_DEVICE device = new FTDI.FT_DEVICE();
                    ftdi_device.GetDeviceType(ref device);

                    // is this a ft2232 device?
                    if (device.ToString() != "FT_DEVICE_2232H")
                    {
                        ftdi_device.Close();
                        continue;
                    }

                    FTDIDevice detected_ftdi_device = new FTDIDevice();
                    
                    // device index
                    detected_ftdi_device.device_index = c;

                    // device serial number
                    ftdi_device.GetSerialNumber(out string SerialNumber);
                    detected_ftdi_device.device_serial_number = SerialNumber;

                    // lets get description
                    ftdi_device.GetDescription(out string DeviceName);
                    detected_ftdi_device.device_description = DeviceName;

                    ftdi_device.Close();

                    ftdi_devices.Add(detected_ftdi_device);
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("FTDI Detect Error: " + Ex.Message);               
            }

            return ftdi_devices;
        }

        public byte ftdi_detect(ref uint i2c_port, ref uint ts_port, ref uint ts_port2, ref string detectedDeviceName)
        {
            Console.WriteLine("**** FTDI Ports Detection ****");

            byte err = 0;
            uint devcount = 0;

            ts_port = 99;
            i2c_port = 99;
            ts_port2 = 99;

            try
            {
                ftStatus = ftdiDevice_i2c.GetNumberOfDevices(ref devcount);
                Console.WriteLine("Number of FTDI Devices: " + devcount.ToString());

                // we need atleast two ports
                if (devcount < 2)
                {
                    Console.WriteLine("Not enough FTDI devices detected");
                    return 1;
                }

                for ( uint c = 0; c < devcount; c++)
                {
                    FTDI ftdi_device = new FTDI();
                    ftdi_device.OpenByIndex(c);

                    FTDI.FT_DEVICE device = new FTDI.FT_DEVICE();
                    ftdi_device.GetDeviceType(ref device);

                    ftdi_device.GetSerialNumber(out string SerialNumber);

                    Console.WriteLine("Serial Number: " + SerialNumber.ToString());

                    // is this a ft2232 device?
                    if (device.ToString() != "FT_DEVICE_2232H")
                    {
                        Console.WriteLine(c.ToString() + ": not a FT2232H device (" + device.ToString() + ") skipping");
                        ftdi_device.Close();
                        continue;
                    }
                    else
                    {
                        Console.WriteLine(c.ToString() + ": is a FT2232H device");
                    }

                    // lets get description
                    string deviceName = "";
                    ftdi_device.GetDescription(out deviceName);

                    Console.WriteLine("Description:" + deviceName);

                    FTDI.FT2232H_EEPROM_STRUCTURE eeprom = new FTDI.FT2232H_EEPROM_STRUCTURE();
                    ftdi_device.ReadFT2232HEEPROM(eeprom);

                    Console.WriteLine("A Fifo: " + eeprom.IFAIsFifo.ToString());
                    Console.WriteLine("B Fifo: " + eeprom.IFBIsFifo.ToString());

                    if (deviceName.Contains("NIM tuner A"))
                    {
                        Console.WriteLine("Should be the i2c port for a BATC V2 Minitiouner");
                        i2c_port = c;
                        detectedDeviceName = "BATC V2 Minitiouner";
                    }

                    if (deviceName.Contains("NIM tuner B"))
                    {
                        Console.WriteLine("Should be the ts port for a BATC V2 Minitiouner");
                        ts_port = c;
                    }

                    if (deviceName.Contains("NIM DB tuner B"))
                    {
                        Console.WriteLine("Should be the 2nd ts port for a BATC V2 Minitiouner");
                        ts_port2 = c;
                    }

                    if (deviceName.Contains("MiniTiouner_Pro_TS2 A"))
                    {
                        Console.WriteLine("Should be the i2c port for a Minitiouner Pro 2");
                        i2c_port = c;
                        detectedDeviceName = "Minitiouner Pro 2";
                    }

                    if (deviceName.Contains("MiniTiouner_Pro_TS2 B"))
                    {
                        Console.WriteLine("Should be the ts port for a Minitiouner Pro 2");
                        ts_port = c;
                    }

                    if (deviceName.Contains("MiniTiouner_Pro_TS1 B"))
                    {
                        Console.WriteLine("Should be the 2nd ts port for a Minitiouner Pro 2");
                        ts_port2 = c;
                    }

                    if (deviceName.Contains("MiniTiouner A"))
                    {
                        Console.WriteLine("Should be the i2c port for a Minitiouner-S");
                        i2c_port = c;
                        detectedDeviceName = "Minitiouner-S";
                    }

                    if (deviceName.Contains("MiniTiouner B"))
                    {
                        Console.WriteLine("Should be the ts port for a Minitiouner-S");
                        ts_port = c;
                    }

                    if (deviceName.Contains("MiniTiouner-Express A"))
                    {
                        Console.WriteLine("Should be the i2c port for a Minitiouner Express");
                        i2c_port = c;
                        detectedDeviceName = "Minitiouner Express";
                    }

                    if (deviceName.Contains("MiniTiouner-Express B"))
                    {
                        Console.WriteLine("Should be the ts port for a Minitiouner Express");
                        ts_port = c;
                    }

                    ftdi_device.Close();
                    Console.WriteLine(" ---- ");
                }

                Console.WriteLine(" **** ");


            }
            catch (Exception Ex)
            {
                Console.WriteLine("FTDI Error: " + Ex.Message);
                return 1;
            }


            return err;
        }

        public byte ftdi_init(uint i2c_device, uint ts_device, uint ts_device2)
        {
            byte err = 0;
            uint devcount = 0;

            // get devices
            try
            {
                ftStatus = ftdiDevice_i2c.GetNumberOfDevices(ref devcount);
                Console.WriteLine("Number of FTDI Devices: " + devcount.ToString());
            }
            catch (Exception Ex)
            {
                Console.WriteLine("FTDI Error: " + Ex.Message);
                return 1;
            }

            ftStatus = ftdiDevice_i2c.OpenByIndex(i2c_device);

            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                return 1;
            }

            ftStatus = ftdiDevice_ts.OpenByIndex(ts_device);

            if (ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                return 1;
            }

            if (ts_device2 != 99)
            {
                ftStatus = ftdiDevice_ts2.OpenByIndex(ts_device2);

                if (ftStatus != FTDI.FT_STATUS.FT_OK)
                {
                    return 1;
                }
            }


            err = ftdi_set_mpsse_mode(ftdiDevice_i2c);
            if (err == 0) err = ftdi_set_ftdi_io(ftdiDevice_i2c);
            if (err == 0) err = ftdi_nim_reset();


            return err;            
        }

        byte ftdi_gpio_write_lowbyte(byte pin_id, bool pin_value)
        {
            Console.WriteLine("Flow: FTDI GPIO Write: pin {0} -> value {1}", pin_id, pin_value);

            Console.WriteLine("ftdi_gpio_value: before: " + Convert.ToString(ftdi_gpio_lowbyte_value, 2));

            if (pin_value)
            {
                ftdi_gpio_lowbyte_value |= (byte)(1 << pin_id);
            }
            else
            {
                ftdi_gpio_lowbyte_value &= (byte)(~(1 << pin_id));
            }

            Console.WriteLine("ftdi_gpio_value: after: " + Convert.ToString(ftdi_gpio_lowbyte_value, 2));

            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0x80; // configure low bytes of mpsse port
            MPSSEbuffer[NumBytesToSend++] = ftdi_gpio_lowbyte_value;
            MPSSEbuffer[NumBytesToSend++] = ftdi_gpio_lowbyte_direction;

            I2C_Status = Send_Data_i2c(NumBytesToSend);

            NumBytesToSend = 0;

            return I2C_Status;
        }

        byte ftdi_gpio_write_highbyte(byte pin_id, bool pin_value)
        {
            //Console.WriteLine("Flow: FTDI GPIO Write: pin {0} -> value {1}", pin_id, pin_value);

            //Console.WriteLine("ftdi_gpio_highbyte_value: before: " + Convert.ToString(ftdi_gpio_highbyte_value, 2).PadLeft(8,'0'));

            if (pin_value)
            {
                ftdi_gpio_highbyte_value |= (byte)(1 << pin_id);
            }
            else
            {
                ftdi_gpio_highbyte_value &= (byte)(~(1 << pin_id));
            }

            //Console.WriteLine("ftdi_gpio_value: after: " + Convert.ToString(ftdi_gpio_highbyte_value, 2).PadLeft(8,'0'));

            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0x82; /* aka. MPSSE_CMD_SET_DATA_BITS_HIGHBYTE */
            MPSSEbuffer[NumBytesToSend++] = ftdi_gpio_highbyte_value;
            MPSSEbuffer[NumBytesToSend++] = ftdi_gpio_highbyte_direction;

            I2C_Status = Send_Data_i2c(NumBytesToSend);

            NumBytesToSend = 0;

            return I2C_Status;
        }

        public byte ftdi_ts_read(int device, ref byte[] data, ref uint bytesRead)
        {
            byte err = 0;

            FTDI.FT_STATUS ts_ftdi_status = FTDI.FT_STATUS.FT_OK;

            if (device == TS2)
            {
                ts_ftdi_status = ftdiDevice_ts.Read(data, Convert.ToUInt32(data.Length), ref bytesRead);
            }
            else
            {
                ts_ftdi_status = ftdiDevice_ts2.Read(data, Convert.ToUInt32(data.Length), ref bytesRead);
            }

            //Console.WriteLine(ts_ftdi_status.ToString());

            if (ts_ftdi_status != FTDI.FT_STATUS.FT_OK)
                err = 1;

            return err;
        }

        public byte ftdi_ts_available(int device, ref uint bytes_available)
        {
            byte err = 0;

            FTDI.FT_STATUS ts_ftdi_status = FTDI.FT_STATUS.FT_OK;

            if (device == TS2)
                ts_ftdi_status = ftdiDevice_ts.GetRxBytesAvailable(ref bytes_available);
            else
                ts_ftdi_status = ftdiDevice_ts2.GetRxBytesAvailable(ref bytes_available);

            if (ts_ftdi_status != FTDI.FT_STATUS.FT_OK)
                err = 1;

            return err;
        }

        public byte ftdi_ts_flush(int device)
        {
            byte err = 0;

            FTDI.FT_STATUS ts_ftdi_status = FTDI.FT_STATUS.FT_OK;

            if (device == TS2)
            {
                ts_ftdi_status = ftdiDevice_ts.Purge(FTDI.FT_PURGE.FT_PURGE_RX);
            }
            else
            {
                ts_ftdi_status = ftdiDevice_ts2.Purge(FTDI.FT_PURGE.FT_PURGE_RX);
            }

            if (ts_ftdi_status != FTDI.FT_STATUS.FT_OK)
                err = 1;

            return err;
        }


        public byte ftdi_ts_led(int led, bool setting)
        {
            byte err = 0;

            switch (led)
            {
                case 0: 
                    ftdi_gpio_write_highbyte(FTDI_GPIO_PINID_LED1, setting);
                    break;
                case 1:
                    ftdi_gpio_write_highbyte(FTDI_GPIO_PINID_LED2, setting);
                    break;
            }


            return err;
        }

        // on minitiouner pro 2 there are 2 outputs for the 2 different lnb switching - longmynd originally only catered for 1 output, the pro 2 needs two outputs. 
        // need to confirm express and S versions.
        public byte ftdi_set_polarization_supply(byte lnb_num, bool supply_enable, bool supply_horizontal)
        {
            byte err = 0;

            if (supply_enable)
            {
                // set voltage
                if (supply_horizontal)
                {
                    if (lnb_num == 0)
                    {
                        Console.WriteLine("Enable LNB2 VSEL");
                        ftdi_gpio_write_highbyte(FTDI_GPIO_PINID_LNB_BIAS_VSEL, true);
                        //ftdi_gpio_write_lowbyte(FTDI_GPIO_PINID_LNB2_BIAS_VSEL, true);
                    }
                }
                else
                {
                    Console.WriteLine("Disable LNB2 VSEL");
                    if (lnb_num == 0)
                    {
                        ftdi_gpio_write_highbyte(FTDI_GPIO_PINID_LNB_BIAS_VSEL, false);
                        //ftdi_gpio_write_lowbyte(FTDI_GPIO_PINID_LNB2_BIAS_VSEL, false);
                    }
                }

                if (lnb_num == 0)
                {
                    Console.WriteLine("Enable LNB2 Power");
                    ftdi_gpio_write_highbyte(FTDI_GPIO_PINID_LNB_BIAS_ENABLE, true);
                    //ftdi_gpio_write_lowbyte(FTDI_GPIO_PINID_LNB2_BIAS_ENABLE, true);
                }
            }
            else
            {
                // disable
                if (lnb_num == 0)
                {
                    Console.WriteLine("Disable LNB2 Power");
                    ftdi_gpio_write_highbyte(FTDI_GPIO_PINID_LNB_BIAS_ENABLE, false);
                    Console.WriteLine("Disable LNB2 VSEL");
                    ftdi_gpio_write_highbyte(FTDI_GPIO_PINID_LNB_BIAS_VSEL, false);
                    //ftdi_gpio_write_lowbyte(FTDI_GPIO_PINID_LNB2_BIAS_ENABLE, false);
                }
            }


            return err;

        }
    }

    public class FTDIDevice
    {
        public uint device_index { get; set; }
        public string device_serial_number { get; set; }
        public string device_description { get; set; }
    }
}
