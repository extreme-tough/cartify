using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using HAP = HtmlAgilityPack;
using Aiplib;
using System.IO;

namespace ParserFactory
{
    public class Parser : IParser
    {
        public HAP.HtmlNode Document;
        public WebBrowser Browser ;
        private ProfileConfig _Config ;
        public string URL ;
        public string CategoryPath;
        public string FeedFile;
        public Dictionary<string, string> PropertyBag ;
        protected List<Category> CategoryURLs;
        protected List<string> ItemURLs;
        protected string[] Languages;
        protected Dictionary<int,string> Titles,Descriptions;
        protected string Model;
        protected int MaxImportCount=-1;

        public Parser()
        {
            CategoryURLs = new List<Category>();
            ItemURLs = new List<string>();
            Descriptions = new Dictionary<int, string>();
            Titles = new Dictionary<int, string>();
            Model = "";
        }

        public ProfileConfig Config
        {
            get { return _Config; }
            set
            {            
                _Config = value;
                Languages = _Config.LANGUAGES.Split(new string[] { "," }, StringSplitOptions.None);
           }
        }

        public virtual bool ValidatePage()
        {
            return true;
        }
        public virtual List<Category> getCategoryURLs()
        {
            Category objCategory = new Category();
            List<Category> urls = new List<Category>();

            string catListFile = Path.GetDirectoryName( this.GetType().Assembly.Location) + @"\catgorylist.txt";
            if (File.Exists(catListFile))
            {
                string[] urllist = File.ReadAllLines(catListFile);
                foreach (string url in urllist)
                {
                    objCategory = new Category(); objCategory.Add("", url); urls.Add(objCategory);
                }

            }

            return urls;
        }

        public async virtual void PreProcessing()
        {

        }

        public virtual void SetFeedData(string[] data)
        {
            
        }

        public virtual void SetFeedData(Dictionary<string, string> data)
        {
            
        }

        public virtual void SetFeedData(DataSet ds)
        {
            
        }

        public virtual string buildCategoryURL(string catURL, int page)
        {
            return catURL; 
        }

        public virtual List<string> getItemURLs()
        {
            return ItemURLs;
        }

        public virtual Dictionary<int,string> getTitles()
        {
            
            return Titles;
        }

        public virtual int getMaxImports()
        {
            return MaxImportCount;
        }

        public virtual Dictionary<int, string> getKeywords()
        {
            return null;
        }

        public virtual string getManufacturer()
        {
            return "";
        }

        public virtual string getModel()
        {
            return Model;
        }

        public virtual string getUPC()
        {
            return "";
        }

        public virtual string getSKU()
        {
            return "";
        }

        public virtual string getMPN()
        {
            return "";
        }

        public virtual string getJAN()
        {
            return "";
        }

        public virtual string getEAN()
        {
            return "";
        }

        public virtual string getISBN()
        {
            return "";
        }

        public virtual string getLocation()
        {
            return "";
        }

        public virtual string getPrice()
        {
            return "";
        }

        public virtual SpecialTable getSpecial()
        {
            return null;
        }

        public virtual string getWeight()
        {
            return "0";
        }

        public virtual string getLength()
        {
            return "0";
        }

        public virtual string getWidth()
        {
            return "0";
        }

        public virtual string getHeight()
        {
            return "0";
        }
        public virtual Dictionary<int,string> getDescriptions()
        {
            return null;
        }

        public virtual Dictionary<int, string> getTags()
        {
            return null;
        }

        public virtual ImageTable getImages()
        {
            return null;
        }

        public virtual CategoryTable getCategoryPath()
        {
            return null;
        }

        public virtual string getStock()
        {
            return "99";
        }

        public virtual string getStockStatus()
        {
            throw new NotImplementedException();
        }

        public virtual string getStatus()
        {
            return "1";
        }

        public virtual List<string> getAdditionalItems()
        {
            return null;
        }

        public virtual AttributeTable getAttributes()
        {
            return null;
        }

        public virtual string getRefField()
        {
            return getModel();
        }

        public virtual List<string> getRelated()
        {
            return null;
        }

        public virtual Dictionary<string, string> getAdditionalFields()
        {
            return null;
        }

        public virtual OptionTable[] getOptions()
        {
            return null;
        }

        public DataSet getAdditionalTableData()
        {
            return null;
        }

        protected string DownloadFile(string url)
        {
            string filename;
            WebClient client;
            client = new System.Net.WebClient();
            client.Encoding = Encoding.UTF8;
            filename = System.IO.Path.GetTempFileName();
            //string data = client.DownloadString(url);
            client.DownloadFile(url, filename);

            return filename;
        }
    }
    public interface IParser
    {
        List<Category> getCategoryURLs();
        void SetFeedData(string[] data);
        void SetFeedData(Dictionary<string, string> data);
        void SetFeedData(DataSet ds);
        string buildCategoryURL(string catURL, int page);
        List<string> getItemURLs();
        Dictionary<int,string> getTitles();
        Dictionary<int, string> getKeywords();
        string getManufacturer();
        string getModel();
        string getUPC();
        string getSKU();
        string getMPN();
        string getJAN();
        string getEAN();
        string getISBN();
        string getLocation();
        string getPrice();
        SpecialTable getSpecial();
        string getWeight();
        string getLength();
        string getWidth();
        string getHeight();
        Dictionary<int,string> getDescriptions();
        Dictionary<int, string> getTags();
        //string getMainImage();
        ImageTable getImages();
        CategoryTable getCategoryPath();
        string getStock();
        string getStockStatus();
        string getStatus();
        List<string> getAdditionalItems();
        AttributeTable getAttributes();
        string getRefField();
        List<string> getRelated();

        Dictionary<string, string> getAdditionalFields();
        //Dictionary<string, string>[] getSpecialPrice(string[] customerGroups);
        OptionTable[] getOptions();
        DataSet getAdditionalTableData();
    }
    public class SpecialTable : DataTable
    {
        public SpecialTable()
        {
            this.Columns.Add(new DataColumn("customer_group_id", typeof(Int32)));
            this.Columns.Add(new DataColumn("price", typeof(String)));
            this.Columns.Add(new DataColumn("priority", typeof(Int16)));
            this.Columns.Add(new DataColumn("date_start", typeof(String)));
            this.Columns.Add(new DataColumn("date_end", typeof(String)));
        }
    }
    public class CategoryTable : DataTable
    {
        public CategoryTable()
        {
            this.Columns.Add(new DataColumn("language_id", typeof(Int32)));
            this.Columns.Add(new DataColumn("category_path", typeof(String)));
        }
    }

    public class ImageTable : DataTable
    {
        public ImageTable()
        {
            this.Columns.Add(new DataColumn("image_name", typeof(String)));
            this.Columns.Add(new DataColumn("url", typeof(String)));
            this.Columns.Add(new DataColumn("isdata", typeof(Boolean)));
            this.Columns.Add(new DataColumn("active", typeof(Boolean)));
            this.Columns[2].DefaultValue = false;
            this.Columns[3].DefaultValue = true;
        }
    }

    public class AttributeTable : DataTable
    {
        public AttributeTable()
        {
            this.Columns.Add(new DataColumn("attribute_id", typeof(Int32)));
            this.Columns.Add(new DataColumn("language_id", typeof(Int16)));
            this.Columns.Add(new DataColumn("name", typeof(String)));
            this.Columns.Add(new DataColumn("value", typeof(String)));
            this.Columns.Add(new DataColumn("attribute_group_id", typeof(String)));
        }
    }

    public class OptionTable : DataTable
    {
        public OptionTable()
        {
            this.Columns.Add(new DataColumn("option_id", typeof(Int32)));
            this.Columns.Add(new DataColumn("option_name", typeof(String)));
            this.Columns.Add(new DataColumn("required", typeof(Byte)));
            this.Columns.Add(new DataColumn("quantity", typeof(Int32)));
            this.Columns.Add(new DataColumn("option_value_id", typeof(Int32)));
            this.Columns.Add(new DataColumn("option_value", typeof(String)));
            this.Columns.Add(new DataColumn("price_prefix", typeof(String)));
            this.Columns.Add(new DataColumn("price", typeof(float)));
            this.Columns.Add(new DataColumn("points_prefix", typeof(String)));
            this.Columns.Add(new DataColumn("points", typeof(float)));
            this.Columns.Add(new DataColumn("weight_prefix", typeof(String)));
            this.Columns.Add(new DataColumn("weight", typeof(float)));
            this.Columns.Add(new DataColumn("subtract", typeof(Int16)));
            this.Columns.Add(new DataColumn("option_image", typeof(String)));
            this.Columns.Add(new DataColumn("option_image_name", typeof(String)));
            this.Columns.Add(new DataColumn("parent_option_value", typeof(String)));
            this.Columns.Add(new DataColumn("parent_option_name", typeof(String)));
            this.Columns.Add(new DataColumn("parent_option_id", typeof(Int32)));
        }
    }
}
