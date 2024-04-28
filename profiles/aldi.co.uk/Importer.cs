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
using Newtonsoft.Json.Linq;
using Aiplib;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace aldi.co.uk
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
        string catPath, moreData;
        JObject itemObj;
        public Importer() 
        {
        }

        public override string buildCategoryURL(string catURL, int page)
        {
            /* string[] urlParts = catURL.Split(new string[] { "/" }, StringSplitOptions.None);
            string cat_slug = urlParts[urlParts.Length - 1];
            string newURL= "https://www.aldi.co.uk/api/productsearch/rr/category/"+ cat_slug + "?showPrevPage=false&q=%3Apopular&privm=false&page="+page+"&firstPlacementTotalCount=0&secondPlacementTotalCount=0";
            */
            string newURL = catURL + "?sortDirection=asc&page=" + page;
            return newURL; 

        }


        public override List<string> getItemURLs()
        {            
            List<string> urls = new List<string>();
            if (Document.SelectSingleNode("//div[@class='products-search-results']") == null) return urls;
            string catData = Document.SelectSingleNode("//div[@class='products-search-results']").GetAttributeValue("data-context", "");
            catData = HttpUtility.HtmlDecode(catData);
            JObject o = JObject.Parse(catData);

            /*
            string catData = Document.InnerText;

            JObject o = JObject.Parse(catData);

            dynamic catObj = JsonConvert.DeserializeObject(catData );
            foreach (JToken result in o.SelectToken("results").Children())
            {
                if (result.SelectToken("productUrl").ToString().Contains("//www.aldi.co.uk"))
                    urls.Add(result.SelectToken("productUrl").ToString());
                else
                    urls.Add("https://www.aldi.co.uk" + result.SelectToken("productUrl").ToString()); 
            }
            /* 
            if (catObj.results != null)
            {
                foreach (dynamic result in catObj.results)
                {
                    urls.Add("https://www.aldi.co.uk" + result.productUrl.Value);
                }
            }*/

            foreach (JToken result in o.SelectToken("SearchResults").Children())
            {
                if (result.SelectToken("Url").ToString().Contains("//www.aldi.co.uk"))
                    urls.Add(result.SelectToken("Url").ToString());
                else
                    urls.Add("https://groceries.aldi.co.uk" + result.SelectToken("Url").ToString());
            }
            return urls;
        }


        public override int getMaxImports()
        {            
            return -1;
        }


        public override Dictionary<int,string> getTitles()
        {

            string[] urlParts = URL.Split(new string[] { "/" },StringSplitOptions.None);
            string sku = urlParts[urlParts.Length - 1];
            Model = sku;


            /*int StartPos = Document.InnerHtml.IndexOf("var gtmData = ");
            int EndPos = Document.InnerHtml.IndexOf("gtmData.", StartPos);
            productData = Document.InnerHtml.Substring(StartPos + 14, EndPos - StartPos - 21);
            productData = productData.Trim();
            productData = productData.Substring(0, productData.Length - 1);
            // itemObj = JsonConvert.DeserializeObject(productData);
            itemObj = JObject.Parse(productData);

            HAP.HtmlNode skunode =  Document.SelectSingleNode("//input[@class='js-rr-data']");
            Model = skunode.GetAttributeValue("data-product-id", "");*/

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


            //string price = itemObj.seoData.offers.price.Value.ToString().Trim();
            // string price = Document.SelectSingleNode("//span[@class='product-price h4 m-0 font-weight-bold']").InnerText;
            //string price = itemObj.SelectToken("seoData.offers.price").ToString().Trim();
            string price;
            moreData =  GetMoreData();
            itemObj = JObject.Parse(moreData);
            JToken item = itemObj.SelectToken("ProductPrices");
            price = item[0]["ListPrice"].ToString().Replace("£", "");
            return price;
        }

        public  string GetMoreData()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://groceries.aldi.co.uk/api/product/calculatePrices");
            request.Method = "POST";
            request.Headers.Add("accept-language", "en-GB");
            request.UserAgent =  "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36";
            request.Headers.Add("x-requested-with", "XMLHttpRequest");
            request.ContentType = "application/json";
            string postData = "{\"products\":[\""+ Model + "\"]}";
            Stream newStream = request.GetRequestStream();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] byte1 = encoding.GetBytes(postData);
            newStream.Write(byte1, 0, byte1.Length);
            var responseText="";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                responseText = streamReader.ReadToEnd();
                //Now you have your response.
                //or false depending on information in the response     
            }
            return responseText;
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
            string desc = Document.SelectNodes("//div[@class='card-body']")[2].InnerHtml;
            // string desc = itemObj.SelectToken("seoData.description").ToString();
            
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
            string imgSrc =Document.SelectNodes("//meta[@property='og:image']")[0].GetAttributeValue("content","");
            //string imgSrc = itemObj.SelectToken("seoData.image").ToString()  ;
            if (imgSrc.StartsWith("//"))
            {
                imgSrc = "http:" + imgSrc;
            }
            if (imgSrc == "") return prodImages;
            //imgSrc = imgSrc.Split(new string[] { "?" }, StringSplitOptions.None)[0];
            uri = new Uri(imgSrc.Replace("%09", ""));
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
            catPath = "";
            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//ol[@class='breadcrumb d-flex']/li");
            //foreach (JToken breadcrumb in itemObj.SelectToken("seoData.breadCrumbList"))
            foreach (HAP.HtmlNode node in nodes)
            {
                if (node.SelectNodes("a") == null) continue;                  
                if (node.InnerText.Trim() == "Home") continue;
                catPath = catPath + System.Web.HttpUtility.HtmlDecode(node.InnerText.Trim()) + "///";
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
