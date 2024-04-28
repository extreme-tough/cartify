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
namespace indoorgardenshop_nl
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

        public void SetFeedLines(List<string[]> data, bool LastLine)
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
            List<string> urls = new List<string>();
            Nodes = root.SelectNodes("//div[@id='menu']/ul/li/a");
            foreach (HAP.HtmlNode aNode in Nodes)
            {
                string href = aNode.GetAttributeValue("href", "");
                urls.Add(href);
            }
            return urls;
        }

        public List<int> getCategoryIDs()
        {
            List<int> CatIDs = new List<int>();            
            return CatIDs;
        }

        public string buildCategoryURL(string catURL, int page)
        {
            if (page > 0) 
                return "";
            else 
                return catURL;
        }

        public List<string> getItemURLs()
        {
            List<string> urls = new List<string>();
            Nodes = root.SelectNodes("//div[@class='product-grid']/div/div[@class='name']/a");
            foreach (HAP.HtmlNode aNode in Nodes)
            {
                string href = aNode.GetAttributeValue("href", "");
                urls.Add(href.Replace("Â®","®"));
            }
            return urls;
        }

        public string[] getTitles()
        {
            string[] titles = new string[1];
            aNode = root.SelectSingleNode("//h1");
            titles[0] = aNode.InnerText;
            return titles;
        }

        public string getModel()
        {
            aNode = root.SelectSingleNode("//input[@name='product_id']");
            string inText = aNode.GetAttributeValue("value","");
            Model = inText;
            return inText;
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
            return "";
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
            aNode = root.SelectSingleNode("//span[@class='price-tax']");
            string inText = aNode.InnerHtml;
            inText = inText.Replace("Excl. BTW: ","").Replace("H.T : ","");
            price = inText.Replace("€", "").Replace(",", "").Replace(".", "").Trim();
            //price = (float.Parse(price) / 100).ToString();
            return price;
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
            aNode = root.SelectSingleNode("//div[@class='description']/a");
            if (aNode == null) return "";
            string inText = aNode.InnerText;
            return inText;
        }

        public string[] getDescriptions()
        {
            string[] desc = new string[1];
            aNode = root.SelectSingleNode("//div[@id='tab-description']");
            string inText = aNode.InnerHtml;
            desc[0] = inText;
            return desc;
        }

        public string[] getKeywords()
        {
            string[] keywords = new string[0];
            return keywords;
        }
        public string getMainImage()
        {
            aNode = root.SelectSingleNode("//div[@class='image']/a/img");
            string src = aNode.GetAttributeValue("src", "");
           
            return src;
        }

        public string[] getOtherImages()
        {
            List<string> OtherImages = new List<string>();

            Nodes = root.SelectNodes("//div[@class='image-additional']/a");
            if (Nodes != null)
            {
                foreach (HAP.HtmlNode aNode in Nodes)
                {
                    string src = aNode.GetAttributeValue("href", "");
                    OtherImages.Add(src);
                }
            }
            return OtherImages.ToArray();
        }

        public List<string[]> getCategoryPath()
        {
            List<string[]> retVal = new List<string[]>();
            Nodes = root.SelectNodes("//div[@class='breadcrumb']/a");
            string[] item = new string[1] { Nodes[1].InnerText };
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
