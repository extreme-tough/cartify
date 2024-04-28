using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using ParserFactory;
using HAP = HtmlAgilityPack;
using System.Net;
namespace dropshipspecialist
{
    public class Importer : Parser
    {
        HAP.HtmlNodeCollection Nodes;
        HAP.HtmlNode aNode;
        HtmlAgilityPack.HtmlNode root;
        string itemURL,tempURL;        
        string MainImage;
        Dictionary<string,string> propertyCollection = new Dictionary<string,string>();
        Dictionary<string, string> dataCollection = new Dictionary<string, string>();
        string price, description;
        HAP.HtmlDocument doc;        
        string Title,Model,Stock;
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
                propertyCollection = value;
            }
        }

        public HtmlAgilityPack.HtmlNode Document
        {
            set
            {
                this.root = value;
            }
        }
        public void SetFeedLine(string[] data, bool LastLine)
        {
            
        }

        public void SetFeedLine(Dictionary<string,string> data, bool LastLine)
        {
            dataCollection = data;
        }

        public void parseFeedLine()
        {

        }
        public List<string> getCategoryURLs()
        {
            throw new NotImplementedException();
        }

        public List<int> getCategoryIDs()
        {
            List<int> CatIDs = new List<int>();            
            return CatIDs;
        }

        public string buildCategoryURL(string catURL, int page)
        {
            throw new NotImplementedException();
        }

        public List<string> getItemURLs()
        {
            throw new NotImplementedException();
        }

        public string[] getTitles()
        {
            string[] titles = new string[1];
            titles[0] = dataCollection["naam"];
            return titles;
        }

        public string getModel()
        {
            Model = dataCollection["ean"];
            return Model;
        }

        public string getRefField()
        {
            return Model;
        }

        public string getUPC()
        {
            return "";
        }

        public string getSKU()
        {
            return dataCollection["sku"];
        }

        public string getMPN()
        {
            return "";
        }

        public string getEAN()
        {
            return Model;
        }

        public string getISBN()
        {
            return "";
        }

        public string getLocation()
        {
            return "";
        }

        public string getWeight()
        {
            return "0";
        }

        public string getPrice()
        {
            return dataCollection["adviesprijs"];
        }

        public string getSpecial()
        {
            return "";
        }

        public Dictionary<string, string>[] getSpecialPrices(string[] customerGroups)
        {
            return new Dictionary<string, string>[0];
        }

        public string getManufacturer()
        {
            return "";
        }

        public string[] getDescriptions()
        {
            string[] desc = new string[1];
            desc[0] = dataCollection["beschrijving"];
            return desc;
        }

        public string[] getKeywords()
        {
            string[] keywords = new string[0];
            return keywords;
        }
        public string getMainImage()
        {
            if (dataCollection.ContainsKey("afbeelding1"))
                return dataCollection["afbeelding1"];
            else
                return "";
        }

        public string[] getOtherImages()
        {
            string ImageURL; int i = 2;
            List<string> OtherImages = new List<string>();
            
            while(true)
            {
                if (dataCollection.ContainsKey("afbeelding" + i.ToString()))
                {
                    OtherImages.Add(dataCollection["afbeelding" + i.ToString()]);
                    i++;
                }
                else break;
            }
            return OtherImages.ToArray();
        }

        public List<string[]> getCategoryPath()
        {
            List<string[]> retVal = new List<string[]>();
            int i = 1;
            List<string> cat = new List<string>();
            while (true)
            {
                if (dataCollection.ContainsKey("categorie" + i.ToString()))
                {
                    cat.Add(dataCollection["categorie" + i.ToString()]);
                    i++;
                }
                else break;
            }
            string[] catArray = cat.ToArray();
            string[] item = new string[1] { String.Join("///", catArray) };
            retVal.Add(item);
            return retVal;
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
            Dictionary<string, string>[] attributes = new Dictionary<string, string>[0];
            return attributes;
        }



        public List<string> getRelated()
        {
            List<string> retVal = new List<string>();
            return retVal;
        }

        public Dictionary<string, Dictionary<string, float>> getOptions()
        {
            //string[][] option_list = new string[][] { };
            
            Dictionary<string, Dictionary<string, float>> option_list = new Dictionary<string, Dictionary<string, float>>();
           
            return option_list;
        }
        private string DownloadFile(string url)
        {
            WebClient client;
            client = new System.Net.WebClient();         
            string filename = System.IO.Path.GetTempFileName();
            client.DownloadFile(url, filename);
            return filename;
        }

        public string[] getTags()
        {
            string[] keywords = new string[0];
            return keywords;
        }

        public Dictionary<string, string> getAdditionalFields()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            return retVal;
        }
    }
}
