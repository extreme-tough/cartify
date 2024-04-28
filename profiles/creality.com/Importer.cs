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
using System.Web;

namespace creality.com
{
    public class Importer : Parser
    {
        dynamic productJSON;
        string productData;
        List<string> optionAttruibuteNames, optionAttruibuteLabels;
        Dictionary<string, string> propertyCollection = new Dictionary<string, string>();
        WebBrowser thisBrowser;
        string currentCat, sku;
        OptionTable[] options;
        Dictionary<string, string> abbrevs = new Dictionary<string, string>();
        ImageTable prodImages;
        bool productStatus = true;
        public Importer()
        {
        }


        public override List<Category> getCategoryURLs()
        {
            Category objCategory = new Category();
            List<Category> urls = new List<Category>();            
            objCategory = new Category(); objCategory.Add("Parts///Tool", "15"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Nozzle", "61"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Fans", "48"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Package", "45"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Motor", "44"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Screen", "43"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Platform", "42"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Profile", "41"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Others", "16"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Motherboard", "7"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Enclosure", "14"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Bed Leveling", "13"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Cable", "12"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Hotbed", "11"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Tube", "10"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Hotend", "9"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Parts///Extruder", "8"); urls.Add(objCategory);

            objCategory = new Category(); objCategory.Add("Filaments///PLA", "17"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Filaments///ABS", "18"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Filaments///PETG", "19"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Filaments///TPU", "20"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Filaments///Silk", "21"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Filaments///Wood", "22"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Filaments///Nylon", "23"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Filaments///Resin", "24"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Filaments///Carbon", "25"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Filaments///PCL", "47"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Filaments///PDS", "62"); urls.Add(objCategory);

            objCategory = new Category(); objCategory.Add("Peripherals///Co-Brand", "26"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("Peripherals///Others", "28"); urls.Add(objCategory);

            return urls;
        }


        public override string buildCategoryURL(string catURL, int page)
        {
            currentCat = catURL;
            catURL=catURL.Replace(",","%2C");
            catURL =  "https://api.trendyol.com/websearchgw/v2/api/infinite-scroll/" + catURL + (catURL.Contains("?") ? "&": "?") + "pi=" + page.ToString() + "&storefrontId=1&culture=tr-TR&userGenderId=1&pId=0&scoringAlgorithmId=2&categoryRelevancyEnabled=false&isLegalRequirementConfirmed=False&searchStrategyType=DEFAULT";
            return catURL;
        }

        public override List<string> getItemURLs()
        {
            List<string> urls = new List<string>();
            string TopCat = currentCat;

            string href = "";
            
            dynamic itemsObj = JsonConvert.DeserializeObject(Document.InnerText);

            
            int iter = 1;
            foreach (dynamic item in itemsObj.result.list)
            {
                urls.Add("https://vip.creality.com/en/goods-detail/" + item.id.Value);
                iter++;
            }

            return urls;
        }


        public override Dictionary<int,string> getTitles()
        {
            string TitleText="";
            HAP.HtmlNode aNode;
            try
            {
                aNode = Document.SelectNodes("//div[@class='goods-name']")[0];
                TitleText = aNode.InnerText;
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
                Titles.Add(int.Parse(language), TitleText.Trim());
            }
            return Titles;
        }

        public override string getModel()
        {
            return "";
        }

        public override string getRefField()
        {
            return "";
        }

        public override string getSKU()
        {
            return sku;
        }


        public override string getStatus()
        {

            if (Document.InnerHtml.Contains("<span class=\"off - shelf\">Sold out</span>"))
                productStatus = false;
            if (productStatus)
                return "1";
            else
                return "0";
        }

        public override string getPrice()
        {
            HAP.HtmlNode aNode;
            string Price;
            if (Document.SelectNodes("//div[@class='price']/span") != null)
            {
                aNode = Document.SelectNodes("//div[@class='price']/span")[0];
                Price = aNode.InnerText.Trim();
            }
            else
            {
                Price = "?";
            }
            if (Price == "?")
                return "0";
            else
                return Price;

        }

        public override SpecialTable getSpecial()
        {
            SpecialTable special = new SpecialTable();
            return special;
        }

        public override string getManufacturer()
        {
            string brand = "";
            return brand;
        }


        public override Dictionary<int,string> getDescriptions()
        {
            string desc;
            try {
                desc = Document.SelectNodes("//div[@class='content']")[0].InnerHtml;
            }
            catch (Exception ex)
            {
                return Descriptions;
            }
            //string desc = productJSON.product.description;
            // desc = desc.Replace("<li><img alt=\"\" src=\"/Assets/ProductImages/chartlar/yerliuretimsiyah1.jpg\" /></li>", "");
            //desc = "";
            Descriptions.Clear();
            foreach (string language in Languages)
            {
                Descriptions.Add(int.Parse(language), desc);
            }
            return Descriptions;
        }

        private void parseImages()
        {

        }

        public override ImageTable getImages()
        {

            string src; Uri uri;
            DataRow dr;
            int i = 0;
            prodImages = new ImageTable();

            int startPos = Document.InnerHtml.IndexOf("\"Item\",");
            int endPos = Document.InnerHtml.IndexOf(",\"Brand\",", startPos);
            if (endPos == -1)
                endPos = Document.InnerHtml.IndexOf(",\"上架中\",", startPos); 
            string content = Document.InnerHtml.Substring(startPos, endPos - startPos );

            string[] variables = content.Split(new string[] { "," }, StringSplitOptions.None);

            
            foreach (string image in variables)
            {
                if (image.StartsWith("\"https:\\"))
                {
                    src = image.Replace("\"", "");
                    src = System.Text.RegularExpressions.Regex.Unescape(src);

                    src = src.Split(new string[] { "?" }, StringSplitOptions.None).First();
                    uri = new Uri(src);
                    dr = prodImages.NewRow();
                    dr["url"] = src;
                    dr["image_name"] = uri.AbsolutePath.Split(new string[] { "/" }, StringSplitOptions.None).Last() + ".jpg" ;
                    prodImages.Rows.Add(dr);
                    i++;
                }
            }
            return prodImages;
        }


        public   CategoryTable getCategoryPath(string catPath)
        {
            return null;
        }

        public override string getStock()
        {
            if (Document.InnerHtml.Contains("<span class=\"off - shelf\">Sold out</span>"))
                return "0";
            else
                return "99";
        }


        public override AttributeTable getAttributes()
        {
            HashSet<string> hs = new HashSet<string>();

            AttributeTable retVal = new AttributeTable();
            DataRow attRow;
            sku = "";
            HAP.HtmlNodeCollection atts = Document.SelectNodes("//div[@class='extra-item']");
            foreach (HAP.HtmlNode att in atts)
            {
                attRow = retVal.NewRow();
                string label = att.SelectNodes("div")[0].InnerText.Trim().Replace("：", "");
                string value = att.SelectNodes("div")[1].InnerText.Trim();
                if (hs.Contains(label))
                    continue;
                else
                    hs.Add(label);
                if (label != "SKU")
                {
                    attRow["name"] = label;
                    attRow["value"] = value;
                    attRow["attribute_group_id"] = "1";
                    attRow["language_id"] = "1";
                    retVal.Rows.Add(attRow);
                } else
                {
                    sku = value;
                }
            }
            return retVal;
        }




        public override OptionTable[] getOptions()
        {
            OptionTable[] options = new OptionTable[1];
            options[0] = ScrapOptions();
            if (options[0] == null) return null;
            return options;
        }

        private OptionTable ScrapOptions()
        {
            OptionTable dtOption = new OptionTable();
            string optionName,optionValue;
            DataRow dr;
            HAP.HtmlNodeCollection optionItems =  Document.SelectNodes("//div[@class='option-item']");
            if (optionItems == null) return null;
            foreach(HAP.HtmlNode optionItem in optionItems)
            {
                optionName = optionItem.SelectNodes("div")[0].InnerText.Trim();
                foreach (HAP.HtmlNode optionItemValue in optionItem.SelectNodes("//div[contains(@class,'option-value-item')]"))
                {
                    optionValue = optionItemValue.InnerText.Trim();
                    dr = dtOption.NewRow();

                    dr["option_name"] = optionName;
                    dr["required"] = 1;
                    dr["option_value"] = optionValue;
                    dr["price"] = 0;
                    dr["quantity"] = "99";
                    dtOption.Rows.Add(dr);
                }
            }
            
            return dtOption;
        }


    }

}
