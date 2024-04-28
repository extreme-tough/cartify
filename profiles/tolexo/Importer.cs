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
using Newtonsoft.Json.Linq;
namespace tolexo
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
        public void SetFeedLine(string[] data)
        {
            
        }
        public string[] getKeywords()
        {
            string[] keywords = new string[0];
            return keywords;
        }

        public void parseFeedLine()
        {

        }
        public List<string> getCategoryURLs()
        {
            List<string> urls = new List<string>();
            Nodes = root.SelectNodes("//div[@class='all-category-box']/a");
            Debug.WriteLine(Nodes.Count + " Nodes");
            foreach (HAP.HtmlNode node in Nodes)
            {
                urls.Add( node.GetAttributeValue("href", ""));
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
            string retURL = catURL + "?p=" + page.ToString();
            return retURL;  
        }

        public List<string> getItemURLs()
        {
            List<string> returnList = new List<string>();

            Nodes = root.SelectNodes("//a[@class='fav-item item']");
            Debug.WriteLine(Nodes.Count + " Nodes");
            foreach (HAP.HtmlNode node in Nodes)
            {
                string url = node.GetAttributeValue("href", "");
                if (url != "")                                    
                    returnList.Add(url);                
            }
            return returnList;
        }

        public string[] getTags()
        {
            string[] keywords = new string[0];
            return keywords;
        }

        public string[] getTitles()
        {
            string[] titles = new string[1];
            HAP.HtmlNode thisNode;
            aNode = root.SelectSingleNode("//h1[@class='prd-name']");
            //thisNode = aNode ?? HAP.HtmlNode.CreateNode("<h1></h1>");
            titles[0] = aNode.InnerText;
            return titles;
        }

        public string getModel()
        {
            aNode = root.SelectSingleNode("//input[@name='tsin']");
            if (aNode == null) return "";
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
            aNode = root.SelectSingleNode("//div[@class='pdp-sp']/small");
            
            float price = float.Parse(aNode.InnerText.Replace("Rs.",""))/62;                
            return price.ToString();
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
            aNode = root.SelectSingleNode("//section[@class='prds-spec-box clearfix mob-spec-review-content']");
            string[] desc = new string[1];
            desc[0] = aNode.InnerHtml;
            return desc;
        }

        public string getMainImage()
        {
            aNode = root.SelectSingleNode("//div[@class='gallery-img']/img");
            if (aNode == null) return "";
            MainImage = aNode.GetAttributeValue("data-src", "");
            return MainImage.Split(new string[]{"?"},StringSplitOptions.None)[0];
        }

        public string[] getOtherImages()
        {
            string ImageURL; int i = 0;
            string[] OtherImages = new string[0];
            //return OtherImages;
            Nodes = root.SelectNodes("//div[@class='gallery-img']/img");
            OtherImages = new string[Nodes.Count - 1];
            foreach (HAP.HtmlNode thisNode in Nodes)
            {
                ImageURL = thisNode.GetAttributeValue("data-src", "");
                if (ImageURL != MainImage)
                {
                    if (ImageURL != "")
                        OtherImages[i] = ImageURL.Split(new string[] { "?" }, StringSplitOptions.None)[0];
                    i++;
                }
            }
            return OtherImages;
        }

        public List<string[]> getCategoryPath()
        {
            Nodes = root.SelectNodes("//div[@typeof='BreadcrumbList']/div/a");            
            int i = 0;
            List<string[]> retCat = new List<string[]>();
            char[] charsToTrim = { ' ', '\t','\n' };
            string catPath = "";
            foreach (HtmlAgilityPack.HtmlNode aNode in Nodes)
            {
                if (i == Nodes.Count - 1) break;
                if (i > 0)
                    if (i > 1)
                        catPath = catPath + "///" + aNode.InnerText.Replace("/", "").Trim();
                    else
                        catPath = aNode.InnerText.Replace("/", "").Trim() ;
                i++;
            }
            retCat.Add(new string[1] { catPath });
            return retCat;
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
            Nodes = root.SelectNodes("//section[@class='prds-spec-sec']/div");    
            Attributes[0] = new Dictionary<string, string>();
            foreach (HtmlAgilityPack.HtmlNode aNode in Nodes)
            {
                key = aNode.SelectSingleNode("strong ").InnerText;
                if (aNode.SelectSingleNode("div").InnerHtml.IndexOf("<") >= 0)
                    value = aNode.SelectSingleNode("div").InnerHtml.Substring(0, aNode.SelectSingleNode("div").InnerHtml.IndexOf("<")).Trim();
                else
                    value = aNode.SelectSingleNode("div").InnerText.Trim();
                if (!Attributes[0].ContainsKey(key))
                    Attributes[0].Add(key, value);
            }
            return Attributes;
        }

        public string getRefField()
        {
            return getModel();
        }

        public List<string> getRelated()
        {
            List<string> retVal = new List<string>();
            return retVal;
        }

        public Dictionary<string, Dictionary<string, float>> getOptionsOld()
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
                    if (thisNode.GetAttributeValue("class", "") == "select-tooltip") continue;
                    //string optionName = thisNode.GetAttributeValue("data-" + OptionCap.ToLower(), "");
                    string optionName = thisNode.InnerText;
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
            return option_list;
        }

        public Dictionary<string, Dictionary<string, float>> getOptions()
        {
            //string[][] option_list = new string[][] { };
            float priceOpt;
            Dictionary<string, Dictionary<string, float>> option_list = new Dictionary<string, Dictionary<string, float>>();
            //Tuple<string, string, float>[] option_list = new Tuple<string, string, float>[0];

            Dictionary<string, float> option_list_item = new Dictionary<string, float>();

            //if (root.SelectSingleNode("//table[@id='table_select_size']") == null) return option_list;

            int startPos = root.InnerHtml.IndexOf("var variations = '");
            if (startPos == -1) 
                return option_list;
            int endPos = root.InnerHtml.IndexOf("',", startPos);
            string jSonData = root.InnerHtml.Substring(startPos + 18, endPos - startPos - 18).Replace("\\\"","\"");
            //dynamic stuff = JsonConvert.DeserializeObject(jSonData);
            //var stuff1 = JObject.Parse(jSonData);
            JArray stuff = JArray.Parse(jSonData);


            option_list_item = new Dictionary<string, float>();
            string[] prop = new string[2];
            foreach (JToken stuffItem in stuff.Children())
            {
                //if (stuffItem.confAttrVal == null) return option_list;
                  //  option_list_item.Add(stuffItem.confAttrVal.size.Value, 0);
                if (stuffItem.Last.ToString().Contains("confAttrVal"))
                    prop = stuffItem.Last.Last.Last.ToString().Split(new char[] { ':' });
                else
                    return option_list;
                if (!option_list_item.ContainsKey(prop[1]))
                    option_list_item.Add(prop[1], 0);
                
                

            }
            option_list.Add(prop[0], option_list_item);
            return option_list;
        }

        public Dictionary<string, string> getAdditionalFields()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            return retVal;
        }
        public string getWeight()
        {
            return "0";
        }
        public void SetFeedLine(string[] data, bool LastLine)
        {

        }

        public void SetFeedLine(Dictionary<string, string> data, bool LastLine)
        {
            
        }
    }
}
