using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using HAP = HtmlAgilityPack;
using ReadWriteCsv;

namespace AllImporterPro
{
    public partial class Form1 : Form
    {
        string[] categoryURLs,productURLs ;
        HAP.HtmlDocument doc;
        WebClient client;
        string filename;
        int StartPage, startItem;
        bool Resume = false;
        Parser siteParser = new Parser();
        public string DIR_IMAGE = Application.StartupPath + @"/images/";
        string[] lastImportPointer;
        int currentRow;
        CsvFileWriter writer;
        public Form1()
        {            
            InitializeComponent();
            client = new System.Net.WebClient();
            doc = new HAP.HtmlDocument();
            filename = System.IO.Path.GetTempFileName();            
        }

        private void importType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowOptions();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            importType.SelectedIndex = 0;
            if (File.Exists("cat_urllist.data"))
            {
                categoryURLs = File.ReadAllLines("cat_urllist.data");
            }
            if (File.Exists("prod_urllist.data"))
            {
                productURLs = File.ReadAllLines("prod_urllist.data");
            }
        }

        private void ShowOptions()
        {
            URLList frmURLlist;
            switch (importType.SelectedIndex)
            {
                case 0:
                    ImportOptButton.Enabled = false;
                    break;
                case 1:
                    ImportOptButton.Enabled = true;
                    frmURLlist = new URLList();                    
                    frmURLlist.Urls.Lines = categoryURLs;
                    if (frmURLlist.ShowDialog() == DialogResult.OK)
                    {
                        categoryURLs= frmURLlist.Urls.Lines;
                        File.WriteAllLines("cat_urllist.data", categoryURLs);
                    }

                    break;
                case 2:
                    ImportOptButton.Enabled = true;
                    frmURLlist = new URLList();
                    frmURLlist.Urls.Lines = productURLs;
                    if (frmURLlist.ShowDialog() == DialogResult.OK)
                    {
                        productURLs = frmURLlist.Urls.Lines;
                        File.WriteAllLines("prod_urllist.data", productURLs);
                    }
                    break;
            }
        }

        private void ImportOptButton_Click(object sender, EventArgs e)
        {
            ShowOptions();
        }

        private void Start_Click(object sender, EventArgs e)
        {
            ClearLog();
            writer = new CsvFileWriter("dear-lover.csv");
            Log("Starting import",true);
            if (File.Exists("last.data"))
            {
                Resume = true;
                string lastData = File.ReadAllText("last.data");
                lastImportPointer = lastData.Split(new string[] { "," }, StringSplitOptions.None);
                StartPage = int.Parse(lastImportPointer[1]);
                startItem = int.Parse(lastImportPointer[2]);
            }
            else
            {
                File.WriteAllText("images.txt", "");
                CsvRow row = new CsvRow();                
                row.Add("Name");
                row.Add("Category");
                row.Add("Item No");
                row.Add("Weight About");
                row.Add("Volumetric Weight About");
                row.Add("Price");
                row.Add("Save");
                row.Add("Material");
                row.Add("Size");
                row.Add("Quantity ALL");
                row.Add("Quantity");
                row.Add("Product link");
                row.Add("Photos download link");
                writer.WriteRow(row);
            }
            Parser siteParser = new Parser();
            switch (importType.SelectedIndex){
                case 0:
                    categoryURLs = siteParser.getCategoryURLs().ToArray();
                    break;
                case 2:
                    foreach(string itemLink in productURLs)
                        processProduct(itemLink);
                    writer.Close();
                    File.Delete("last.data");
                    MessageBox.Show("Import is finished", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information);  
                    return;
            }
            foreach (string categoryURL in categoryURLs)
            {
                if (Resume)
                    if (categoryURL != lastImportPointer[0])
                        continue;                
                Application.DoEvents();
                processCategory(categoryURL);
                Resume = false;
                StartPage = 0;
                startItem = 0;
            }
            File.Delete("last.data");
            writer.Close();
            MessageBox.Show("Import is finished", "Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void processCategory(string catLink)
        {
            Log("Processing Category - " + catLink,true);
            Parser siteParser = new Parser();
            int page ;
            if (StartPage > 0)
                page = StartPage;
            else
                page = ImportConfig.START_PAGE;
            string catPageURL;
            List<string> itemLinks;
            int itemCount = 0;
            while (true)
            {
                itemCount = 0;
                Application.DoEvents();
                catPageURL = siteParser.buildCategoryURL(catLink, page);
                Log("Category page  - " + catPageURL ,true);
                try
                {
                    client.DownloadFile(catPageURL, filename);
                }
                catch (WebException ex)
                {
                    if (ex.Message.Contains("404"))
                    {
                        Log("ERROR: 404 found", true);
                        break;
                    }
                    Log("ERROR: " + ex.Message, true);
                }
                doc.Load(filename);
                var root = doc.DocumentNode;
                siteParser.root = root;
                itemLinks = siteParser.getItemURLs(page);
                Log("Item links found " + itemLinks.ToString(), true);
                Log("StartItem " + startItem.ToString(), true);
                if (itemLinks.Count > 0)
                {
                    foreach (string itemLink in itemLinks)
                    {
                        if (startItem > 0)
                        {
                            if (itemCount < startItem)
                            {
							    itemCount++;
							    continue;
						    }
                        }
                        Application.DoEvents();
                        processProduct(itemLink);
                        startItem = 0;
                        itemCount++;
                        File.WriteAllText("last.data", catLink + "," + page.ToString() + "," + itemCount);
                    }
                }
                else
                    break;                
                page += ImportConfig.PAGE_INCREMENTER;
            }
        }

        private void processProduct(string itemLink)
        {
            

            Log("Processing product - " + itemLink,true);
            while (true)
            {

                try
                {
                    client.DownloadFile(itemLink, filename);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("remote name")) return;
                    Log("Error : " + ex.Message + "...Trying again");
                    continue;
                }
                break;
            }
            doc.Load(filename);
            var root = doc.DocumentNode;
            siteParser.root = root;
            siteParser.URL = itemLink;

            string Title = siteParser.getTitle();
            Log("Import data >>>>>");
            Log("Title  : " + Title);
            string Category = siteParser.getCategoryPath();
            Log("Category  : " + Category);
            string Description = siteParser.getDescription();
            //Log("Description  : " + Description);
            
            string Model = siteParser.getModel();
            Log("Model  : " + Model);
            string Weight = siteParser.getWeight();
            Log("Weight  : " + Weight);
            string Volume = siteParser.getVolume();
            Log("Volume  : " + Volume);
            string Price = siteParser.getPrice();
            Log("Price  : " + Price);
            string Save = siteParser.getSave();
            Log("Save  : " + Save);
            string Material = siteParser.getMaterial();
            Log("Material  : " + Material);
            string Size = siteParser.getSize();
            Log("Size  : " + Size);
            List<string[]> Stock = siteParser.getStock();
            //Log("Stock  : " + Stock);
            string Download = siteParser.getDownloadLink();
            Log("Download Link  : " + Download);
            int TotalStock = 0;
            foreach (string[] stockItem in Stock)
            {
                TotalStock += int.Parse(stockItem[1]);
            }            
            foreach(string[] stockItem in Stock) 
            {
                CsvRow row = new CsvRow();
                row.Add(Title);
                row.Add( Category.Trim());
                row.Add(Model.Trim());
                row.Add(Weight.Trim());
                row.Add(Volume.Trim());
                row.Add(Price.Trim());
                row.Add(Save.Trim());
                row.Add(Material.Trim());
                row.Add(stockItem[0].Trim());
                row.Add(TotalStock.ToString());
                row.Add(stockItem[1].Trim());
                row.Add(itemLink);
                row.Add(Download);
                writer.WriteRow(row);
            }            
            currentRow++;
            Log("Product Added on row : "  + currentRow,true);
        }

        void Log(string message,bool toFile = false)
        {
            if (Result.Lines.Length > 500)
                Result.Clear();
            Result.AppendText(DateTime.Now + "  : " + message + "\n");
            if (toFile)
                File.AppendAllText("Log.txt", DateTime.Now + "  : " + message + "\n");
        }

        void ClearLog()
        {
            File.WriteAllText("Log.txt", "");
        }


        
        void save_image_as(string img,string fullpath)
        {
	        if (File.Exists(fullpath))
            {
		        return;
	        }
	        WebClient webClient = new WebClient();
            webClient.DownloadFile(img, fullpath);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void downloadImages_Click(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines("images.txt");
            string[] lineParts;
            foreach (string line in lines)
            {                
                lineParts = line.Split(new string[] { "," }, StringSplitOptions.None);
                Log("Saving " + lineParts[0] + "\n");
                string path =  Path.GetDirectoryName(lineParts[1]);
                Directory.CreateDirectory(DIR_IMAGE + path);
                save_image_as(lineParts[0], DIR_IMAGE + lineParts[1]);
            }
        }
    }
}
