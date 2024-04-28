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
using System.Globalization;
using System.IO;
using EntityLib;

namespace casadolojista.com.br
{
    public class Importer : Parser
    {
        HAP.HtmlNodeCollection Nodes;
        HAP.HtmlNode aNode;
        HtmlAgilityPack.HtmlNode root;
        string itemURL,tempURL,catPath;
        string nextCatURL = "";
        string MainImage;
        Dictionary<string,string> propertyCollection = new Dictionary<string,string>();
        Dictionary<string, string> dataCollection = new Dictionary<string, string>();
        string price, description, SKU;
        HAP.HtmlDocument doc;        
        string Title,Model,Stock;
        int packSize;
        string[] lookup;
        Dictionary<string, string> abbrevs = new Dictionary<string, string>();
        public Importer()
        {
            string[] words;
            lookup = File.ReadAllLines("lookup.txt");
            foreach (string line in lookup)
            {
                words = line.Split(new string[] { "," }, StringSplitOptions.None);
                if (words[0].Trim() !="")
                    abbrevs.Add(words[0], words[1]);
            }
        }
        public string URL 
        {            
            set
            {
                itemURL = value;
            }
        }

        public string CategoryPath
        {
            set
            {
                catPath = value;
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
        public List<Category> getCategoryURLs()
        {
            List<Category> urls = new List<Category>();
            Category category;
            string TopCategory,SubCategory;
            WebClient client;
            HAP.HtmlDocument doc;
            doc = new HAP.HtmlDocument();
            Nodes = root.SelectNodes("//li[@class='static']");
            int i = 0;
            foreach (HAP.HtmlNode aNode in Nodes)
            {
                i++;
                if (i == 1) continue;
                string href;
                //= aNode.SelectSingleNode("a").GetAttributeValue("href", "");
                if (aNode.SelectSingleNode("a") == null) continue;
                TopCategory = aNode.SelectSingleNode("a").InnerText;
                if (TopCategory == "PROMO&Ccedil;&Atilde;O") continue;
                doc.LoadHtml(aNode.InnerHtml);
                HAP.HtmlNodeCollection CatNodes = doc.DocumentNode.SelectNodes("//a[@class='color_dark f_size_medium']");
                if (CatNodes == null) continue;
                foreach (HAP.HtmlNode CatNode in CatNodes)
                {
                    href = CatNode.GetAttributeValue("href", "");
                    SubCategory = CatNode.InnerText.Trim();
                    category = new Category();
                    //if (TopCategory + "///" + SubCategory != "EL&Eacute;TRICA ///CONTROLE P/ VENTILADOR") continue;
                    category.Add(TopCategory + "///" + SubCategory, href);
                    urls.Add(category);
                }
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
                return nextCatURL;
            else 
                return catURL;
        }

        public List<string> getItemURLs()
        {
            List<string> urls = new List<string>();
            HAP.HtmlNode catNode;
            //Nodes = root.SelectNodes("//a[@class='fw_medium d_inline_b f_size_medium color_dark m_bottom_5']");
            Nodes = root.SelectNodes("//table[@class='table_type_8 full_width t_align_l']/tbody/tr/td/a[@class='fw_medium d_inline_b f_size_medium color_dark m_bottom_5']");
            if (Nodes == null) return urls;
            foreach (HAP.HtmlNode aNode in Nodes)
            {
                string href = aNode.GetAttributeValue("href", "");
                urls.Add(href);
            }
            catNode = root.SelectSingleNode("//table[@class='table_type_8 full_width t_align_l']/tbody/tr/td[@colspan='4']/strong");
            if (catNode != null)
            {
                catNode = catNode.NextSibling.NextSibling;
                if (catNode !=null)
                    nextCatURL = catNode.GetAttributeValue("href", "");
                else
                    nextCatURL = "";
            }
            else
                nextCatURL = "";
            return urls;
        }

        public string[] getTitles()
        {
            string[] titles = new string[1];

            Nodes = root.SelectNodes("//table[@class='description_table m_bottom_10']/tr");
            foreach (HAP.HtmlNode oneNode in Nodes)
            {
                string tr_caption = oneNode.SelectNodes("td")[0].InnerText;
                switch (tr_caption)
                {
                    case "C&oacute;digo:":
                        Model = oneNode.SelectNodes("td")[1].InnerText;
                        break;
                    case "Embalagem c/:":
                        SKU = oneNode.SelectNodes("td")[1].InnerText;
                        break;
                    case "Estoque:":
                        if (oneNode.SelectNodes("td")[1].InnerText.Trim() == "SIM")
                            Stock = "9999";
                        else
                            Stock = "0";
                        break;
                }
            }

            try
            {
                string[] packParts = SKU.Split(new string[] { "&nbsp;" }, StringSplitOptions.None);
                packSize = int.Parse(packParts[0]);
            }
            catch
            {
                packSize = 1;
            }

            aNode = root.SelectSingleNode("//h2");
            string title = aNode.InnerText;
            foreach (string key in abbrevs.Keys)
            {
                if (title.Contains(key))
                {
                    title=title.Replace(key, abbrevs[key]);
                }
            }
            try
            {
                TextInfo textInfo = new CultureInfo("pt-PT", false).TextInfo;
                if (packSize>1)
                    titles[0] = textInfo.ToTitleCase(title.ToLower()) + " C/" + SKU;
                else
                    titles[0] = textInfo.ToTitleCase(title.ToLower());
                Title = titles[0];

            }
            catch (Exception ex)
            {
                titles[0] = "";
            }
            return titles;
        }

        public string getModel()
        {
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
            return Model;
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

        public string getJAN()
        {
            return SKU;
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
            try
            {
                aNode = root.SelectNodes("//div[@class='m_bottom_15']/p")[1];
            }
            catch (Exception ex)
            {
                return "0";
            }
            string inText = aNode.InnerText.Trim();
            price = inText.Replace("R$", "").Replace(",", "").Replace(".", "").Trim();
            //price = (float.Parse(price) / 100).ToString();
            
            
            price = ((float.Parse(price) * packSize)/10).ToString();
            return price ;
        }

        public Dictionary<string, string> getSpecial()
        {
            Dictionary<string, string> special = new Dictionary<string, string>();
            return special;
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
            aNode = root.SelectSingleNode("//div[@id='tab-1']");
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
            aNode = root.SelectSingleNode("//img[@id='zoom_image']");
            string src = aNode.GetAttributeValue("src", "");
            if (src != "")
                return "http://clmarginaltiete.casadolojista.com.br" + src;
            else
                return "";
        }

        public string[] getOtherImages()
        {
            List<string> OtherImages = new List<string>();
           
            return OtherImages.ToArray();
        }

        public List<string[]> getCategoryPath()
        {
            List<string[]> categoryPaths = new List<string[]>();
            string[] catItem = new string[1];
            catItem[0] = catPath;
            categoryPaths.Add(catItem);
            return categoryPaths;
        }
 

        public string getStock()
        {
            return Stock;
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

        public Dictionary<string, List<string>> getPostOptionSQLCommands()
        {
            return null;
        }
    }
}
