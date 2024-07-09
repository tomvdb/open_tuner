
// mostly ported from longmynd - https://github.com/myorangedragon/longmynd - Heather Lomond

using System;
using System.Collections.Generic;
using FTD2XX_NET;
using System.Threading;
using LibUsbDotNet.Info;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using System.Collections.ObjectModel;
using opentuner.MediaSources.Minitiouner.HardwareInterfaces;
using Serilog;

namespace opentuner
{
    public class PicoTunerInterface : MTHardwareInterface
    {
        const int USB_TIMEOUT = 5000;

        static UsbDevice i2c_pt_device;

        static UsbEndpointWriter i2cEndPointWriter = null;
        static UsbEndpointReader i2cEndPointReader = null;
        static UsbEndpointReader ts2EndPointReader = null;
        static UsbEndpointReader ts1EndPointReader = null;

        public const byte TS1 = 0;
        public const byte TS2 = 1;

        // Clock
        const uint ClockDivisor = 0x0095;

        // Sending and receiving
        static uint NumBytesToSend = 0;
        int NumBytesSent = 0;
        static int NumBytesRead = 0;
        static byte[] MPSSEbuffer = new byte[500];
        static byte[] InputBuffer = new byte[500];
        static byte[] InputBuffer2 = new byte[500];
        static uint BytesAvailable = 0;
        static byte I2C_Status = 0;
        public bool Running = true;

        // we only have 1 port on picotuner for control (old FTDI AC Port)

        // high byte
        /* Default GPIO value 0x6f = 0b01101111 = LNB Bias Off, LNB Voltage 12V, NIM not reset */
        byte gpio_value = 0x01;
      
        // all pins on picotuner set as output
        byte gpio_direction = 0xFF;

        /*
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
        */

        /*
            #define AC0 2         //NIM RESET LOW
            #define AC1 3         //
            #define AC2 4         //J8-6
            #define AC3 5         //J8-5
            #define AC4 6         //J8-4  LNB Bias Enable 
            #define AC5 7         //J8-3 
            #define AC6 8         //J8-2 
            #define AC7 9         //J8-1  LNB BIAS Voltage Select

        */

        const byte GPIO_NIM_RESET = 0;

        const byte GPIO_LNB1_ENABLE = 4;
        const byte GPIO_LNB2_ENABLE = 5;
        const byte GPIO_LNB2_VSEL = 6;
        const byte GPIO_LNB1_VSEL = 7;

        public override bool RequireSerialTS => true;

        public override string GetName => "PicoTuner";

        private byte Receive_Data_i2c(uint BytesToRead)
        {
            uint QueueTimeOut = 0;
            uint Buffer1Index = 0;
            uint Buffer2Index = 0;
            int TotalBytesRead = 0;
            bool QueueTimeoutFlag = false;
            int NumBytesRxd = 0;

            if (i2cEndPointReader == null)
            {
                Log.Information("error: i2cEndPointReader is null");
                return 1;
            }

            // Keep looping until all requested bytes are received or we've tried 5000 times (value can be chosen as required)
            while ((TotalBytesRead < BytesToRead) && (QueueTimeoutFlag == false))
            {
                //ftStatus = ftdiDevice_i2c.GetRxBytesAvailable(ref NumBytesInQueue);       // Check bytes available


                var error = i2cEndPointReader.Read(InputBuffer, USB_TIMEOUT, out NumBytesRxd);

                //ftStatus = ftdiDevice_i2c.Read(InputBuffer, NumBytesInQueue, ref NumBytesRxd);  // if any available read them

                if (NumBytesRxd < 3)
                {
                    Log.Information("Empty Response");
                }

                if (error == LibUsbDotNet.Error.Success)
                {
                    // first two bytes are ftdi bytes
                    Buffer1Index = 2;

                    while (Buffer1Index < NumBytesRxd)
                    {
                        InputBuffer2[Buffer2Index] = InputBuffer[Buffer1Index];     // copy into main overall application buffer
                        Buffer1Index++;
                        Buffer2Index++;
                    }
                    TotalBytesRead = TotalBytesRead + NumBytesRxd;                  // Keep track of total
                }
                else
                {
                    Log.Information("EndPointError: " + error.ToString());
                    return 1;
                }

                QueueTimeOut++;
                if (QueueTimeOut == 5000)
                    QueueTimeoutFlag = true;
                else
                    Thread.Sleep(0);                                                // Avoids running Queue status checks back to back
            }
            // returning globals NumBytesRead and the buffer InputBuffer2
            NumBytesRead = TotalBytesRead;

            if (QueueTimeoutFlag == true)
            {
                Log.Information("Queue Timout Error");
                return 1;
            }
            else
            {
                //Log.Information("Read: " + (NumBytesRead - 2).ToString());
                return 0;
            }
        }

        private byte Send_Data_i2c(uint BytesToSend)
        {
            if (i2cEndPointWriter == null)
            {
                Log.Information("Error: Endpointwriter is null");
                return 1;
            }

            NumBytesToSend = BytesToSend;

            var error = i2cEndPointWriter.Write(MPSSEbuffer, 0, (int)BytesToSend, USB_TIMEOUT, out NumBytesSent);

            // Ensure that call completed OK and that all bytes sent as requested
            if ((NumBytesSent != NumBytesToSend) || error != LibUsbDotNet.Error.Success)
            {
                Log.Information("Error: " + error.ToString());
                Log.Information("Send: " + NumBytesToSend.ToString());
                Log.Information("Sent: " + NumBytesSent.ToString());
                return 1;   // error   calling function can check NumBytesSent to see how many got sent
            }
            else
                return 0;   // success
        }

        private byte FlushBuffer(FTD2XX_NET.FTDI ftdi)
        {
            byte err = 0;

            Log.Information("Flush Buffer Requested");
            i2cEndPointReader.ReadFlush();

            return err;
        }


        // ***********************************************
        private byte ftdi_set_mpsse_mode(FTD2XX_NET.FTDI ftdi)
        {
            Log.Information("Flow: FTDI set mpsse mode");

            return 0;
        }

        private byte ftdi_set_ftdi_io(FTD2XX_NET.FTDI ftdi)
        {
            /***** Flush the buffer *****/
            I2C_Status = FlushBuffer(ftdi);

            Log.Information(i2c_pt_device.IsOpen.ToString());
            Log.Information(i2cEndPointReader.ToString());
            Log.Information(i2cEndPointWriter.ToString());

            /***** Synchronize the MPSSE interface by sending bad command 0xAA *****/
            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0xAA;
            I2C_Status = Send_Data_i2c(NumBytesToSend);
            if (I2C_Status != 0) return 1; // error();
            I2C_Status = Receive_Data_i2c(2);
            if (I2C_Status != 0) return 1; //error();

            if ((InputBuffer2[0] == 0xFA) && (InputBuffer2[1] == 0xAA))
            {
                Log.Information("mppse synced");
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
            MPSSEbuffer[NumBytesToSend++] = gpio_value; // value
            MPSSEbuffer[NumBytesToSend++] = gpio_direction; // direction

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
            Log.Information("Nim Reset");

            byte err = gpio_write(0, false);
            Thread.Sleep(10);

            if (err != 0)
                return 1;

            gpio_write(0, true);
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


        private byte ftdi_i2c_send_byte_check_ack(byte b)
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

            int i = NumBytesRead;

            if (err == 0 && (InputBuffer2[0] & 0x01) != 0)
            {
                err = 18;
            }

            return err;
        }

        private byte ftdi_i2c_read_byte_send_nak(ref byte b)
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


        private byte ftdi_i2c_output()
        {
            byte err;

            err = Send_Data_i2c(NumBytesToSend);
            NumBytesToSend = 0;

            return err;
        }

        public override byte nim_read_reg8(byte addr, byte reg, ref byte val)
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

        public override byte nim_write_reg8(byte addr, byte reg, byte val)
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

        public override byte nim_write_reg16(byte addr, ushort reg, byte val)
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

        public override byte nim_read_reg16(byte addr, ushort reg, ref byte val)
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
            List<FTDIDevice> ftdi_devices = new List<FTDIDevice>();
            /*

            try
            {
                ftStatus = ftdiDevice_i2c.GetNumberOfDevices(ref device_count);
                Log.Information("Number of FTDI Devices: " + device_count.ToString());

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
                Log.Information("FTDI Detect Error: " + Ex.Message);               
            }
            */

            return ftdi_devices;
        }

        public override byte hw_detect(ref uint i2c_port, ref uint ts_port, ref uint ts_port2, ref string detectedDeviceName, string i2c_serial, string ts_serial, string ts2_serial)
        {
            byte err = 0;

            i2c_port = 0;
            ts_port = 0;
            ts_port2 = 0;
            detectedDeviceName = "PicoTuner";

            return err;
        }

        public override byte hw_detect(ref uint i2c_port, ref uint ts_port, ref uint ts_port2, ref string detectedDeviceName)
        {

            byte err = 0;

            i2c_port = 0;
            ts_port = 0;
            ts_port2 = 0;
            detectedDeviceName = "PicoTuner";

            return err;
        }

        public override byte hw_init(uint i2c_device, uint ts_device, uint ts_device2)
        {
            byte err = 0;


            UsbContext usbContext = new UsbContext();
            var usbDeviceCollection = usbContext.List();

            i2c_pt_device = null;

            for (int c = 0; c < usbDeviceCollection.Count;c++)
            {
                if (usbDeviceCollection[c].Info.VendorId == 0x2E8A)
                {
                    i2c_pt_device = (UsbDevice)usbDeviceCollection[c].Clone();
                    //ts_pt_device = (UsbDevice)usbDeviceCollection[c].Clone();
                    break;
                }
            }

            if (i2c_pt_device == null)
            {
                Log.Information("pt device is null");
                return 1;
            }

            if (i2c_pt_device.TryOpen() )
            {
                Log.Information("Device Open");
            }
            else
            {
                Log.Information("Error o2c Device Open");
                return 1;
            }
                       
            // got all the info, lets see if we can do something with it
            bool claim0 = i2c_pt_device.ClaimInterface(i2c_pt_device.Configs[0].Interfaces[0].Number);
            bool claim1 = i2c_pt_device.ClaimInterface(i2c_pt_device.Configs[0].Interfaces[1].Number);

            Log.Information("Claim 0: " + claim0);
            Log.Information("Claim 1: " + claim1);

            i2cEndPointWriter = i2c_pt_device.OpenEndpointWriter(WriteEndpointID.Ep02);
            i2cEndPointReader = i2c_pt_device.OpenEndpointReader(ReadEndpointID.Ep01);

            Log.Information("I2C Endpoint Reader Address : " + i2cEndPointReader.EndpointInfo.EndpointAddress.ToString("X"));
            Log.Information("I2C Endpoint Writer Address : " + i2cEndPointWriter.EndpointInfo.EndpointAddress.ToString("X"));

            ts2EndPointReader = i2c_pt_device.OpenEndpointReader(ReadEndpointID.Ep03);
            ts1EndPointReader = i2c_pt_device.OpenEndpointReader(ReadEndpointID.Ep04);

            Log.Information("TS2 Endpoint Reader Address : " + ts2EndPointReader.EndpointInfo.EndpointAddress.ToString("X"));
            Log.Information("TS1 Endpoint Reader Address : " + ts1EndPointReader.EndpointInfo.EndpointAddress.ToString("X"));

            Log.Information(i2c_pt_device.IsOpen.ToString());

            err = ftdi_set_mpsse_mode(null);
            if (err == 0) err = ftdi_set_ftdi_io(null);
            if (err == 0) err = ftdi_nim_reset();

            return err;            
        }

        byte gpio_write(byte pin_id, bool pin_value)
        {
            Log.Information("Flow: GPIO Write: pin {0} -> value {1}", pin_id, pin_value);

            //Log.Information("ftdi_gpio_highbyte_value: before: " + Convert.ToString(gpio_value, 2).PadLeft(8,'0'));

            if (pin_value)
            {
                gpio_value |= (byte)(1 << pin_id);
            }
            else
            {
                gpio_value &= (byte)(~(1 << pin_id));
            }


            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0x82; /* aka. MPSSE_CMD_SET_DATA_BITS_HIGHBYTE */
            MPSSEbuffer[NumBytesToSend++] = gpio_value;
            MPSSEbuffer[NumBytesToSend++] = gpio_direction;

            Log.Information(Convert.ToString(gpio_value, 2).PadLeft(8, '0'));
            Log.Information("0x82: [" + gpio_value + "][" + gpio_direction + "]");

            I2C_Status = Send_Data_i2c(NumBytesToSend);

            Log.Information("Result:" + I2C_Status.ToString());

            NumBytesToSend = 0;

            return I2C_Status;
        }

        public override byte transport_read(int device, ref byte[] data, ref uint bytesRead)
        {
            byte[] readdata = new byte[4096];

            int iBytesRead = 0;

            LibUsbDotNet.Error error;

            if (device == TS2)
            {
                error = ts2EndPointReader.Read(readdata, USB_TIMEOUT, out iBytesRead);
            }
            else
            {
                error = ts1EndPointReader.Read(readdata, USB_TIMEOUT, out iBytesRead);
            }

            if (error != LibUsbDotNet.Error.Success)
            {
                Log.Information("TS Read Error" + error.ToString());
                return 1;
            }

            // empty response
            if (iBytesRead == 2)
            {
                bytesRead = 0;
                return 0;
            }

            // we receive the data in chunks of 512, first two bytes of each chunk needs to be removed
            if (iBytesRead % 512 != 0)
            {
                Log.Information("Ignoring: " + iBytesRead + "," + (iBytesRead % 512).ToString());
                bytesRead = 0;
                return 0;
            }

            int chunks = iBytesRead / 512;

            for ( int c = 0; c < chunks; c++ )
                Array.Copy(readdata, (c * 512) + 2, data, c * 510, 510);

            bytesRead = (uint)(chunks * 510);

            return 0;
        }

        public override byte transport_flush(int device)
        {           
            byte err = 0;

            if (device == TS2)
                ts2EndPointReader.ReadFlush();
            else
                ts1EndPointReader.ReadFlush();

            return err;
        }


        public override byte hw_ts_led(int led, bool setting)
        {
            /*
            byte err = 0;

            switch (led)
            {
                case 0: 
                    ftdi_gpio_write_highbyte(FTDI_GPIO_PINID_LED1, !setting);
                    break;
                case 1:
                    ftdi_gpio_write_highbyte(FTDI_GPIO_PINID_LED2, !setting);
                    break;
            }


            return err;

            */

            return 0;
        }

        // on minitiouner pro 2 there are 2 outputs for the 2 different lnb switching - longmynd originally only catered for 1 output, the pro 2 needs two outputs. 
        // need to confirm express and S versions.


        public override byte hw_set_polarization_supply(byte lnb_num, bool supply_enable, bool supply_horizontal)
        {
            byte err = 0;

        
            if (supply_enable)
            {
                // set voltage
                if (supply_horizontal)
                {
                    if (lnb_num == 0)
                    {
                        Log.Information("Enable LNB2 VSEL");
                        gpio_write(GPIO_LNB2_VSEL, true);
                    }
                    else
                    {
                        Log.Information("Enable LNB1 VSEL");
                        gpio_write(GPIO_LNB1_VSEL, true);
                    }

                }
                else
                {
                    if (lnb_num == 0)
                    {
                        Log.Information("Disable LNB2 VSEL");
                        gpio_write(GPIO_LNB2_VSEL, false);
                    }
                    else
                    {
                        Log.Information("Disable LNB1 VSEL");
                        gpio_write(GPIO_LNB1_VSEL, false);
                    }
                }

                if (lnb_num == 0)
                {
                    Log.Information("Enable LNB1 Power");
                    gpio_write(GPIO_LNB2_ENABLE, true);
                }
                else
                {
                    Log.Information("Enable LNB2 Power");
                    gpio_write(GPIO_LNB1_ENABLE, true);
                }
            }
            else
            {
                // disable
                if (lnb_num == 0)
                {
                    Log.Information("Disable LNB1 Power");
                    gpio_write(GPIO_LNB2_ENABLE, false);
                    gpio_write(GPIO_LNB2_VSEL, false);
                }
                else
                {
                    Log.Information("Disable LNB1 Power");
                    gpio_write(GPIO_LNB1_ENABLE, false);
                    gpio_write(GPIO_LNB1_VSEL, false);

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
