using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.Win32;

namespace Cartify
{
    public partial class frmProductView : Form
    {
        public string selectedProfileText;
        public string connectionString;
        public string oc_version;

        DbWrapper opencartDB;
        public frmProductView(string version)
        {
            InitializeComponent();
            oc_version = version;
        }

        private void frmProductView_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            productsView.Columns[0].Width =(int) (productsView.Width * 0.07);
            productsView.Columns[1].Width = (int)(productsView.Width * 0.40);
            productsView.Columns[2].Width = (int)(productsView.Width * 0.15);
            productsView.Columns[3].Width = (int)(productsView.Width * 0.10);
            productsView.Columns[4].Width = (int)(productsView.Width * 0.10);
            productsView.Columns[5].Width = (int)(productsView.Width * 0.10);

            

            opencartDB = new Opencart();
            opencartDB.Open(connectionString);
            opencartDB.Version = oc_version;
            DataTable products = opencartDB.getImportedProducts("","","","","","","", selectedProfileText, "");
            FillView(products);
            DataTable catList = opencartDB.getCategoriesProper();
           
            cboCategory.DataSource = catList;
            cboCategory.DisplayMember = "name";
            cboCategory.ValueMember = "category_id";
            cboCategory.SelectedIndex = -1;
            

            cboBrand.DataSource = opencartDB.getManufacturers();
            cboBrand.DisplayMember = "name";
            cboBrand.ValueMember= "manufacturer_id";
            cboBrand.SelectedIndex = -1;

            Cursor.Current = Cursors.Default;
        }

        private void productsView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cmdFilter_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            string status="";
            switch (cboStatus.Text)
            {
                case "":
                    status = "";break;
                case "Enabled":
                    status = "1";break;
                case "Disabled":
                    status = "0";break;
            }
            DataTable products = opencartDB.getImportedProducts(txtName.Text,txtModel.Text,
                txtPrice.Text,txtQuantity.Text,status, 
                cboCategory.SelectedValue == null ?"" : cboCategory.SelectedValue.ToString(),
                cboBrand.SelectedValue == null ? "" : cboBrand.SelectedValue.ToString(), selectedProfileText, ""
                );
            FillView(products);
            Cursor.Current = Cursors.Default;
        }

        private void FillView(DataTable products)
        {
            productsView.Items.Clear();
            foreach (DataRow product in products.Rows)
            {
                ListViewItem newItem = productsView.Items.Add(product["product_id"].ToString());
                newItem.SubItems.Add(product["name"].ToString());
                newItem.SubItems.Add(product["model"].ToString());
                newItem.SubItems.Add(product["price"].ToString());
                newItem.SubItems.Add(product["quantity"].ToString());
                if (product["status"].ToString() == "True")
                    newItem.SubItems.Add("Enabled");
                else
                    newItem.SubItems.Add("Disabled");
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            DialogResult ques= MessageBox.Show("Are you sure to delete the selected products?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            int delCount, TotalImportsReg;
            if (ques == DialogResult.Yes)
            {
                foreach(ListViewItem item in productsView.CheckedItems)
                {
                    opencartDB.DeleteProduct(int.Parse(item.Text));
                }
                delCount = productsView.CheckedItems.Count;
                foreach (ListViewItem eachItem in productsView.CheckedItems)
                {
                    productsView.Items.Remove(eachItem);
                }
                RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Cartify\" + selectedProfileText);
                int.TryParse(regkey.GetValue("TotalImports").ToString(), out TotalImportsReg);
                TotalImportsReg = TotalImportsReg - delCount;
                regkey.Close();
                regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Cartify\" + selectedProfileText);
                regkey.SetValue("TotalImports", TotalImportsReg.ToString());
                regkey.Close();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtModel.Text = "";
            txtName.Text = "";
            txtPrice.Text = "";
            txtQuantity.Text = "";
            cboBrand.SelectedIndex = -1;
            cboBrand.Text = "";
            cboCategory.SelectedIndex = -1;
            cboCategory.Text = "";
            cboStatus.SelectedIndex = -1;
        }
    }
}
