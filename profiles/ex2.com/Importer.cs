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
using System.Configuration;

namespace ex2.com
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
        public Importer()
        {
        }


        public override List<Category> getCategoryURLs()
        {
            Category objCategory = new Category();
            List<Category> urls = new List<Category>();
            HAP.HtmlNode catLinks= Document.SelectSingleNode("//div[@class='nav-secondary']");
            foreach (HAP.HtmlNode node in catLinks.Descendants("a").Where(o => "" != o.GetAttributeValue("href", "")))
            {
                var att = node.GetAttributeValue("href", "");
                objCategory = new Category(); objCategory.Add("", att.Replace("&amp;","&")); urls.Add(objCategory);
            }
            return urls;
        }


        public override string buildCategoryURL(string catURL, int page)
        {
            currentCat = catURL;
            catURL = catURL + "&page=" + page.ToString();

            return catURL;
        }

        public override List<string> getItemURLs()
        {
            List<string> urls = new List<string>();
            string TopCat = currentCat;

            string href = "";

            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//div[@class='product-item-container']/div[@class='right-block']/h4/a");

            foreach (HAP.HtmlNode item in nodes)
            {
                urls.Add( item.GetAttributeValue("href","").Replace("&amp;", "&"));
            }

            return urls;
        }


        public override int getMaxImports()
        {
            string maxCountURL; string maxCountData;
            WebClient wc = new WebClient();
            if (!maxCountObtained)
            {
                maxCountURL = ConfigurationManager.AppSettings["STORE_URL"] + "index.php?route=account/package&email=" + Config.USERNAME;
                maxCountData = wc.DownloadString(maxCountURL);

                try
                {
                    maxCount = int.Parse(maxCountData);
                }
                catch
                {
                    maxCount = 0;
                }
                maxCountObtained = true;
            }
            return maxCount;
        }


        public override Dictionary<int,string> getTitles()
        {

           
            HAP.HtmlNode aNode;
            try
            {
                aNode = Document.SelectNodes("//h1")[0];
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
            Model = Document.SelectNodes("//div[@class='model']")[0].InnerText.Replace("Product Code: ", "").Trim();
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getPrice()
        {

            HAP.HtmlNode priceElem= Document.SelectSingleNode("//span[@id='price-old']");
            string price = priceElem.InnerText.Replace(".", "").Replace(",", ".").Replace("TL","").Trim();
            return price;

        }

        public override SpecialTable getSpecial()
        {
            SpecialTable special = new SpecialTable();
            DataRow dr = special.NewRow();
            HAP.HtmlNode priceElem = Document.SelectSingleNode("//span[@id='price-special']");
            if (priceElem == null) return null;
            string price = priceElem.InnerText.Replace(".", "").Replace(",", ".").Replace("TL", "").Trim();
            dr["customer_group_id"] = "1";
            dr["price"] = price;
            special.Rows.Add(dr);
            return special;
        }

        public override string getManufacturer()
        {
            HAP.HtmlNode brandElem = Document.SelectSingleNode("//span[@itemprop='name']");
            string brand = brandElem.InnerText;
            return brand;
        }


        public override Dictionary<int,string> getDescriptions()
        {
            string desc ="";
            Descriptions.Clear();
            foreach (string language in Languages)
            {
                Descriptions.Add(int.Parse(language), desc);
            }
            return Descriptions;
        }


        public override ImageTable getImages()
        {
            string src; Uri uri;
            DataRow dr;
            int i = 0;

            foreach (HAP.HtmlNode image in Document.SelectNodes("//a[contains(@class,'img thumbnail')]"))
            {
                if (i == 0)
                {
                    i++; continue;
                }
                src = image.GetAttributeValue("data-image","");
                if (src!="") { 
                uri = new Uri(src);
                dr = prodImages.NewRow();
                dr["url"] = src;
                dr["image_name"] = Model + "_" + i.ToString() + System.IO.Path.GetExtension(uri.LocalPath); 
                prodImages.Rows.Add(dr);
                i++;
                }
            }

            options = new OptionTable[Languages.Length];
            options[0] = ScrapOptions();
            options[1] = (OptionTable) options[0].Copy();
            options[2] = (OptionTable)options[0].Copy();

            return prodImages;
        }


        public override CategoryTable getCategoryPath()
        {
            CategoryTable categoryPathTable = new CategoryTable();
            string catPath = "";
            int totalLi=Document.SelectNodes("//ul[@class='breadcrumb']/li").Count;
            int curLi=0;
            foreach (HAP.HtmlNode categoryItem in Document.SelectNodes("//ul[@class='breadcrumb']/li"))
            {
                if (curLi == totalLi - 2) break;
                if (categoryItem.InnerText.Trim() == "")
                    continue;
                else
                    catPath = catPath + categoryItem.InnerText.Trim() + "///";
                curLi++;
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
            return "99";
        }


        public override AttributeTable getAttributes()
        {
            AttributeTable retVal = new AttributeTable();
            if (Document.SelectNodes("//ul[@class='product-property-list util-clearfix']/li") != null)
            {
                foreach (HAP.HtmlNode attribute in Document.SelectNodes("//ul[@class='product-property-list util-clearfix']/li"))
                {
                    foreach (string language in Languages)
                    {
                        DataRow dr = retVal.NewRow();
                        dr["language_id"] = language;
                        dr["name"] = attribute.SelectNodes("span")[0].InnerText;
                        dr["value"] = attribute.SelectNodes("span")[1].InnerText;
                        retVal.Rows.Add(dr);
                    }
                }
            }
            return retVal;
        }




        public override OptionTable[] getOptions()
        {
            
            return options;
        }

        private OptionTable ScrapOptions()
        {
            OptionTable dtColor = new OptionTable();
            OptionTable dtSize = new OptionTable();
            DataRow drSize, drColor;
            string colorSK,colorParentSK;
            HAP.HtmlNode colors = Document.SelectNodes("//select")[1];
            string prodID = Document.SelectSingleNode("//input[@name='product_id']").GetAttributeValue("value","");

            foreach (HAP.HtmlNode color in colors.SelectNodes("option"))
            {
                if (color.InnerText.Trim() == "--- Please Select ---") continue;

                drColor = dtColor.NewRow();
                drColor["option_name"] = "Renk";
                drColor["required"] = 1;
                drColor["option_value"] = color.InnerText.Trim();
                drColor["price"] = 0;
                colorSK = color.GetAttributeValue("value", "");
                colorParentSK = color.ParentNode.GetAttributeValue("id", "").Replace("input-option","");
                WebClient wc = new WebClient();
                string jsonData = wc.DownloadString("http://e-x2.com/index.php?route=extension/module/dependent_options&parent_id=" + colorParentSK + "&value="+colorSK +"&product_id="+ prodID);
                dynamic sizeJson = JsonConvert.DeserializeObject(jsonData);
                if (sizeJson.Count != 0)
                {
                    foreach (dynamic variant in sizeJson.option[0].option_value)
                    {
                        if (variant.name != "")
                        {
                            drSize = dtSize.NewRow();
                            drSize["option_name"] = "Beden";
                            drSize["required"] = 1;
                            drSize["option_value"] = variant.name;
                            drSize["price"] = 0;
                            drSize["quantity"] = "99";
                            drSize["option_image"] = "";
                            drSize["parent_option_value"] = color.InnerText.Trim();
                            drSize["parent_option_name"] = "Renk";
                            dtSize.Rows.Add(drSize);
                        }
                    }
                }
                
            }


            dtColor.Merge(dtSize);            

            return dtColor;
        }


    }

}
