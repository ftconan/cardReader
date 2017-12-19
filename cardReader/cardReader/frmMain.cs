using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace cardReader
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.cbbComList.SelectedIndex = 0;
            this.cbbBaudRate.SelectedIndex = 5;
            this.cbbDataBits.SelectedIndex = 0;
            this.cbbDataBits.SelectedIndex = 0;
            this.cbbStopBits.SelectedIndex = 0;
            this.cbbDataBits.SelectedIndex = 0;
            this.cbbParity.SelectedIndex = 0;
            this.cbbTagShow.SelectedIndex = 0;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            // 判断串口是否打开
            if (!this.serialPort1.IsOpen)
            {
                this.serialPort1.PortName = this.cbbComList.SelectedItem.ToString();
                this.serialPort1.BaudRate = Convert.ToInt32(this.cbbBaudRate.SelectedItem.ToString());
                if (this.cbbDataBits.SelectedIndex != 0)
                {
                    this.serialPort1.DataBits = Convert.ToInt32(this.cbbDataBits.SelectedItem.ToString());
                }
                if (this.cbbDataBits.SelectedIndex != 0)
                {
                    this.serialPort1.StopBits = (StopBits)Convert.ToInt32(this.cbbStopBits.SelectedItem.ToString());
                }
                if (this.cbbParity.SelectedIndex != 0)
                {
                    this.serialPort1.Parity = (Parity)Convert.ToInt32(this.cbbParity.SelectedItem.ToString());
                }
                // 打开串口
                try
                {
                    this.serialPort1.Open();
                    if (this.serialPort1.IsOpen)
                    {
                        MessageBox.Show("打开串口成功!");
                        this.pictureBox1.BackgroundImage = Properties.Resources.green;
                        this.btnOpen.Text = "关闭串口";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("打开串口失败,请检查串口名称!");
                }
            }
            else
            {
                this.serialPort1.Close();
                MessageBox.Show("关闭串口成功!");
                this.pictureBox1.BackgroundImage = Properties.Resources.red;
                this.btnOpen.Text = "打开串口";
            }
        }
    }
}
