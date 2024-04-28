using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using ParserFactory;
using HAP = HtmlAgilityPack;
namespace sammydress
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
            List<string> urls = new List<string>();
            urls.Add("http://www.sammydress.com/Wholesale-Women-b-1.html");
            urls.Add("http://www.sammydress.com/Wholesale-Men-b-89.html");
            urls.Add("http://www.sammydress.com/Wholesale-Jewelry-b-159.html");
            urls.Add("http://www.sammydress.com/Wholesale-Bags-b-44.html");
            urls.Add("http://www.sammydress.com/Wholesale-Shoes-b-170.html");
            urls.Add("http://www.sammydress.com/Wholesale-Watches-b-308.html");
            urls.Add("http://www.sammydress.com/Wholesale-Beauty-Accessories-b-11188.html/");
            urls.Add("http://www.sammydress.com/Wholesale-Home-Living-b-145.html");
            urls.Add("http://www.sammydress.com/Wholesale-Home-Living-b-145.html");
            urls.Add("http://www.sammydress.com/Wholesale-Wedding-Events-b-209.html");
            return urls;
        }

        public string buildCategoryURL(string catURL, int page)
        {
            currentPage = page+1; 
            if (page == 0)
                return catURL;
            else
            {
                return catURL.Replace(".html", "-page-" + currentPage.ToString() + ".html");
            }
        }

        public List<string> getItemURLs()
        {
            List<string> returnList = new List<string>();
            if (root.SelectSingleNode("//span[@class='current']")!=null)
                if (root.SelectSingleNode("//span[@class='current']").InnerText!=currentPage.ToString()) return null;

            Nodes = root.SelectNodes("//ul[@id='js_cateListUl']/li/p[@class='proName']/a");
            Debug.WriteLine(Nodes.Count + " Nodes");
            foreach (HAP.HtmlNode node in Nodes)
            {
                string url = node.GetAttributeValue("href", "");
                if (url != "")                                    
                    returnList.Add(url);                
            }
            return returnList;
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
            aNode = root.SelectSingleNode("//input[@id='save_goodsId']");
            string model = aNode.GetAttributeValue("value", "0");
            return model;
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
            aNode = root.SelectSingleNode("//span[@itemprop='price']");
            price = aNode.GetAttributeValue("content", "");
            /*
            string[] priceParts = root.InnerText.Split(new string[] {"ecomm_totalvalue: " }, StringSplitOptions.None);
            priceParts = priceParts[1].Split(new string[] { "," }, StringSplitOptions.None);
            price = priceParts[0];*/
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
            aNode = root.SelectSingleNode("//div[@class='clearfix goodsInfoWarp mb20 js_p_infoBlack ']");
            int startPos = aNode.InnerHtml.IndexOf("<div");
            return aNode.InnerHtml.Substring(startPos);
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
            throw new NotImplementedException();
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
    }
}
