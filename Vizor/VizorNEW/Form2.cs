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

        private void button2_Click(object sender, EventArgs e)
        {
            var startObj = new CtrKeys.Start();
            var result = startObj.Pysch(6);
            textBox1.Text = result.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var startObjEvent = new CtrEvents.StartForMonitorThread();
            var result = startObjEvent.HandlerEvent(3);
            textBox1.Text = result.ToString();
        }

        private void bExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bShowNewEvents_Click(object sender, EventArgs e)
        {
            var startObjEvent = new CtrEvents.Start();
            var result = startObjEvent.PyschEvent(1);
            textBox1.Text = result.ToString();
        }

        private void bShowAllEvents_Click(object sender, EventArgs e)
        {
            var startObjEvent = new CtrEvents.Start();
            var result = startObjEvent.PyschEvent(2);
            textBox1.Text = result.ToString();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            
        }


    }
}
