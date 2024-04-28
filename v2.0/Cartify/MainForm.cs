using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using Forms = System.Windows.Forms;
using System.Net;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Diagnostics;
using System.Configuration;
using System.Threading;
using System.Data.OleDb;
using ParserFactory;
using Aiplib;
using HAP = HtmlAgilityPack;
using ExcelLibrary.SpreadSheet;
using ExcelLibrary.CompoundDocumentFormat;
using Newtonsoft.Json;
// using Com.StellmanGreene.CSVReader;
using LumenWorks.Framework.IO.Csv;
using Microsoft.Win32;
using CefSharp.WinForms;
using System.Windows.Forms;
using CefSharp;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Runtime.InteropServices;

namespace Cartify

{
    public partial class Form1 : Forms.Form
    {
        string[] categoryURLs, productURLs;
        HAP.HtmlDocument doc;
        string chromiumResult = "";
        bool chromiumLoading = false;
        string filename;
        
        string store_url;
        int StartPage, startItem;
        bool isStopped = false;
        bool Resume = false,isLoading;
        Parser siteParser;
        ProfileConfig config;
        int StockMulitplier;
        public string DIR_IMAGE, img_path, oc_version, lastCategoryPath ,subFolder;
        public bool IsAuto = false;
        public string DebugLog;
        DbWrapper targetDB;
        StreamWriter DebugWriter;
        List<string> additionalItems;
        string[] lastImportPointer;
        string selectedProfileFolder,selectedProfileAssembly, selectedProfileConfig;
        System.Data.DataTable catList,attList;
        delegate void SetTextCallback(string text,bool NewLine);
        delegate void SetDebugCallback(string text);
        Process process;
        string selectedProfileText;
        Thread[] newThreads = new Thread[2];
        string FeedFilePath, FeedFileName;
        public string AutoProfile;
        public string AutoType;
        public string AutoSource;
        public string ShowForm;
        const string userRoot = "HKEY_LOCAL_MACHINE";
        string cachedMessage = "";
        int max_pro_category;
        int totProdCat;
        string verbose;
        bool simulate=false;
        bool ContinueImport = true;
        bool lastRow = false;
        string[] Languages;
        int TotalImportsNow = 0,TotalImportsDB=0,TotalImports=0;
        string SelectedCategories="";
        public Form1()
        {
            InitializeComponent();            
            doc = new HAP.HtmlDocument();
            filename = System.IO.Path.GetTempFileName();
            img_path = ConfigurationManager.AppSettings["IMAGE_PATH"];
            oc_version = ConfigurationManager.AppSettings["OC_VERSION"];
            verbose = ConfigurationManager.AppSettings["Verbose_Log"];
            additionalItems = new List<string>();
            if (ConfigurationManager.AppSettings["Simulate"] == "1")
                simulate = true;
            max_pro_category = int.Parse( ConfigurationManager.AppSettings["MaxProductPerCategory"]);
        }

        private void importType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!isLoading)
              //  ShowOptions();
            if (importType.SelectedIndex == 3)
            {
                FeedFileName = Functions.GetRegValue(selectedProfileText, "feedfile");
                lblFeedFile.Text = FeedFileName;
            }
            else
            {
                lblFeedFile.Text = "(None)";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION
            //270f
            string filename;
            isLoading = true;
            webBrowser.ScriptErrorsSuppressed = true;
            chromium.Dock = DockStyle.Fill;
            webBrowser.Dock = DockStyle.Fill;
            Result.Dock = DockStyle.Fill;
            Result.BringToFront();

            if (IsAuto && ShowForm=="1")
                this.Show();
            //Forms.MessageBox.Show("Trying to open db");
            
            //Forms.MessageBox.Show("Open success");
            
            LoadProfiles();            
            isLoading = false;
            if (IsAuto)
            {
                cboProfiles.SelectedIndex = int.Parse(AutoProfile);
                importType.SelectedIndex = int.Parse(AutoType);
                Uri uriResult;
                bool isURL = Uri.TryCreate(AutoSource, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                if (isURL)
                    filename = DownloadFile(AutoSource);
                else
                    filename = AutoSource;
                FeedFileName = filename;
                Start_Click(this.Start, e);
                UploadButton_Click(this.UploadButton, e);
                Forms.Application.Exit();
                if (File.Exists(filename)) File.Delete(filename);
            }
            else
            {
                string a = Properties.Settings.Default["LastProfile"].ToString();
                cboProfiles.SelectedIndex = int.Parse(a);
            }

            
        }

        private void ShowOptions()
        {
            string subkey = "Cartify";
            string keyName = userRoot + "\\" + subkey;
            switch (importType.SelectedIndex)
            {
                case 0:
                    ImportOptButton.Enabled = false;
                    Start.Enabled = true;
                    buttonStop.Enabled = false;
                    break;
                case 1:
                    ImportOptButton.Enabled = true;
                    URLList frmURLlist = new URLList();                    
                    frmURLlist.Urls.Lines = categoryURLs;
                    if (frmURLlist.ShowDialog() == Forms.DialogResult.OK)
                    {
                        categoryURLs= frmURLlist.Urls.Lines;
                        File.WriteAllLines(selectedProfileFolder + "cat_urllist.data", categoryURLs);
                    }
                    Start.Enabled = true;
                    buttonStop.Enabled = false;
                    break;
                case 2:
                    ImportOptButton.Enabled = true;
                    frmURLlist = new URLList();
                    frmURLlist.Urls.Lines = productURLs;
                    if (frmURLlist.ShowDialog() == Forms.DialogResult.OK)
                    {
                        productURLs = frmURLlist.Urls.Lines;
                        File.WriteAllLines(selectedProfileFolder + "prod_urllist.data", productURLs);                        
                    }
                    Start.Enabled = true;
                    buttonStop.Enabled = false;
                    break;
                case 3:
                    Start.Enabled = false;
                    buttonStop.Enabled = true;
                    ImportOptButton.Enabled = true;

                    //FeedFilePath = (string)Registry.LocalMachine.GetValue("FeedFilePath", selectedProfileFolder);

                    openFileDialog.InitialDirectory = FeedFilePath;
                    openFileDialog.FileName = FeedFileName;
                    
                    if (openFileDialog.ShowDialog() == Forms.DialogResult.OK)
                    {
                        string dir = Path.GetDirectoryName(openFileDialog.FileName);
                        string filename = Path.GetFileName(openFileDialog.FileName);
                        //Registry.LocalMachine.SetValue("FeedFilePath", dir,RegistryValueKind.String);
                        //Registry.LocalMachine.SetValue("FeedFileName", filename, RegistryValueKind.String);
                        FeedFileName = openFileDialog.FileName;
                        Functions.SetRegValue(selectedProfileFolder, "feedfile", openFileDialog.FileName);
                        lblFeedFile.Text = openFileDialog.FileName;
                    }
                    
                    lblFeedFile.Text = FeedFileName;
                    Start.Enabled = true;
                    buttonStop.Enabled = false;
                    break;
                case 4:
                    //return; 
                    CategorySelect frmCatSelect = new CategorySelect();
                    catList = targetDB.getCategories();
                    frmCatSelect.catList = catList;
                    frmCatSelect.SelectedCategories = SelectedCategories;
                    frmCatSelect.ShowDialog();
                    SelectedCategories = frmCatSelect.SelectedCategories;
                    break;
            }
        }

        private void ImportOptButton_Click(object sender, EventArgs e)
        {
            ShowOptions();
        }

        private bool LoginToSite(string username,string password)
        {
            webBrowser.Navigate(config.LOGIN_URL);
            while (webBrowser.ReadyState != Forms.WebBrowserReadyState.Complete)
            {
                Forms.Application.DoEvents();
            }
            if (webBrowser.Document.Url.AbsoluteUri == config.LOGGEDIN_URL)
                return true;

            Forms.HtmlElement userElement=null,passwordElement; 
            try
            {
                if (config.USER_INPUT_METHOD == "NAME")
                    userElement = webBrowser.Document.GetElementsByTagName("input").GetElementsByName(config.USER_INPUT_NAME)[0];
                else
                    userElement = webBrowser.Document.GetElementsByTagName("input")[config.PASSWORD_INPUT_INDEX];
                if (config.PASSWORD_INPUT_METHOD == "NAME")
                    passwordElement = webBrowser.Document.GetElementsByTagName("input").GetElementsByName(config.PASSWORD_INPUT_NAME)[0];
                else
                    passwordElement = webBrowser.Document.GetElementsByTagName("input")[config.PASSWORD_INPUT_INDEX];
            }
            catch
            {
                //Field not present could indicate the login is already done
                return true;
            }


            userElement.SetAttribute("value", username);
            Forms.Application.DoEvents();
            passwordElement.SetAttribute("value", password);
            Forms.Application.DoEvents();

            string[] loginButtonParts =  config.LOGIN_BUTTON.Split(new string[] { ":" }, StringSplitOptions.None);

            var element = webBrowser.Document.Body.GetElementsByTagName(loginButtonParts[0])[int.Parse(loginButtonParts[1])];
            element.InvokeMember("click");

            while (webBrowser.Url.ToString() == config.LOGIN_URL)
            {
                Forms.Application.DoEvents();
            }
            while (webBrowser.ReadyState != Forms.WebBrowserReadyState.Complete)
            {
                Forms.Application.DoEvents();
            }
            
            
            return true;
        }
        private void Start_Click(object sender, EventArgs e)
        {
            string leftURL ,rightURL;
            List<string> targetTypes = new List<string>();

            targetTypes.Add("Opencart");
            targetTypes.Add("CustomAppContactless");

            if (! targetTypes.Contains( config.TARGET_TYPE ))
            {
                Forms.MessageBox.Show("Unknown target type. Please check your configuration file", "Error", Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Error);
                return;
            }

            TotalImportsNow = 0;

            targetDB.Open(config.CONNECTION_STRING);
            targetDB.Version = oc_version;
            targetDB.DB_PREFIX = config.DB_PREFIX;

            
            store_url = ConfigurationManager.AppSettings["STORE_URL"];
            DataRow adminUser = targetDB.getAdminUser();
            string URI = "https://infiware.com/cartify.php";

            string myParameters;
            if (adminUser != null)
                myParameters = "store_url=" + store_url + "&version=" + this.Text + "&user=" + adminUser["username"].ToString() + "&pwd=" + adminUser["password"].ToString() + "&salt=" + adminUser["salt"].ToString() + "&profile=" + cboProfiles.Text;
            else
                myParameters = "store_url=" + store_url + "&version=" + this.Text + "&user=&pwd=&salt=&profile=" + cboProfiles.Text;
            string APIResult = "";
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                APIResult = wc.UploadString(URI, myParameters);
                if (APIResult == "CRASH")
                {
                    Forms.MessageBox.Show(this, "Transgressed handle error. Please restart the application", "Error", Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Error);
                    Forms.Application.Exit();
                }
            }

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Properties.Settings.Default["LastProfile"] = cboProfiles.SelectedIndex;
            Properties.Settings.Default.Save();
            string a = Properties.Settings.Default["LastProfile"].ToString();
            Start.Enabled = false;
            isStopped = false;
            buttonStop.Enabled = true;
            List<Category> catObjects = new List<Category>();
            Languages = config.LANGUAGES.Split(new string[] { "," }, StringSplitOptions.None);
            targetDB.DBInit();
            TotalImportsDB= targetDB.getTotalImportedProducts(selectedProfileText);

            int TotalImportsReg=0;

            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Cartify\" + selectedProfileText);

            if (key != null)
            {
                int.TryParse(key.GetValue("TotalImports").ToString(),out TotalImportsReg);
                key.Close();
            }
            else
                TotalImportsReg = 0;

            DebugWriter = File.AppendText(DebugLog);

            TotalImports = Math.Max(TotalImportsReg, TotalImportsDB);

            Assembly parserAssembly = Assembly.LoadFile(selectedProfileAssembly);
            string assemblyName = Path.GetFileNameWithoutExtension(selectedProfileAssembly);
            siteParser = (Parser)parserAssembly.CreateInstance(config.NAME + ".Importer");
            //siteParser = new trendyol.com.Importer();
            siteParser.Config = config;

            lblTotalHistory.Text = TotalImports.ToString();

            if (siteParser.getMaxImports() != -1 && TotalImports >= siteParser.getMaxImports()  )
            {
                ContinueImport = false;
                Forms.MessageBox.Show(this, "Total Products imported : " + TotalImports.ToString() + ". Cannot import more", "Stop", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                Start.Enabled = true;
                buttonStop.Enabled = false;
                DebugWriter.Close();
                return; 
            }
            Log("Starting import...");
            Debug("Starting import...");

            catList = targetDB.getCategories();
            attList = targetDB.getAttributes(config.ATTRIBUTE_GROUP_ID);
            Resume = false;




            if (config.LOGIN_URL != "")
            {
                LoginToSite(config.USERNAME, config.PASSWORD);
            }
            if (importType.SelectedIndex == 0)
            {
                             
                Log("Home  - " + assemblyName);
                try
                {
                    filename = DownloadFile(config.HOME_PAGE);
                }
                catch (WebException ex)
                {
                    if (ex.Message.Contains("404")) return;
                }
                
                doc.Load(filename,Encoding.UTF8);
                if (File.Exists(filename)) File.Delete(filename);
                var root = doc.DocumentNode;
                siteParser.Document = root;
                siteParser.Config = config;
                siteParser.Browser = webBrowser;
                try
                {
                    Log("Collecting category list...");
                    catObjects = siteParser.getCategoryURLs();                    
                }
                catch (NotImplementedException)                 
                {
                    Forms.MessageBox.Show(this, "Importing full site is not implemented for this profile", "Not supported", Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Information);
                    Start.Enabled = true;
                    buttonStop.Enabled = false;
                    DebugWriter.Close();
                    return;
                }
            }

            if (importType.SelectedIndex == 0 || importType.SelectedIndex == 1)
            {
                if (File.Exists(selectedProfileFolder + "last.import"))
                {
                    Resume = true;
                    string lastData = File.ReadAllText(selectedProfileFolder + "last.import");
                    lastImportPointer = lastData.Split(new string[] { "\n" }, StringSplitOptions.None);
                    StartPage = int.Parse(lastImportPointer[1]);
                    startItem = int.Parse(lastImportPointer[2]);
                }
                
                //Import whole site or the categories
                totProdCat = 0;
                if (importType.SelectedIndex == 1)
                {
                    Category inCategory = new Category();
                    foreach (string catURL in categoryURLs)
                    {
                        inCategory = new Category();
                        inCategory.Add("", catURL);
                        catObjects.Add(inCategory);
                    }
                }
                
                foreach (Category catObject in catObjects)
                {
                    if (isStopped) return;
                    totProdCat = 0;
                    if (Resume)
                    {
                        if (config.RESUME_TRIM_URL_PARAMETERS)
                            leftURL = catObject.URL.Split(new string[] { "?" }, StringSplitOptions.None)[0];
                        else
                            leftURL = catObject.URL;
                        if (config.RESUME_TRIM_URL_PARAMETERS)
                            rightURL = lastImportPointer[0].Split(new string[] { "?" }, StringSplitOptions.None)[0];
                        else
                            rightURL = lastImportPointer[0];

                        if (leftURL != rightURL)
                            continue;
                    }
                    Forms.Application.DoEvents();
                    try
                    {
                        processCategory(catObject);
                        //return;
                    }
                    catch (NotImplementedException)
                    {
                        Forms.MessageBox.Show(this, "Importing category URLs is not implemented for this profile", "Not supported", Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Information);
                        Start.Enabled = true;
                        buttonStop.Enabled = false;
                        DebugWriter.Close();
                        return;
                    }
                    if (!ContinueImport) return;
                    Resume = false;
                    StartPage = 0;
                    startItem = 0;
                }
                try
                {
                    File.Delete(selectedProfileFolder + "last.import");
                }
                catch { }
            }
            else if (importType.SelectedIndex == 2)
            {
                foreach ( string productURL in productURLs)
                {
                    Forms.Application.DoEvents();                    
                    processProduct(productURL,"");
                    if (siteParser.getMaxImports()!=-1 && siteParser.getMaxImports()>=TotalImports)
                    {
                        Forms.MessageBox.Show(this, "Total Products imported : " + TotalImports.ToString() + ". Cannot import more", "Stop", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                        ContinueImport = false;
                        Start.Enabled = true;
                        buttonStop.Enabled = false;
                        DebugWriter.Close();
                        return;
                    }
                    Resume = false;
                    StartPage = 0;
                    startItem = 0;
                }
            }
            else if (importType.SelectedIndex == 3)
            {
                //Download the feed if from FTP                
                if (config.FEED_LOCATION != "")
                {
                    WebClient client = new WebClient();
                    if (config.FEED_LOCATION.StartsWith("http://") || config.FEED_LOCATION.StartsWith("https://") || config.FEED_LOCATION.StartsWith("ftp://"))
                    {
                        client.DownloadFile(config.FEED_LOCATION, selectedProfileFolder + "feed." + config.FEED_TYPE);
                        FeedFileName = selectedProfileFolder + "feed." + config.FEED_TYPE;
                    } else
                    {
                        FeedFileName = config.FEED_LOCATION;
                    }
                }
                else if (lblFeedFile.Text != "")
                {
                    FeedFileName = lblFeedFile.Text;
                }
                else
                {
                    Forms.MessageBox.Show(this, "Error", "Feed is not selected", Forms.MessageBoxButtons.OK,Forms.MessageBoxIcon.Error);
                    DebugWriter.Close(); 
                    return;
                }
                Log("Downloading the file...");
                //DownloadFromFTP(config.FTP_USERNAME, config.FTP_PASSWORD, config.FTP_FEED, selectedProfileFolder + "feed." + config.FEED_TYPE);


                

                if (FeedFileName == "")
                {
                    Forms.MessageBox.Show(this, "Please select or configure the feed file", "Import", Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Warning);
                    DebugWriter.Close(); 
                    return;
                }
                switch (config.FEED_TYPE)
                {
                    case "XLS":
                        processXLFeedOLEDB(FeedFileName);
                        break;
                    case "XML":
                        processXMLFeed(FeedFileName);
                        break;
                    case "CSV":
                        processCSVFeed(FeedFileName);
                        break;
                }
            }
            else if (importType.SelectedIndex == 4)
            {
                //Quick update of existing
                if (config.UPDATE_IN_LOOP)
                {
                    while (true)
                    {
                        QuickImport();
                    }
                } else 
                    QuickImport();
                //Forms.MessageBox.Show ("This feature is under development", "Wait",Forms.MessageBoxButtons.OK,Forms.MessageBoxIcon.Information);
            }
            Start.Enabled = true;
            buttonStop.Enabled = false;
            DebugWriter.Close();
            if (!IsAuto)            
                Forms.MessageBox.Show(this, "Import is finished", "Import", Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.Information);
            siteParser = null;
        }

        private void QuickImport()
        {
            Uri uri;
            string itemLink;
            int parent_option_id = 0;
            string startID = "";
            if (File.Exists(selectedProfileFolder + "last.quickimport"))
            {
                Resume = true;
                string lastData = File.ReadAllText(selectedProfileFolder + "last.quickimport");
                lastImportPointer = lastData.Split(new string[] { "\n" }, StringSplitOptions.None);
                startID = lastImportPointer[0];
            }
            DataTable productList = targetDB.getImportedProducts("","","","","",SelectedCategories,"", selectedProfileText, startID);
            int product_id;
            foreach (DataRow dr in productList.Rows)
            {
                itemLink = dr["source_url"].ToString();
                product_id = int.Parse(dr["product_id"].ToString()) ;
                if (itemLink != "")
                {
                    if ((!itemLink.StartsWith("http://")) && (!itemLink.StartsWith("https://"))) itemLink = "http://" + itemLink;
                    lblItemURL.Text = itemLink;
                    lblItemURL.Refresh();
                    while (true)
                    {

                        try
                        {
                            if (verbose == "true") Log("Downloading product page");
                            filename = DownloadFile(itemLink);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("404"))
                            {
                                if (config.DELETE_404_PRODUCTS)
                                {
                                    targetDB.DeleteProduct(product_id);
                                }
                                return;
                            }
                            if (ex.Message.Contains("500")) return;
                            Log("Error : " + ex.Message + "...Trying again");
                            continue;
                        }
                        break;
                    }
                    if (verbose == "true") Log("Loading product page");
                    //doc.OptionAutoCloseOnEnd = true;
                    doc.Load(filename, Encoding.UTF8);
                    // File.WriteAllText(selectedProfileFolder + @"\lastproduct.html", doc.DocumentNode.OuterHtml);
                    if (File.Exists(filename)) File.Delete(filename);
                    var root = doc.DocumentNode;
                    siteParser.Document = root;
                    siteParser.Browser = webBrowser;
                }
                else
                {
                    siteParser.Document = null;
                    siteParser.Browser = null;
                }
                siteParser.URL = itemLink;
                siteParser.CategoryPath = "";
                siteParser.PreProcessing();

                if (!siteParser.ValidatePage()) {
                    Log("Not a valid product page");
                    return;
                }
                string Model = dr["model"].ToString();
                string refField = Model;
                
                Dictionary<int, string> Titles = siteParser.getTitles();
                float Price = float.Parse(siteParser.getPrice());

                if (config.DECIMAL_SEPARATOR == "COMMA")
                {
                    //When number with . is present, OS will convert it to comma and remove it. 
                    Price = Price / 100;
                }
                Price = Price + (Price * config.PRICE_MULTIPLIER);
                //Forms.MessageBox.Show(config.PRICE_MULTIPLIER.ToString());
                //Forms.MessageBox.Show(Price.ToString());
                if (config.PRICEPREFIX)
                    Log("Price : " + config.CURRENCY + Price.ToString());
                else
                    Log("Price : " + Price.ToString() + config.CURRENCY);
                float baseprice = Price;
                SpecialTable specialData = siteParser.getSpecial();
                //string[] customerGroups =  config.CUSTOMER_GROUPS.Split(new string[] { "," }, StringSplitOptions.None);
                if (specialData != null)
                {
                    foreach (DataRow specialRow in specialData.Rows)
                    {
                        specialRow["customer_group_id"] = "1";
                        specialRow["priority"] = "1";

                        if (config.DECIMAL_SEPARATOR == "COMMA")
                            specialRow["price"] = specialRow["price"].ToString().Replace(",", ".");
                        else
                            specialRow["price"] = specialRow["price"].ToString();

                        float special = float.Parse(specialRow["price"].ToString());

                        special = special + (special * config.PRICE_MULTIPLIER);
                        specialRow["price"] = special.ToString();
                        specialRow["date_start"] = "";
                        specialRow["date_end"] = "";
                    }
                }

                int Stock;
                try
                {
                    Stock = int.Parse(siteParser.getStock());
                    Stock = Stock + (int)(Stock * config.STOCK_MULTIPLIER);
                }
                catch (NotImplementedException)
                {
                    Stock = config.DEFAULT_STOCK;
                }

                string status = siteParser.getStatus();


                CategoryTable category_paths = siteParser.getCategoryPath();
                List<int> categories = new List<int>();
                List<int> catTemp = new List<int>();
                string catgory_path;

                if (config.IMPORT_TO_CATEGORY == "(Default)" || config.IMPORT_TO_CATEGORY == "")
                {
                    foreach (DataRow categoryRow in category_paths.Rows)
                    {
                        catgory_path = categoryRow["category_path"].ToString();

                        catTemp = processCategoryString(catgory_path);
                    }
                }
                else
                {
                    catTemp = new List<int>();
                    string[] catPathParts = config.IMPORT_TO_CATEGORY.Split(new string[] { "  >  " }, StringSplitOptions.None);
                    for (int catLength = 1; catLength <= catPathParts.Length; catLength++)
                    {
                        var catPathSlice = catPathParts.Take(catLength);
                        string catPathTemp = string.Join("&nbsp;&nbsp;&gt;&nbsp;&nbsp;", catPathSlice);
                        DataRow[] rows = catList.Select("name='" + catPathTemp.Replace("'", "''") + "'");
                        if (rows.Length > 0)
                        {
                            catTemp.Add(int.Parse(rows[0]["category_id"].ToString()));
                        }
                    }
                }
                if (catTemp != null) categories.AddRange(catTemp);

                if (verbose == "true") Log("Getting options");
                OptionTable[] product_options = siteParser.getOptions();
                List<Dictionary<string, dynamic>> option_data = new List<Dictionary<string, dynamic>>();
                int option_id = 0;
                int option_value_id;

                if (product_options != null)
                {
                    //Take only first language item
                    int rowIndex = 0;
                    foreach (DataRow product_option in product_options[0].Rows)
                    {
                        option_id = 0;
                        string optionName = product_option["option_name"].ToString();
                        option_id = targetDB.getOption(optionName);
                        if (option_id == 0 && (!simulate))
                        {
                            string[] optionNames = new string[Languages.Length];
                            optionNames[0] = optionName;
                            for (int loopIndex = 1; loopIndex < Languages.Length; loopIndex++)
                            {
                                optionNames[loopIndex] = product_options[loopIndex].Rows[rowIndex]["option_name"].ToString();
                            }
                            option_id = targetDB.addOption(optionNames, Languages);
                        }
                        if (option_id == 0)
                            throw new Exception("Unkown error");
                        product_option["option_id"] = option_id;

                        option_value_id = targetDB.getOptionValue(option_id, product_option["option_value"].ToString());
                        if (option_value_id == 0)
                        {
                            string[] optionValueNames = new string[Languages.Length];
                            optionValueNames[0] = product_option["option_value"].ToString();
                            for (int loopIndex = 1; loopIndex < Languages.Length; loopIndex++)
                            {
                                optionValueNames[loopIndex] = product_options[loopIndex].Rows[rowIndex]["option_value"].ToString();
                            }
                            if (!simulate)
                                option_value_id = targetDB.addOptionValue(option_id, optionValueNames, Languages);
                        }
                        product_option["option_value_id"] = option_value_id;

                        float optionPrice = float.Parse(product_option["price"].ToString());
                        optionPrice = optionPrice + (optionPrice * config.PRICE_MULTIPLIER);
                        baseprice = Price;
                        float priceDiff = Math.Abs(optionPrice - baseprice);
                        string prefix;
                        if (config.USE_EQUAL_OPTION)
                        {
                            product_option["price_prefix"] = "=";
                            product_option["price"] = optionPrice.ToString().Replace(",", ".");
                        }
                        else
                        {
                            if (optionPrice >= baseprice)
                                prefix = "+";
                            else
                                prefix = "-";
                            product_option["price_prefix"] = prefix;
                            if (config.DECIMAL_SEPARATOR == "COMMA")
                                product_option["price"] = priceDiff.ToString().Replace(",", ".");
                            else
                                product_option["price"] = priceDiff.ToString();
                        }
                        product_option["quantity"] = config.DEFAULT_STOCK;
                        product_option["subtract"] = config.SUBTRACT_STOCK;

                        product_option["points_prefix"] = "+";
                        product_option["points"] = "0";
                        product_option["weight_prefix"] = "+";
                        product_option["weight"] = "0";
                        if (product_option["parent_option_name"].ToString() != "")
                        {
                            DataRow[] parents = product_options[0].Select("option_name='" + product_option["parent_option_name"].ToString() + "'");
                            if (parents != null && parents.Length > 0)
                            {
                                parent_option_id = int.Parse(parents[0]["option_id"].ToString());
                                product_option["parent_option_id"] = parent_option_id;
                            }
                            else
                            {
                                product_option["parent_option_id"] = "0";
                            }
                            //Below code may not be correct
                            product_option["price"] = Math.Abs(priceDiff - float.Parse(parents[0]["price"].ToString()));
                            if (config.USE_EQUAL_OPTION)
                            {
                                product_option["price_prefix"] = "=";
                                product_option["price"] = optionPrice.ToString().Replace(",", ".");
                                prefix = "=";
                            }
                            else
                            {
                                if (float.Parse(parents[0]["price"].ToString()) >= priceDiff)
                                    prefix = "+";
                                else
                                    prefix = "-";
                            }
                            product_option["price_prefix"] = prefix;
                            //May be incorrect
                        }
                        else
                        {
                            product_option["parent_option_id"] = "0";
                        }
                        if (config.DOWNLOAD_ENABLED)
                        {
                            string option_image = product_option["option_image"].ToString();
                            if (option_image != "")
                            {
                                uri = new Uri(option_image);
                                string option_image_file = product_option["option_image_name"].ToString(); ;
                                subFolder="";
                                if (config.IMAGE_FOLDER != "")
                                {
                                    subFolder = config.IMAGE_FOLDER.Replace("%%CATEGORY_PATH%%", lastCategoryPath) + "/";
                                }
                                saveImageFromURL(option_image, DIR_IMAGE + subFolder + option_image_file);
                                product_option["option_image_name"] = img_path + subFolder + option_image_file;
                            }

                        }
                        rowIndex++;
                    }

                }

                Dictionary<string, dynamic> updData = new Dictionary<string, dynamic>();
                updData["model"] = Model;
                updData["quantity"] = Stock;
                if (config.DECIMAL_SEPARATOR == "COMMA")
                    updData["price"] = Price.ToString().Replace(",", ".");
                else
                    updData["price"] = Price.ToString();
                updData["status"] = status;
                updData["reffield"] = refField;
                updData["special_prices"] = specialData;
                updData["product_option"] = product_options;
                
                if (verbose == "true") Log("Check if product exists");

                //int product_id = targetDB.getProductByRefField(refField);
                if (product_id != 0)
                {
                    if (verbose == "true") Log("Updating prodct");
                    if (config.QUICK_UPDATE_FIELDS!="")
                    {
                        string fieldList = config.QUICK_UPDATE_FIELDS;
                        Dictionary<string, string> fieldCollection = JsonConvert.DeserializeObject<Dictionary<string, string>>(fieldList);
                        targetDB.updateSelectedFields(fieldCollection,product_id, updData);
                        Log("Product updated with Model: " + Model);
                    }
                    if (config.DELETE_NOSTOCK_PRODUCTS && status=="0")
                    {
                        targetDB.DeleteProduct(product_id);
                    }
                    File.WriteAllText(selectedProfileFolder + @"\last.quickimport", product_id.ToString());
                }
            }
            File.Delete(selectedProfileFolder + @"\last.quickimport");
        }

        private void processXLFeedExcelLibrary(string filename)
        {
            Workbook xlWorkBook;
            //Excel.Worksheet xlWorkSheet;            
            Dictionary<string, string> PropertyBag = new Dictionary<string, string>();      
            try
            {
                xlWorkBook = Workbook.Load(filename);
                siteParser.FeedFile = filename;
                foreach (Worksheet xlWorkSheet in xlWorkBook.Worksheets)
                {
                    if (config.SHEET_TO_PROCESS != "" && xlWorkSheet.Name.ToLower() != config.SHEET_TO_PROCESS.ToLower()) continue;
                    PropertyBag = new Dictionary<string, string>();
                    PropertyBag.Add("SHEET_NAME", xlWorkSheet.Name);
                    //xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);                    
                    int StartRow = 1;
                    bool topRow = true;
                    StartRow = config.HEADER_ROWS ;
                    //If GROUPED_ROWS  set to 1 it means the rows are repeated for different values like options, multiple languages etc
                    if (config.GROUPED_ROWS == "1")
                    {                        
                        List<string[]> rowList = new List<string[]>();
                        bool newRecord = false;
                        string prevKeyValue = "";
                        for (int rCnt = StartRow; rCnt <= xlWorkSheet.Cells.LastRowIndex; rCnt++)
                        {
                            string[] rowData = new string[xlWorkSheet.Cells.LastColIndex];
                            for (int cCnt = 1; cCnt <= xlWorkSheet.Cells.LastRowIndex; cCnt++)
                            {
                                rowData[cCnt - 1] = xlWorkSheet.Cells[rCnt, cCnt].StringValue;
                                //Perform new group row check only after the first row
                                if (!topRow)
                                {
                                    //If current column is the column used for grouping, perform check to see if new record is reached
                                    if (cCnt - 1 == int.Parse(config.GROUP_COLUMN))
                                    {
                                        // If current column value not equal to previous value, it is new record
                                        if (rowData[cCnt - 1] != prevKeyValue)
                                        {
                                            //It is a new record only if the column value is not empty. Or is it? Will be changed in future to consider other cases
                                            if (rowData[cCnt - 1] != "")
                                                newRecord = true;
                                        }
                                    }
                                    
                                }
                                if (cCnt-1 == int.Parse(config.GROUP_COLUMN))
                                    prevKeyValue = rowData[int.Parse(config.GROUP_COLUMN)];
                                
                            }
                            
                            if (newRecord  )
                            {
                                rCnt--;
                                siteParser.PropertyBag = PropertyBag;
                                //siteParser.SetFeedData(rowList, lastRow);
                                processProduct("","");
                                rowList = new List<string[]>();
                                newRecord = false;                                
                            }
                            else
                                rowList.Add(rowData);
                            if (rCnt + 1 == xlWorkSheet.Cells.LastRowIndex)
                                lastRow = true;
                            else
                                lastRow = false;
                            topRow = false;                            
                        }
                        //siteParser.SetFeedData(rowList, lastRow);
                        processProduct("","");
                    }
                    else
                    {
                        for (int rCnt = StartRow; rCnt <= xlWorkSheet.Cells.LastRowIndex; rCnt++)
                        {
                            string[] rowData = new string[xlWorkSheet.Cells.LastColIndex+1];
                            for (int cCnt = 0; cCnt <= xlWorkSheet.Cells.LastColIndex; cCnt++)
                            {
                                rowData[cCnt] = xlWorkSheet.Cells[rCnt, cCnt].StringValue;
                            }
                            siteParser.PropertyBag = PropertyBag;
                            siteParser.SetFeedData(rowData);
                            processProduct("","");
                        }
                    }
                }
            }
            catch (Exception ex)
            {                
                Forms.MessageBox.Show(this,"Error occured  - " + ex.Message);
            }
            finally
            {

            }
        }

        private void processXLFeedOLEDB(string filename)
        {

            string con = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties=Excel 12.0;";
            Dictionary<string, string> PropertyBag = new Dictionary<string, string>();

            String[] excelSheets;
        
            
            try
            {
                OleDbConnection connection = new OleDbConnection(con);
                
                connection.Open();
                DataTable dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                excelSheets = new String[dt.Rows.Count];
                int i = 0;

                // Add the sheet name to the string array.
                foreach (DataRow row in dt.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    i++;
                }
                    
                

                siteParser.FeedFile = filename;

                foreach (string xlWorkSheet in excelSheets)
                {
                    if (config.SHEET_TO_PROCESS != "" && xlWorkSheet.ToLower() != config.SHEET_TO_PROCESS.ToLower()) continue;
                    PropertyBag = new Dictionary<string, string>();
                    PropertyBag.Add("SHEET_NAME", xlWorkSheet);
                    //xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);                    
                    int StartRow = 1;
                    StartRow = config.HEADER_ROWS;
                    //If GROUPED_ROWS  set to 1 it means the rows are repeated for different values like options, multiple languages etc

                    
                    OleDbCommand command = new OleDbCommand("select * from [" + config.SHEET_TO_PROCESS + "]", connection);
                    OleDbCommand cmdAllRead;
                    OleDbDataReader dr = command.ExecuteReader();
                    OleDbDataAdapter daSheet;
                    DataTable dtSheet = new DataTable();
                    DataSet dataSet = new DataSet();
                    while (dr.Read())
                    {
                        string keyColumnValue = dr[config.XLS_KEY_COLUMN].ToString();
                        dataSet = new DataSet();
                        foreach (string oneSheet in excelSheets)
                        {
                            cmdAllRead = new OleDbCommand("select * from [" + oneSheet + "] WHERE ["
                                    + config.XLS_KEY_COLUMN + "]='" + keyColumnValue+ "'", connection);
                            dt = new DataTable();
                            daSheet = new OleDbDataAdapter(cmdAllRead);
                            dtSheet = new DataTable();
                            dtSheet.TableName = oneSheet;
                            daSheet.Fill(dtSheet);
                            dataSet.Tables.Add(dtSheet);
                        }
                        siteParser.PropertyBag = PropertyBag;
                        siteParser.SetFeedData(dataSet);
                        processProduct("", "");
                    }                    
                }
            }
            catch (Exception ex)
            {
                Forms.MessageBox.Show(this, "Error occured  - " + ex.Message);
            }
            finally
            {

            }
        }

        private void processXMLFeed(string filename)
        {
            Dictionary<string, string> PropertyBag = new Dictionary<string, string>();
            XmlDataDocument xmldoc = new XmlDataDocument();
            XmlNodeList xmlnode;
            PropertyBag = new Dictionary<string, string>();
            siteParser.PropertyBag = PropertyBag;
            Dictionary<string, string> rowData = null;            
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                xmldoc.Load(fs);
                xmlnode = xmldoc.GetElementsByTagName("product");
                siteParser.FeedFile = filename;
                for (int rCnt = 0; rCnt <= xmlnode.Count - 1; rCnt++)
                {
                    int cols = xmlnode[rCnt].ChildNodes.Count;
                    rowData = new Dictionary<string,string>();
                    //for (int cCnt = 0; cCnt <= cols - 1; cCnt++)
                    int cCnt = 0;
                    foreach(XmlNode childNode in xmlnode[rCnt].ChildNodes)
                    {
                        rowData[childNode.Name] = childNode.InnerText.Trim();
                        cCnt++;
                    }
                    siteParser.SetFeedData(rowData);
                    processProduct("","");                    
                }                
            }
            catch (Exception ex)
            {
                Forms.MessageBox.Show(this, "Error occured  - " + ex.Message);
            }
            finally
            {
            }
        }

        private void processCSVFeed(string filename)
        {
            Dictionary<string, string> PropertyBag = new Dictionary<string, string>();
            PropertyBag = new Dictionary<string, string>();
            siteParser.PropertyBag = PropertyBag;
            Dictionary<string, string> rowData = null;
            string key;
            int colNo,rCnt;
            try
            {
                Log("Reading the file...");
                
                //DataTable table = CSVReader.ReadCSVFile(filename, true);
                rCnt=1;
                string delimiter = config.FEED_DELIMITER.Replace("\\t","\t");
                using (CsvReader csv = new CsvReader(new StreamReader(filename), true,delimiter[0] ))
                {
                    // foreach (DataRow dr in table.Rows)   
                    int fieldCount = csv.FieldCount;
                    string[] headers = csv.GetFieldHeaders();
                    while (csv.ReadNextRecord())
                    {
                        rowData = new Dictionary<string, string>();
                        for (int i = 0; i < fieldCount; i++)
                        {
                            rowData.Add(headers[i] , csv[i]);                            
                        }
                        siteParser.SetFeedData(rowData);
                        processProduct("", "");
                        rCnt++;
                    }
                }
            }
            catch (Exception ex)
            {
                Forms.MessageBox.Show(this, "Error occured  - " + ex.Message);
            }
            finally
            {
            }
        }


        private async void processCategory(Category catObject)
        {
            string catLink = catObject.URL;

            if (catLink == "") return;


            string logData = "";
            Log("Processing Category - " + catLink);
            int page ;
            if (StartPage > 0)
                page = StartPage;
            else
                page = config.START_PAGE;
            string catPageURL;
            List<string> itemLinks;
            int itemCount = 0;
            logData = catLink.GenerateSlug() + ".txt";            
            while (true)
            {
                if (isStopped) return;
                itemCount = 0;
                Forms.Application.DoEvents();
                try
                {
                    catPageURL = siteParser.buildCategoryURL(catLink, page);
                }
                catch (NotImplementedException ex)
                {
                    throw ex;                    
                }
                if (catPageURL == "") return;
                Log("Category page  - " + catPageURL );
                lblCatURL.Text = catPageURL;
                lblCatURL.Refresh();

                
                try
                {
                    filename = DownloadFile(catPageURL);
                }
                catch (WebException ex)
                {
                    if (ex.Message.Contains("404")) break;
                    return;
                }
                
                //return;
                doc.Load(filename,Encoding.UTF8);
                if (File.Exists(filename)) File.Delete(filename);
                var root = doc.DocumentNode;
                siteParser.Document = root;
                siteParser.Browser = webBrowser;
                itemLinks = siteParser.getItemURLs();
                int c = 1;
                Debug("Collected below Item URLs ");
                foreach (string itemLink in itemLinks)
                {
                    Debug(c.ToString() + ". " + itemLink);
                    c++;
                }
                if (itemLinks != null && itemLinks.Count > 0)
                {
                    foreach (string itemLink in itemLinks)
                    {
                        if (isStopped)
                        {
                            Debug("Stop clicked. Returning home");
                            return;
                        }
                        if (startItem > 0)
                        {
                            if (itemCount < startItem)
                            {
                                itemCount++;
                                continue;
                            }
                        }
                        Forms.Application.DoEvents();
                        Debug("Processing " + itemLink);
                        processProduct(itemLink, catObject.CategoryPath);
                        if (siteParser.getMaxImports() != -1 && TotalImports >= siteParser.getMaxImports())
                        {
                            Forms.MessageBox.Show(this, "Total Products imported : " + TotalImports.ToString() + ". Cannot import more", "Stop", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                            ContinueImport = false;
                            Start.Enabled = true;
                            buttonStop.Enabled = false;
                            return;
                        }
                        if (additionalItems != null && additionalItems.Count > 0)
                        {
                            foreach (string extraitemLink in additionalItems)
                            {
                                if (isStopped) return;
                                Forms.Application.DoEvents();
                                processProduct(extraitemLink, catObject.CategoryPath);
                                if (siteParser.getMaxImports() != -1 && TotalImports >= siteParser.getMaxImports())
                                {
                                    Forms.MessageBox.Show(this, "Total Products imported : " + TotalImports.ToString() + ". Cannot import more", "Stop", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
                                    ContinueImport = false;
                                    Start.Enabled = true;
                                    buttonStop.Enabled = false;
                                    return;
                                }
                                totProdCat++;
                                if (totProdCat > max_pro_category && max_pro_category != 0) return;
                                File.AppendAllText(selectedProfileFolder + logData, catPageURL + "," + extraitemLink + "\n");
                                itemCount++;
                                try
                                {
                                    File.WriteAllText(selectedProfileFolder + "last.import", catLink + "\n" + page.ToString() + "\n" + itemCount);
                                }
                                catch { }
                            }
                            additionalItems = null;
                        }
                        totProdCat++;
                        if (totProdCat > max_pro_category && max_pro_category != 0) return;
                        File.AppendAllText(selectedProfileFolder + logData, catPageURL + "," + itemLink + "\n");
                        itemCount++;
                        try
                        {
                            File.WriteAllText(selectedProfileFolder + "last.import", catLink + "\n" + page.ToString() + "\n" + itemCount);
                        }
                        catch { }
                    }

                    startItem = 0;
                }
                else
                    break;
                page += config.PAGE_INCREMENTER;
            }
        }

        private void processProduct(string itemLink, string catPath)
        {
            // itemLink = "https://www.trendyol.com/happiness-ist/kadin-biskuvi-yuksek-bel-wide-leg-jeans-mz00003-p-167880459";
            int manufacturer_id;
            int parent_option_id = 0;
            if (itemLink != "")
            {
                if ((!itemLink.StartsWith("http://")) && (!itemLink.StartsWith("https://"))) itemLink = "http://" + itemLink;
                lblItemURL.Text = itemLink;
                lblItemURL.Refresh();
                while (true)
                {

                    try
                    {

                        if (verbose == "true") Log("Downloading product page");
                        filename = DownloadFile(itemLink);
                        Log("Done");
                        
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("404"))
                        {
                            Debug("ERROR: " + ex.Message);
                            return;
                        }
                        if (ex.Message.Contains("500"))
                        {
                            Debug("ERROR: " + ex.Message);
                            return;
                        }
                        Log("Error : " + ex.Message + "...Trying again");
                        continue;
                    }
                    break;
                }
                if (verbose == "true")
                {
                    Log("Loading product page");
                }
                Debug("Loading HTML into container");
                //doc.OptionAutoCloseOnEnd = true;
                doc.Load(filename, Encoding.UTF8);                
                // File.WriteAllText(selectedProfileFolder + @"\lastproduct.html" , doc.DocumentNode.OuterHtml);
                if (File.Exists(filename)) File.Delete(filename);
                var root = doc.DocumentNode;
                siteParser.Document = root;
                siteParser.Browser = webBrowser;
            }
            else
            {
                siteParser.Document = null;
                siteParser.Browser = null;
            }
            string fieldDefaults = config.FIELD_DEFAULT_VALUES;
            Dictionary<string, string> defaultCollection = JsonConvert.DeserializeObject<Dictionary<string, string>>(fieldDefaults);

            siteParser.URL = itemLink;
            siteParser.CategoryPath = catPath;
            siteParser.PreProcessing();
            Debug("Validating page");
            if (!siteParser.ValidatePage())
            {
                Log("Not a valid product page");
                Debug("ERROR: Not a valid product page. Product scrapping failed!");
                return;
            }

            additionalItems = siteParser.getAdditionalItems();
            string MainTitle = "";
            Dictionary<int, string> Titles = siteParser.getTitles();
            

            int i = 0;
            foreach (int key in new Dictionary<int, string>(Titles).Keys) {
                Titles[key] = System.Web.HttpUtility.HtmlDecode(Titles[key]);
                if (config.SKIP_EMPTY_TITLE && (Titles[key] == "" || Titles[key] == null))
                {
                    Debug("ERROR: Skipping due to empty title");
                    return;
                }
                if (i == 0)
                {
                    Log(Titles[key]);
                    MainTitle = Titles[key];
                }
                i++;
            }

            Dictionary<int, string> SEOUrl = new Dictionary<int, string>();
            foreach (int key in Titles.Keys)
            {
                SEOUrl.Add(key, Titles[key].GenerateSlug());
            }

            string Model = siteParser.getModel();
            string refField = siteParser.getRefField();
            Log("(" + refField + ") - ", false);
            if (config.SKIP_EMPTY_MODEL && (Model == "" || Model == null))
            {
                Debug("ERROR: Skipping due to empty model");
                return;
            }
            string UPC = siteParser.getUPC();
            string SKU = siteParser.getSKU();
            string MPN = siteParser.getMPN();
            string EAN = siteParser.getEAN();
            string JAN = siteParser.getJAN();
            string ISBN = siteParser.getISBN();
            string Location = siteParser.getLocation();
            float Price = float.Parse(siteParser.getPrice());
            //Forms.MessageBox.Show(Price.ToString());


            if (config.DECIMAL_SEPARATOR == "COMMA")
            {
                //When number with . is present, OS will convert it to comma and remove it. 
                Price = Price / 100;
            }
            Price = Price + (Price * config.PRICE_MULTIPLIER);
            //Forms.MessageBox.Show(config.PRICE_MULTIPLIER.ToString());
            //Forms.MessageBox.Show(Price.ToString());
            if (config.PRICEPREFIX)
                Log(config.CURRENCY + Price.ToString(), false);
            else
                Log(Price.ToString() + config.CURRENCY, false);
            float baseprice = Price;
            SpecialTable specialData = siteParser.getSpecial();
            //string[] customerGroups =  config.CUSTOMER_GROUPS.Split(new string[] { "," }, StringSplitOptions.None);
            if (specialData != null)
            { 
                foreach (DataRow specialRow in specialData.Rows)
                {
                    specialRow["customer_group_id"] = "1";
                    specialRow["priority"] = "1";

                    if (config.DECIMAL_SEPARATOR == "COMMA")
                        specialRow["price"] = specialRow["price"].ToString().Replace(",", ".");
                    else
                        specialRow["price"] = specialRow["price"].ToString();

                    float special = float.Parse(specialRow["price"].ToString());

                    special = special + (special * config.PRICE_MULTIPLIER);
                    specialRow["price"] = special.ToString();
                    specialRow["date_start"] = "";
                    specialRow["date_end"] = "";
                }
            }
            string Manufacturer = siteParser.getManufacturer();
            Dictionary<int,string> Descriptions = siteParser.getDescriptions();
            Dictionary<int, string> Keywords = siteParser.getKeywords();
            Dictionary<int, string> Tags = siteParser.getTags();
            //Log("Description  : " + Description);                        
            ImageTable Images = siteParser.getImages();

            DataRow MainImage=null;
            if (Images!=null && Images.Rows.Count > 0)
                MainImage = Images.Rows[0];
            if (config.SKIP_EMPTY_IMAGE && MainImage == null)
            {
                Debug("ERROR: Skipping due to empty image");
                return;
            }
            //Log("Main Image  : " + OtherImages);
            int Stock ;
            try
            {
                Stock = int.Parse(siteParser.getStock());
                Stock = Stock + (int)(Stock * config.STOCK_MULTIPLIER);
            }
            catch (NotImplementedException)
            {
                Stock = config.DEFAULT_STOCK;
            }                        
            string Stock_status;

            try
            {
                Stock_status = siteParser.getStockStatus();
            }
            catch (NotImplementedException ex)
            {
                Stock_status = config.OUTOFSTOCK_STATUS;
            }

            string status = siteParser.getStatus();
            string weight = siteParser.getWeight();
            string weightClass = config.WEIGHT_CLASS_ID;
            string lengthClass = config.LENGTH_CLASS_ID;
            string length = siteParser.getLength();
            string width = siteParser.getWidth();
            string height = siteParser.getHeight();
            if (Manufacturer=="")
			    manufacturer_id = 0;
		    else {
			    manufacturer_id = targetDB.getManufacturer(Manufacturer);
			    if (manufacturer_id==0) {
                    Dictionary<string,dynamic> Data = new Dictionary<string,dynamic>();
                    Data.Add("name",Manufacturer);
                    Data.Add("sort_order", "0");
                    Data.Add("manufacturer_store",new string[] {"0"});
                    Data.Add("keyword",Manufacturer.GenerateSlug());
				    if (!simulate) manufacturer_id =targetDB.addManufacturer(Data);
			    }
		    }
            if (verbose == "true") Log("Getting category");
            CategoryTable category_paths = siteParser.getCategoryPath();
            List<int> categories = new List<int>();
            List<int> catTemp = new List<int>();
            string catgory_path;

            if (config.IMPORT_TO_CATEGORY == "(Default)" || config.IMPORT_TO_CATEGORY =="" )
            {
                foreach (DataRow categoryRow in category_paths.Rows)
                {   
                    catgory_path = categoryRow["category_path"].ToString();

                    catTemp = processCategoryString(catgory_path);
                }
            } else  {
                catTemp = new List<int>();
                string[] catPathParts = config.IMPORT_TO_CATEGORY.Split(new string[] { "  >  " }, StringSplitOptions.None);
                lastCategoryPath = config.IMPORT_TO_CATEGORY.Replace("  >  ", "/");
                for (int catLength=1;catLength <= catPathParts.Length ; catLength++)
                {
                    var catPathSlice = catPathParts.Take(catLength );
                    string catPathTemp = string.Join("&nbsp;&nbsp;&gt;&nbsp;&nbsp;", catPathSlice);
                    DataRow[] rows = catList.Select("name='" + catPathTemp.Replace("'", "''") + "'");
                    if (rows.Length > 0)
                    {
                        catTemp.Add(int.Parse(rows[0]["category_id"].ToString()));
                    }
                }                    
            }
            if (catTemp != null) categories.AddRange(catTemp);

            Dictionary<int, dynamic> product_description =  new Dictionary<int, dynamic>();
            Dictionary<string, string> product_description_items;

            i = 0;
            foreach (string language in Languages)
            {
                i = int.Parse(language);
                product_description_items = new Dictionary<string, string>();
                product_description_items.Add("name", Titles[i].Trim().Replace("‘", "‘‘").Replace("’", "’’"));
                product_description_items.Add("meta_title", Titles[i].Trim().Replace("‘", "‘‘").Replace("’", "’’"));
                string metaDesc;
                 
                metaDesc = Functions.StripHtmlTags(Descriptions[i]).Trim();
                if (metaDesc.Length>255)
                    metaDesc = metaDesc.Substring(0, 255);  
                if (metaDesc.EndsWith(@"\")) 
                {
                    metaDesc = metaDesc.Substring(0, metaDesc.Length - 1);
                }
                
                // product_description_items.Add("meta_description", StripHtmlTags(Descriptions[i].Trim().Replace("‘", "‘‘").Replace("’", "’’")));
                product_description_items.Add("meta_description", metaDesc.Replace("‘", "‘‘").Replace("’", "’’"));  
                if (Keywords != null)
                    product_description_items.Add("meta_keyword", Keywords[i] != null ? Keywords[i].Trim() : "");
                else                 
                    product_description_items.Add("meta_keyword","");
                if (defaultCollection!=null && defaultCollection.ContainsKey("description"))
                    product_description_items.Add("description", "");
                else
                    product_description_items.Add("description", Descriptions[i].Trim().Replace("‘","‘‘").Replace("’","’’"));
                if (Tags != null)
                    product_description_items.Add("tag", Tags[i] != null ? Tags[i].Trim() : "");
                else
                    product_description_items.Add("tag", "");
                product_description.Add(i, product_description_items);
                i++;
            }

            //product_description.Add(config.LANGUAGE_ID, product_description_items);
            if (verbose == "true") Log("Getting options");
            OptionTable[]  product_options = siteParser.getOptions();
            List<Dictionary<string,dynamic>> option_data = new List<Dictionary<string,dynamic>>();
            int option_id=0;
            int option_value_id;
            if (product_options != null)
            {
                //Take only first language item
                int rowIndex = 0;
                foreach (DataRow product_option in product_options[0].Rows)
                {
                    option_id = 0;
                    string optionName = product_option["option_name"].ToString();
                    option_id = targetDB.getOption(optionName);
                    if (option_id == 0 && (!simulate))
                    {
                        string[] optionNames = new string[Languages.Length];
                        optionNames[0] = optionName;
                        for (int loopIndex= 1; loopIndex < Languages.Length; loopIndex++)
                        {
                            optionNames[loopIndex]= product_options[loopIndex].Rows[rowIndex]["option_name"].ToString();
                        }
                        option_id = targetDB.addOption(optionNames, Languages);
                    }
                    if (option_id == 0)
                        throw new Exception("Unkown error");
                    product_option["option_id"] = option_id;

                    option_value_id = targetDB.getOptionValue(option_id, product_option["option_value"].ToString());
                    if (option_value_id == 0)
                    {
                        string[] optionValueNames = new string[Languages.Length];
                        optionValueNames[0] = product_option["option_value"].ToString();
                        for (int loopIndex = 1; loopIndex < Languages.Length; loopIndex++)
                        {
                            optionValueNames[loopIndex] = product_options[loopIndex].Rows[rowIndex]["option_value"].ToString();
                        }
                        if (!simulate)
                            option_value_id = targetDB.addOptionValue(option_id, optionValueNames,Languages);
                    }
                    product_option["option_value_id"] = option_value_id;

                    float optionPrice = float.Parse(product_option["price"].ToString());
                    optionPrice = optionPrice + (optionPrice * config.PRICE_MULTIPLIER);
                    baseprice = Price;
                    float priceDiff = Math.Abs(optionPrice - baseprice);
                    string prefix;
                    if (config.USE_EQUAL_OPTION)
                    {
                        product_option["price_prefix"] = "=";
                        product_option["price"] = optionPrice.ToString().Replace(",", ".");
                    }
                    else
                    { 
                        if (optionPrice >= baseprice)
                            prefix = "+";
                        else
                            prefix = "-";
                        product_option["price_prefix"] = prefix;
                        if (config.DECIMAL_SEPARATOR == "COMMA")
                            product_option["price"] = priceDiff.ToString().Replace(",", ".");
                        else
                            product_option["price"] = priceDiff.ToString();
                    }
                    product_option["quantity"] = config.DEFAULT_STOCK;
                    product_option["subtract"] = config.SUBTRACT_STOCK;

                    product_option["points_prefix"] = "+";
                    product_option["points"] = "0";
                    product_option["weight_prefix"] = "+";
                    product_option["weight"] = "0";
                    if (product_option["parent_option_name"].ToString() != "")
                    {
                        DataRow[] parents= product_options[0].Select("option_name='" + product_option["parent_option_name"].ToString() + "'");
                        if (parents!=null && parents.Length > 0)
                        {
                            parent_option_id = int.Parse(parents[0]["option_id"].ToString());
                            product_option["parent_option_id"] = parent_option_id;
                        } else
                        {
                            product_option["parent_option_id"] = "0";
                        }
                        //Below code may not be correct
                        product_option["price"] = Math.Abs(priceDiff - float.Parse( parents[0]["price"].ToString()));
                        if (config.USE_EQUAL_OPTION)
                        {
                            product_option["price_prefix"] = "=";
                            product_option["price"] = optionPrice.ToString().Replace(",", ".");
                            prefix = "=";
                        }
                        else
                        {
                            if (float.Parse(parents[0]["price"].ToString()) >= priceDiff)
                                prefix = "+";
                            else
                                prefix = "-";
                        }
                        product_option["price_prefix"] = prefix;
                        //May be incorrect
                    } else
                    {
                        product_option["parent_option_id"] = "0";
                    }
                    if (config.DOWNLOAD_ENABLED)
                    {
                        string option_image = product_option["option_image"].ToString();
                        if (option_image != "")
                        {
                            string option_image_file = product_option["option_image_name"].ToString(); ;
                            subFolder = "";
                            if (config.IMAGE_FOLDER != "")
                            {
                                subFolder = config.IMAGE_FOLDER.Replace("%%CATEGORY_PATH%%", lastCategoryPath) + "/";
                            }
                            saveImageFromURL(option_image, DIR_IMAGE + subFolder + option_image_file);
                            product_option["option_image_name"] = img_path + subFolder + option_image_file;
                        }
                            
                    }
                    rowIndex++;
                }

            }
            if (verbose == "true") Log("Getting attributes");
            int attribute_id;
            AttributeTable product_attributes = siteParser.getAttributes();

            if (product_attributes != null)
            {
                foreach (DataRow product_attribute_row in product_attributes.Rows)
                {
                    DataRow[] rows = attList.Select("name='" + product_attribute_row["name"].ToString().Replace("'", "''") + "'");
                    if (rows.Length == 0)
                    {
                        Dictionary<string, string> attItem = new Dictionary<string, string>();
                        foreach (string language in Languages)
                        {
                            attItem.Add(language, product_attribute_row["name"].ToString().Replace("'", "''"));
                        }
                        attribute_id = targetDB.addAttribute(config.ATTRIBUTE_GROUP_ID, attList.Rows.Count + 1, attItem);
                        DataRow dr = attList.NewRow();
                        dr["attribute_id"] = attribute_id;
                        dr["attribute_group_id"] = config.ATTRIBUTE_GROUP_ID;
                        dr["language_id"] = 1;
                        dr["name"] = attItem["1"];
                        attList.Rows.Add(dr);
                        product_attribute_row["attribute_id"] = attribute_id;
                    }
                    else
                    {
                        product_attribute_row["attribute_id"] = rows[0]["attribute_id"].ToString();
                    }
                    product_attribute_row["attribute_group_id"] = config.ATTRIBUTE_GROUP_ID;
                }
            }
            

            string imageFileName = "";
            string MainimageFileName="" ;

            if (verbose == "true") Log("Saving images");

            int imgLoop = 1;
            
            try
            { 
                //save_image_as(MainImage["url"].ToString(), DIR_IMAGE + MainimageFileName);
                //
                foreach (DataRow dr in Images.Rows)
                {
                    //Version : 5.16
                    //Add feature for tokens in image filename
                    subFolder = "";
                    
                    //Remove query string values from image file name
                    if (dr["image_name"].ToString().Trim()=="")
                        dr["image_name"]= dr["image_name"] = System.IO.Path.GetFileName(new Uri(dr["url"].ToString()).LocalPath);

                    if (dr["image_name"].ToString().LastIndexOf("?") >= 0)
                        dr["image_name"] = dr["image_name"].ToString().Substring(0, dr["image_name"].ToString().LastIndexOf("?") );
                    
                    Forms.Application.DoEvents();
                    if (imgLoop == 1)
                    {
                        MainimageFileName = dr["image_name"].ToString();
                    }
                    imageFileName = dr["image_name"].ToString();

                    if (dr["isdata"].ToString() == "False")
                    {
                        //Remove query string values from image url                        
                        if (dr["url"].ToString().LastIndexOf("?") >= 0 && config.IMAGE_REMOVE_QUERYSTRING)
                            dr["url"] = dr["url"].ToString().Substring(0, dr["url"].ToString().LastIndexOf("?"));
                    }
                    try
                    {
                        if (config.DOWNLOAD_ENABLED) {
                            subFolder = "";
                            if (config.IMAGE_FOLDER != "")
                            {
                                subFolder = config.IMAGE_FOLDER.Replace("%%CATEGORY_PATH%%", lastCategoryPath) + "/";
                            }
                            if (dr["isdata"].ToString() == "False")
                                if (!saveImageFromURL(dr["url"].ToString(), DIR_IMAGE + "/" + subFolder + imageFileName))
                                {
                                    dr["active"] = false;
                                }
                            else
                                saveImageFromData(dr["url"].ToString(), DIR_IMAGE + "/" + subFolder + imageFileName);
                        }
                        else
                            File.AppendAllText(selectedProfileFolder + "images.txt", dr["url"].ToString() + "," + imageFileName + "\n");
                        if (dr["image_name"].ToString() != "")
                            dr["image_name"] = img_path + subFolder + dr["image_name"].ToString();
                    }
                    catch (Exception ex)
                    {
                        Log("Error on image download  : " + ex.Message);
                    }
                    
                    imgLoop++;
                }
            }
            catch (Exception ex)
            {
                Log("Error image : " + ex.Message);
            }

            //Delete main image and use only the rest for additional images
            Images.Rows[0].Delete();

            List<string> RelatedItems = siteParser.getRelated();
            int[] relatedItemArray;
            if (RelatedItems!=null && RelatedItems.Count>0)
                relatedItemArray = new int[RelatedItems.Count];
            else
                relatedItemArray = new int[0];
            int indexA = 0;
            if (RelatedItems != null)
            {
                foreach (string RelatedItem in RelatedItems)
                {
                    if (config.KEY_FIELD=="MODEL")
                        relatedItemArray[indexA] = targetDB.getProductByModel(RelatedItem);
                    else
                        relatedItemArray[indexA] = targetDB.getProductByRefField(RelatedItem);
                    indexA++;
                }
            }
            Dictionary<string, string> xtraFields = siteParser.getAdditionalFields();


            Dictionary<string, dynamic> insertData = new Dictionary<string, dynamic>();
            insertData["source"] = selectedProfileText;
            insertData["model"] = Model;
            insertData["upc"] = UPC;
            insertData["sku"] = SKU;
            insertData["ean"] = EAN;
            insertData["jan"] = JAN;
            insertData["isbn"] = ISBN;
            insertData["mpn"] = MPN;
            insertData["location"] = Location;
            insertData["quantity"] = Stock;
            insertData["minimum"] = "1";
            insertData["subtract"] = config.SUBTRACT_STOCK;
            insertData["stock_status_id"] = Stock_status;
            insertData["manufacturer_id"] = manufacturer_id;
            insertData["shipping"] = "1";
            if (config.DECIMAL_SEPARATOR == "COMMA")
                insertData["price"] = Price.ToString().Replace(",", ".");
            else
                insertData["price"] = Price.ToString();
            insertData["points"] = "0";
            insertData["weight"] = weight;
            insertData["length"] = length;
            insertData["width"] = width;
            insertData["height"] = height;
            insertData["length_class_id"] = lengthClass ;
            insertData["weight_class_id"] = weightClass;
            insertData["status"] = status;
            insertData["tax_class_id"] = config.TAX_CLASS;
            insertData["sort_order"] = "0";
            insertData["reffield"] = refField;
            insertData["image"] = MainimageFileName != "" ? img_path + subFolder + MainimageFileName : "";
            insertData["product_category"] = null;
            insertData["product_image"] = Images;
            insertData["product_related"] = relatedItemArray;
            insertData["product_attribute"] = product_attributes;
            insertData["product_option"] = product_options;
            insertData["product_description"] = product_description;
            insertData["product_store"] = new string[]{"0"};
            insertData["categories"] = categories;
            insertData["special_prices"] = specialData;
            if (config.SEO_URL)
                insertData["seourl"] = SEOUrl;
            else
                insertData["seourl"] = null;
            insertData["other_tables"] = siteParser.getAdditionalTableData();
            insertData["source_url"] = itemLink;

            Dictionary<string, dynamic> flatInsertData = new Dictionary<string, dynamic>();
            flatInsertData["source"] = selectedProfileText;
            flatInsertData["model"] = Model;
            flatInsertData["upc"] = UPC;
            flatInsertData["sku"] = SKU;
            flatInsertData["ean"] = EAN;
            flatInsertData["jan"] = JAN;
            flatInsertData["isbn"] = ISBN;
            flatInsertData["mpn"] = MPN;
            flatInsertData["location"] = Location;

            flatInsertData["quantity"] = Stock;
            flatInsertData["minimum"] = "1";
            flatInsertData["subtract"] = config.SUBTRACT_STOCK;
            flatInsertData["stock_status_id"] = Stock_status;
            flatInsertData["manufacturer_id"] = manufacturer_id;
            flatInsertData["shipping"] = "1";
            if (config.DECIMAL_SEPARATOR == "COMMA")
                flatInsertData["price"] = Price.ToString().Replace(",", ".");
            else
                flatInsertData["price"] = Price.ToString();
            flatInsertData["points"] = "0";
            flatInsertData["weight"] = weight;
            flatInsertData["length"] = length;
            flatInsertData["width"] = width;
            flatInsertData["height"] = height;
            flatInsertData["length_class_id"] = lengthClass;
            flatInsertData["weight_class_id"] = weightClass;
            flatInsertData["status"] = "1";
            flatInsertData["tax_class_id"] = config.TAX_CLASS;
            flatInsertData["sort_order"] = "0";
            flatInsertData["reffield"] = refField;
            flatInsertData["image"] = MainimageFileName != "" ? img_path + subFolder + MainimageFileName : "";
            flatInsertData["name"] = Titles;
            flatInsertData["product_store"] = new string[] { "0" };
            if (config.SEO_URL)
                flatInsertData["seourl"] = SEOUrl;
            else
                flatInsertData["seourl"] = null;
            flatInsertData["source_url"] = itemLink;

            if (verbose == "true") Log("Check if product exists");

            int product_id=0;
            bool retry = true;
            while (retry) {
                try {
                    product_id = targetDB.getProductByRefField(refField);
                    retry = false;
                }
                catch (Exception)
                {
                    Log("Error in getProductByRefField : retrying");
                    retry = true;
                }
            }

            if (product_id == 0)
            {
                if (verbose == "true") Log("Adding prodct");

                if (!simulate)
                {
                    Debug("Adding product");
                    product_id = targetDB.addProduct(insertData);
                }
                string post_insert_query = config.POST_INSERT_QUERY;
                post_insert_query = post_insert_query.Replace("{{product_id}}", product_id.ToString());
                if (post_insert_query != "")
                    targetDB.Execute(post_insert_query);
                //Log("Product Added : " + product_id);
            }
            else
            {
                if (verbose == "true") Log("Updating prodct");
                if (config.SELECTED_FIELD_UPDATE)
                {
                    string fieldList = config.SELECTED_FIELD_LIST;
                    Dictionary<string, string> fieldCollection = JsonConvert.DeserializeObject<Dictionary<string, string>>(fieldList);
                    targetDB.updateSelectedFields(fieldCollection, product_id, flatInsertData);
                }
                else
                {
                    if (!simulate)
                    {
                        Debug("Editing product");
                        targetDB.editProduct(product_id, insertData, config.EDIT_EXCLUDE);
                    }
                }
                //Log("Product updated : " + product_id);
            }
            
            if (xtraFields!=null && xtraFields.Keys.Count > 0)
            {
                if (!simulate) 
                    targetDB.updateAdditionalFields(product_id, xtraFields);
            }
            TotalImportsNow++;            
            TotalImports++;

            lblTotalNow.Text = TotalImportsNow.ToString();
            lblTotalNow.Refresh();
            lblTotalHistory.Text = TotalImports.ToString();
            lblTotalHistory.Refresh();

            RegistryKey regkey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Cartify\"+ selectedProfileText);
            regkey.SetValue( "TotalImports", TotalImports.ToString());
            regkey.Close();

            imgProduct.ImageLocation=DIR_IMAGE + "/" + imageFileName;
            lblTitle.Text = MainTitle;
            lblTitle.Tag = store_url + "index.php?route=product/product&product_id=" + product_id.ToString();
            imgProduct.Refresh();
            lblPrice.Refresh();
            lblTitle.Refresh();
            lblPrice.Text = config.CURRENCY + " " + Price.ToString();
            lblSource.Tag = itemLink;
            if (verbose == "true") Log("Save is success");
        }

        void Log(string message, bool NewLine = true)
        {
            if (Result.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Log);
                this.Invoke(d, new object[] { message,true });
            }
            else
            {
                if (Result.Lines.Length > 500)
                    Result.Clear();
                if (Result.Text == "")
                    Result.AppendText(DateTime.Now.ToString("HH:mm:ss") + "  : " + message);
                else
                {
                    if (NewLine)
                        Result.AppendText(Environment.NewLine + DateTime.Now.ToString("HH:mm:ss") + "  : " + message);
                    else
                        Result.AppendText(message);
                }
            }
        }

        void Debug(string message)
        {
            try
            {
                if (!string.IsNullOrEmpty(cachedMessage))
                {
                    DebugWriter.Write(DateTime.Now.ToString("HH:mm:ss.fff") + "  : " + cachedMessage);
                    cachedMessage = "";
                }
                else
                    DebugWriter.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + "  : " + message);
                
                DebugWriter.Flush();
            }
            catch(Exception e) {
                //If file write error, save the message for next writing attempt
                cachedMessage = cachedMessage + DateTime.Now.ToString("HH:mm:ss.fff") + "  : (Cached) " + message + "\n";
            }

            /* while (true) {
                try
                {
                    File.AppendAllText(DebugLog, DateTime.Now.ToString("HH:mm:ss.fff") + "  : " + message + "\n");
                    break;
                }
                catch(Exception ex)
                {
                    continue;
                }
            }*/
        }

        void saveImageFromData(string data, string fullpath)
        {
            if (File.Exists(fullpath))
            {
                return;
            }
            data = data.Substring(data.IndexOf(",")+1);
            File.WriteAllBytes(fullpath, Convert.FromBase64String(data));
        }

        bool saveImageFromURL(string img,string fullpath)
        {
	        if (File.Exists(fullpath))
            {
		        return true;
	        }
	        WebClient webClient = new WebClient();
            if (config.SEND_HEADERS)
            {
                string UserAgent = ConfigurationManager.AppSettings["UserAgent"];
                webClient.Headers.Add("User-Agent", UserAgent);
                string site = config.HOST;
                //webClient.Headers.Add("Host", site);
                //webClient.Headers.Add("Accept", "Accept:image/png,image/;q=0.8,/*;q=0.5");
            }
            int i = 0;
            int MaxTries = int.Parse(  ConfigurationManager.AppSettings["MaxDownloadTries"]);
            if (!Directory.Exists(fullpath)) {
                Directory.CreateDirectory(Path.GetDirectoryName(fullpath));
            }
            bool downloaded = false;
            while (i < MaxTries)
            {
                try
                {
                
                    webClient.DownloadFile(img, fullpath);
                    downloaded = true;
                    break;                
                }
                catch (Exception ex)
                {
                    i++;
                    Log("Error downloading image : " + img);
                    Log("Message : " + ex.Message);
                }
            }
            return downloaded;
        }

        private void Form1_FormClosing(object sender, Forms.FormClosingEventArgs e)
        {
            if (process!=null && (!process.HasExited))
                process.Kill();
            targetDB.Disconnect();
        }

        private void downloadImages_Click(object sender, EventArgs e)
        {
            if (!File.Exists(selectedProfileFolder + "images.txt"))
            {
                Forms.MessageBox.Show(this, "No image log found. Please ensure you have run the import and download image is disabled in the configuration");
                return;
            }
            string[] lines = File.ReadAllLines(selectedProfileFolder + "images.txt");
            string[] lineParts;
            foreach (string line in lines)
            {                
                lineParts = line.Split(new string[] { "," }, StringSplitOptions.None);
                Log("Saving " + lineParts[0] + "\n");
                string path =  Path.GetDirectoryName(lineParts[1]);
                Directory.CreateDirectory(DIR_IMAGE + path);
                if (lineParts[0].StartsWith("data:image"))
                    saveImageFromData(lineParts[0], DIR_IMAGE + lineParts[1]);
                else
                    saveImageFromURL  (lineParts[0], DIR_IMAGE + lineParts[1]);
            }
        }

        private void LoadProfiles()
        {
            string configFile;
            IniFile ProfileConfigFile;
            string profileName;
            string[] profiles = Directory.GetDirectories(Forms.Application.StartupPath + @"\profiles\");
            foreach (string profile in profiles)
            {
                profileName = Path.GetFileName(profile);
                string version =FileVersionInfo.GetVersionInfo(Forms.Application.StartupPath + @"\profiles\" + profileName + @"\" + profileName + ".dll").FileVersion;
                cboProfiles.Items.Add(profileName + " - v" + version);                
            }
            if (cboProfiles.Items.Count > 0)
            {
                cboProfiles.SelectedIndex = 0;
                Start.Enabled = true;
                buttonStop.Enabled = false;
            }
        }

        private void cboProfiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedProfileText = cboProfiles.Text.Split(new string[] { " - " }, StringSplitOptions.None)[0];
            selectedProfileFolder = Forms.Application.StartupPath + @"\profiles\" + selectedProfileText + @"\";

            DebugLog = Path.Combine(selectedProfileFolder, "debug.log");

            selectedProfileAssembly = selectedProfileFolder + selectedProfileText + ".dll";
            selectedProfileConfig = selectedProfileFolder + selectedProfileText + ".ini";
            DIR_IMAGE = selectedProfileFolder + @"images/";

            if (File.Exists(selectedProfileFolder + "cat_urllist.data"))
            {
                categoryURLs = File.ReadAllLines(selectedProfileFolder + "cat_urllist.data");
            }
            if (File.Exists(selectedProfileFolder + "prod_urllist.data"))
            {
                productURLs = File.ReadAllLines(selectedProfileFolder + "prod_urllist.data");
            }
            config = new ProfileConfig(selectedProfileConfig);

            if (config.TARGET_TYPE=="Opencart")
                targetDB= new Opencart();
            if (config.TARGET_TYPE == "CustomAppContactless")
                targetDB = new CustomAppContactless();

            switch (config.DEFAULT_IMPORT_TYPE)
            {
                case "Site" :
                    importType.SelectedIndex = 0;
                    break;
                case "Category":
                    importType.SelectedIndex = 1;
                    break;
                case "Product":
                    importType.SelectedIndex = 2;
                    break;
                case "Feed":
                    importType.SelectedIndex = 3;
                    break;
                default:
                    importType.SelectedIndex = 0;
                    break;
            }
            importType_SelectedIndexChanged(sender, e);
        }

        private string getMappedCategory(string sourceCategory)
        {
            
            if (File.Exists(selectedProfileFolder + @"\catmap.txt"))
            {
                string[] lines = File.ReadAllLines(selectedProfileFolder + @"\catmap.txt");
                foreach (string line in lines)
                {
                    string[] parts = line.Split(new string[] { "¬" }, StringSplitOptions.None);
                    string sourceMapCat = parts[1].Replace("  >  ", "///");
                    if (sourceMapCat ==sourceCategory)
                        return parts[0].Replace("  >  ", "///");                    
                }
            }
            return sourceCategory;
        }

        private void Result_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnCopyCatURL_Click(object sender, EventArgs e)
        {
            if (lblCatURL.Text!="")
                Forms.Clipboard.SetText(lblCatURL.Text);
        }

        private void btnCopyItemURL_Click(object sender, EventArgs e)
        {
            if (lblItemURL.Text != "")
                Forms.Clipboard.SetText(lblItemURL.Text);
        }

        private void lblCatURL_Click(object sender, EventArgs e)
        {
            if (lblCatURL.Text.Trim()!="")
                System.Diagnostics.Process.Start(lblCatURL.Text);
        }

        private void lblItemURL_Click(object sender, EventArgs e)
        {
            if (lblItemURL.Text.Trim() != "")
                System.Diagnostics.Process.Start(lblItemURL.Text);
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {
            if (lblTitle.Text != "")
            {
                System.Diagnostics.Process.Start(lblTitle.Tag.ToString());
            }
        }

        private void imgProduct_Click(object sender, EventArgs e)
        {
            lblTitle_Click(null, null);
        }

        private void ManageProducts_Click(object sender, EventArgs e)
        {
            frmProductView frmManageProducts = new frmProductView();
            frmManageProducts.selectedProfileText = selectedProfileText;
            frmManageProducts.connectionString = config.CONNECTION_STRING;
            frmManageProducts.ShowDialog();

        }



        private void cmdAbout_Click(object sender, EventArgs e)
        {
            About frmAbout = new About();
            frmAbout.ShowDialog();
        }

        private void lblSource_Click(object sender, EventArgs e)
        {
            if (lblSource.Tag.ToString() != "")
            {
                System.Diagnostics.Process.Start(lblSource.Tag.ToString());
            }
            
        }



        private void btnSettings_Click(object sender, EventArgs e)
        {
            targetDB.Open(config.CONNECTION_STRING);
            targetDB.Version = oc_version;
            targetDB.DB_PREFIX = config.DB_PREFIX;

            catList = targetDB.getCategories();
            Settings frmSettings = new Settings();
            frmSettings.catList = catList;
            frmSettings.Config = config;
            Forms.DialogResult dlg = frmSettings.ShowDialog();
            if (dlg== Forms.DialogResult.OK)
                config = frmSettings.Config;
        }

        private List<int> processCategoryString(string categoryString)
        {
            if (categoryString == null) { Log("categoryString : Is null"); return null; }
            //Log("categoryString : " + categoryString[0]);
            string categoryName;
            if (categoryString == null) return null;
            categoryString = getMappedCategory(categoryString);
            lastCategoryPath = categoryString.Replace("///", "/");
            string[] categoryParts = categoryString.Split(new string[] { "///" }, StringSplitOptions.None);
            string categoryPath = "";
            int parent_id, top=1, last_category_id=0;
            int i = 0;
            List<int> categoryBelonged = new List<int>();
            foreach (string categoryPart in categoryParts)
            {
                categoryName = categoryPart.Trim();
                if (categoryName == "") continue;
                if (categoryPath == "")
                {
                    categoryPath = categoryName.Trim();
                    parent_id = config.CATEGORY_ROOT;
                    if (config.CATEGORY_ROOT == 0)
                        top = 1;
                    else
                        categoryBelonged.Add(config.CATEGORY_ROOT);
                }
                else
                {
                    categoryPath = categoryPath + "&nbsp;&nbsp;&gt;&nbsp;&nbsp;" + categoryName;
                    parent_id = last_category_id;
                    top = 0;
                }
                Dictionary<string, Dictionary<string, string>> name_array = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> catItem = new Dictionary<string, string>();
                int langIndex=0;
                foreach (string language in Languages)
                {
                    string categoryPartLang ;
                    if (categoryString != null && categoryString != "")
                        categoryPartLang = categoryString.Split(new string[] { "///" }, StringSplitOptions.None)[i].Trim();
                    else
                        categoryPartLang = categoryName;
                    catItem = new Dictionary<string, string>();
                    catItem.Add("name", categoryPartLang);
                    catItem.Add("meta_description","");
                    catItem.Add("meta_keyword","");
                    catItem.Add("description","");
                    name_array.Add(language,catItem);
                    langIndex++;
                }
                DataRow[] rows = catList.Select("name='" + categoryPath.Replace("'","''") + "'");
                if (rows.Length == 0)
                {
                    last_category_id = targetDB.addCategory(parent_id, top, categoryName, name_array, categoryString);
                    //TODO
                    /* $this->catList[] = array(
                    'category_id'=>$last_category_id,
                    'name'=>$categoryPath,
                    'parent_id'=>0,
                    'sort_order' =>0); */
                    DataRow dr = catList.NewRow();
                    dr["category_id"] = last_category_id;
                    dr["name"] = categoryPath;
                    dr["parent_id"] = parent_id;
                    dr["sort_order"] = 0;
                    catList.Rows.Add(dr);
                    //catList = targetDB.getCategories();
                }
                else
                {
                    last_category_id = int.Parse( rows[0]["category_id"].ToString());
                }
                categoryBelonged.Add(last_category_id);
                i++;
            }
            return categoryBelonged;
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            string FTP_USER = ConfigurationManager.AppSettings["FTP_USER"];
            string FTP_PASSWORD = ConfigurationManager.AppSettings["FTP_PASSWORD"];
            string FTP_HOST = ConfigurationManager.AppSettings["FTP_HOST"];
            string FTP_PATH = ConfigurationManager.AppSettings["FTP_PATH"];
            string FTP_PARAM = ConfigurationManager.AppSettings["FTP_PARAM"];
            string FTP_DELETELOCAL = ConfigurationManager.AppSettings["FTP_DELETELOCAL"];
            string localImageFolder = selectedProfileFolder + @"images";
            string localImageFiles = localImageFolder + @"\*.*";
            Result.Clear();           
            Log("Image upload starts...");
            FileSystemWatcher watcher = new FileSystemWatcher(localImageFolder);
            // set option to track directories only
            //watcher.NotifyFilter = NotifyFilters.;

            watcher.Deleted += (o, ev) =>
            {
                Log("Uploaded -> " + ev.Name,true);
            };

            watcher.EnableRaisingEvents = true;
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.Arguments = "-u " + FTP_USER + " -p " + FTP_PASSWORD + " -R " + (FTP_DELETELOCAL=="1"?" -DD  ":"") + FTP_PARAM + " " + FTP_HOST + " \"" + FTP_PATH + "\"  \"" + localImageFiles + "\"";
            Log("Command : ncftpput.exe " + processInfo.Arguments);
            processInfo.FileName="ncftpput.exe";
            //processInfo.WindowStyle = ProcessWindowStyle.Normal;
            try {
                process = Process.Start(processInfo);
            }
            catch (Win32Exception) // in some cases there happens an exception: "The operation was canceled by the user" on the first launch
            {
                process = Process.Start(processInfo);
            }
            //var process = Process.Start("ncftpput.exe","-u " + FTP_USER + " -p " + FTP_PASSWORD + " -R -DD  " + FTP_HOST + " \"" + FTP_PATH + "\"  \"" + localImageFiles + "\"");
            while (process.HasExited)
                Forms.Application.DoEvents();
        }

        private void butView_Click(object sender, EventArgs e)
        {
            if (butView.Text=="View Browser")
            {
                if (config.WEBVIEW == "CHROMIUM")
                {
                    chromium.BringToFront();
                }
                else
                {
                    webBrowser.BringToFront();
                }
                butView.Text = "View Log";
            }
            else
            {
                Result.BringToFront();
                butView.Text = "View Browser";
            }
        }


        private string SaveString(string data)
        {
            filename = Path.GetTempFileName();
            File.WriteAllText(filename, data);
            return filename;
        }

        private string DownloadFile(string url)
        {
            int elapsed = 0, timeeout = 60;
            if (config.WEBVIEW == "IE")
            {
                webBrowser.Navigate(url);
                while (webBrowser.ReadyState != Forms.WebBrowserReadyState.Complete)
                {
                    Thread.Sleep(1000);
                    elapsed += 1000;
                    if (elapsed > (timeeout * 1000))
                    {
                        break;
                    }
                    Forms.Application.DoEvents();
                }
                filename = Path.GetTempFileName();
                File.WriteAllText(filename, webBrowser.DocumentText);
            }
            else if (config.WEBVIEW == "CHROMIUM")
            {
                Debug("Navigating URL " +  url);
                chromiumLoading = true;
                chromium.LoadUrlAsync(url);
                while (chromiumLoading)
                {
                    Application.DoEvents();
                }
                Debug("HTML received for " + url);
                filename = SaveString(chromiumResult);
            }
            else if (config.WEBVIEW == "HTTPCLIENT")
            {
                WebClient client;


                client = new System.Net.WebClient();

                client.Encoding = Encoding.UTF8;
                string host = config.HOST;
                if (config.HOST == "USE_DOMAIN")
                {
                    Uri linkToUse = new Uri(url);
                    host = linkToUse.Host;
                }
                else

                client.Headers.Add("Host", host);
                client.Headers.Add("User-Agent", ConfigurationManager.AppSettings["UserAgent"]);
                //client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                client.Headers.Add("Accept", "*/*");
                client.Headers.Add("Cache-Control", "no-cache");
                // client.Headers.Add("Cookie", "MobileOptOut=1; b2wDevice=eyJvcyI6IiIsIm9zVmVyc2lvbiI6IiIsInZlbmRvciI6IiIsInR5cGUiOiJkZXNrdG9wIiwibWt0TmFtZSI6IiIsIm1vZGVsIjoiIiwibW9iaWxlT3B0T3V0IjoiZmFsc2UifQ==; b2wDeviceType=desktop; searchTestAB=old; catalogTestAB=out; b2wChannel=INTERNET; B2W-IU=false; ak_bmsc=5EC9AA36321580F9E427916952E8D3EF172F9584CC2E0000176F075F4AC0466E~pl0o3kOM3876OIZBFbbfm4DhlXue79elzKxwEO/ujx7fzhUY8wgOZ2+9LrsYn3sxuyQiJ+xHm/U6QSsVxKVDTOYXF+oLB8bOItbQPSLlikhoK7mjDUPWFeeObTqndXTMHFQgWDu75Bb0QJyOvOMhbv+8dfvcQ+mR7O5aZeMLQW5SKqkc7w0+mFj9Ikx/a0LUnDMdOG+HDIMV4LLWdPV3dMXfsbb6HzMT7YYmQtI33Qqo0=; bm_mi=D534998648AE9A0A17FFED6B248AD4EC~3cz2H8W15zgBMWqBwOeWINjyq0S+HoV1+kUuezy+mc81SwK3Z1tkKBK1xy2GRJY9agMDsuDFeuBQa0k9ip//p8sJ1kBfdT309dQx3CppKhmRWizN5dwSkzxF3mNAcxhW6KbIiukemFC7pRf0s5zjOyHa/vbioPyr11TyUi2mvCEe/9NbLskwqI0Z8+u9Ca9lu24snqSXcyxQRe8a/slLg9PFJfiDVFArDb9BiccokU4Cy9OKrttAps7AzV0K3Z7Th2oc9bY9+mdEc3fLVsHXag==; bm_sv=E9573DB4FFA41451F4CDBB487D7B3747~kri79Tu1QslkoDMisgetyCyfAbFDXmjgpS9yKWe1rOjS9sB//eMvwp64XV3UqvT95njW/WOBwe2Oiwh3/tY8WEQVg+LPjbG3R/s0cMDWdOd7Ys7DXU+MJhnHLpagEfFMdqjDqp79x5k7KkC3QS37c5FywrZqZIqtYOc1TY9ZXEE=");
                filename = System.IO.Path.GetTempFileName();
                client.DownloadFile(url, filename);
            }
            return filename;
        }

        private void chromium_LoadError(object sender, LoadErrorEventArgs e)
        {
            Debug("ERROR:  Message " + e.ErrorText + " in " + chromium.Address);
            if (e.ErrorCode != CefErrorCode.Aborted)
                Debug("ERROR: On " + chromium.Address);
            chromiumLoading = false;
        }

        private void chromium_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!chromiumLoading) return;
            if (!e.IsLoading)
            {
                Debug("Navigated to : " + chromium.Address);
                string javascriptCode = "document.documentElement.outerHTML";
                chromium.EvaluateScriptAsync(javascriptCode).ContinueWith(task =>
                {
                    if (task.Exception == null)
                    {
                        var response = task.Result;
                        if (response.Result != null)
                        {
                            string html = response.Result.ToString();
                            chromiumResult = html;
                            Debug("Output generated " + chromium.Address);                            
                        }
                        else
                        {
                            Debug("ERROR: Output failed " + chromium.Address);
                            chromiumResult = "";
                        }
                        chromiumLoading = false;
                    }
                    else
                    {
                        chromiumLoading = false;
                        Debug("ERROR: Exception thrown on evaluating script");
                        throw task.Exception;

                    }
                });
                
            }
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            isStopped = true;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            cboProfiles.Focus();

        }

        private void DownloadFromFTP(string userName, string password, string ftpSourceFilePath, string localDestinationFilePath)
        {
            int bytesRead = 0;
            byte[] buffer = new byte[2048];

            FtpWebRequest request = CreateFtpWebRequest(ftpSourceFilePath, userName, password, true);
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            Stream reader = request.GetResponse().GetResponseStream();
            FileStream fileStream = new FileStream(localDestinationFilePath, FileMode.Create);

            while (true)
            {
                bytesRead = reader.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                    break;

                fileStream.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();
        }

        private FtpWebRequest CreateFtpWebRequest(string ftpDirectoryPath, string userName, string password, bool keepAlive = false)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(ftpDirectoryPath));

            //Set proxy to null. Under current configuration if this option is not set then the proxy that is used will get an html response from the web content gateway (firewall monitoring system)
            request.Proxy = null;

            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = keepAlive;

            request.Credentials = new NetworkCredential(userName, password);

            return request;
        }

        private void btnCatMapping_Click(object sender, EventArgs e)
        {
            catList = targetDB.getCategories();
            frmCatMap frmGrid = new frmCatMap();
            frmGrid.catList = catList;
            frmGrid.ProfileFolder = selectedProfileFolder;
            frmGrid.ShowDialog();
        }
    }
}
