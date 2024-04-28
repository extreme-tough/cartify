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
    public partial class CategorySelect : Form
    {
        public DataTable catList;
        public String SelectedCategories;
        public CategorySelect()
        {
            InitializeComponent();
        }

        private void CategorySelect_Load(object sender, EventArgs e)
        {
            string[] checkedCats = SelectedCategories.Split(new string[] { "," },StringSplitOptions.None);
            TargetCategory cat = new TargetCategory();
            foreach (DataRow dRow in catList.Rows)
            {
                cat = new TargetCategory();
                cat.CategoryID = dRow[0].ToString();                
                string ocCatName = dRow[1].ToString().Replace("&gt;", ">").Replace("&nbsp;", " ");
                cat.CategoryPath = ocCatName;
                ListViewItem lvCat;
                lvCat = lstCategories.Items.Add(ocCatName);
                lvCat.Tag = cat.CategoryID;
                if (checkedCats.Contains<string>(cat.CategoryID)){
                    lvCat.Checked = true;
                }
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            string[] selectedIDs = new string[lstCategories.CheckedItems.Count];
            int i = 0;
            foreach (ListViewItem item in lstCategories.CheckedItems)
            { 
                selectedIDs[i] = item.Tag.ToString();
                i++;
            }
            SelectedCategories = string.Join(",", selectedIDs);
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
