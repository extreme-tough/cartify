using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Cartify
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void lblLink_Click(object sender, EventArgs e)
        {
            Process.Start(lblLink.Text);   
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            Process.Start("mailto:" +label5.Text);
        }

        private void About_Load(object sender, EventArgs e)
        {
            whatsnew.Text = File.ReadAllText("whatsnew.txt");
        }
    }
}
