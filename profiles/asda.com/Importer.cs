using System.Data;
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
using System.Linq;
using System.IO;

namespace asda.com
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
            string newURL = catURL + "?page=" + page.ToString();
            return newURL;
        }


        public override List<string> getItemURLs()
        {

            List<string> urls = new List<string>();
            string TopCat = currentCat;

            HAP.HtmlNode node = Document.SelectSingleNode("//ul[contains(@class,'co-product-list__main-cntr')]"); ;
            HAP.HtmlNodeCollection nodes = node.SelectNodes(".//a[@class='co-product__anchor']");
            int i = 0;
            if (node != null)
            {
                foreach (HAP.HtmlNode item in nodes)
                {
                    urls.Add("https://groceries.asda.com" + item.GetAttributeValue("href", "").Replace("&amp;", "&"));
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
                if (titleNode == null) 
                    return Titles;
                foreach (string language in Languages)
                {
                    Titles.Add(int.Parse(language), titleNode.InnerText);
                }
            } catch (Exception ex)
            {
                
            }
            prodImages = new ImageTable();
            return Titles;
        }

        
        public override string getModel()
        {
            HAP.HtmlNode modelNode = Document.SelectSingleNode("//span[@class='pdp-main-details__product-code']");
            Model = modelNode.InnerText.Replace("Product code:", "").Trim();
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getPrice()
        {

            string price;

            var priceNode = Document.SelectSingleNode("//strong[contains(@class, 'pdp-main-details__price')]");
            var priceText = priceNode?.InnerText.Trim();
            price = priceText?.Split(' ').LastOrDefault();

            if (priceNode != null)
            {
                if (priceNode.InnerText.EndsWith("p"))
                    price = "0." + price.Replace("p", "");
                else
                    price = price.Replace("£","");
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
            string desc = "";

            HAP.HtmlNodeCollection  descNodes = Document.SelectNodes("//div[@class='pdp-description-reviews__product-details-cntr']");
            foreach (var descNode in descNodes)
            {
                desc += descNode.OuterHtml;
            }
            

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
            DataRow dr;
            int i = 0;
            HAP.HtmlNodeCollection imageNodes = Document.SelectNodes("//img[contains(@class,'asda-image-zoom__zoomed-image-placeholder')]");
            foreach (var item in imageNodes)
            {
                string imgSrc = item.GetAttributeValue("src", "");
                if ((imgSrc != null) && (!imgSrc.StartsWith("data:image")))
                {
                    uri = new Uri(imgSrc);
                    dr = prodImages.NewRow();
                    dr["url"] = imgSrc;
                    var localPath = uri.LocalPath;

                    if (!IsValidImageFile(uri.LocalPath, validExtensions))
                        localPath = uri.LocalPath + ".jpg";

                    dr["image_name"] = Model + "_" + i.ToString() + System.IO.Path.GetExtension(localPath);
                    prodImages.Rows.Add(dr);
                }

            }


            options = null;
            return prodImages;

        }



        public override CategoryTable getCategoryPath()
        {
            catPath = "";
            HAP.HtmlNodeCollection catNodes = Document.SelectNodes("//a[contains(@class,'breadcrumb__link')]");
            CategoryTable categoryPathTable = new CategoryTable();
            if (catNodes != null)
            {
                foreach (HAP.HtmlNode catNode in catNodes)
                {
                    catPath = catPath + System.Web.HttpUtility.HtmlDecode(catNode.SelectSingleNode("./div").NextSibling.InnerText.Trim()) + "///";
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
