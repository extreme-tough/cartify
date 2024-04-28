using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TestWebView2
{
    public partial class Form1 : Form
    {
        bool pageLoaded = false;
        
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            // "https://www.tesco.com/groceries/en-GB/products/313514046"
            string url = txtURL.Text;
            chromiumWebBrowser1.LoadUrlAsync(url).ContinueWith(x =>
            {
                if (x.IsCompleted)
                {
                    pageLoaded = true;
                }
            });
        }
    }
}
