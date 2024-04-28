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
namespace sammydress
{
    public class Importer : Parser
    {
        HAP.HtmlNodeCollection Nodes;
        HAP.HtmlNode aNode;
        HtmlAgilityPack.HtmlNode root;
        string itemURL,tempURL;        
        string MainImage;
        Dictionary<string,string> propertyCollection = new Dictionary<string,string>();
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
        public void SetFeedLine(string[] data)
        {
            Title = data[3];
            Model = data[0];
            price = data[4];
            description = data[11];
            Stock = data[1];
            tempURL = data[9];            
        }

        public void parseFeedLine()
        {

        }
        public List<string> getCategoryURLs()
        {
            throw new NotImplementedException();
        }

        public string buildCategoryURL(string catURL, int page)
        {
            throw new NotImplementedException();
        }

        public List<string> getItemURLs()
        {
            throw new NotImplementedException();
        }

        public string getTitle()
        {
            itemURL = tempURL;
            string filename = DownloadFile(itemURL);
            doc = new HAP.HtmlDocument();
            doc.Load(filename);
            var root = doc.DocumentNode;
            Document = root;
            return Title;
        }

        public string getModel()
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
            return "";
        }

        public string getDescription()
        {
            return description;
        }

        public string getMainImage()
        {
            aNode = root.SelectSingleNode("//span[@id='js_jqzoom']/img");
            MainImage = aNode.GetAttributeValue("src", "");
            return MainImage;
        }

        public string[] getOtherImages()
        {
            string ImageURL; int i = 0;
            string[] OtherImages = new string[0];
            return OtherImages;
            Nodes = root.SelectNodes("//ul[@class='js_scrollableDiv']/li/img");
            OtherImages = new string[Nodes.Count - 1];
            foreach (HAP.HtmlNode thisNode in Nodes)
            {
                ImageURL = thisNode.GetAttributeValue("data-big-img", "");
                if (ImageURL != MainImage)
                {
                    if (ImageURL != "")
                        OtherImages[i] = ImageURL;
                    i++;
                }
            }
            return OtherImages;
        }

        public string getCategoryPath()
        {
            aNode = root.SelectSingleNode("//div[@class='path']");
            string[] categoryParts=  aNode.InnerText.Split(new string[] { "&gt;" }, StringSplitOptions.None);
            string[] retValArr = new string[categoryParts.Length-1];
            int i = 0;
            foreach (string categoryPart in categoryParts)
            {             
                if (i>0)
                    retValArr[i-1] = categoryPart.Trim();
                i++;
            }
            if (retValArr.Count()>0)
                retValArr = retValArr.Take(retValArr.Count() - 1).ToArray();
            return string.Join("///", retValArr);
        }

        public string getStock()
        {
            return Stock;
        }

        public string getStockStatus()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> getAttributes()
        {
            Dictionary<string, string> Attributes = new Dictionary<string, string>();
            return Attributes;
        }

        public string getRefField()
        {
            string[] URLParts = itemURL.ToLower().Split(new string[] { "/" }, StringSplitOptions.None);
            return URLParts[3].Replace("product", "").Replace(".html", ""); ;

        }

        public List<int> getRelated()
        {
            return null;
        }

        public Dictionary<string, Dictionary<string, float>> getOptions()
        {
            //string[][] option_list = new string[][] { };
            float priceOpt;
            Dictionary<string, Dictionary<string, float>> option_list = new Dictionary<string, Dictionary<string, float>>();
            //Tuple<string, string, float>[] option_list = new Tuple<string, string, float>[0];

            Dictionary<string, float> option_list_item = new Dictionary<string,float>();

            //if (root.SelectSingleNode("//table[@id='table_select_size']") == null) return option_list;

            Nodes = root.SelectNodes("//ul[@id='js_property_size']/li");
            if (Nodes != null)
            {
                int i = 0;
                foreach (HAP.HtmlNode thisNode in Nodes)
                {
                    string optionName = thisNode.InnerText.Trim();
                    if (optionName == "") continue;
                    priceOpt =0;
                    if (!option_list_item.ContainsKey(optionName))
                    {
                        option_list_item.Add(optionName, priceOpt);
                        i++;
                    }
                }
                if (option_list_item.Count > 0)
                    option_list.Add("OPTION1", option_list_item);
            }
            option_list_item = new Dictionary<string, float>();
            Nodes = root.SelectNodes("//ul[@id='js_property_color']/li/a");
            if (Nodes != null)
            {
                int i = 0;
                foreach (HAP.HtmlNode thisNode in Nodes)
                {
                    string optionName = thisNode.GetAttributeValue("title", "");
                    if (optionName == "") continue;
                    priceOpt = 0;
                    if (!option_list_item.ContainsKey(optionName))
                        option_list_item.Add(optionName, priceOpt);
                    i++;
                }
                if (option_list_item.Count > 0)
                    option_list.Add("OPTION2", option_list_item);
            }
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
    }
}
