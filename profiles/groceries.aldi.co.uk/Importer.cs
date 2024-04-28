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

namespace groceries.aldi.co.uk
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
            if (page == 1)
                return catURL;
            else
                return catURL + "?sortDirection=asc&page=" + page.ToString();
        }


        public override List<string> getItemURLs()
        {
            
            List<string> urls = new List<string>();
            int StartPos = Document.InnerHtml.IndexOf("data-context='");
            if (StartPos==-1)
                StartPos = Document.InnerHtml.IndexOf("data-context=\"");
            if (StartPos > 0)
            {
                int EndPos = Document.InnerHtml.IndexOf("\'>", StartPos);
                if (EndPos==-1)
                    EndPos = Document.InnerHtml.IndexOf("\">", StartPos);

                string jsonData = Document.InnerHtml.Substring(StartPos + 14, EndPos - StartPos - 14);
                File.WriteAllText("catpage.txt", jsonData);
                jsonData = jsonData.Replace("&quot;", "\"");
                dynamic catObj = JsonConvert.DeserializeObject(jsonData);

                if (catObj != null)
                {
                    foreach (dynamic result in catObj.SearchResults)
                    {
                        urls.Add("https://groceries.aldi.co.uk" + result.Url.Value);
                    }
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
            string title = "";
            if (Document.SelectNodes("//div[@id='product-summary']") != null)
            {

                aNode = Document.SelectNodes("//div[@id='product-summary']")[0];
                Model = aNode.GetAttributeValue("data-productid", "");
                title = aNode.InnerText;
                try
                {
                    aNode = Document.SelectNodes("//h1")[0];
                    title = aNode.InnerText;
                }
                catch (Exception ex)
                {
                    title = "";
                }
            }

            Titles.Clear();
            foreach (string language in Languages)
            {
                Titles.Add(int.Parse(language), title);
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

        public string priceData()
        {
            var client = new RestClient("https://groceries.aldi.co.uk/api/product/calculatePrices");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("authority", "groceries.aldi.co.uk");
            request.AddHeader("accept-language", "en-GB");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36";
            request.AddHeader("content-type", "application/json");
            request.AddHeader("accept", "application/json, text/javascript, */*; q=0.01");
            request.AddHeader("x-requested-with", "XMLHttpRequest");
            request.AddHeader("Cookie", "_abck=B9A906403FDCFBDC4E728E38B072ED11~-1~YAAQnqtkX3R+eeF7AQAAw0QV/gYkWekmraBkXjUa8J5ryMU+GVVS/KHjlISzWwFcnp5UFSf7FY8AifYgafdexFm0svkttPn9xKTKFijd991wePpGzpx45lY8bUkTjXonEdrepUw6EtivTFB8JL0t2PoTwJoBTI649Ma7SVbf7t7nQhphsctC8qJXLcsg8qhylB4Nrr/6dfPH2pvlhfo1fr+7LASvlsX9BlmASAIyqjLdBOgZ+Z1W4hztk5g3SOA6MDUlRcGq1/F3azpbnU932tBVXGdLBw4Vey0lbn+KCecWaSqMWCbAh+IGWebHP3eJ7pftP74FfgFSpCf3+MHjaNgW31bE43erEsoG+8VhC8knve2kju3EkjI+v5T8yPoESKZ1RhFgRCAfEl17zSI+C7GTlfqp2Egs~0~-1~-1; incap_ses_1135_2400122=jyajYCAh5wCf4/9X5VTAD5RnZGEAAAAAYWGpYkA7svbhT9y7dquVrg==; visid_incap_2400122=c+Z34Ye8QxavHmxOVIDfwZNnZGEAAAAAQUIPAAAAAACVZqH05hrLOBQCsn5rUhiq; .CUST_f3dbd28d-365f-4d3e-91c3-7b730b39b294=1|cqx+lGeWYSy8x/RF7S1vFJWVu3r9QJmsErjkoGfW6780/htPoUn4kg2UG6l3tyDfJ8Nd+h1JJIEhuicoYMeL1cqIzEJ1KAksfoy7OKkD71R7gSdzR4P0LE/uy05JDHBC1lYg6RY1s9UvjMQh8DO5fGjaSkh6S8I6/eN31VWR12zNJBWs8eSQ4BbTG8eJL3fYwvfuIUV2aodiSVpBAEW92B8b769hFCv44q6DaEapPJxOJ9te4rRBfMlLREEx8t6G62PAq2BvUb1SQqz0bfeceCEgpXzIu5wBlLjvE7tT3fg=");
            var body = @"{""products"":[""" + Model + @"""]}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response.Content;
        }


        public override string getPrice()
        {
            string pricedata = priceData();
            dynamic priceObj = JsonConvert.DeserializeObject(pricedata);

            return priceObj.ProductPrices[0].DefaultListPrice.Value.Replace("£","");
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
            HAP.HtmlNodeCollection Nodes = Document.SelectNodes("//div[@class='card-body']");

            foreach(HAP.HtmlNode aNode in Nodes)
            {
                if (aNode.InnerText.Contains("Specifications"))
                {
                    string desc = aNode.InnerHtml;

                    Descriptions.Clear();
                    foreach (string language in Languages)
                    {
                        Descriptions.Add(int.Parse(language), desc.ToString());
                    }
                    return Descriptions;
                }
            }
            return Descriptions;
        }


        public override ImageTable getImages()
        {
            string src; Uri uri;
            DataRow dr;
            int i = 0;

            HAP.HtmlNodeCollection Nodes = Document.SelectNodes("//section[@class='js-zoom-thumbnails']/div/a/img");
            if (Nodes == null)
            {
                // No thumbnail. Take main image alone
                Nodes = Document.SelectNodes("//meta[@property='og:image']");
            }

            foreach (HAP.HtmlNode aNode in Nodes)
            {
                string imgSrc = aNode.GetAttributeValue("data-zoom-src","");
                if (imgSrc.Trim() == "")
                    continue;
                uri = new Uri(imgSrc);
                dr = prodImages.NewRow();
                dr["url"] = imgSrc;
                dr["image_name"] = Model + "_" + i.ToString() + System.IO.Path.GetExtension(uri.LocalPath);
                prodImages.Rows.Add(dr);
                i++;


                options = new OptionTable[Languages.Length];
                options[0] = ScrapOptions();
                if (options[0] == null) options = null;
            }

            
            return prodImages;

        }


        public override CategoryTable getCategoryPath()
        {

            HAP.HtmlNodeCollection Nodes = Document.SelectNodes("//ol[@class='breadcrumb d-flex']/li/a");

            catPath = "";
            foreach (HAP.HtmlNode aNode in Nodes)
            {
                if (aNode.InnerText.Trim() != "Home")
                    catPath = catPath +  System.Web.HttpUtility.HtmlDecode(aNode.InnerText)+"///";
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
