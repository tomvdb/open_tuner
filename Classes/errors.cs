namespace opentuner
{
    static class errors
    {
        public const byte ERROR_NONE = 0;
        public const byte ERROR_ARGS = 1;
        public const byte ERROR_ARGS_INPUT = 2;
        public const byte ERROR_STATE = 3;
        public const byte ERROR_DEMOD_STATE = 4;
        public const byte ERROR_FTDI_READ_HIGH = 5;
        public const byte ERROR_READ_DEMOD = 6;
        public const byte ERROR_WRITE_DEMOD = 7;
        public const byte ERROR_READ_OTHER = 8;
        public const byte ERROR_WRITE_OTHER = 9;
        public const byte ERROR_FTDI_SYNC_AA = 10;
        public const byte ERROR_FTDI_SYNC_AB = 11;
        public const byte ERROR_NIM_INIT = 12;
        public const byte ERROR_LNA_ID = 13;
        public const byte ERROR_TUNER_ID = 14;
        public const byte ERROR_TUNER_LOCK_TIMEOUT = 15;
        public const byte ERROR_TUNER_CAL_TIMEOUT = 16;
        public const byte ERROR_TUNER_CAL_LOWPASS_TIMEOUT = 17;
        public const byte ERROR_I2C_NO_ACK = 18;
        public const byte ERROR_FTDI_I2C_WRITE_LEN = 19;
        public const byte ERROR_FTDI_I2C_READ_LEN = 20;
        public const byte ERROR_MPSSE = 21;
        public const byte ERROR_DEMOD_INIT = 22;
        public const byte ERROR_BAD_DEMOD_HUNT_STATE = 23;
        public const byte ERROR_TS_FIFO_WRITE = 24;
        public const byte ERROR_OPEN_TS_FIFO = 25;
        public const byte ERROR_TS_FIFO_CREATE = 26;
        public const byte ERROR_STATUS_FIFO_CREATE = 27;
        public const byte ERROR_OPEN_STATUS_FIFO = 28;
        public const byte ERROR_TS_FIFO_CLOSE = 29;
        public const byte ERROR_STATUS_FIFO_CLOSE = 30;
        public const byte ERROR_USB_TS_READ = 31;
        public const byte ERROR_LNA_AGC_TIMEOUT = 32;
        public const byte ERROR_DEMOD_PLL_TIMEOUT = 33;
        public const byte ERROR_FTDI_USB_DEVICE_LIST = 34;
        public const byte ERROR_FTDI_USB_BAD_DEVICE_NUM = 35;
        public const byte ERROR_FTDI_USB_DEVICE_NUM_OPEN = 36;
        public const byte ERROR_UDP_WRITE = 37;
        public const byte ERROR_UDP_SOCKET_OPEN = 38;
        public const byte ERROR_UDP_CLOSE = 39;
        public const byte ERROR_VITERBI_PUNCTURE_RATE = 40;
        public const byte ERROR_TS_BUFFER_MALLOC = 41;
        public const byte ERROR_THREAD_ERROR = 41;
    }

}
