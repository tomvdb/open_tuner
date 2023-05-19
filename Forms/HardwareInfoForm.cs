using System;
using System.Collections.Generic;
using System.Windows.Forms;
using opentuner.Hardware;

namespace opentuner.Forms
{
    public partial class HardwareInfoForm : Form
    {
        private ftdi ftdi_hw = null;
        List<FTDIDevice> ftdi_devices = null;

        public HardwareInfoForm(ftdi ftdi_hardware)
        {
            InitializeComponent();

            ftdi_hw = ftdi_hardware;
            detect_display();
        }

        private void detect_display()
        {
            listBox1.Items.Clear();

            ftdi_devices = ftdi_hw.detect_all_ftdi();

            if (ftdi_devices.Count == 0)
            {
                MessageBox.Show("No FT2232 Devices Detected");
                return;
            }

            for ( int c = 0; c < ftdi_devices.Count; c++ ) 
            {
                listBox1.Items.Add(ftdi_devices[c].device_serial_number + " - " + ftdi_devices[c].device_description);
            }

            if (listBox1.Items.Count > 0) { listBox1.SelectedIndex = 0; }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;

            int item_index = listBox1.SelectedIndex;

            lblDeviceIndex.Text = ftdi_devices[item_index].device_index.ToString();
            lblDeviceSerialNumber.Text = ftdi_devices[item_index].device_serial_number.ToString();
            lblDeviceDescription.Text = ftdi_devices[item_index].device_description.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            detect_display();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void lblDeviceSerialNumber_Click(object sender, EventArgs e)
        {

        }

        private void lblDeviceIndex_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void lblDeviceDescription_Click(object sender, EventArgs e)
        {

        }
    }
}
