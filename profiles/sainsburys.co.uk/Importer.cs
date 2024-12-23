﻿using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using ParserFactory;
using HAP = HtmlAgilityPack;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;
using System;
using System.Security.Cryptography;
using System.Windows.Forms.VisualStyles;

namespace sainsburys.co.uk
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
        dynamic itemObj;
        public Importer()
        {
            CategoryPageLoadedIndicator = ".pt__link";
            CategoryPageLoadedIndicatorType = PageLoadedIndicatorType.CSS_TYPE;
            ProductPageLoadedIndicator = ".pd__header";
            ProductPageLoadIndicatorType = PageLoadedIndicatorType.CSS_TYPE;
        }

        public override string buildCategoryURL(string catURL, int page)
        {
            string[] urlParts = catURL.Split(new string[] { "#" }, StringSplitOptions.None);            
            string newURL = urlParts[0] + "/opt/page:" + page.ToString();
            return newURL;
        }


        public override List<string> getItemURLs()
        {

            List<string> urls = new List<string>();
            string TopCat = currentCat;

            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//a[@class='pt__link']");

            int i = 0;
            if (nodes != null)
            {
                foreach (HAP.HtmlNode item in nodes)
                {
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


        public override async void PreProcessing()
        {

        }
        public override Dictionary<int, string> getTitles()
        {
            string prodData="";
            try
            {
                HAP.HtmlNode titleNode = Document.SelectSingleNode("//h1");
                Titles.Clear();
                foreach (string language in Languages)
                {
                    Model = ConvertToUniqueEncryptedString(titleNode.InnerText);
                    Titles.Add(int.Parse(language), titleNode.InnerText);
                }
            } catch (Exception ex)
            {
                
            }
            prodImages = new ImageTable();
            return Titles;
        }

        static string ConvertToUniqueEncryptedString(string input)
        {
            // Step 1: Hash the input string using SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Step 2: Convert the hash to a Base64 string
                string base64String = Convert.ToBase64String(hashBytes);

                // Step 3: Ensure the string is 15 characters long
                // Base64 strings can contain + and / characters, which can be URL-encoded
                // To make it URL-safe, you can replace + with - and / with _
                base64String = base64String.Replace('+', '-').Replace('/', '_');

                // Ensure the output is exactly 15 characters long
                // If the base64 string is shorter, pad with zeros; if longer, truncate
                if (base64String.Length > 15)
                {
                    return base64String.Substring(0, 15);
                }
                else
                {
                    return base64String.PadRight(15, '0');
                }
            }
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

            HAP.HtmlNode priceNode =  Document.SelectSingleNode("//span[contains(@class,'pd__cost__retail-price')]");
            string price;
            if (priceNode != null)
            {
                if (priceNode.InnerText.EndsWith("p"))
                    price = "0." + priceNode.InnerText.Replace("p", "");
                else
                    price = priceNode.InnerText.Replace("£","");
                return price;
            }
            else
                return "0.00";

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
            string desc ;

            HAP.HtmlNode descNode = Document.SelectSingleNode("//htmlcontent");
            
            if (descNode==null)
                descNode = Document.SelectSingleNode("//div[@id='mainPart']");
            if (descNode == null)
                descNode = Document.SelectSingleNode("//div[@id='tabpanel-product-details']");
            if (descNode == null)
                descNode = Document.SelectSingleNode("//details");
            if (descNode == null)
                desc = "";
            else
               desc = descNode.InnerHtml;

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
            HAP.HtmlNode imageNode = Document.SelectSingleNode("//img[contains(@class,'pd__image__nocursor')]");
            if (imageNode==null)
                imageNode = Document.SelectSingleNode("//img[contains(@class,'pd__image')]");
            string imgSrc = imageNode.GetAttributeValue("srcset","");
            if ((imgSrc != null) && (!imgSrc.StartsWith("data:image"))) {
                string[] imgParts = imgSrc.Split(new string[] { "," }, StringSplitOptions.None);
                imgSrc = imgParts[imgParts.Length-1].Trim();
                imgParts = imgSrc.Split(new string[] { " " }, StringSplitOptions.None);
                imgSrc = imgParts[0];
                uri = new Uri(imgSrc);
                dr = prodImages.NewRow();
                dr["url"] = imgSrc;
                dr["image_name"] = Model + "_" + i.ToString() + System.IO.Path.GetExtension(uri.LocalPath);
                prodImages.Rows.Add(dr);
            }


            options = null;
            return prodImages;

        }



        public override CategoryTable getCategoryPath()
        {
            catPath = "";
            HAP.HtmlNodeCollection catNodes = Document.SelectNodes("//ol[contains(@class,'pd__breadcrumbs')]/li");
            CategoryTable categoryPathTable = new CategoryTable();
            if (catNodes != null)
            {
                foreach (HAP.HtmlNode catNode in catNodes)
                {
                    catPath = catPath + System.Web.HttpUtility.HtmlDecode(catNode.InnerText.Trim()) + "///";
                }


                DataRow categoryPath = categoryPathTable.NewRow();
                categoryPath["language_id"] = "1";
                categoryPath["category_path"] = catPath;
                categoryPathTable.Rows.Add(categoryPath);
            }
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
            return null;
            //return options;
        }

        private OptionTable ScrapOptions()
        {
            OptionTable dtOption = new OptionTable();

            string optionName, optionValue;
            DataRow dr;

            if (itemObj.products[0].catchweight == null)
                return null;
            foreach (dynamic catchweight in itemObj.products[0].catchweight)
            {
                dr = dtOption.NewRow();
                dr["option_name"] = "Weight";
                dr["required"] = 1;
                dr["option_value"] = catchweight.range;
                dr["price"] = catchweight.retail_price.price.Value.ToString(); ;
                dr["quantity"] = "99";
                dtOption.Rows.Add(dr);
            }


            return dtOption;
        }


    }

}
