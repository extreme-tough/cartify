using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.Net;
using ParserFactory;
using HAP = HtmlAgilityPack;
using Newtonsoft.Json;
namespace tmart
{
    public class Importer : Parser 
    {
        HAP.HtmlNodeCollection Nodes;
        HAP.HtmlNode aNode;
        HtmlAgilityPack.HtmlNode root;
        string itemURL;
        string MainImage;
        Dictionary<string,string> propertyCollection = new Dictionary<string,string>();
        string price;
        string[] otherImages ;
        int currentPage;
        public string URL 
        {            
            set
            {
                itemURL = value;
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
        public string FeedFile
        {
            set
            {
            }
        }
        public void parseFeedLine()
        {

        }
        public List<string> getCategoryURLs()
        {
            string[] catURLs = new string[12];
            List<string> retVal = new List<string>();
            HAP.HtmlDocument doc;        
            doc = new HAP.HtmlDocument();
            string filename;
            string link;
            catURLs[0] = "http://www.tmart.com/Electronics/";
            catURLs[1] = "http://www.tmart.com/Computers-Networking/";
            catURLs[2] = "http://www.tmart.com/Flashlight-Lamp/";
            catURLs[3] = "http://www.tmart.com/Cell-Phones/";
            catURLs[4] = "http://www.tmart.com/Automotive/";
            catURLs[5] = "http://www.tmart.com/Health-Beauty/";
            catURLs[6] = "http://www.tmart.com/Fashion/";
            catURLs[7] = "http://www.tmart.com/Home-Office-Garden/";
            catURLs[8] = "http://www.tmart.com/Toys-Hobbies/";
            catURLs[9] = "http://www.tmart.com/Sports-Outdoors/";
            catURLs[10] = "http://www.tmart.com/Novelties/";
            catURLs[11] = "http://www.tmart.com/Holiday-Supplies-Gifts/";
            foreach (string catURL in catURLs)
            {
                filename = DownloadFile(catURL);
                doc.Load(filename);
                var root = doc.DocumentNode;
                Nodes = root.SelectNodes("//div[@class='second-level-menu  box']/ul/li");
                foreach (HAP.HtmlNode thisNode in Nodes)
                {
                    HAP.HtmlNodeCollection children = thisNode.SelectNodes("div/ul/li");
                    if (children != null)
                    {
                        foreach (HAP.HtmlNode menu in children)
                        {
                            link = menu.SelectSingleNode("a").GetAttributeValue("href", "");
                            retVal.Add(link);
                        }
                    }
                    else
                    {
                        link  =thisNode.SelectSingleNode("a").GetAttributeValue("href", "");
                        retVal.Add(link);
                    }
                }
            }
            return retVal;
        }
        public List<int> getCategoryIDs()
        {
            List<int> CatIDs = new List<int>();
            return CatIDs;
        }
        public string buildCategoryURL(string catURL, int page)
        {
            if (page > 1)
            {
                catURL = catURL.Substring(0, catURL.Length - 1);
                catURL = catURL + "-20" + page.ToString() + "gallery/";
                return catURL;
            }
            else
                return catURL;
        }

        public List<string> getItemURLs()
        {
            List<string> retURLs = new List<string>();
            Nodes = root.SelectNodes("//div[@class='list-name  bottom-margin4px']/a");
            foreach (HAP.HtmlNode thisNode in Nodes )
            {
                string href = thisNode.GetAttributeValue("href", "");
                retURLs.Add(href);
            }
            return retURLs;
        }

        public string[] getTitles()
        {
            HAP.HtmlNode thisNode;
            aNode = root.SelectSingleNode("//h1");
            return new string[] { aNode.InnerText.Trim() };
        }

        public string getModel()
        {
            aNode = root.SelectSingleNode("//input[@name='products_id']");
            if (aNode == null) return "";
            string model = aNode.GetAttributeValue("value","");
            return model;
        }

        public string getUPC()
        {
            return "";
        }

        public string getSKU()
        {
            return getModel();
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

        public string getWeight()
        {
            return "";
        }

        public string getPrice()
        {
            aNode = root.SelectSingleNode("//span[@class='J_pprice']/span[@class='font24  font-red']");
            if (aNode != null)
                price = aNode.InnerText.Replace("$", "");
            else
            {
                price = "0.0";
            }
            /*
            string[] priceParts = root.InnerText.Split(new string[] {"ecomm_totalvalue: " }, StringSplitOptions.None);
            priceParts = priceParts[1].Split(new string[] { "," }, StringSplitOptions.None);
            price = priceParts[0];*/
            return price.Trim();
        }

        public string getSpecial()
        {
            return getPrice();
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
            string[] descriptions = new string[1];
            aNode = root.SelectSingleNode("//div[@id='description']");
            descriptions[0] = aNode.InnerHtml;
            return descriptions;
        }

        public string getMainImage()
        {
            Nodes = root.SelectNodes("//div[@id='products_image_box']/ul/li/a/img");
            if (Nodes == null) return "";
            otherImages = new string[Nodes.Count - 1];
            string MainImage = "";
            int i=0;
            foreach (HAP.HtmlNode aNode in Nodes)
            {
                if (MainImage == "")
                {
                    string[] stringParts = aNode.GetAttributeValue("data-large", "").Split(new string[] { "?" }, StringSplitOptions.None);
                    MainImage = stringParts[0];
                }
                else
                {
                    string[] stringParts = aNode.GetAttributeValue("data-large", "").Split(new string[] { "?" }, StringSplitOptions.None);
                    otherImages[i] = stringParts[0];
                    i++;   
                }
            }
            return MainImage;
        }

        public string[] getKeywords()
        {
            return null;
        }

        public string[] getOtherImages()
        {

            return otherImages;
        }

        public List<string[]> getCategoryPath()
        {
            List<string[]> retVal = new List<string[]>();
            Nodes = root.SelectNodes("//ul[@class='breadcrumb']/li/a");
            if (Nodes == null) return retVal;            
            string catPath = "";
            int i = 0;
            foreach (HAP.HtmlNode aNode in Nodes)
            {
                if (i == 0) { i++; continue; }
                if (i == 1)
                    catPath = catPath + aNode.InnerText;
                else
                    catPath = catPath + "///" + aNode.InnerText;
                i++;
            }
            retVal.Add ( new string[] { catPath });
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
            Dictionary<string, string>[] Attributes = new Dictionary<string, string>[1];
            string key,value;
           /* Nodes = root.SelectNodes("//table[@id='product-attribute-specs-table']/tbody/tr");
            foreach (HtmlAgilityPack.HtmlNode aNode in Nodes)
            {
                key = aNode.SelectSingleNode("th").InnerText;
                value = aNode.SelectSingleNode("td").InnerText;
                if (!Attributes.ContainsKey(key))
                    Attributes.Add(key, value);
            }*/
            return Attributes;
        }

        public string getRefField()
        {
            return getModel();
        }

        public List<string> getRelated()
        {
            return null;
        }

        public string[] getTags()
        {
            return null;
        }
        public Dictionary<string, Dictionary<string, float>> getOptions()
        {
            //string[][] option_list = new string[][] { };
            string optionName;
            Dictionary<string, Dictionary<string, float>> option_list = new Dictionary<string, Dictionary<string, float>>();
            //Tuple<string, string, float>[] option_list = new Tuple<string, string, float>[0];

            Dictionary<string, float> option_list_item = new Dictionary<string, float>();

            Nodes = root.SelectNodes("//div[@class='bottom-margin10px  clearfix J_attrs_main']/div/ul/li/a/img");
            if (Nodes!=null) {
                foreach (HAP.HtmlNode aNode in Nodes)
                {
                    optionName = aNode.GetAttributeValue("title", "");
                    option_list_item.Add(optionName, 0);
                }
            }
            if (option_list_item.Count>0)
                option_list.Add("Color", option_list_item);

            option_list_item = new Dictionary<string, float>();
            Nodes = root.SelectNodes("//div[@class='bottom-margin15px  clearfix J_attrs_sub']/div/ul/li/a/b");
            if (Nodes != null)
            {
                foreach (HAP.HtmlNode aNode in Nodes)
                {
                    optionName = aNode.InnerText;
                    option_list_item.Add(optionName, 0);
                }
            }
            if (option_list_item.Count > 0)
                option_list.Add("Size", option_list_item);

            return option_list;
        }

        public Dictionary<string, string> getAdditionalFields()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            return retVal;
        }

        private string DownloadFile(string url)
        {
            WebClient client;
            client = new System.Net.WebClient();
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:43.0) Gecko/20100101 Firefox/43.0");
            client.Headers.Add("Host", "www.tmart.com");
            client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            string filename = System.IO.Path.GetTempFileName();
            client.DownloadFile(url, filename);
            return filename;
        }
    }
}
