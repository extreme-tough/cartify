using System;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using ParserFactory;
using HAP = HtmlAgilityPack;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.NetworkInformation;

namespace tesco.com
{
    public class Importer : Parser
    {
        dynamic productJSON;
        string productData;
        List<string> optionAttruibuteNames, optionAttruibuteLabels;
        Dictionary<string, string> propertyCollection = new Dictionary<string, string>();
        WebBrowser thisBrowser;
        string currentCat;
        OptionTable[] options;
        int maxCount, TotalImported = 0;
        bool maxCountObtained = false;
        Dictionary<string, string> abbrevs = new Dictionary<string, string>();
        ImageTable prodImages;
        string firstItemURL = "";
        string catPath;
        string price;
        dynamic itemObj;
        public Importer()
        {
        }

        public override string buildCategoryURL(string catURL, int page)
        {
            string[] urlParts = catURL.Split(new string[] { "&" }, StringSplitOptions.None);
            string newURL = catURL + "?page=" + page.ToString();            
            return newURL;
        }


        public override List<string> getItemURLs()
        {

            List<string> urls = new List<string>();
            string TopCat = currentCat;

            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//a[@class='product-image-wrapper']");

            int i = 0;
            if (nodes != null)
            {
                foreach (HAP.HtmlNode item in nodes)
                {
                    urls.Add("https://www.tesco.com" + item.GetAttributeValue("href", "").Replace("&amp;", "&"));
                    i++;
                }
            }
            return urls;
        }


        public override int getMaxImports()
        {
            return -1;
        }


        public override bool ValidatePage()
        {
            HAP.HtmlNode node = Document.SelectSingleNode("//h1[@data-auto='pdp-product-title']");
            if (node == null)            
                return false;
            else
                return true;
        }

        public override Dictionary<int, string> getTitles()
        {
            
            Titles.Clear();
            HAP.HtmlNode node = Document.SelectSingleNode("//h1[@data-auto='pdp-product-title']");;
            if (node == null) 
            {
                return null;
            }
            foreach (string language in Languages)
            {
                Titles.Add(int.Parse(language), node.InnerText.Trim());
            }
            optionAttruibuteNames = new List<string>();
            optionAttruibuteLabels = new List<string>();

            prodImages = new ImageTable();
            return Titles;
        }

        public override string getModel()
        {
            string[] urlParts = URL.Split(new string[] { "/" }, StringSplitOptions.None);
            Model= urlParts[urlParts.Length - 1];
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getPrice()
        {
            HAP.HtmlNode node = Document.SelectSingleNode("//div[contains(@class,'ddsweb-price__container')]");
            if (node == null) 
                price = "0";
            else
                price = node.FirstChild.InnerText.Trim().Replace("£","");
            

            return price;
            

        }

        public override SpecialTable getSpecial()
        {
            SpecialTable special = new SpecialTable();
            return special;
        }

        public override string getManufacturer()
        {
            return "";
        }


        public override Dictionary<int, string> getDescriptions()
        {


            Descriptions.Clear();

            HAP.HtmlNode node = Document.SelectSingleNode("//div[@id='accordion-panel-product-description']");
            string desc = node.InnerHtml.Trim();

            
            foreach (string language in Languages)
            {
                Descriptions.Add(int.Parse(language), desc);
            }
            return Descriptions;
        }


        public override ImageTable getImages()
        {
            string src; Uri uri;
            HAP.HtmlNode node = Document.SelectSingleNode("//div[@data-testid='image-slider']");
            if (node == null)
            {
                node = Document.SelectSingleNode("//meta[@property='og:image']");
                if (node == null) return prodImages;
                src = node.GetAttributeValue("content", "");

                DataRow dr;
                src = src.Split(new string[] { "?" }, StringSplitOptions.None)[0];

                uri = new Uri(src);
                dr = prodImages.NewRow();
                dr["url"] = src;
                dr["image_name"] = Model + System.IO.Path.GetExtension(uri.LocalPath);
                prodImages.Rows.Add(dr);
            } 
            else
            {
                //Product has multiple images
                DataRow dr;
                int i = 0;
                HAP.HtmlNodeCollection nodes = Document.SelectNodes("//div[@data-testid='image-slider']/img");
                /*if (nodes == null)
                    nodes = Document.SelectNodes("//img[@class='product-image grayscale']");
                if (nodes == null)
                    nodes = Document.SelectNodes("//img[@class='product-image grayscale product-image-visible']");*/
                foreach (HAP.HtmlNode nodeImg in nodes)
                {
                    string imgSrc = nodeImg.GetAttributeValue("src", "");
                    if (imgSrc.StartsWith("//"))
                    {
                        imgSrc = "http:" + imgSrc;
                    }
                    uri = new Uri(imgSrc);
                    dr = prodImages.NewRow();
                    dr["url"] = imgSrc.Split(new string[] { "?" },StringSplitOptions.None)[0];
                    dr["image_name"] = Model + "_" + i.ToString() + System.IO.Path.GetExtension(uri.LocalPath);
                    prodImages.Rows.Add(dr);
                    i++;
                }
            }
            options = new OptionTable[Languages.Length];
            options[0] = ScrapOptions();
            if (options[0] == null) options = null;
            return prodImages;
        }



        public override CategoryTable getCategoryPath()
        {
            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//ol/li");
            catPath = "";
            int i = 0;
            foreach (HAP.HtmlNode node in nodes)
            {
                if (node.SelectSingleNode("div/a") == null)
                    continue;

                if  (i>0 && node.InnerText.Trim()!="")
                    catPath = catPath + System.Web.HttpUtility.HtmlDecode(node.InnerText) + "///";
                i++;
            }
            if (catPath.EndsWith("///"))
                catPath = catPath.Substring(0, catPath.Length- 3);
            CategoryTable categoryPathTable = new CategoryTable();
            DataRow categoryPath = categoryPathTable.NewRow();
            categoryPath["language_id"] = "1";
            categoryPath["category_path"] = catPath;
            categoryPathTable.Rows.Add(categoryPath);

            return categoryPathTable;
        }

        public override string getStock()
        {
            if (float.Parse(price) == 0)
                return "0";
            else
                return "99";
        }


        public override AttributeTable getAttributes()
        {
            AttributeTable retVal = new AttributeTable();
            return null;
        }


        public override OptionTable[] getOptions()
        {
            return null;
            //return options;
        }

        private OptionTable ScrapOptions()
        {
            OptionTable dtOption = new OptionTable();

            return dtOption;
        }


    }

}
