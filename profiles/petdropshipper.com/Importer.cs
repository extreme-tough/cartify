using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Text;
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

namespace petdropshipper.com
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
        public Importer()
        {
        }
        

        public override string buildCategoryURL(string catURL, int page)
        {
            currentCat = catURL;
            string[] urlParts = catURL.Split(new string[] { ".htm" }, StringSplitOptions.None);
            urlParts = urlParts[0].Split(new string[] { "/" }, StringSplitOptions.None);
            string catID = urlParts[urlParts.Length - 1];
            catURL = catURL + "?searching=Y&sort=7&cat="+ catID + "&show=300&page="+ page;
            return catURL;
        }

        public override List<string> getItemURLs()
        {
            List<string> urls = new List<string>();
            string TopCat = currentCat;

            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//div[@id='content_area']");
            catPath = nodes[0].SelectNodes("table/tr/td/table/tr/td")[0].InnerText.Trim().Replace(" &gt; ", "///").Replace("Home///", "");

            nodes = Document.SelectNodes("//div[@class='v-product']/a[1]");

            int i = 0;
            if (nodes != null)
            {
                foreach (HAP.HtmlNode item in nodes)
                {
                    if (i == 0)
                    {
                        if (item.GetAttributeValue("href", "").Replace("&amp;", "&") == firstItemURL)
                        {
                            return null;
                        }
                        firstItemURL = item.GetAttributeValue("href", "").Replace("&amp;", "&");
                    }
                    urls.Add(item.GetAttributeValue("href", "").Replace("&amp;", "&"));
                    i++;
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

           
            HAP.HtmlNode aNode;
            try
            {
                aNode = Document.SelectNodes("//span[@itemprop='name']")[0];
            }
            catch (Exception ex)
            {
                return Titles;
            }
            if (aNode == null)
                return Titles;
            Titles.Clear();
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
            if (Document.SelectNodes("//span[@class='product_code']") == null) return "";
            Model = Document.SelectNodes("//span[@class='product_code']")[0].InnerText.Trim();
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getPrice()
        {

            HAP.HtmlNode priceElem= Document.SelectSingleNode("//span[@itemprop='price']");
            if (priceElem == null) return "0.00";
            string price = priceElem.InnerText.Trim();
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
            HAP.HtmlNode descElem = Document.SelectSingleNode("//div[@id='ProductDetail_ProductDetails_div']");
            string desc = descElem.InnerHtml;
            Descriptions.Clear();
            foreach (string language in Languages)
            {
                Descriptions.Add(int.Parse(language), desc);
            }
            return Descriptions;
        }


        public override ImageTable getImages()
        {
            string src; Uri uri;
            DataRow dr;
            int i = 0;
            HAP.HtmlNodeCollection imageTags = Document.SelectNodes("//a[@id='product_photo_zoom_url']");
            if (imageTags == null)
            {
                string imgSrc = Document.SelectNodes("//img[@itemprop='image']")[0].GetAttributeValue("src","").Replace("-2T","-2");
                if (imgSrc.StartsWith("//"))
                {
                    imgSrc = "http:" + imgSrc;
                }
                uri = new Uri(imgSrc);
                dr = prodImages.NewRow();
                dr["url"] = imgSrc;
                dr["image_name"] = Model + "_" + i.ToString() + System.IO.Path.GetExtension(uri.LocalPath);
                prodImages.Rows.Add(dr);
                options = new OptionTable[Languages.Length];
                options[0] = ScrapOptions();

                return prodImages;
            }
            foreach (HAP.HtmlNode image in Document.SelectNodes("//a[@id='product_photo_zoom_url']"))
            {
                src = image.GetAttributeValue("href", "");
                if (src!="") { 
                    if (src.StartsWith("//")) {
                        src = "http:" + src;
                    }
                    uri = new Uri(src);
                    dr = prodImages.NewRow();
                    dr["url"] = src;
                    dr["image_name"] = Model + "_" + i.ToString() + System.IO.Path.GetExtension(uri.LocalPath); 
                    prodImages.Rows.Add(dr);
                    i++;
                }
            }

            options = new OptionTable[Languages.Length];
            options[0] = ScrapOptions();

            return prodImages;
        }


        public override CategoryTable getCategoryPath()
        {
            CategoryTable categoryPathTable = new CategoryTable();
            DataRow categoryPath = categoryPathTable.NewRow();
            categoryPath["language_id"] = "1";
            categoryPath["category_path"] = catPath;
            categoryPathTable.Rows.Add(categoryPath);

            return categoryPathTable;
        }

        public override string getStock()
        {
            HAP.HtmlNode offer= Document.SelectNodes("//div[@itemprop='offers']")[0];
            int startPos = offer.InnerHtml.IndexOf("<b>Stock Status</b>");
            if (startPos == -1) return "99";
            startPos  = startPos + ("<b>Stock Status</b>").Length;
            int endPos = offer.InnerHtml.IndexOf("<meta itemprop='availability'");
            if (endPos == -1) return "0";
            string stock = offer.InnerHtml.Substring(startPos, endPos - startPos).Replace(":","");
            if (stock.Contains("Currently Out of Stock"))
                return "0";
            return stock;
        }


        public override AttributeTable getAttributes()
        {
            AttributeTable retVal = new AttributeTable();
            return null;
        }




        public override OptionTable[] getOptions()
        {
            
            return null;
        }

        private OptionTable ScrapOptions()
        {
            OptionTable dtColor = null;
            
            return dtColor;
        }


    }

}
