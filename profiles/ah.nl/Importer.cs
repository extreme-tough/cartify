using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using ParserFactory;
using HAP = HtmlAgilityPack;
using System.Net;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Aiplib;
using System.Configuration;

namespace ah.nl
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
        string firstItemURL="";
        string catPath;
        dynamic itemObj;
        public Importer() 
        {
        }

        public override string buildCategoryURL(string catURL, int page)
        {
            string[] urlParts = catURL.Split(new string[] { "/" }, StringSplitOptions.None);
            string cat_slug = urlParts[urlParts.Length - 1];
            string newURL = "https://www.ah.nl/zoeken/api/products/search?page="+page+"&size=36&taxonomySlug="+ cat_slug;
            return newURL;            
        }


        public override List<string> getItemURLs()
        {
            
            List<string> urls = new List<string>();
            string catData = Document.InnerText;
            dynamic catObj = JsonConvert.DeserializeObject(catData);

            if (catObj.cards != null)
            {
                foreach (dynamic card in catObj.cards)
                {
                    urls.Add("https://www.ah.nl" + card.products[0].link.Value);
                }
            }
            return urls;
        }


        public override int getMaxImports()
        {            
            return -1;
        }


        public override Dictionary<int,string> getTitles()
        {

            string sku;

            int StartPos = Document.InnerHtml.IndexOf("\"sku\": \"");
            if (StartPos > -1)
            {
                int EndPos = Document.InnerHtml.IndexOf("\",", StartPos);
                productData = Document.InnerHtml.Substring(StartPos + 8, EndPos - StartPos - 8);
                Model = productData.Trim();
            } else
            {
                string[] urlParts = URL.Split(new string[] { "/" }, StringSplitOptions.None);
                Model = urlParts[urlParts.Length - 1];
            }
            Titles.Clear();

            HAP.HtmlNode aNode;
            try
            {
                aNode = Document.SelectSingleNode("//h1");
            }
            catch (Exception ex)
            {
                return Titles;
            }

            foreach (string language in Languages)
            {
                Titles.Add(int.Parse(language), aNode.InnerText);
            }
            optionAttruibuteNames = new List<string>();
            optionAttruibuteLabels = new List<string>();
            
            prodImages = new ImageTable();
            TotalImported++;
            return Titles;
        }

        public override string getModel()
        {
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getPrice()
        {
            HAP.HtmlNode aNode;
            aNode = Document.SelectSingleNode("//div[@class='price-amount_root__37xv2 product-card-hero-price_now__PlF9u']");
            if (aNode==null)
                aNode = Document.SelectSingleNode("//div[@class='price-amount_root__37xv2 price-amount_bonus__27nxZ product-card-hero-price_now__PlF9u']");
            if (aNode == null)
                aNode = Document.SelectSingleNode("//div[@class='price-amount_root__37xv2 price-amount_infinite__3asY6 product-card-hero-price_now__PlF9u']");
            string price =aNode.InnerText.Trim();

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


        public override Dictionary<int,string> getDescriptions()
        {

            HAP.HtmlNode aNode;
            aNode = Document.SelectSingleNode("//div[@class='product-info-content-block']");
            if (aNode == null)
            {
                aNode =Document.SelectSingleNode("//div[@class='product-summary_root__3_CJs']");
            }
            string desc="";
            if (aNode != null)            
                desc = aNode.InnerHtml.Trim();

            Descriptions.Clear();
            foreach (string language in Languages)
            {
                Descriptions.Add(int.Parse(language), desc.ToString());
            }
            return Descriptions;
        }


        public override ImageTable getImages()
        {
            string src; Uri uri;
            DataRow dr;
            int i = 0;

            /* int StartPos = Document.InnerHtml.IndexOf("\"image\": \"");
            int EndPos = Document.InnerHtml.IndexOf("\"", StartPos+11);
            productData = Document.InnerHtml.Substring(StartPos + 10, EndPos - StartPos - 10);
            string imgSrc = productData.Split(new string[] { "?" },StringSplitOptions.None)[0].Trim(); */

            HAP.HtmlNode aNode;
            aNode = Document.SelectSingleNode("//img[@class='lazy-image_image__2025k']");
            string imgSrc = aNode.GetAttributeValue("src", "");
            imgSrc = imgSrc.Split(new string[] { "?" }, StringSplitOptions.None)[0].Trim();

            if (imgSrc.StartsWith("http"))
            {
                uri = new Uri(imgSrc);
                dr = prodImages.NewRow();
                dr["url"] = imgSrc;
                dr["image_name"] = Model + "_" + i.ToString() + System.IO.Path.GetExtension(uri.LocalPath);
                prodImages.Rows.Add(dr);
                i++;
            }

            options = new OptionTable[Languages.Length];
            options[0] = ScrapOptions();
            if (options[0] == null) options = null;
            return prodImages;

        }


        public override CategoryTable getCategoryPath()
        {
            catPath = "";
            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//ol/li");

            foreach (HAP.HtmlNode node in nodes)
            {
                if (node.InnerText.Trim() == "Home" || node.InnerText.Trim() == "Producten")
                    continue;
                catPath = catPath + node.InnerText + "///";
            }
            CategoryTable categoryPathTable = new CategoryTable();
            DataRow categoryPath = categoryPathTable.NewRow();
            categoryPath["language_id"] = "1";
            categoryPath["category_path"] = catPath;
            categoryPathTable.Rows.Add(categoryPath);

            return categoryPathTable;
        }

        public override string getStock()
        {
            return "99";

        }


        public override AttributeTable getAttributes()
        {
            AttributeTable retVal = new AttributeTable();
            return null;
        }




        public override OptionTable[] getOptions()
        {
            
            return options;
        }

        private OptionTable ScrapOptions()
        {
            OptionTable dtOption = new OptionTable();

            return dtOption;
        }


    }

}
