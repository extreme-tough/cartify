using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
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
using EntityLib;

namespace plushrugs.com
{
    public class Importer : Parser
    {
        HAP.HtmlNodeCollection Nodes;
        HAP.HtmlNode aNode;
        HtmlAgilityPack.HtmlNode root;
        string itemURL,tempURL,catPath;
        string nextCatURL = "";
        string MainImage;
        Dictionary<string,string> propertyCollection = new Dictionary<string,string>();
        Dictionary<string, string> dataCollection = new Dictionary<string, string>();
        Dictionary<string, Dictionary<string, float>> option_list;
        string price, description, SKU, ISBN,rISBN, MPN, Location, Construction, Origin,refField;
        HAP.HtmlDocument doc;
        string currentCat;
        List<string> related;
        string Title,Model,rModel;
        int TotalStock;
        int packSize;
        Product rug;
        string[] lookup;
        string[] urlParts;
        List<string> optionData ;
        Dictionary<string, List<string>> postOptionCmd;
        string colorCode;
        Dictionary<string, string> abbrevs = new Dictionary<string, string>();
        public Importer()
        {
            
        }
        public string URL 
        {            
            set
            {
                itemURL = value;    
                urlParts=itemURL.Split(new string[] { "#" }, StringSplitOptions.None);
                if (urlParts.Length > 1)
                    colorCode = urlParts[1];
                else
                    colorCode = "";
            }
        }

        public string CategoryPath
        {
            set
            {
                catPath = value;
            }
        }

        public string FeedFile
        {
            set
            {
            }
        }

        public Dictionary<string, string> PropertyBag
        {
            set
            {
                propertyCollection = value;
            }
        }

        public HtmlAgilityPack.HtmlNode Document
        {
            set
            {
                this.root = value;
            }
        }

        public void SetFeedData(string[] data, bool LastLine)
        {
            
        }

        public void SetFeedLines(List<string[]> data, bool LastLine)
        {


        }
        public void SetFeedData(Dictionary<string,string> data, bool LastLine)
        {
           
        }

        public void SetFeedData(DataSet ds)
        {

        }

        public DataSet getAdditionalTableData()
        {
            return null;
        }

        public void parseFeedLine()
        {

        }
        public List<Category> getCategoryURLs()
        {
            List<Category> urls = new List<Category>();
            
            return urls;
        }

        public List<int> getCategoryIDs()
        {
            List<int> CatIDs = new List<int>();            
            return CatIDs;
        }

        public string buildCategoryURL(string catURL, int page)
        {
            currentCat = catURL;
            if (page > 0)
                return nextCatURL;
            else 
                return catURL;
        }

        private string DownloadFile(string url)
        {
            string filename;
            WebClient client;
            client = new System.Net.WebClient();
            client.Encoding = Encoding.UTF8;
            filename = System.IO.Path.GetTempFileName();
            //string data = client.DownloadString(url);
            client.DownloadFile(url, filename);
            
            return filename;
        }

        public List<string> getItemURLs()
        {
            int PageNo = 1;
            List<string> urls = new List<string>();
            HAP.HtmlNode catNode,subCatNode;
            HAP.HtmlDocument catDoc;
            string TopCat = currentCat;
            doc = new HAP.HtmlDocument();
            catDoc = new HAP.HtmlDocument();
            
            string firstURL="",href="";
            while (true)
            {
                string filename = DownloadFile(TopCat);
                doc.Load(filename, Encoding.UTF8);
                catNode = doc.DocumentNode;

                HAP.HtmlNodeCollection subCats = catNode.SelectNodes("//div[@class='media']/a");
                foreach (HAP.HtmlNode aNode in subCats)
                {
                    href = "https://plushrugs.com" + aNode.GetAttributeValue("href", "");

                    if (href==firstURL) 
                        break ;

                    if (firstURL == "")
                        firstURL = href;
                    
                    filename = DownloadFile(href);
                    catDoc.Load(filename, Encoding.UTF8);
                    subCatNode = catDoc.DocumentNode;

                    Nodes = subCatNode.SelectNodes("//a[@class='big-thumbnail']");
                    if (Nodes == null) return urls;
                    foreach (HAP.HtmlNode itemNode in Nodes)
                    {
                        string itemHref = "https://plushrugs.com" + itemNode.GetAttributeValue("href", "");
                        urls.Add(itemHref);
                    }
                }

                if (href == firstURL) 
                    break;

                PageNo++;
                TopCat = currentCat + "?page=" + PageNo.ToString();
            }
            //Nodes = root.SelectNodes("//a[@class='fw_medium d_inline_b f_size_medium color_dark m_bottom_5']");
           
            return urls;
        }

        public List<string> getAdditionalItems()
        {
            List<string> retVal = new List<string>();
            string url;
            string[] urlParts;
            if (root.OwnerDocument.GetElementbyId("color-tabs") != null)
            {
                HAP.HtmlNodeCollection nodes = root.OwnerDocument.GetElementbyId("color-tabs").SelectNodes("li/a");
                foreach (HAP.HtmlNode oneNode in nodes)
                {
                    url = oneNode.GetAttributeValue("href", "");
                    urlParts = itemURL.Split(new string[] { "#" }, StringSplitOptions.None);
                    url = urlParts[0] + url;
                    if (url != itemURL)
                        retVal.Add(url);
                }
            }
            return retVal;
        }


        public string[] getTitles()
        {
            string[] titles = new string[1];
            HAP.HtmlNode avail = root.SelectSingleNode("//div[contains(@class,'" + colorCode + "_tab tab-pane')]/h3");
            if (avail != null && avail.InnerText == "This product is no longer available.") 
                return titles;
            this.aNode = root.SelectNodes("//div[contains(@class,'"+colorCode+"_tab tab-pane')]/table[@class='table table-condensed table-style-details']")[0];
            Nodes = this.aNode.SelectNodes("tr");
            foreach (HAP.HtmlNode oneNode in Nodes)
            {
                string tr_caption = oneNode.SelectNodes("th")[0].InnerText;
                switch (tr_caption)
                {
                    case "Collection":
                        Title = oneNode.SelectNodes("td")[0].InnerText;
                        break;
                    case "Style":
                        Model = oneNode.SelectNodes("td")[0].InnerText;
                        break;
                    case "Color":
                        ISBN = oneNode.SelectNodes("td")[0].InnerText;
                        refField = Model + " " + ISBN;
                        break;
                    case "Pile Height":
                        MPN = oneNode.SelectNodes("td")[0].InnerText;
                        break;
                    case "Weave":
                        Construction = oneNode.SelectNodes("td")[0].InnerText;
                        break;
                    case "Origin":
                        Origin = oneNode.SelectNodes("td")[0].InnerText;
                        break;
                    case "Material":
                        Location = oneNode.SelectNodes("td")[0].InnerText;
                        break;
                }
            }


            Nodes = root.SelectNodes("//table[@class='table table-condensed table-style-details']");
            int LoopCount = 1;
            string relatedRef;
            related = new List<string>();
            foreach (HAP.HtmlNode oneNode in Nodes)
            {
                string d=oneNode.ParentNode.GetAttributeValue("class","");

                if (d.Contains(colorCode + "_tab"))
                {
                    LoopCount++;
                    continue;
                }

                HAP.HtmlNodeCollection rows = oneNode.SelectNodes("tr");
                foreach (HAP.HtmlNode row in rows)
                {
                    string tr_caption = row.SelectNodes("th")[0].InnerText;
                    switch (tr_caption)
                    {
                        case "Style":
                            rModel = row.SelectNodes("td")[0].InnerText;
                            break;
                        case "Color":
                            rISBN = row.SelectNodes("td")[0].InnerText;
                            relatedRef = rModel + " " + rISBN;
                            related.Add(relatedRef);
                            break;
                    }
                }
                LoopCount++;
            }



            optionData = new List<string>();
            
            HAP.HtmlNode page = this.root.OwnerDocument.DocumentNode;
            HAP.HtmlNode element;
            int jsonStart = root.InnerHtml.IndexOf("application/ld+json\">");
            int jsonEnd = root.InnerHtml.IndexOf("</script>", jsonStart);
            string JSON = root.InnerHtml.Substring(jsonStart + 22, jsonEnd - jsonStart - 22);
            rug = JsonConvert.DeserializeObject<Product>(JSON);

            titles[0] = Title;


            Dictionary<string, float> optionItem = new Dictionary<string, float>();
            string size, price;
            string[] priceParts;
            int ProductSeqNo = 0;
            int stock;
            TotalStock = 0;
            string stockText;
            postOptionCmd = new Dictionary<string, List<string>>();
            option_list = new Dictionary<string, Dictionary<string, float>>();            
            Nodes = root.SelectNodes("//div[contains(@class,'" + colorCode + "_tab tab-pane')]/table[@class='style_table table table-striped table-condensed']/tbody/tr");
            Nodes = root.SelectNodes("//table[@class='style_table table table-striped table-condensed']/tbody/tr");
            if (Nodes != null)
            {
                foreach (HAP.HtmlNode aNode in Nodes)
                {
                    if (!aNode.ParentNode.ParentNode.ParentNode.GetAttributeValue("class", "").Contains(colorCode + "_tab tab-pane"))
                    {
                        if (aNode.GetAttributeValue("class","")!="danger")
                            ProductSeqNo++;
                        continue;
                    }
                    size = WebUtility.HtmlDecode(aNode.SelectNodes("td")[0].InnerText.Trim());
                    if (size == "" || aNode.SelectNodes("td").Count < 3) break;
                    if (aNode.SelectNodes("td").Count == 5)
                    {
                        priceParts = aNode.SelectNodes("td")[2].InnerText.Trim().Split(new string[] { "\n" }, StringSplitOptions.None);
                        stockText = new String(aNode.SelectNodes("td")[3].InnerText.Trim().TakeWhile(char.IsDigit).ToArray());
                        if (stockText == "")
                            stock = 0;
                        else
                            stock = int.Parse(stockText);
                    }
                    else
                    {
                        priceParts = aNode.SelectNodes("td")[1].InnerText.Trim().Split(new string[] { "\n" }, StringSplitOptions.None);
                        stockText = new String(aNode.SelectNodes("td")[2].InnerText.Trim().TakeWhile(char.IsDigit).ToArray());
                        if (stockText == "")
                            stock = 0;
                        else
                            stock = int.Parse(stockText);

                    }
                    if (priceParts.Length > 1)
                        price = priceParts[1].Replace("$", "").Replace(",", "");
                    else
                        price = priceParts[0].Replace("$", "").Replace(",", "");
                    optionData = new List<string>();
                    string sku = rug.offers[ProductSeqNo].mpn;
                    optionItem.Add(size, float.Parse(price));
                    optionData.Add("sku='" + sku + "'");
                    optionData.Add("quantity='" + stock + "'");
                    postOptionCmd.Add("Size:::" + size, optionData);
                    ProductSeqNo++;
                    TotalStock = TotalStock + stock;
                }
            }
            option_list.Add("Size", optionItem);

            if (option_list["Size"].Count == 0)
            {
                titles[0] = "";
            }

            return titles;
        }

        public string getModel()
        {
            return Model;
        }

        public string getRefField()
        {
            return refField;
        }

        public string getUPC()
        {
            return "";
        }

        public string getSKU()
        {
            return "";
        }

        public string getMPN()
        {
            return MPN;
        }

        public string getEAN()
        {
            return "";
        }

        public string getISBN()
        {
            return ISBN;
        }

        public string getJAN()
        {
            return SKU;
        }

        public string getLocation()
        {
            return Location;
        }

        public string getWeight()
        {
            return "0";
        }
        public string getLength()
        {
            return "0";
        }
        public string getWidth()
        {
            return "0";
        }
        public string getHeight()
        {
            return "0";
        }
        public string getStatus()
        {
            return "0";
        }
        public string getPrice()
        {
            price = "0";
            return price ;
        }

        public Dictionary<string, string> getSpecial()
        {
            Dictionary<string, string> special = new Dictionary<string, string>();
            return special;
        }

        public Dictionary<string, string>[] getSpecialPrices(string[] customerGroups)
        {
            return new Dictionary<string, string>[0];
        }

        public string getManufacturer()
        {            
            return "";
        }

        public string[] getDescriptions()
        {
            string[] desc = new string[1];            
            desc[0] = "";
            return desc;
        }

        public string[] getKeywords()
        {
            string[] keywords = new string[0];
            return keywords;
        }
        public string getMainImage()
        {
            aNode = root.SelectSingleNode("//div[contains(@class,'" + colorCode + "_tab tab-pane')]/div[@class='style-main-image']/a");
            //aNode = root.SelectSingleNode("//div[@class='style-main-image']/a");
            string src = aNode.GetAttributeValue("href", "");
            return src;            
        }

        public string[] getOtherImages()
        {
            List<string> OtherImages = new List<string>();
            Nodes = root.SelectNodes("//div[@class='col-lg-3 col-md-4 col-xs-6 product-thumbnail']/a");
            if (Nodes == null) return OtherImages.ToArray();
            foreach (HAP.HtmlNode aNode in Nodes)
            {
                string src = aNode.GetAttributeValue("href", "");
                OtherImages.Add(src);
            }
            return OtherImages.ToArray();
        }

        public List<string[]> getCategoryPath()
        {
            List<string[]> categoryPaths = new List<string[]>();
            string[] catItem = new string[1];
            catItem[0] = catPath;
            categoryPaths.Add(catItem);
            return categoryPaths;
        }
 

        public string getStock()
        {
            return TotalStock.ToString();
        }

        public string getStockStatus()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string>[] getAttributes()
        {
            Dictionary<string, string>[] attributes = new Dictionary<string, string>[0];
            return attributes;
        }



        public List<string> getRelated()
        {            
            return related;
        }

        public Dictionary<string, Dictionary<string, float>> getOptions()
        {
            //string[][] option_list = new string[][] { };
            
            return option_list;
        }

        public string[] getTags()
        {
            string[] keywords = new string[0];
            return keywords;
        }

        public Dictionary<string, string> getAdditionalFields()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            retVal.Add("construction", Construction);
            retVal.Add("origin", Origin);            
            return retVal;
        }

        public Dictionary<string, List<string>> getPostOptionSQLCommands()
        {
            
            return postOptionCmd;
        }
    }

    public class Brand
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class Seller
    {
        public string type { get; set; }
        public string name { get; set; }
    }

    public class Offer
    {
        public string priceCurrency { get; set; }
        public string mpn { get; set; }
        public string price { get; set; }
        public string availability { get; set; }
        public Seller seller { get; set; }
        public string itemCondition { get; set; }
        public string gtin12 { get; set; }
        public string type { get; set; }
    }

    public class Product
    {
        public string name { get; set; }
        public Brand brand { get; set; }
        public List<Offer> offers { get; set; }
        public string image { get; set; }
        public string context { get; set; }
        public string type { get; set; }
    } 
}
