using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace VK_get
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();

           
            
            if (f.t.ThreadState == System.Threading.ThreadState.Running)
            {
                f.t.Abort();
            }

            
            //this.Visible = false;
        }
    }
}
