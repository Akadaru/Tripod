using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VizorNEW
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var startObj = new CtrKeys.Start();
            var result = startObj.Pysch(1);
            textBox1.Text = result.ToString();
        }
    }
}
