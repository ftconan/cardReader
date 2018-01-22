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
        // dataGridView mMsg
        private DataTable dt = new DataTable();
        private List<TagMsg> mMsg = new List<TagMsg>();
        private Dictionary<int, Temp> mDMsg = new Dictionary<int, Temp>();

        public frmMain()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// 主窗体加载，初始化组件信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_Load(object sender, EventArgs e)
        {
            // 初始化选择框
            //this.cbbComList.SelectedIndex = 0;
            this.cbbBaudRate.SelectedIndex = 7;
            this.cbbDataBits.SelectedIndex = 0;
            this.cbbDataBits.SelectedIndex = 0;
            this.cbbStopBits.SelectedIndex = 0;
            this.cbbDataBits.SelectedIndex = 0;
            this.cbbParity.SelectedIndex = 0;
            // 打开串口获得串口名称
            if (SerialPort.GetPortNames().Length > 0)
            {
                this.cbbComList.Items.AddRange(SerialPort.GetPortNames());
                this.cbbComList.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("请先接入读卡器设备!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }

            // 初始化dataGridView
            DataColumn col1 = new DataColumn("设备ID", typeof(string));
            DataColumn col2 = new DataColumn("激活ID", typeof(string));
            DataColumn col3 = new DataColumn("标签ID", typeof(string));
            DataColumn col4 = new DataColumn("激活Rssi", typeof(string));
            DataColumn col5 = new DataColumn("标签Rssi", typeof(string));
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

        /// <summary>
        /// 打开串口按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        MessageBox.Show("打开串口成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.pictureBox1.BackgroundImage = Properties.Resources.green;
                        this.btnOpen.Text = "关闭串口";
                        // 设置定时器为true
                        dt.Rows.Clear();
                        this.dataGridView1.DataSource = dt.DefaultView;
                        this.timer1.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                this.serialPort1.Close();
                MessageBox.Show("关闭串口成功!", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.pictureBox1.BackgroundImage = Properties.Resources.red;
                this.btnOpen.Text = "打开串口";
                // 设置定时器为false
                mDMsg.Clear();
                dt.Rows.Clear();
                this.COUNT.Text = "0";
                this.dataGridView1.DataSource = dt.DefaultView;
                this.timer1.Enabled = false;
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
            List<Byte> mRecv = new List<Byte>();
            mRecv.AddRange(bRead);
            #region
            while (mRecv.Count > 9)
            {
                if (mRecv[0] == 0x55 && mRecv[1] == 0xAA)
                {
                    int dataLength = (mRecv[3] << 8 | mRecv[2]) + 2;
                    if (mRecv.Count >= dataLength)
                    {
                        byte[] data = new byte[dataLength];
                        mRecv.CopyTo(0, data, 0, dataLength);
                        int nDeviceId = data[5] << 8 | data[4];
                        switch (data[6])
                        {
                            case 0x01:
                                // 接收数据
                                #region " 0x01 "
                                DateTime now = DateTime.Now;
                                int nCount = dataLength / 9 - 1;
                                for (int i = 0; i < nCount; i++)
                                {
                                    int nTagId = data[13 + i * 9] << 16 | data[12 + i * 9] << 8 | data[11 + i * 9];
                                    int nDeviceRssi = data[10 + i * 9] << 8 | data[9 + i * 9];
                                    int nActiveId = data[15 + i * 9] << 8 | data[16 + i * 9];
                                    int nTagRssi = data[17 + i * 9];
                                    // set value 
                                    TagMsg tagMsg = new TagMsg();
                                    tagMsg.ReciveDt = now;
                                    tagMsg.State = data[14 + i * 9];
                                    tagMsg.DeviceId = nDeviceId;
                                    tagMsg.DeviceRssi = nDeviceRssi;
                                    tagMsg.ActiveId = nActiveId;
                                    tagMsg.TagId = nTagId;
                                    tagMsg.TagRssi = nTagRssi;
                                    if (mMsg.Count >= 200)
                                    {
                                        mMsg.RemoveAt(0);
                                    }
                                    // 过滤为0的数据
                                    if (tagMsg.ActiveId != 0)
                                    {
                                        mMsg.Add(tagMsg);
                                    }
                                }
                                #endregion
                                break;
                            case 0x02:
                                // 心跳
                                break;
                            case 0x03:
                                // 配置集中器参数指令
                                break;
                            case 0x04:
                                // 设置控制器参数指令
                                break;
                            case 0x05:
                                // 设置主机出厂指令
                                break;
                            case 0x0B:
                                // 配置基站网关参数指令
                                break;
                            case 0x0C:
                                // 读取基站网关参数指令
                                break;
                            default:
                                break;
                        }
                        mRecv.RemoveRange(0, dataLength);
                        continue;
                    }
                    else
                    {
                        // 数据长度不一样就跳出
                        break;
                    }
                }
                else
                {
                    mRecv.RemoveAt(0);
                }
            } // endwhile
            #endregion
        }

        /// <summary>
        /// 定时器，用来刷新dataGridView数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            int nCount = mMsg.Count();
            while (nCount > 0)
            {
                if (mMsg.Count == 0)
                    break;
                TagMsg tmsg = mMsg[0];
                mMsg.RemoveAt(0);
                TagMsg msg = tmsg;
                // mMsg添加数据到mDMsg
                if (mDMsg.ContainsKey(msg.TagId))
                {
                    mDMsg[msg.TagId].Tagmsg = msg;
                    mDMsg[msg.TagId].Count += 1;
                }
                else
                {
                    mDMsg[msg.TagId] = new Temp(msg);
                }
                nCount--;
            }
            this.Invoke((EventHandler)(delegate
            {
                dt.Rows.Clear();
                int COUNT = 0;
                
                // 遍历mDMsg,把数据添加到dataGridView
                foreach (var item in mDMsg)
                {
                    Temp temp = item.Value;
                    TagMsg msg = temp.Tagmsg;
                    // 过滤标签
                    if (!String.IsNullOrEmpty(this.textBox1.Text))
                    {
                        try
                        {
                            this.TagShow.Text = "显示单个标签";
                            int ntDeviceId = Convert.ToInt32(this.textBox1.Text);

                            if (ntDeviceId == item.Key)
                            {
                                DataRow row = dt.NewRow();
                                row[0] = msg.DeviceId;
                                row[1] = msg.ActiveId;
                                row[2] = msg.TagId;
                                row[3] = msg.TagRssi;
                                row[4] = msg.DeviceRssi;
                                row[5] = msg.State.ToString("X2");
                                row[6] = msg.ReciveDt.ToString("HH:mm:ss");
                                row[7] = temp.Count.ToString();
                                dt.Rows.Add(row);
                                COUNT++;
                            }
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        continue;
                    }
                    else
                    {
                        this.TagShow.Text = "显示所有标签";
                        DataRow row = dt.NewRow();
                        row[0] = msg.DeviceId;
                        row[1] = msg.ActiveId;
                        row[2] = msg.TagId;
                        row[3] = msg.TagRssi;
                        row[4] = msg.DeviceRssi;
                        row[5] = msg.State.ToString("X2");
                        row[6] = msg.ReciveDt.ToString("HH:mm:ss");
                        row[7] = temp.Count.ToString();
                        dt.Rows.Add(row);
                        COUNT++;
                    }
                    continue;
                }
                this.COUNT.Text = COUNT.ToString();
                this.dataGridView1.DataSource = dt.DefaultView;
            }));
        }

        // 清空
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource != null)
            {
                mDMsg.Clear();
                dt.Rows.Clear();
                this.COUNT.Text = "0";
                this.dataGridView1.DataSource = dt.DefaultView;
            }

            else
            {
                dt.Rows.Clear();
            }
        }

        // 向串口写数据
        //private void Write(byte[] data)
        //{
        //    if (this.serialPort1.IsOpen)
        //    {
        //        this.serialPort1.Write(data, 0, data.Length);
        //    }


        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    byte[] data1 = new byte[12];
        //    data1[0] = 0x01;
        //    data1[1] = 0x02;
        //    data1[3] = 0x03;
        //    for (int i = 0; i < 12; i++)
        //    {
        //        data1[i] = (byte)i;
        //    }
        //    Write(data1);
        //}
    }
}
