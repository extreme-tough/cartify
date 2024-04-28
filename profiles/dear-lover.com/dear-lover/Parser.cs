using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using HAP = HtmlAgilityPack;

namespace AllImporterPro
{
    class Parser
    {
        public HAP.HtmlNode root;
        private HAP.HtmlNode aNode;
        private HAP.HtmlNodeCollection Nodes;
        string Brand,Collection,Style,Color,EAN,Location;
        string MainImage;
        public string URL;

        public Parser()
        {
        }
        public List<string> getCategoryURLs()
        {
            //Required only for full site import
            List<string> urls = new List<string>();
            urls.Add("http://www.dear-lover.com/Sexy-Lingerie/");
            urls.Add("http://www.dear-lover.com/wholesale-fashion-dresses/");
            urls.Add("http://www.dear-lover.com/Sexy-Costumes/");
            urls.Add("http://www.dear-lover.com/wholesale-swimwear/");
            urls.Add("http://www.dear-lover.com/Sexy-Lingerie/Sexy-Clubwear/");
            urls.Add("http://www.dear-lover.com/Sexy-Lingerie/Panties-G-String/");
            urls.Add("http://www.dear-lover.com/wholesale-womens-clothes/");
            urls.Add("http://www.dear-lover.com/Sexy-Lingerie/Leg-Wear-Stockings/");
            urls.Add("http://www.dear-lover.com/Sexy-Costumes/Accessories-Costume/");
            urls.Add("http://www.dear-lover.com/wholesale-Sexy-Jewelry/");
            urls.Add("http://www.dear-lover.com/wholesale-mens-clothing/");
            urls.Add("http://www.dear-lover.com/wholesale-baby-kids-clothes/");
            urls.Add("http://www.dear-lover.com/Apparel-Clearance/");
            return urls;
        }

        public string buildCategoryURL(string catURL, int page)
        {
            if (page == 1)
                return catURL;
            else
            {
                return catURL + "index_" + page.ToString() + ".html";
            }
        }
        public List<string> getItemURLs(int page)
        {
            List<string> returnList = new List<string>();
            //page++;

            HAP.HtmlNode Node = root.SelectSingleNode("//span[@class='pageon']");
            string pageNo = Node.InnerText;

            if (pageNo == page.ToString())
            {
                Nodes = root.SelectNodes("//ul[@class='products']/li/a[@class='pic']");
                foreach (HAP.HtmlNode node in Nodes)
                {
                    string url = node.GetAttributeValue("href", "");
                    if (url!="html")
                        returnList.Add("http://www.dear-lover.com" + url);
                }
            }
            return returnList;
        }

        public string getTitle()
        {
            aNode = root.SelectSingleNode("//div[@class='pro_base_info']/h1");
            string Title = HttpUtility.HtmlDecode( aNode.InnerText);

            return Title;
        }

        public string getManufacturer()
        {
		    return Brand;
        }
        public string getModel()
        {
            aNode = root.SelectSingleNode("//ul[@class='pro_base_info_list']/li");
            string Model = HttpUtility.HtmlDecode(aNode.InnerText);
            return Model.Replace("Item No. :  ","");
	    }
        public string getWeight()
        {
            Nodes = root.SelectNodes ("//ul[@class='pro_base_info_list']/li");
            foreach (HAP.HtmlNode Node in Nodes)
            {
                if (Node.InnerText.StartsWith("Weight About:"))
                {
                    string weight = Node.InnerText.Replace("Weight About: ","").Replace(" KG","");
                    string[] weightParts = weight.Split(new string[] { "(" }, StringSplitOptions.None);
                    return weightParts[0];
                }
            }
            return "";
        }
        public string getVolume()
        {
            Nodes = root.SelectNodes("//ul[@class='pro_base_info_list']/li/span");
            if (Nodes != null)
            {
                foreach (HAP.HtmlNode Node in Nodes)
                {
                    if (Node.InnerText.StartsWith("(Volumetric Weight About: : "))
                    {
                        string weight = Node.InnerText.Replace("(Volumetric Weight About: : ", "").Replace(" KG", "");
                        string[] weightParts = weight.Split(new string[] { ")" }, StringSplitOptions.None);
                        return weightParts[0];
                    }
                }
            }
            return "";
        }
        public string getUPC()
        {
            return "";
        }
        public string getCategoryPath()
        {
            aNode = root.SelectSingleNode("//div[@class='daohang']");
            string catPath = HttpUtility.HtmlDecode(aNode.InnerText);

            return catPath;
        }
        public string getSKU()
        {
            return "";
        }
	    public string getMPN(){
		    return "";
	    }
        public string getEAN(){
            return EAN;
	    }
	    public string getISBN(){
		    return Color;
	    }
        public string getLocation(){
		    return Location;
        }
        public string getPrice()
        {
            string thisprice;
            aNode = root.SelectSingleNode("//span[@class='red']");
            string[] priceParts = aNode.InnerText.Split(new string[] { " ~ " }, StringSplitOptions.None);
            if (priceParts.Length > 1)
                thisprice = priceParts[1];
            else
                thisprice = priceParts[0];
            return thisprice.Replace("US$", "");
	    }
        public string getSave()
        {
            Nodes = root.SelectNodes("//ul[@class='pro_base_info_list']/li/strong");
            if (Nodes != null)
            {
                foreach (HAP.HtmlNode Node in Nodes)
                {
                    if (Node.InnerText.StartsWith("Save US$ "))
                    {
                        string save = Node.InnerText.Replace("Save US$ ", "");
                        return save;
                    }
                }
            }
            return "";
        }
        public string getMaterial()
        {
            Nodes = root.SelectNodes("//ul[@class='pro_base_info_list']/li");
            if (Nodes != null)
            {
                foreach (HAP.HtmlNode Node in Nodes)
                {
                    if (Node.InnerText.StartsWith("Material : "))
                    {
                        string material = Node.InnerText.Replace("Material : ", "");
                        return material;
                    }
                }
            }
            return "";
        }
        public string getSize()
        {
            aNode = root.SelectSingleNode("//span[@id='span_style_0_0']");
            if (aNode != null)
            {
                string size = aNode.InnerText;
                return size;
            }
            return "";
        }
        public List<string[]> getStock()
        {
            List<string[]> optionList = new List<string[]>();


            string[] stockParts = root.InnerHtml.Split(new string[] { "setProductStock('{" }, StringSplitOptions.None);
            stockParts = stockParts[1].Split(new string[] { "}')" }, StringSplitOptions.None);            
            string options = stockParts[0];
            string[] optionData = options.Split(new string[] { "," }, StringSplitOptions.None);
            string[] optionParts;
            foreach (string option in optionData)
            {
                optionParts = option.Split(new string[] { ":" }, StringSplitOptions.None);
                if (optionParts[0] == "\"os\"") optionParts[0] = "One Size";
                optionParts[0] = optionParts[0].Replace("\"", "");
                optionParts[1] = optionParts[1].Replace("\"", "");
                if (optionParts[1].StartsWith("-"))
                    optionParts[1] = "0";
                optionList.Add(optionParts);
            }
            return optionList;
        }
        public string getDownloadLink()
        {
            Nodes = root.SelectNodes("//div[@class='pview_tab_info']/div[@class='pview_info']/a");
            string URL="";
            if (Nodes!=null)
                URL = Nodes[0].GetAttributeValue("href", "");

            return URL;
        }
        public float getSpecial()
        {
            return 0;
        }
        public string getDescription()
        {
            aNode = root.SelectSingleNode("//div[@class='pview_tab_info']/div");
            return aNode.InnerText;
        }
        public string getMainImage()
        {
            string ImageURL = root.SelectSingleNode("//a[@rel='gal1']").GetAttributeValue("href", "");
            MainImage = ImageURL;
            return ImageURL;
        }
        public string[] getOtherImages()
        {
            string ImageURL;    int i=0;
            string[] OtherImages = new string[0] ;
            if (root.SelectSingleNode("//div[@class='prod_images_thumb']") == null) return OtherImages;
            Nodes = root.SelectSingleNode("//div[@class='prod_images_thumb']").SelectSingleNode("ul").SelectNodes("li");
            OtherImages = new string[Nodes.Count-1];
            foreach (HAP.HtmlNode thisNode in Nodes)
            {
                ImageURL = thisNode.SelectSingleNode("a").GetAttributeValue("data-full", "");
                if (ImageURL != MainImage)
                {
                    if (ImageURL != "")
                        OtherImages[i] = ImageURL;
                    i++;
                }
            }
            return OtherImages;
	    }
        public string getStockStatus(){
            return ImportConfig.OUTOFSTOCK_STATUS;
	    }
        public string[] getAttributes()
        {
            return null;
        }
        public string getRefField()
        {
            string[] URLparts = URL.Split(new string[] { "/" }, StringSplitOptions.None);
            string refField = URLparts[URLparts.Length - 1];
            return refField;
        }
        public List<int> getRelated()
        {
            string itemURL;
            string[] URLparts;
            string refField;
            int product_id;
            List<int> related = new List<int>();
            if (root.SelectSingleNode("//div[@class='prod_color_thumb']") == null) return related;
            Nodes = root.SelectSingleNode("//div[@class='prod_color_thumb']").SelectSingleNode("ul").SelectNodes("li");
            foreach (HAP.HtmlNode thisNode in Nodes)
            {
                itemURL = thisNode.SelectSingleNode("a").GetAttributeValue("href", "");
                if (itemURL != "")
                {
                    URLparts = itemURL.Split(new string[] { "/" }, StringSplitOptions.None);
                    refField = URLparts[URLparts.Length - 1];
                    product_id = 0;
                    if (product_id!=0)
                        related.Add(product_id);                    
                }
            }
            return related;
        }
        public Dictionary<string, Dictionary<string, float>> getOptions()
        {
            //string[][] option_list = new string[][] { };
            Dictionary<string, Dictionary<string, float>> option_list = new Dictionary<string, Dictionary<string, float>>();
            //Tuple<string, string, float>[] option_list = new Tuple<string, string, float>[0];

            Dictionary<string, float> option_list_item = new Dictionary<string,float>();

            if (root.SelectSingleNode("//table[@id='table_select_size']") == null) return option_list;

            Nodes = root.SelectSingleNode("//table[@id='table_select_size']").SelectSingleNode("tbody").SelectNodes("tr");
            int i=0;
            foreach (HAP.HtmlNode thisNode in Nodes)
            {
                string optionName = thisNode.SelectNodes("td")[1].InnerText.Trim();
                float priceOpt = float.Parse(thisNode.SelectNodes("td")[2].InnerText.Trim().Trim(new char[] { '$' }));                
                option_list_item.Add(optionName,priceOpt);
                i++;    
            }
            option_list.Add(ImportConfig.OPTION_NAME, option_list_item);
            return option_list;
        }
    }
}
