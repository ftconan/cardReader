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
        private DataTable dt = new DataTable();

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // 初始化选择框
            this.cbbComList.SelectedIndex = 0;
            this.cbbBaudRate.SelectedIndex = 5;
            this.cbbDataBits.SelectedIndex = 0;
            this.cbbDataBits.SelectedIndex = 0;
            this.cbbStopBits.SelectedIndex = 0;
            this.cbbDataBits.SelectedIndex = 0;
            this.cbbParity.SelectedIndex = 0;
            this.cbbTagShow.SelectedIndex = 0;
            // 初始化dataGridView
            DataColumn col1 = new DataColumn("设备ID", typeof(string));
            DataColumn col2 = new DataColumn("激活ID", typeof(string));
            DataColumn col3 = new DataColumn("标签ID", typeof(string));
            DataColumn col4 = new DataColumn("标签Rssi", typeof(string));
            DataColumn col5 = new DataColumn("激活Rssi", typeof(string));
            DataColumn col6 = new DataColumn("状态", typeof(string));
            DataColumn col7 = new DataColumn("时间", typeof(string));
            DataColumn col8 = new DataColumn("次数", typeof(string));
            dt.Columns.Add(col1);
            dt.Columns.Add(col2);
            dt.Columns.Add(col3);
            dt.Columns.Add(col4);
            dt.Columns.Add(col5);
            dt.Columns.Add(col6);
            dt.Columns.Add(col7);
            dt.Columns.Add(col8);
            this.dataGridView1.DataSource = dt.DefaultView;
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

        /// <summary>
        /// 接收数据事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int nReadBytes = this.serialPort1.BytesToRead;
            if (nReadBytes == 0)
                return;

            byte[] bRead = new byte[nReadBytes];
            this.serialPort1.Read(bRead, 0, nReadBytes);
            // 已经读取完成
            Console.WriteLine(bRead.Length);
            for (int i = 0;  i < bRead.Length;  i++)
            {
                Console.Write(i);
            }
            if (bRead.Length > 9)
            {
                if (bRead[0] == 0x55 && bRead[1] == 0xAA)
                {

                }
            }
        }
    }
}
