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
using RestSharp;

namespace loblaws.ca
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

        private string GetProduct(string prodCode)
        {
            string date = DateTime.Today.ToString("ddMMyyyy");
            var client = new RestClient("https://api.pcexpress.ca/product-facade/v4/products/"+ prodCode 
                + "?lang=en&date="+ date + "&pickupType=STORE&storeId=1032&banner=loblaw&features=loyaltyServiceIntegration,inventoryServiceIntegration");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("sec-ch-ua", "\"Google Chrome\";v=\"93\", \" Not;A Brand\";v=\"99\", \"Chromium\";v=\"93\"");
            request.AddHeader("Site-Banner", "loblaw");
            request.AddHeader("x-apikey", "1im1hL52q9xvta16GlSdYDsTsG0dmyhF");
            request.AddHeader("Accept-Language", "en");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.63 Safari/537.36";
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json, text/plain, */*");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("Origin", "https://www.loblaws.ca");
            request.AddHeader("Sec-Fetch-Site", "cross-site");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("Referer", "https://www.loblaws.ca/");
            IRestResponse response = client.Execute(request);
            File.WriteAllText("product.json", response.Content);
            return response.Content;
        }
        private string GetCategory(string CategoryID,string Page)
        {
            var client = new RestClient("https://api.pcexpress.ca/product-facade/v3/products/category/listing");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("sec-ch-ua", "\"Google Chrome\";v=\"93\", \" Not;A Brand\";v=\"99\", \"Chromium\";v=\"93\"");
            request.AddHeader("Site-Banner", "loblaw");
            request.AddHeader("x-apikey", "1im1hL52q9xvta16GlSdYDsTsG0dmyhF");
            request.AddHeader("Accept-Language", "en");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.63 Safari/537.36";
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json, text/plain, */*");
            request.AddHeader("use-variants", "true");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
            request.AddHeader("Origin", "https://www.loblaws.ca");
            request.AddHeader("Sec-Fetch-Site", "cross-site");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("Referer", "https://www.loblaws.ca/");
            var body = @"{""pagination"":{""from"":"+ Page + @",""size"":48},""banner"":""loblaw"",""cartId"":""5a5825d2-230b-4e64-9595-b9f4ec22155c""," +
                @"""lang"":""en"",""date"":""14092021"",""storeId"":""1032"",""pcId"":null,""pickupType"":""STORE"",""enableSeldonIntegration"":true,"+
                @"""features"":[""loyaltyServiceIntegration"",""sunnyValeServiceIntegration""],""inventoryInfoRequired"":true,""offerType"":""ALL"",""categoryId"":"""
                +CategoryID+@"""}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            File.WriteAllText("product.json", response.Content);
            return response.Content;
        }

        public override string buildCategoryURL(string catURL, int page)
        {
            catURL = catURL + "/" + page.ToString();
            currentCat = catURL;
            return catURL;
        }


        public override List<string> getItemURLs()
        {
            
            List<string> urls = new List<string>();
            string[] catParts = currentCat.Split(new string[] { "/" }, StringSplitOptions.None);
            string catData = GetCategory(catParts[catParts.Length - 2],catParts[catParts.Length - 1]);            
            itemObj = JsonConvert.DeserializeObject(catData);
            if (itemObj.results != null)
            {
                foreach (dynamic result in itemObj.results)
                {
                    urls.Add("https://www.loblaws.ca" + result.link.Value.Replace(" & amp;", "&"));
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

            string[] urlParts = URL.Split(new string[] { "/" }, StringSplitOptions.None);
            Model = urlParts[urlParts.Length - 1];
            string itemData = GetProduct(Model);
            itemObj = JsonConvert.DeserializeObject(itemData);

            Titles.Clear();
            foreach (string language in Languages)
            {
                if (itemObj.name == null)
                    Titles.Add(int.Parse(language), "");
                else
                    Titles.Add(int.Parse(language), itemObj.name.Value);
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

            if (itemObj.variants[0].offers[0].price.value.Value != null)
            {
                string price = itemObj.variants[0].offers[0].price.value.Value.ToString();
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


        public override Dictionary<int,string> getDescriptions()
        {
            string desc;
            if (itemObj.description != null)
                desc = itemObj.description.Value;
            else
                desc = "";

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
            foreach (dynamic image in itemObj.imageAssets)
            {
                string imgSrc;
                if (image.largeUrl!=null)
                    imgSrc  = image.largeUrl.Value;
                else if (image.imageUrl != null)
                    imgSrc = image.imageUrl.Value;
                else if (image.mediumUrl != null)
                    imgSrc = image.mediumUrl.Value;
                else if (image.smallUrl != null)
                    imgSrc = image.smallUrl.Value;
                else if (image.largeRetinaUrl != null)
                    imgSrc = image.largeRetinaUrl.Value;
                else if (image.mediumRetinaUrl != null)
                    imgSrc = image.mediumRetinaUrl.Value;
                else
                    imgSrc = image.smallRetinaUrl.Value;
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
            foreach (dynamic breadcrumb in itemObj.breadcrumbs)
            {
                catPath = catPath +  System.Web.HttpUtility.HtmlDecode( breadcrumb.name.Value)+"///";
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
            if (itemObj.variants[0].offers[0].stockStatus.Value =="OK")
            {
                return "9999";
            }
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
            
            return options;
        }

        private OptionTable ScrapOptions()
        {
            OptionTable dtOption = new OptionTable();
            return null;

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
