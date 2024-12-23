using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using ParserFactory;
using HAP = HtmlAgilityPack;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System;
using System.Security.Cryptography;
using System.Windows.Forms.VisualStyles;
using System.Linq;
using System.IO;

namespace morrisons.com
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
            
            CategoryPageLoadedIndicator = ".co-product-list__main-cntr";
            CategoryPageLoadedIndicatorType = PageLoadedIndicatorType.CSS_TYPE;
            ProductPageLoadedIndicator = ".asda-image-zoom__zoomed-image-placeholder";
            ProductPageLoadIndicatorType = PageLoadedIndicatorType.CSS_TYPE;            
        }

        public override string buildCategoryURL(string catURL, int page)
        {
            string newURL = catURL;
            return newURL;
        }


        public override List<string> getItemURLs()
        {
            var docHTML = Document.InnerHtml;
            var anchorText = "window.INITIAL_STATE = ";
            var startPos = docHTML.IndexOf(anchorText);
            var endPos = docHTML.IndexOf("}};", startPos);
            var jsonData = docHTML.Substring(startPos + anchorText.Length, endPos - startPos - anchorText.Length+2);
            
            JObject jsonObject = JObject.Parse(jsonData);            
            JObject prodPages = (JObject)jsonObject["catalogue"]["productsPagesByRoute"];

            JObject pageRoot;
            List<string> urls = new List<string>();

            // Iterate over child properties of "Email"
            foreach (var property in prodPages.Properties())
            {
                string key = property.Name;      // This is the unknown key (e.g., "Personal")
                pageRoot = (JObject)property.Value; // This is the value of the unknown key

                JArray sections = (JArray)pageRoot["mainFopCollection"]["sections"];
                foreach (var section in sections)
                {
                    JArray fops = (JArray)section["fops"];
                    foreach (var fop in fops)
                    {
                        string sku = (string) fop["sku"];
                        urls.Add("https://groceries.morrisons.com/products/" + sku);
                    }
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
                if (titleNode == null) 
                    return Titles;
                foreach (string language in Languages)
                {
                    Titles.Add(int.Parse(language), titleNode.InnerText);
                }
            } catch (Exception ex)
            {
                
            }            
            return Titles;
        }

        
        public override string getModel()
        {

            Model = URL.Split('/').Last();
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getPrice()
        {

            string price;

            var priceNode = Document.SelectSingleNode("//meta[@itemprop='price']");
            var priceText = priceNode?.GetAttributeValue("content","").Trim();
            price = priceText?.Split(' ').LastOrDefault();

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
            string desc = "";

            HAP.HtmlNode descNode = Document.SelectSingleNode("//div[@class='bop-section bop-productDetails gn-accordion']");
            HAP.HtmlNode nutritionNode = Document.SelectSingleNode("//div[@class='bopbop-section bop-nutritionDetails']");
            desc = descNode.OuterHtml + nutritionNode?.OuterHtml;

            Descriptions.Clear();
            foreach (string language in Languages)
            {
                Descriptions.Add(int.Parse(language), desc.ToString());
            }
            return Descriptions;
        }

        string[] validExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".webp", ".svg" };

        static bool IsValidImageFile(string filePath, string[] validExtensions)
        {
            string fileExtension = Path.GetExtension(filePath).ToLower();

            foreach (string ext in validExtensions)
            {
                if (fileExtension == ext)
                {
                    return true;
                }
            }

            return false;
        }

        public override ImageTable getImages()
        {
            string src; Uri uri;
            prodImages = new ImageTable();
            DataRow dr;
            int i = 0;
            HAP.HtmlNodeCollection imageNodes = Document.SelectNodes("//img[@class='bop-gallery__image']");
            foreach (var item in imageNodes)
            {
                string imgSrc = item.GetAttributeValue("src", "").Split('?').First();
                if ((imgSrc != null) && (!imgSrc.StartsWith("data:image")))
                {
                    uri = new Uri("https://groceries.morrisons.com" + imgSrc);
                    dr = prodImages.NewRow();
                    dr["url"] = "https://groceries.morrisons.com" + imgSrc;
                    dr["image_name"] = Model + "_" + i.ToString() + System.IO.Path.GetExtension("https://groceries.morrisons.com" + imgSrc);
                    prodImages.Rows.Add(dr);
                }

            }


            options = null;
            return prodImages;

        }



        public override CategoryTable getCategoryPath()
        {
            catPath = "";
            HAP.HtmlNodeCollection catNodes = Document.SelectNodes("//li[@class='gn-breadcrumbs__element u-inline']");
            CategoryTable categoryPathTable = new CategoryTable();
            if (catNodes != null)
            {
                int iCount = 0;
                foreach (HAP.HtmlNode catNode in catNodes)
                {
                    if (iCount < catNodes.Count-1) 
                        catPath = catPath + System.Web.HttpUtility.HtmlDecode(catNode.InnerText.Trim()) + "///";
                    iCount++;
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

            return dtOption;
        }


    }

}
