using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using ParserFactory;
using Excel = Microsoft.Office.Interop.Excel;
using HAP = HtmlAgilityPack;
namespace handall
{
    public class Importer : Parser
    {
        // Article number in  keyword search,article attributes tabs are missing the decimal point
        // Row 1 should be the column title in keyword search
        // article attributes contain only two languages
        HAP.HtmlNodeCollection Nodes;
        HAP.HtmlNode aNode;
        HtmlAgilityPack.HtmlNode root;
        string itemURL, feedFile;
        Dictionary<string,string> propertyCollection = new Dictionary<string,string>();
        string[] feedLine = new string[0];
        string[] lastFeedLine = new string[0];

        string[] descriptions;
        string[] keywords;
        string MainImage;
        int currentPage;
        string model, SKU, Location, Size, Title, Price, CommonName, Origin, HRef, Category;
        OleDbConnection connection;
        OleDbCommand command;
        Dictionary<string, string> catList = new Dictionary<string, string>();
        Dictionary<string, Dictionary<string, float>> option_list;
        Dictionary<string, float> color_option_list_item, size_option_list_item;
        public Importer()
        {
            
            
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            color_option_list_item = new Dictionary<string, float>();
            size_option_list_item = new Dictionary<string, float>();
            option_list = new Dictionary<string, Dictionary<string, float>>();
        }
        ~Importer()
        {
           
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
                feedFile = value;
                string con = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + feedFile + ";" + 
                        @"Extended Properties='Excel 8.0;HDR=Yes;'";
                connection = new OleDbConnection(con);                
                connection.Open();                                
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

        public List<string> getCategoryURLs()
        {
            List<string> urls = new List<string>();
            return urls;
        }

        public void SetFeedLine(string[] data,bool LastLine)
        {
            lastFeedLine = feedLine;
            feedLine = data;
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
            model = getModel();
            string[] titles = new string[4];
            descriptions = new string[4];
            if (lastFeedLine.Length > 0)
            {
                if (feedLine[0] != lastFeedLine[0])
                {
                    // New product line reached           
                    command = new OleDbCommand("select * from [language$] WHERE `article number short`='" + lastFeedLine[0] + "' and `language`='english'", connection);
                    using (OleDbDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            titles[0] = dr["article_name"].ToString();
                            descriptions[0] = dr["article_description"].ToString();
                            descriptions[0] = descriptions[0].Replace(",", "</br>");
                            descriptions[0] = descriptions[0].Replace(".", "</br>");
                            descriptions[0] = descriptions[0].Replace("'", "</br>");
                            titles[3] = dr["article_name"].ToString();
                            descriptions[3] = dr["article_description"].ToString();
                            descriptions[3] = descriptions[3].Replace(",", "</br>");
                            descriptions[3] = descriptions[3].Replace(".", "</br>");
                            descriptions[3] = descriptions[3].Replace("'", "</br>");
                            break;
                        }
                    }
                    command = new OleDbCommand("select * from [language$] WHERE `article number short`='" + lastFeedLine[0] + "' and `language`='german'", connection);
                    using (OleDbDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            titles[1] = dr["article_name"].ToString();
                            descriptions[1] = dr["article_description"].ToString();
                            descriptions[1] = descriptions[1].Replace(",", "</br>");
                            descriptions[1] = descriptions[1].Replace(".", "</br>");
                            descriptions[1] = descriptions[1].Replace("'", "</br>");
                            break;
                        }
                    }
                    command = new OleDbCommand("select * from [language$] WHERE `article number short`='" + lastFeedLine[0] + "' and `language`='swedish'", connection);
                    using (OleDbDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            titles[2] = dr["article_name"].ToString();
                            descriptions[2] = dr["article_description"].ToString();
                            descriptions[2] = descriptions[2].Replace(",", "</br>");
                            descriptions[2] = descriptions[2].Replace(".", "</br>");
                            descriptions[2] = descriptions[2].Replace("'", "</br>");
                            
                            break;
                        }
                    }
                }

                //option_list = new Dictionary<string, Dictionary<string, float>>();
                if (!color_option_list_item.ContainsKey(lastFeedLine[8]))
                    color_option_list_item.Add(lastFeedLine[8], float.Parse( getPrice()));
                if (!size_option_list_item.ContainsKey(lastFeedLine[6]))
                    size_option_list_item.Add(lastFeedLine[6], float.Parse( getPrice()));

            }
            return titles;
        }

        public string getModel()
        {
            if (lastFeedLine.Length > 0)
                return lastFeedLine[0];
            else
                return "";
        }

        public string getUPC()
        {
            return "";
        }

        public string getSKU()
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

        public string getLocation()
        {
            return "";
        }

        public string getPrice()
        {
            if (lastFeedLine[14] != "")
                return lastFeedLine[14];
            else
                return "0.00";
        }

        public string getSpecial()
        {
            return getPrice();
        }

        public string getWeight()
        {
            return lastFeedLine[22];
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
            return lastFeedLine[4];
        }

        public string[] getDescriptions()
        {
            return descriptions;
        }

        public string[] getKeywords()
        {
            keywords = new string[4];            
            int i = 0;
            //var sheets = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            command = new OleDbCommand("select * from [keyword search$] where `article number short`='" + model + "' and `language`='en'", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    keywords[0] = dr["keyword"].ToString();
                    keywords[3] = dr["keyword"].ToString();
                    break;
                }
            }
            command = new OleDbCommand("select * from [keyword search$] where `article number short`='" + model + "' and `language`='de'", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    keywords[1] = dr["keyword"].ToString();
                    break;
                }
            }
            command = new OleDbCommand("select * from [keyword search$] where `article number short`='" + model + "' and `language`='se'", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    keywords[2] = dr["keyword"].ToString();
                    break;
                }
            }
            return keywords;
        }


        public string[] getTags()
        {
            return keywords;
        }

        public string getMainImage()
        {
            int index;
            string mainImage = "";
            command = new OleDbCommand("select * from [article pictures$] where `article number short`='" + model + "' and `order`='1' and `colour number`='-'", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    mainImage = dr["file"].ToString();
                    MainImage = mainImage;
                    break;
                }                
            }
            return mainImage;
        }

        public string[] getOtherImages()
        {                        
            int index;
            command = new OleDbCommand("select * from [article pictures$] where `article number short`='" + model + "' and  `file`<>'" + MainImage+ "'", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {
                index = 0;
                string[] OtherImages = new string[index];
                while (dr.Read())
                {
                    Array.Resize(ref OtherImages, index + 1);
                    OtherImages[index] = dr["file"].ToString() ;
                    index++;
                }
                return OtherImages;
            }            
        }

        public List<string[]> getCategoryPath()
        {
            string[] itemsEn;
            List<string[]> retVal = new List<string[]>();
            int index;
            command = new OleDbCommand("select * from [category$] where `article number short`='" + model + "' and `language`='english'", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {
                index = 0; 
                string[] items = new string[index];
                itemsEn = new string[index];                 
                while (dr.Read())
                {
                    Array.Resize(ref items, index + 1);
                    Array.Resize(ref itemsEn, index + 1);
                    items[index] = dr["article group"].ToString() + "///" + dr["article subgroup"].ToString();
                    itemsEn[index] = dr["article group"].ToString() + "///" + dr["article subgroup"].ToString();
                    index++;
                }
                retVal.Add(items);
            }
            command = new OleDbCommand("select * from [category$] where `article number short`='" + model + "' and `language`='german'", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {
                index = 0;
                string[] items = new string[index];
                while (dr.Read())
                {
                    Array.Resize(ref items, index + 1);
                    items[index] = dr["article group"].ToString() + "///" + dr["article subgroup"].ToString();
                    index++;
                }
                retVal.Add(items);
            }
            command = new OleDbCommand("select * from [category$] where `article number short`='" + model + "' and `language`='swedish'", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {
                index = 0;
                string[] items = new string[index];
                while (dr.Read())
                {
                    Array.Resize(ref items, index + 1);
                    items[index] = dr["article group"].ToString() + "///" + dr["article subgroup"].ToString();
                    index++;
                }
                retVal.Add(items);
                retVal.Add(itemsEn);
            }
            return retVal;
        }

        public List<int> getCategoryIDs()
        {
            List<int> CatIDs = new List<int>();           
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
            Dictionary<string, string>[] attributes= new Dictionary<string, string>[4];
            attributes[0] = new Dictionary<string, string>();
            attributes[1] = new Dictionary<string, string>();
            attributes[2] = new Dictionary<string, string>();
            attributes[3] = new Dictionary<string, string>();
            command = new OleDbCommand("select * from [acticle attributes$] where `article number short`='" + model + "' and `language`='en'", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    if (!attributes[0].ContainsKey(dr["type of article attribute"].ToString()))
                        attributes[0].Add(dr["type of article attribute"].ToString(), dr["article attribute"].ToString());
                    //break;
                    if (!attributes[3].ContainsKey(dr["type of article attribute"].ToString()))
                        attributes[3].Add(dr["type of article attribute"].ToString(), dr["article attribute"].ToString());
                    //break;
                }
            }
            command = new OleDbCommand("select * from [acticle attributes$] where `article number short`='" + model + "' and `language`='de'", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {

                while (dr.Read())
                {
                    if (!attributes[1].ContainsKey(dr["type of article attribute"].ToString()))
                        attributes[1].Add(dr["type of article attribute"].ToString(), dr["article attribute"].ToString());
                    //break;
                }
            }
            command = new OleDbCommand("select * from [acticle attributes$] where `article number short`='" + model + "' and `language`='se'", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {

                while (dr.Read())
                {
                    if (!attributes[2].ContainsKey(dr["type of article attribute"].ToString()))
                        attributes[2].Add(dr["type of article attribute"].ToString(), dr["article attribute"].ToString());
                    //break;
                }
            } 
            return attributes;
        }

        public string getRefField()
        {
            return getModel();
        }

        public List<string> getRelated()
        {
            List<string> retVal = new List<string>();
            command = new OleDbCommand("select * from [article cross-selling$] where `article number short`='" + model + "'", connection);
            using (OleDbDataReader dr = command.ExecuteReader())
            {

                while (dr.Read())
                {
                    retVal.Add(dr["link to"].ToString());
                }
            }
            return retVal;
        }

        public Dictionary<string, Dictionary<string, float>> getOptions()
        {
            
            option_list.Add("Color", color_option_list_item);
            option_list.Add("Size", size_option_list_item);
            Dictionary<string, Dictionary<string, float>> retList = new Dictionary<string, Dictionary<string, float>>(option_list);
            option_list.Clear();
            color_option_list_item = new Dictionary<string, float>();
            size_option_list_item = new Dictionary<string, float>();
            return retList;
        }

        public Dictionary<string, string> getAdditionalFields()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            return retVal;
        }
    }
}
