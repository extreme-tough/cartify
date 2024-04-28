using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;

using Cells = SourceGrid2.Cells.Real;

namespace Cartify
{
	/// <summary>
	/// Summary description for frmSampleGrid1.
	/// </summary>
	public class frmCatMap : System.Windows.Forms.Form
    {
		private System.Windows.Forms.Panel panel1;
        private SourceGrid2.Grid grid1;
        private System.Windows.Forms.Label label1;
        public DataTable catList;
        private Button btnOK;
        public string ProfileFolder;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        private Dictionary<string, string> catMapping ;
		public frmCatMap()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCatMap));
            this.panel1 = new System.Windows.Forms.Panel();
            this.grid1 = new SourceGrid2.Grid();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.grid1);
            this.panel1.Location = new System.Drawing.Point(8, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(748, 344);
            this.panel1.TabIndex = 3;
            // 
            // grid1
            // 
            this.grid1.AutoSizeMinHeight = 20;
            this.grid1.AutoSizeMinWidth = 20;
            this.grid1.AutoStretchColumnsToFitWidth = false;
            this.grid1.AutoStretchRowsToFitHeight = false;
            this.grid1.BackColor = System.Drawing.Color.White;
            this.grid1.ContextMenuStyle = ((SourceGrid2.ContextMenuStyle)(((((SourceGrid2.ContextMenuStyle.ColumnResize | SourceGrid2.ContextMenuStyle.RowResize) 
            | SourceGrid2.ContextMenuStyle.AutoSize) 
            | SourceGrid2.ContextMenuStyle.ClearSelection) 
            | SourceGrid2.ContextMenuStyle.CopyPasteSelection)));
            this.grid1.CustomSort = false;
            this.grid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid1.FocusStyle = SourceGrid2.FocusStyle.None;
            this.grid1.GridToolTipActive = true;
            this.grid1.Location = new System.Drawing.Point(0, 0);
            this.grid1.Name = "grid1";
            this.grid1.Size = new System.Drawing.Size(746, 342);
            this.grid1.SpecialKeys = ((SourceGrid2.GridSpecialKeys)(((((((((SourceGrid2.GridSpecialKeys.Ctrl_C | SourceGrid2.GridSpecialKeys.Ctrl_V) 
            | SourceGrid2.GridSpecialKeys.Ctrl_X) 
            | SourceGrid2.GridSpecialKeys.Delete) 
            | SourceGrid2.GridSpecialKeys.Arrows) 
            | SourceGrid2.GridSpecialKeys.Tab) 
            | SourceGrid2.GridSpecialKeys.PageDownUp) 
            | SourceGrid2.GridSpecialKeys.Enter) 
            | SourceGrid2.GridSpecialKeys.Escape)));
            this.grid1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Location = new System.Drawing.Point(8, 368);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 23);
            this.label1.TabIndex = 6;
            this.label1.Text = "Click column header to sort the grid";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(656, 361);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(99, 28);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // frmCatMap
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(764, 395);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCatMap";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Category Mapping";
            this.Load += new System.EventHandler(this.frmSampleGrid1_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private SourceGrid2.DataModels.IDataModel m_CellEditor_Id;
		private SourceGrid2.DataModels.IDataModel m_CellEditor_Name;

		private void frmSampleGrid1_Load(object sender, System.EventArgs e)
		{
            loadCatMapFile();
            

            grid1.RowsCount = 1;
            grid1.ColumnsCount = 3;
            grid1.FixedRows = 1;
            grid1.FixedColumns = 1;
            grid1.Selection.SelectionMode = SourceGrid2.GridSelectionMode.Row;
            grid1.AutoStretchColumnsToFitWidth = true;
            grid1.Columns[0].AutoSizeMode = SourceGrid2.AutoSizeMode.None;
            grid1.Columns[0].Width = 30;
            grid1.Columns[1].AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize;

            #region Create Header Row and Editor
            Cells.Header l_00Header = new Cells.Header();
            grid1[0, 0] = l_00Header;

            m_CellEditor_Id = SourceGrid2.Utility.CreateDataModel(typeof(string));
            m_CellEditor_Id.EnableEdit = false;
            //m_CellEditor_Id.EditableMode = SourceGrid2.EditableMode.Focus | SourceGrid2.EditableMode.AnyKey | SourceGrid2.EditableMode.SingleClick;
            grid1[0, 1] = new Cells.ColumnHeader("Opencart Category Path");

            m_CellEditor_Name = SourceGrid2.Utility.CreateDataModel(typeof(string));
            m_CellEditor_Name.EditableMode = SourceGrid2.EditableMode.Focus | SourceGrid2.EditableMode.AnyKey | SourceGrid2.EditableMode.SingleClick;
            grid1[0, 2] = new Cells.ColumnHeader("Source Category Path");


            #endregion


            #region Visual Properties
            //set Cells style
            m_VisualProperties = new SourceGrid2.VisualModels.Common(false);

            m_VisualPropertiesPrice = (SourceGrid2.VisualModels.Common)m_VisualProperties.Clone(false);
            m_VisualPropertiesPrice.TextAlignment = ContentAlignment.MiddleRight;

            m_VisualPropertiesCheckBox = (SourceGrid2.VisualModels.CheckBox)SourceGrid2.VisualModels.CheckBox.Default.Clone(false);

            m_VisualPropertiesLink = (SourceGrid2.VisualModels.Common)SourceGrid2.VisualModels.Common.LinkStyle.Clone(false);
            #endregion

            int l_RowsCount = 1;
            foreach (DataRow dRow in catList.Rows)
            {
                #region Pupulate RowsCount
                grid1.Rows.Insert(grid1.RowsCount, new Cells.RowHeader(),
                new Cells.Cell(grid1.RowsCount, m_CellEditor_Id, m_VisualProperties),
                new Cells.Cell(m_CellEditor_Name.DefaultValue, m_CellEditor_Name, m_VisualProperties));
                grid1[l_RowsCount, 0] = new Cells.RowHeader();
                string ocCatName = dRow[1].ToString().Replace("&gt;", ">").Replace("&nbsp;", " ");
                grid1[l_RowsCount, 1] = new Cells.Cell(ocCatName);
                grid1[l_RowsCount, 1].DataModel = m_CellEditor_Id;
                grid1[l_RowsCount, 1].VisualModel = m_VisualProperties;

                if (catMapping.ContainsKey(ocCatName))
                    grid1[l_RowsCount, 2] = new Cells.Cell(catMapping[ocCatName]);
                else
                    grid1[l_RowsCount, 2] = new Cells.Cell("");
                
                grid1[l_RowsCount, 2].DataModel = m_CellEditor_Name;
                grid1[l_RowsCount, 2].VisualModel = m_VisualProperties;

               
                #endregion

                l_RowsCount++;
            }

            grid1.AutoSizeAll();
		}

		private SourceGrid2.VisualModels.Common m_VisualProperties;
		private SourceGrid2.VisualModels.Common m_VisualPropertiesPrice;
		private SourceGrid2.VisualModels.Common m_VisualPropertiesCheckBox;
		private SourceGrid2.VisualModels.Common m_VisualPropertiesLink;


        private void loadCatMapFile()
        {
            catMapping = new Dictionary<string, string>();
            if (File.Exists(ProfileFolder + @"\catmap.txt"))
            {
                string[] lines = File.ReadAllLines(ProfileFolder + @"\catmap.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split(new string[] { "¬" }, StringSplitOptions.None);
                    catMapping.Add(parts[0], parts[1]);
                }
            }
        }

		private void CellLink_Click(object sender, SourceGrid2.PositionEventArgs e)
		{
			try
			{
				SourceLibrary.Utility.Shell.OpenFile( ((Cells.Link)sender).Value.ToString());
			}
			catch(Exception)
			{
			}
		}

        private void btnOK_Click(object sender, EventArgs e)
        {
            File.WriteAllText(ProfileFolder + @"\catmap.txt","");
            string OCCategory, sourceCategory;
            for( int i=1 ; i< grid1.RowsCount-1; i++)
            {
                OCCategory = grid1[i,1].Value.ToString();
                if (grid1[i, 2].Value != null)
                    sourceCategory = grid1[i, 2].Value.ToString();
                else
                    sourceCategory = "";
                File.AppendAllText(ProfileFolder + @"\catmap.txt", OCCategory + "¬" + sourceCategory + "\n");
            }
            this.Close();
        }


	}
}
