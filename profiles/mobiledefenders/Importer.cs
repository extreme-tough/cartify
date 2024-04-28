using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using ParserFactory;
using HAP = HtmlAgilityPack;
using Newtonsoft.Json;
namespace mobiledefenders
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
        public void SetFeedLine(string[] data)
        {
            
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
            if (page>1) return "";
            else
                return catURL;
        }

        public List<string> getItemURLs()
        {
            List<string> retURLs = new List<string>();
            Nodes = root.SelectNodes("//li[@itemtype='http://schema.org/Product']/h2/a");
            foreach (HAP.HtmlNode thisNode in Nodes )
            {
                string href = thisNode.GetAttributeValue("href", "");
                retURLs.Add(href);
            }
            return retURLs;
        }

        public string getTitle()
        {
            HAP.HtmlNode thisNode;
            aNode = root.SelectSingleNode("//h1[@itemprop='name']");
            thisNode = aNode ?? HAP.HtmlNode.CreateNode("<h1></h1>");
            return thisNode.InnerText;
        }

        public string getModel()
        {
            aNode = root.SelectSingleNode("//div[@class='md_attributes']/p");
            if (aNode == null) return "";
            string model = aNode.InnerText.Replace("SKU:", "");
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

        public string getPrice()
        {
            aNode = root.SelectSingleNode("//span[@class='regular-price']/span[@class='price']");
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
            return price;
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

        public string getDescription()
        {
            aNode = root.SelectSingleNode("//div[@id='product_tabs_additional_contents']");
            return aNode.InnerHtml;
        }

        public string getMainImage()
        {
            aNode = root.SelectSingleNode("//a[@class='cloud-zoom']");
            if (aNode == null) return "";
            MainImage = aNode.GetAttributeValue("href", "");
            return MainImage.Split(new string[]{"?"},StringSplitOptions.None)[0];
        }

        public string[] getOtherImages()
        {
            string ImageURL; int i = 0;
            string[] OtherImages = new string[0];
            //return OtherImages;
            Nodes = root.SelectNodes("//a[@class='cloud-zoom-gallery']");
            OtherImages = new string[Nodes.Count];
            foreach (HAP.HtmlNode thisNode in Nodes)
            {
                ImageURL = thisNode.GetAttributeValue("href", "");
                if (ImageURL != MainImage)
                {
                    if (ImageURL != "")
                        OtherImages[i] = ImageURL.Split(new string[] { "?" }, StringSplitOptions.None)[0];
                    i++;
                }
            }
            return OtherImages;
        }

        public string getCategoryPath()
        {
            string retCat = "";

            return retCat;
        }

        public string getStock()
        {
            aNode = root.SelectSingleNode("//span[@class='erp-in-stock']");
            if (aNode == null) return "0";
            if (aNode.InnerText == "In Stock") return "99"; else return "0";
        }

        public string getStockStatus()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> getAttributes()
        {
            Dictionary<string, string> Attributes = new Dictionary<string, string>();
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

            Nodes = root.SelectNodes("//div[@class='psku2-conf-value']/span");
            HtmlAgilityPack.HtmlNode DIVNode = root.SelectSingleNode("//div[@class='psku2-conf-value']");
            if (DIVNode == null) return option_list;
            string OptionCap = DIVNode.GetAttributeValue("data-attr-name", "");
            int startPos = root.InnerHtml.IndexOf("variationObj        =");
            int endPos = root.InnerHtml.IndexOf(",\n                    ",startPos);            
            string jSonData = root.InnerHtml.Substring(startPos + 21, endPos - startPos - 21);
            dynamic stuff = JsonConvert.DeserializeObject(jSonData);
            if (Nodes != null)
            {
                int i = 0,j=0;
                foreach (HAP.HtmlNode thisNode in Nodes)
                {
                    string optionName = thisNode.GetAttributeValue("data-" + OptionCap.ToLower(), "");
                    if (optionName == "") continue;
                    priceOpt =0;                    
                    if (!option_list_item.ContainsKey(optionName))
                    {
                        option_list_item.Add(optionName, float.Parse(Convert.ToString(stuff[j].articles[0].new_price.Value)));
                        i++;
                    }
                    j++;
                }
                if (option_list_item.Count > 0)
                    option_list.Add(OptionCap, option_list_item);
            }
            /* option_list_item = new Dictionary<string, float>();
            Nodes = root.SelectNodes("//ul[@id='js_property_color']/li/a");
            if (Nodes != null)
            {
                int i = 0;
                foreach (HAP.HtmlNode thisNode in Nodes)
                {
                    string optionName = thisNode.GetAttributeValue("title", "");
                    if (optionName == "") continue;
                    priceOpt = 0;
                    option_list_item.Add(optionName, priceOpt);
                    i++;
                }
                if (option_list_item.Count>0)
                    option_list.Add("OPTION2", option_list_item);
            }*/
            return option_list;
        }
    }
}
