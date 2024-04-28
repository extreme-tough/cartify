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

namespace mokiproducts.com
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
            string newURL = catURL + "***";
            if (page == 1)
                return newURL;
            else
                return catURL;
        }


        public override List<string> getItemURLs()
        {
            
            List<string> urls = new List<string>();
            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//h3[@class='card__heading h5']/a");
            if (nodes == null) return urls;
            foreach (HAP.HtmlNode item in nodes)
            {
                urls.Add("https://mokiproducts.com" + item.GetAttributeValue("href", "").Replace("&amp;", "&") + "?" + item.GetAttributeValue("data-extraQuery", "").Replace("&amp;", "&"));
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

            
            Titles.Clear();

            HAP.HtmlNode aNode;
            try
            {
                aNode = Document.SelectSingleNode("//h1[@class='product__title']");
            }
            catch (Exception ex)
            {
                return Titles;
            }

            foreach (string language in Languages)
            {
                Titles.Add(int.Parse(language), aNode.InnerText.Trim());
            }
            optionAttruibuteNames = new List<string>();
            optionAttruibuteLabels = new List<string>();
            
            prodImages = new ImageTable();
            TotalImported++;
            return Titles;
        }

        public override string getModel()
        {
            Model = Math.Abs(URL.GetHashCode()).ToString();
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getPrice()
        {
            HAP.HtmlNode aNode;
            aNode = Document.SelectSingleNode("//span[@class='hidePrice']");
            string price =aNode.InnerText.Replace("$", "").Replace("CAD","").Trim();

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
            aNode = Document.SelectSingleNode("//div[@class='product__description rte']");

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
            aNode = Document.SelectSingleNode("//div[@class='product__media media gradient global-media-settings']/img");
            string imgSrc = aNode.GetAttributeValue("src", "");
            imgSrc = "https:" + imgSrc.Split(new string[] { "?" }, StringSplitOptions.None)[0].Trim();

          
            uri = new Uri(imgSrc);
            dr = prodImages.NewRow();
            dr["url"] = imgSrc;
            dr["image_name"] = Model + "_" + i.ToString() + System.IO.Path.GetExtension(uri.LocalPath);
            prodImages.Rows.Add(dr);
            i++;
            

            options = new OptionTable[Languages.Length];
            options[0] = ScrapOptions();
            if (options[0] == null) options = null;
            return prodImages;

        }


        public override CategoryTable getCategoryPath()
        {

            CategoryTable categoryPathTable = new CategoryTable();

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
