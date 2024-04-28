using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllImporterPro
{
    public partial class URLList : Form
    {
        public URLList()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {            
            this.Close();
            this.DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
