using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aiplib;

namespace Cartify
{
    public partial class Settings : Form
    {
        public DataTable catList;
        public ProfileConfig Config;
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            string ocCatName;
            categoryCombo.Items.Add("(Default)");
            foreach (DataRow dRow in catList.Rows)
            {
                ocCatName = dRow[1].ToString().Replace("&gt;", ">").Replace("&nbsp;", " ");
                categoryCombo.Items.Add(ocCatName);
            }
            LoadSettings();
        }

        private void LoadSettings()
        {
            categoryCombo.SelectedIndex = categoryCombo.FindStringExact(Config.IMPORT_TO_CATEGORY);
            txtUsername.Text= Config.USERNAME;
            txtPassword.Text= Config.PASSWORD;
            chkUpdateLoop.Checked = Config.UPDATE_IN_LOOP;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            Config.IMPORT_TO_CATEGORY = categoryCombo.Text;
            Config.USERNAME = txtUsername.Text;
            Config.PASSWORD = txtPassword.Text;
            Config.UPDATE_IN_LOOP = chkUpdateLoop.Checked;
            Config.SaveSettings();
            this.Close();
        }
    }
}
