using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using ParserFactory;
using Excel = Microsoft.Office.Interop.Excel;
using HAP = HtmlAgilityPack;
using EntityLib;
namespace coralstock
{
    public class Importer : Parser
    {
        HAP.HtmlNodeCollection Nodes;
        HAP.HtmlNode aNode;
        HtmlAgilityPack.HtmlNode root;
        string itemURL;
        Dictionary<string,string> propertyCollection = new Dictionary<string,string>();
        string[] feedLine = new string[0];
        string MainImage;
        int currentPage;
        string SKU, Location, Size, Title, Price, CommonName, Origin, HRef, Category;
        Excel.Application xlApp;
        Excel.Workbook xlWorkBook, xlLookup;
        Excel.Range range;
        Dictionary<string, string> catList = new Dictionary<string, string>();
        public Importer()
        {
            Excel.Range range;
            xlApp = new Excel.Application();
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string filename = Path.GetDirectoryName(path )+ @"\data\Coral Aquarium Conditions.xlsx";
            xlWorkBook = xlApp.Workbooks.Open(filename, 0, true);
            string lookupfile = Path.GetDirectoryName(path) + @"\data\Category SKU Match.xlsx";
            xlLookup = xlApp.Workbooks.Open(lookupfile, 0, true);

            Excel.Worksheet xlWorkSheet = xlLookup.Worksheets["Sheet1"];            
            range = xlWorkSheet.UsedRange;
            for (int rCnt = 1; rCnt <= range.Rows.Count; rCnt++)
            {
                string RefID = (string)(range.Cells[rCnt, 1] as Excel.Range).Text;
                string catIDs = (string)(range.Cells[rCnt, 2] as Excel.Range).Text;
                catList.Add(RefID, catIDs);
            }
            xlLookup.Close();

        }
        ~Importer()
        {
            xlApp.Quit();

        }

        public string CategoryPath
        {
            set
            {
                
            }
        }

        public string URL 
        {            
            set
            {
                itemURL = value;
            }
        }

        public string FeedFile
        {
            set
            {
            }
        }

        public Dictionary<string, string> PropertyBag
        {
            set
            {
                propertyCollection  = value;
            }
        }

        public HtmlAgilityPack.HtmlNode Document
        {
            set
            {
                this.root = value;
            }
        }

        public List<Category> getCategoryURLs()
        {
            List<Category> urls = new List<Category>();
            return urls;
        }

        public void SetFeedLines(List<string[]> data, bool LastLine)
        {


        }

        public void SetFeedLine(string[] data, bool LastLine)
        {
            feedLine = data;
            SKU = data[0];
            Location = data[1];
            Price = data[2];
            Size = data[3];            
            Title= data[4];
            CommonName = data[5];
            Category = data[6];
            Origin = data[7];
            HRef= data[8];
        }

        public void SetFeedLine(Dictionary<string, string> data, bool LastLine)
        {
            
        }
        
        public string buildCategoryURL(string catURL, int page)
        {
            return "";
        }

        public List<string> getItemURLs()
        {
            return null;
        }

        public string[] getTitles()
        {
            string[] titles = new string[1];
            titles[0] = feedLine[4];
            return titles;
        }

        public string getModel()
        {
            return SKU;
        }

        public string getUPC()
        {
            return "";
        }

        public string getSKU()
        {
            return SKU;
        }
        public string getJAN()
        {
            return "";
        }
        public string getMPN()
        {
            return "";
        }

        public string getEAN()
        {
            return "";
        }

        public string getISBN()
        {
            return "";
        }

        public string getWeight()
        {
            return "0";
        }

        public string getLocation()
        {
            return feedLine[1];
        }

        public string getPrice()
        {
            if (feedLine[2] != null)
                return feedLine[2].Replace("$", "");
            else
                return "0.0";
        }

        public Dictionary<string, string> getSpecial()
        {
            string price;
            if (feedLine[2] != null)
                price = feedLine[2].Replace("$", "");
            else
                price = "0.0";

            Dictionary<string, string> special = new Dictionary<string, string>();
            special.Add("1", price);
            return special;
        }

        public Dictionary<string, string>[] getSpecialPrices(string[] customerGroups)
        {
            Dictionary<string, string>[] specialPrices = new Dictionary<string, string>[customerGroups.Length];
            int i=0;
            foreach (string customerGroup in customerGroups)
            {
                Dictionary<string,string> priceItem = new Dictionary<string,string>();
                priceItem.Add("customer_group_id",customerGroup);
                priceItem.Add("priority","1");
                priceItem.Add("price",getPrice());
                priceItem.Add("date_start", "0000-00-00 00:00:00");
                priceItem.Add("date_end", "0000-00-00 00:00:00");
                specialPrices[i] = priceItem;
                i++;
            }
            return specialPrices;
        }

        public string getManufacturer()
        {
            return "";
        }

        public string[] getKeywords()
        {
            string[] keywords = new string[0];
            return keywords;
        }

        public string[] getTags()
        {
            string[] keywords = new string[0];
            return keywords;
        }

        public string[] getDescriptions()
        {
            string careLevel = "", placement = "", lighting = "", flow = "", temperature = "", salinity = "", calcium = "", alkalinity = "", magnesium = "";
            try
            {
                Excel.Worksheet xlWorkSheet = xlWorkBook.Worksheets[Category];
                careLevel = (xlWorkSheet.Cells[1, 2] as Excel.Range).Value;
                placement = (xlWorkSheet.Cells[2, 2] as Excel.Range).Value;
                lighting = (xlWorkSheet.Cells[3, 2] as Excel.Range).Value;
                flow = (xlWorkSheet.Cells[4, 2] as Excel.Range).Value;
                temperature = (xlWorkSheet.Cells[5, 2] as Excel.Range).Value;
                salinity = (xlWorkSheet.Cells[6, 2] as Excel.Range).Value;
                calcium = (xlWorkSheet.Cells[7, 2] as Excel.Range).Value;
                alkalinity = (xlWorkSheet.Cells[8, 2] as Excel.Range).Value;
                magnesium = (xlWorkSheet.Cells[9, 2] as Excel.Range).Value;
            }
            catch (Exception ex)
            {
            }
            
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = Path.GetDirectoryName(path);
            
            string[] desc = new string[1];
            desc[0] = File.ReadAllText(path + @"\data\html\index.html");
            desc[0] = desc[0].Replace("$$COMMONNAME$$", CommonName);
            desc[0] = desc[0].Replace("$$ORIGIN$$", Origin);
            desc[0] = desc[0].Replace("$$SIZE$$", Size);
            desc[0] = desc[0].Replace("$$CARELEVEL$$", careLevel);
            desc[0] = desc[0].Replace("$$PLACEMENT$$", placement);
            desc[0] = desc[0].Replace("$$LIGHTING$$", lighting);
            desc[0] = desc[0].Replace("$$FLOW$$", flow);
            desc[0] = desc[0].Replace("$$TEMPERATURE$$", temperature);
            desc[0] = desc[0].Replace("$$SALINITY$$", salinity);
            desc[0] = desc[0].Replace("$$CALCIUM$$", calcium);
            desc[0] = desc[0].Replace("$$ALKALINITY$$", alkalinity);
            desc[0] = desc[0].Replace("$$MAGNESIUM$$", magnesium);
            return desc;
        }

        public string getMainImage()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = Path.GetDirectoryName(path);
            string sheet = propertyCollection["SHEET_NAME"];
            string[] files ;
            if (Directory.Exists(path + @"\images\corals\" + sheet))
                files = Directory.GetFiles(path + @"\images\corals\" + sheet, SKU + ".*");
            else
                return "";
            if (files.Length > 0)
                return "corals/" +sheet + "/" + Path.GetFileName(files[0]);
            else
                return "";
        }

        public string[] getOtherImages()
        {
            string ImageURL; int i = 0, j = 0;
            string[] OtherImages = new string[0];
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            path = Path.GetDirectoryName(path);
            string sheet = propertyCollection["SHEET_NAME"];
            string[] files;
            if (Directory.Exists(path + @"\images\corals\" + sheet))
            {
                files = Directory.GetFiles(path + @"\images\corals\" + sheet, SKU + "_*.*");
                OtherImages = new string[files.Length];
                foreach (string file in files)
                {
                    OtherImages[i] = "corals/" + sheet + "/" + Path.GetFileName(file);
                    i++;
                }
            }            
            return OtherImages;
        }

        public List<string[]> getCategoryPath()
        {            
            return null;
        }

        public List<int> getCategoryIDs()
        {
            List<int> CatIDs = new List<int>();
            if (!catList.ContainsKey(Category)) return CatIDs;
            string catIDList = catList[Category];
            foreach (string cat in catIDList.Split(new string[] { "," }, StringSplitOptions.None))
            {
                if (cat != "")
                    CatIDs.Add(int.Parse(cat));
            }            
            return CatIDs;
        }

        public string getStock()
        {
            throw new NotImplementedException();
        }

        public string getStockStatus()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string>[] getAttributes()
        {
            Dictionary<string, string>[] attributes= new Dictionary<string, string>[0];
            return attributes;
        }

        public string getRefField()
        {
            return feedLine[0];
        }


        public Dictionary<string, Dictionary<string, float>> getOptions()
        {
            //string[][] option_list = new string[][] { };
            float priceOpt;
            Dictionary<string, Dictionary<string, float>> option_list = new Dictionary<string, Dictionary<string, float>>();
            //Tuple<string, string, float>[] option_list = new Tuple<string, string, float>[0];

            Dictionary<string, float> option_list_item = new Dictionary<string,float>();
           
            return option_list;
        }

        public Dictionary<string, string> getAdditionalFields()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            retVal.Add("supplier_id", "2");
            retVal.Add("cg_quantity", "1:1,2:1,3:1");
            retVal.Add("group_access", "1,2,3");
            return retVal;
        }

        public List<string> getRelated()
        {
            List<string> retVal = new List<string>();
            return retVal;
        }
        public Dictionary<string, List<string>> getPostOptionSQLCommands()
        {
            return null;
        }
    }
}
