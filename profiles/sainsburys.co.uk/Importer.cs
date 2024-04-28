using CefSharp;
using CefSharp.WinForms;using System;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using ParserFactory;
using HAP = HtmlAgilityPack;
using System.Net;
using Newtonsoft.Json;
using System.Configuration;

namespace sainsburys.co.uk
{
    public class Importer : Parser
    {
        CefSharp.WinForms.ChromiumWebBrowser chromiumWebBrowser1 = new CefSharp.WinForms.ChromiumWebBrowser();
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
        }

        public override string buildCategoryURL(string catURL, int page)
        {
            string[] urlParts = catURL.Split(new string[] { "&" }, StringSplitOptions.None);
            string newURL = "https://www.sainsburys.co.uk/shop/CategoryDisplay?catalogId=10241&storeId=10151&pageSize=60";
            foreach (string urlPart in urlParts)
            {
                if (urlPart.StartsWith("categoryId"))
                {
                    newURL = newURL + "&categoryId=" + urlPart.Replace("categoryId=", "");
                }
            }
            newURL = newURL + "&beginIndex=" + page.ToString();
            return newURL;
        }


        public override List<string> getItemURLs()
        {

            List<string> urls = new List<string>();
            string TopCat = currentCat;

            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//h3/a");

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
            string[] urlParts = URL.Split(new string[] { "/" }, StringSplitOptions.None);
            string seo_url = urlParts[urlParts.Length - 1];

            string prodUrl = "https://www.sainsburys.co.uk/groceries-api/gol-services/product/v1/product?filter[product_seo_url]=gb%2Fgroceries%2F" + seo_url;
            chromiumWebBrowser1.ActivateBrowserOnCreation = true;
            chromiumWebBrowser1.LoadUrl(prodUrl);
            while (true) {
                Application.DoEvents();
                try
                {
                    string a = await chromiumWebBrowser1.GetBrowser().MainFrame.GetSourceAsync();                    
                    break;
                }
                catch
                {
                    continue;
                }                
            }
        }
        public override Dictionary<int, string> getTitles()
        {

            
            
            string[] urlParts = URL.Split(new string[] { "/" }, StringSplitOptions.None);
            string seo_url = urlParts[urlParts.Length - 1];

            string prodUrl = "https://www.sainsburys.co.uk/groceries-api/gol-services/product/v1/product?filter[product_seo_url]=gb%2Fgroceries%2F" + seo_url;
            
            
            

            bool pageLoaded = false;

            WebClient client = new WebClient();

            client.Headers.Add("User-Agent", ConfigurationManager.AppSettings["UserAgent"]);
            //client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.Headers.Add("Accept", "*/*");
            client.Headers.Add("Cache-Control", "no-cache");

            string prodData="";
            try
            {
                prodData = client.DownloadString(prodUrl);
            } catch (Exception ex)
            {
                Titles.Clear();
                foreach (string language in Languages)
                {
                    Titles.Add(int.Parse(language), "");
                }
                return Titles;
            }
            itemObj = JsonConvert.DeserializeObject(prodData);

            Model = itemObj.products[0].product_uid.Value;

            Titles.Clear();
            foreach (string language in Languages)
            {
                Titles.Add(int.Parse(language), itemObj.products[0].name.Value);
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

            if (itemObj.products[0].retail_price != null)
            {
                string price = itemObj.products[0].retail_price.price.Value.ToString();
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
            StringBuilder desc = new StringBuilder("");
            foreach (dynamic description in itemObj.products[0].description)
            {
                desc.Append("<p>" + description + "</p>");
            }

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
            foreach (dynamic image in itemObj.products[0].assets.images)
            {
                string imgSrc = image.sizes[image.sizes.Count - 1].url.Value;
                if (imgSrc.StartsWith("//"))
                {
                    imgSrc = "http:" + imgSrc;
                }
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
            foreach (dynamic breadcrumb in itemObj.products[0].breadcrumbs)
            {
                catPath = catPath + System.Web.HttpUtility.HtmlDecode(breadcrumb.label.Value) + "///";
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
            if (itemObj.products[0].is_available.Value)
                return "99";
            else
                return "0";
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
