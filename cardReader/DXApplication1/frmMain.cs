using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DXApplication1
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        public frmMain()
        {
            InitializeComponent();

        }

        public IList<Class1> data = new List<Class1>();

        private void frmMain_Load(object sender, EventArgs e)
        {

            for (int i = 0; i < 10; i++)
            {
                Class1 c = new Class1();
                c.Name = "111";
                c.Age = i + 100;
                c.Height = i + 100000;
                data.Add(c);
            }
            this.gridControl1.DataSource = data;
            this.timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Random r = new Random();
            for (int i = 0; i < 10; i++)
            {
                data[i].Age = r.Next(100000000);
            }
            this.Invoke((EventHandler)delegate
            {
                this.gridControl1.RefreshDataSource();
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //this.gridControl1.ExportToXls();
        }
    }
}
