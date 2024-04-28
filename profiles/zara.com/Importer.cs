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

namespace zara.com
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
            int i;
            Category objCategory = new Category();
            List<Category> urls = new List<Category>();
            WebClient wc = new WebClient();
            string jsonData = wc.DownloadString("https://www.zara.com/in/en/categories?ajax=true");
            dynamic catJson = JsonConvert.DeserializeObject(jsonData);
            string url="",catPath="";
            if (catJson.Count != 0)
            {
                foreach (dynamic category in catJson.categories)
                {
                    if (category.hasSubcategories == "True")
                    {
                        foreach (dynamic subCategory1 in category.subcategories)
                        {
                            if (subCategory1.hasSubcategories == "True")
                            {
                                foreach (dynamic subCategory2 in subCategory1.subcategories)
                                {
                                    if (subCategory2.hasSubcategories == "True")
                                    {
                                        foreach (dynamic subCategory3 in subCategory2.subcategories)
                                        {
                                            url = Config.HOME_PAGE + "/" + subCategory3.seo.keyword + "-l" + subCategory3.seo.seoCategoryId + ".html";
                                            catPath = "";
                                            i = 0;
                                            foreach (dynamic breadCrumb in subCategory3.seo.breadCrumb)
                                            {
                                                catPath = catPath + breadCrumb.text + "///";
                                            }
                                            if (catPath.EndsWith("///")) catPath = catPath.Substring(0, catPath.Length - 3);
                                            if ((!catPath.Contains("DIVIDER_MENU")) && (!catPath.Contains("///View all")))
                                            {
                                                objCategory = new Category(); objCategory.Add(catPath, url.Replace("&amp;", "&")); urls.Add(objCategory);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        url = Config.HOME_PAGE + "/" + subCategory2.seo.keyword + "-l" + subCategory2.seo.seoCategoryId + ".html";
                                        catPath = "";
                                        i = 0;
                                        foreach (dynamic breadCrumb in subCategory2.seo.breadCrumb)
                                        {
                                            catPath = catPath + breadCrumb.text + "///";
                                        }
                                        if (catPath.EndsWith("///")) catPath = catPath.Substring(0, catPath.Length - 3);
                                        if ((!catPath.Contains("DIVIDER_MENU")) && (!catPath.Contains("///View all")))
                                        {
                                            objCategory = new Category(); objCategory.Add(catPath, url.Replace("&amp;", "&")); urls.Add(objCategory);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                url = Config.HOME_PAGE + "/" + subCategory1.seo.keyword + "-l" + subCategory1.seo.seoCategoryId + ".html";
                                catPath = "";
                                i = 0;
                                foreach (dynamic breadCrumb in subCategory1.seo.breadCrumb)
                                {
                                    catPath = catPath + breadCrumb.text + "///";
                                }
                                if (catPath.EndsWith("///")) catPath = catPath.Substring(0,catPath.Length - 3);
                                if ((!catPath.Contains("DIVIDER_MENU")) && (!catPath.Contains("///View all"))) {
                                    objCategory = new Category(); objCategory.Add(catPath, url.Replace("&amp;", "&")); urls.Add(objCategory);
                                }
                            }
                        }
                    }
                }                
            }




            return urls;
        }


        public override string buildCategoryURL(string catURL, int page)
        {
            if (page == 1)
                return catURL;
            else
                return "";
        }

        public override List<string> getItemURLs()
        {
            List<string> urls = new List<string>();
            string TopCat = currentCat;

            string href = "";

            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//a[@class='product-link product-grid-product__link link']");
            if (nodes == null) return urls;
            foreach (HAP.HtmlNode item in nodes)
            {
                urls.Add( item.GetAttributeValue("href","").Replace("&amp;", "&") + "?" + item.GetAttributeValue("data-extraQuery", "").Replace("&amp;", "&"));
            }

            return urls;
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
                Titles.Add(int.Parse(language), aNode.InnerHtml.Replace("<span class=\"offleft\"> Ayrıntılar</span>", ""));
            }
            optionAttruibuteNames = new List<string>();
            optionAttruibuteLabels = new List<string>();
            
            prodImages = new ImageTable();
            TotalImported++;
            return Titles;
        }

        public override string getModel()
        {
            string itemLink= Document.SelectSingleNode("//meta[@property='og:url']").GetAttributeValue("content", "").Replace("&amp;", "").Trim();
            string[] urlParts = itemLink.Split(new string[] { "?" }, StringSplitOptions.None);
            urlParts  = urlParts[0].Split(new string[] { "-" }, StringSplitOptions.None);
            Model = urlParts[urlParts.Length - 1].Replace(".html","");
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getPrice()
        {
            int startPos = Document.InnerHtml.IndexOf("],\"price\":");
            int endPos = Document.InnerHtml.IndexOf("}],\"", startPos);
            string jsonPart = "{\"" + Document.InnerHtml.Substring(startPos + 3, endPos - startPos - 1) + "}";
            dynamic catJson = JsonConvert.DeserializeObject(jsonPart);
            float tempPrice = catJson.price.Value/100;


            options = new OptionTable[Languages.Length];
            options[0] = ScrapOptions(catJson);
            if (options.Length>1)
                options[1] = (OptionTable)options[0].Copy();
            if (options.Length > 2)
                options[2] = (OptionTable)options[0].Copy();


            return tempPrice.ToString();

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
            HAP.HtmlNode descNode =Document.SelectNodes("//div[@class='product-info-wrapper _product-info']")[0];
            descNode.SelectSingleNode("//div[@class='expand-view-buttons-container _expand-view-buttons-container']").Remove();
            string desc= descNode.InnerHtml;
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
            int startPos = Document.InnerHtml.IndexOf("<script type=\"application/ld+json\">");
            int endPos = Document.InnerHtml.IndexOf("</script>", startPos);
            string jsonData = Document.InnerHtml.Substring(startPos + ("<script type=\"application/ld+json\">").Length, endPos - startPos- ("<script type=\"application/ld+json\">").Length);
            dynamic imageJson = JsonConvert.DeserializeObject(jsonData);
            foreach (dynamic image in imageJson[0].image)
            {

                src = image.Value;
                if (src!="") { 
                    dr = prodImages.NewRow();
                    dr["url"] = src;
                    dr["image_name"] = Model + "_" + i.ToString() + ".png";
                    dr["isdata"] = "false";
                    prodImages.Rows.Add(dr);
                    i++;
                }
            }

           

            return prodImages;
        }


        public override CategoryTable getCategoryPath()
        {
            CategoryTable categoryPathTable = new CategoryTable();

            DataRow categoryPath = categoryPathTable.NewRow();
            categoryPath["language_id"] = "1";
            categoryPath["category_path"] = CategoryPath;
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
            return retVal;
        }




        public override OptionTable[] getOptions()
        {
            
            return options;
        }

        private OptionTable ScrapOptions(dynamic catJson)
        {
            OptionTable dtSize = new OptionTable();
            DataRow drSize;

            foreach (dynamic size in catJson.sizes)
            {

                drSize = dtSize.NewRow();
                drSize["option_name"] = "Beden";
                drSize["required"] = 1;
                drSize["option_value"] = size.name;
                drSize["price"] = 0;
                if (catJson.availability== "in_stock")
                    drSize["quantity"] = "99";
                else
                    drSize["quantity"] = "0";
                drSize["option_image"] = "";
                drSize["parent_option_value"] = "";
                drSize["parent_option_name"] = "";
                dtSize.Rows.Add(drSize);
                
            }            
            return dtSize;
        }


    }

}
