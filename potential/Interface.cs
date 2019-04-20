using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HidSharp;
using static potential.RazerDeviceHelper;

namespace potential
{
    public partial class Interface : Form
    {
        public Interface()
        {
            InitializeComponent();
        }

        private IEnumerable<HidDevice> devices;
        private HidDevice selectedDevice;
        private void button1_Click(object sender, EventArgs e)
        {
            comboBox1.Text = "Select a device...";
            comboBox1.Items.Clear();
            devices = RazerDeviceHelper.EnumerateRazerDevices();
            foreach (var hidDevice in devices)
            {
                comboBox1.Items.Add(hidDevice.GetProductName());
            }

            comboBox1.Enabled = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBox1.Enabled = true;
            selectedDevice = devices.Skip(comboBox1.SelectedIndex).First();
            groupBox1.Text = String.Format("{0} ({1:X}:{2:X})", selectedDevice.GetProductName(), selectedDevice.VendorID,
                selectedDevice.ProductID);
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RazerRgb rgb1 = new RazerRgb(button2.BackColor), rgb2 = new RazerRgb(button3.BackColor);
            byte speed = (byte) ((double) numericUpDown1.Value / 100.0 * 255.0);
            switch (comboBox2.SelectedIndex)
            {
                case 0:
                    RazerKeyboard.RazerAttrWriteModeStatic(selectedDevice, rgb1);
                    break;
                case 1:
                    RazerKeyboard.RazerAttrWriteModeReactive(selectedDevice, speed, rgb1);
                    break;
                case 2:
                    RazerKeyboard.RazerAttrWriteModeBreath(selectedDevice, rgb1, rgb2);
                    break;
                case 3:
                    RazerKeyboard.RazerAttrWriteModeWave(selectedDevice, RazerChroma.WAVE_DIRECTION.LEFT);
                    break;
                case 4:
                    RazerKeyboard.RazerAttrWriteModeStarlight(selectedDevice);
                    break;
                case 5:
                    RazerKeyboard.RazerAttrWriteModeStarlight(selectedDevice, rgb1);
                    break;
                case 6:
                    RazerKeyboard.RazerAttrWriteModeStarlight(selectedDevice, rgb1, rgb2);
                    break;
            }
            RazerKeyboard.RazerAttrWriteSetBrightness(selectedDevice, (byte)((double)numericUpDown2.Value / 100.0 * 255.0));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                button2.BackColor = colorDialog1.Color;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                button3.BackColor = colorDialog1.Color;
            }
        }
    }
}
