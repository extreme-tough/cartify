using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using ParserFactory;
using HAP = HtmlAgilityPack;
namespace workwearexpress
{
    public class Importer : Parser
    {
        HAP.HtmlNodeCollection Nodes;
        HAP.HtmlNode aNode;
        HtmlAgilityPack.HtmlNode root;
        string itemURL; 
        string MainImage;
        string price;
        Dictionary<string,string> propertyCollection = new Dictionary<string,string>();
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

        public void parseFeedLine()
        {
            return;
        }

        public List<string> getCategoryURLs()
        {
            List<string> urls = new List<string>();
            return urls;
        }

        public void SetFeedLine(string[] data)
        {
        }

        public string buildCategoryURL(string catURL, int page)
        {
            currentPage = page+1; 
            if (page == 0)
                return catURL;
            else
            {
                if (catURL.Contains("/?s="))
                    return "";
                else
                    return catURL + "?pg=" + currentPage;
            }
        }

        public List<string> getItemURLs()
        {
            List<string> returnList = new List<string>();
            Nodes = root.SelectNodes("//div[@class='brok_item']/a");
            if (Nodes == null) return null;
            if (Nodes.Count==0) return null;

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
            aNode = root.SelectSingleNode("//p[@itemprop='sku']");
            string model = aNode.InnerText;
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
            aNode = root.SelectSingleNode("//span[@itemprop='lowPrice']");
            price = aNode.InnerText;
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
            return null;
        }

        public string getManufacturer()
        {
            return "";
        }

        public string getDescription()
        {
            aNode = root.SelectSingleNode("//div[@class='prod_detail_text']");
            return aNode.InnerHtml;
        }

        public string getMainImage()
        {
            aNode = root.SelectSingleNode("//img[@itemprop='image']");
            MainImage = aNode.GetAttributeValue("src", "");
            Uri aURL = new Uri(itemURL);
            MainImage = aURL.Scheme + "://" + aURL.Host  + MainImage;
            return MainImage;
        }

        public string[] getOtherImages()
        {
            string ImageURL; int i = 0, j=0;
            string[] OtherImages = new string[0];
            Nodes = root.SelectNodes("//div[@id='product_image_alternate']/img");
            if (Nodes==null) return OtherImages;
            OtherImages = new string[Nodes.Count - 1];
            foreach (HAP.HtmlNode thisNode in Nodes)
            {
                if (i == 0)
                {
                    i++;
                    continue;
                }
                ImageURL = thisNode.GetAttributeValue("src", "");
                ImageURL = ImageURL.Replace("/small/", "/large/");
                Uri aURL = new Uri(itemURL);
                ImageURL = aURL.Scheme + "://" + aURL.Host + ImageURL;
                if (ImageURL != MainImage)
                {
                    if (ImageURL != "")
                    {
                        OtherImages[j] = ImageURL;
                        j++;
                    }
                }
                i++;
            }
            return OtherImages;
        }

        public string getCategoryPath()
        {
            Nodes = root.SelectNodes("//ul[@id='breadcrumb']/li/span");
            return Nodes[1].InnerText;
        }

        public string getStock()
        {
            throw new NotImplementedException();
        }

        public string getStockStatus()
        {
            throw new NotImplementedException();
        }

        public string[] getAttributes()
        {
            throw new NotImplementedException();
        }

        public string getRefField()
        {
            aNode = root.SelectSingleNode("//p[@itemprop='identifier']");
            string model = aNode.InnerText;
            return model;
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

            aNode = root.SelectSingleNode("//div[@id='product_grid']/table/thead/tr");
            Nodes= aNode.SelectNodes("th");
            if (Nodes != null)
            {
                int i = 0;
                foreach (HAP.HtmlNode thisNode in Nodes)
                {
                    string optionName = thisNode.InnerText.Trim();
                    if (optionName == "&nbsp;") continue;
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
            Nodes = root.SelectNodes("//div[@id='product_grid']/table/tbody/tr");
            if (Nodes != null)
            {
                int i = 0;
                foreach (HAP.HtmlNode thisNode in Nodes)
                {
                    string optionName = thisNode.InnerText.Trim();
                    if (optionName == "") continue;
                    priceOpt = 0;
                    option_list_item.Add(optionName, priceOpt);
                    i++;
                }
                if (option_list_item.Count>0)
                    option_list.Add("OPTION2", option_list_item);
            }
            return option_list;
        }
    }
}
