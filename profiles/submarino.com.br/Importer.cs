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

namespace submarino.com.br
{
    public class Importer : Parser
    {
        HAP.HtmlNodeCollection Nodes;
        HAP.HtmlNode aNode;
        dynamic productJSON;
        string productData;
        Dictionary<string, string> propertyCollection = new Dictionary<string, string>();
        Dictionary<string, string> dataCollection = new Dictionary<string, string>();
        WebBrowser thisBrowser;
        string Title;

        Dictionary<string, string> abbrevs = new Dictionary<string, string>();
        public Importer()
        {

        }
        public override List<Category> getCategoryURLs()
        {
            Category objCategory = new Category();
            List<Category> urls = new List<Category>();
            Nodes = Document.SelectNodes("//li[contains(@class,'mmn-item')]");
            bool start = false;
            foreach(HAP.HtmlNode node in Nodes)
            {
                HAP.HtmlNode aNode = node.SelectSingleNode("a");
                if (aNode == null) continue;
                if (aNode.InnerText.Trim().StartsWith("Volta às Aulas Escolar"))
                    start = true;
                if (start)
                {
                    if (aNode.GetAttributeValue("href", "") != "#")
                    {
                        objCategory = new Category();
                        objCategory.Add("", aNode.GetAttributeValue("href", ""));
                        urls.Add(objCategory);
                    }
                }
            }
            
            
            return urls;
        }


        public override string buildCategoryURL(string catURL, int page)
        {
            if (catURL.Contains("?"))
                catURL = catURL + "&limite=24&offset=" + page.ToString();
            else
                catURL = catURL + "?limite=24&offset=" + page.ToString();

            return catURL;
        }

        public override List<string> getItemURLs()
        {
            List<string> urls = new List<string>();
            Nodes= Document.SelectNodes("//div[@class='row product-grid no-gutters main-grid']/div/div/div/a");
            if (Nodes == null) return urls;
            foreach (HAP.HtmlNode node in Nodes)
            {
                urls.Add("https://www.submarino.com.br" + node.GetAttributeValue("href",""));
            }

            return urls;
        }


        public override Dictionary<int, string> getTitles()
        {
            HAP.HtmlNode aNode;
            int startPos = Document.InnerHtml.IndexOf("<script type=\"application/ld+json\">");
            startPos = startPos + "<script type=\"application/ld+json\">".Length;
            int endPos = Document.InnerHtml.IndexOf("</script>", startPos);
            
            try
            {
                productData = Document.InnerHtml.Substring(startPos, endPos - startPos);
                productJSON = JsonConvert.DeserializeObject(productData.Replace("\"@", "\""));
            }
            catch(Exception ex)
            {
                return Titles;
            }
            

            Title = productJSON.graph[4].name;

            Titles.Clear();
            foreach (string language in Languages)
            {
                Titles.Add(int.Parse(language), Title);
            }


            return Titles;
        }

        public override string getModel()
        {
            Model = productJSON.graph[4].id;
            string[] parts = Model.Split(new string[] { "/" }, StringSplitOptions.None);
            Model = parts[2];
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }


        public override string getSKU()
        {
            return productJSON.graph[4].sku;
        }

        public override string getStatus()
        {
            return "0";
        }
        public override string getPrice()
        {

            string price = productJSON.graph[4].offers.price;
            if (price == null)
                return "0";
            return price;

        }


        public override Dictionary<int, string>  getDescriptions()
        {
            string[] desc = new string[1];
            aNode = Document.SelectNodes("//div[contains(@class,'info-description')]/div")[0];


            Descriptions.Clear();
            foreach (string language in Languages)
            {
                Descriptions.Add(int.Parse(language), aNode.InnerHtml);
            }

            return Descriptions;
        }



        public override CategoryTable getCategoryPath()
        {
            CategoryTable categoryPathTable = new CategoryTable();
            string catPath = "";
            foreach (dynamic categoryItem in productJSON.graph[3].itemListElement)
            {
                if (categoryItem.position != "1")
                {
                    catPath = catPath + categoryItem.item.name + "///";
                }
            }
            if (catPath.Length != 0)
                catPath = catPath.Substring(0, catPath.Length - 3);


            DataRow categoryPath = categoryPathTable.NewRow();
            categoryPath["language_id"] = "1";
            categoryPath["category_path"] = catPath;
            categoryPathTable.Rows.Add(categoryPath);

            return categoryPathTable;
        }

        public override string getStock()
        {
            string stockText = productJSON.graph[4].offers.availability;
            if (stockText.Contains("InStock"))
                return "99";
            else
                return "0";
        }

        public override AttributeTable getAttributes()
        {
            AttributeTable retVal = new AttributeTable();
            aNode = Document.SelectNodes("//div[@id='info-section']/div")[1];
            Nodes = aNode.SelectNodes("//table/tbody/tr");
            foreach (HAP.HtmlNode row in Nodes)
            {
                foreach (string language in Languages)
                {
                    DataRow dr = retVal.NewRow();
                    dr["language_id"] = language;
                    dr["name"] = row.SelectNodes("td")[0].InnerText;
                    dr["value"] = row.SelectNodes("td")[1].InnerText;
                    retVal.Rows.Add(dr);
                }
            }
            return retVal;
        }


        public override ImageTable getImages()
        {
            ImageTable retData = new ImageTable();
            DataRow dr;
            int startPos = Document.InnerHtml.IndexOf("\"digital\":false,");
            startPos = startPos + "\"digital\":false,".Length;
            int endPos = Document.InnerHtml.IndexOf("}]", startPos);
            string imageData = "{" + Document.InnerHtml.Substring(startPos, endPos - startPos +2) + "}";
            dynamic imageJSON = JsonConvert.DeserializeObject(imageData);
            int i = 0; Uri uri;
            foreach (dynamic image in imageJSON.images)
            {
                dr = retData.NewRow();
                if (image.extraLarge!=null)
                    dr["url"] = image.extraLarge;
                else if (image.large != null)
                    dr["url"] = image.large;
                else if (image.big != null)
                    dr["url"] = image.big;
                else if (image.medium != null)
                    dr["url"] = image.medium;
                else if (image.small != null)
                    dr["url"] = image.small;
                if (dr["url"] != null)
                {
                    uri = new Uri(dr["url"].ToString());
                    dr["image_name"] = Config.IMAGE_FOLDER + Model + "_" + i.ToString() + System.IO.Path.GetExtension(uri.LocalPath);
                    retData.Rows.Add(dr);
                }
                i++;
            }
            return retData;
        }

    }

}
