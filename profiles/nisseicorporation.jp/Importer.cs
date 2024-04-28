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
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using Aiplib;
using System.Configuration;

namespace nisseicorporation.jp
{
    public class Importer : Parser
    {
        dynamic productJSON;
        string productData;
        string firstItem="";
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
        public Importer()
        {
        }

        public override List<Category> getCategoryURLs()
        {
            List<Category> retval = new List<Category>();
            HAP.HtmlNodeCollection links =  Document.SelectNodes("//ul[@class='tree dhtml']/li/a");
            foreach (HAP.HtmlNode link in links)
            {
                Category catItem = new Category();
                string CatLink = link.GetAttributeValue("href", "");

                int elapsed = 0, timeeout = 60;
                Browser.Navigate(CatLink);
                while (Browser.ReadyState != WebBrowserReadyState.Complete)
                {
                    Thread.Sleep(1000);
                    elapsed += 1000;
                    if (elapsed > (timeeout * 1000))
                    {
                        break;
                    }
                    Application.DoEvents();
                }
               
                HAP.HtmlDocument subDoc = new HAP.HtmlDocument();
                subDoc.LoadHtml(Browser.Document.Body.InnerHtml);

                HAP.HtmlNodeCollection subCats = subDoc.DocumentNode.SelectNodes("//ul[@class='inline_list']/li/a");
                if (subCats != null)
                {
                    foreach (HAP.HtmlNode subCat in subCats)
                    {
                        string subCatLink;
                        if (subCat.GetAttributeValue("title", "") != "")
                        {
                            subCatLink = subCat.GetAttributeValue("href", "");
                            catItem = new Category();
                            catItem.Add("", subCatLink);
                            retval.Add(catItem);
                        }
                    }
                }
                else {
                    catItem = new Category();
                    catItem.Add("", CatLink);
                    retval.Add(catItem);
                } 
            }
            return retval;
        }
        public override string buildCategoryURL(string catURL, int page)
        {
            currentCat = catURL;
            catURL = catURL + "&p=" + page;
            return catURL;
        }

        public override List<string> getItemURLs()
        {
            List<string> urls = new List<string>();
            string TopCat = currentCat;
            string subCatLink;
            HAP.HtmlNodeCollection nodes;

            /*
            HAP.HtmlNodeCollection subCats = Document.SelectNodes("//ul[@class='inline_list']/li/a");
            foreach (HAP.HtmlNode subCat in subCats)
            {
                subCatLink = subCat.GetAttributeValue("href", "");
                

                nodes = Document.SelectNodes("//a[@class='product_img_link']");

                if (nodes != null)
                {
                    foreach (HAP.HtmlNode item in nodes)
                    {
                        urls.Add(item.GetAttributeValue("href", "").Replace("&amp;", "&"));
                    }
                }
            }
            */


            nodes = Document.SelectNodes("//a[@class='product_img_link']");
        
            if (nodes != null)
            {
                int k = 0;
                foreach (HAP.HtmlNode item in nodes)
                {
                    if (item.GetAttributeValue("href", "").Replace("&amp;", "&") == firstItem && firstItem!="")
                        return null;

                    if (k == 0)
                        firstItem = item.GetAttributeValue("href", "").Replace("&amp;", "&");
                    urls.Add(item.GetAttributeValue("href", "").Replace("&amp;", "&"));
                    k++;
                }
            }
            catPath = Document.SelectNodes("//a[@class='selected']")[0].InnerText;
            return urls;
        }


        public override int getMaxImports()
        {            
            return -1;
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
            
            prodImages = new ImageTable();
            TotalImported++;
            return Titles;
        }

        public override string getModel()
        {
            if (Document.SelectNodes("//input[@name='id_product']") == null) return "";
            Model = Document.SelectNodes("//input[@name='id_product']")[0].GetAttributeValue("value","").Trim();
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getPrice()
        {

            HAP.HtmlNode priceElem= Document.SelectSingleNode("//span[@id='our_price_display']");
            if (priceElem == null) return "0.00";
            string price = priceElem.InnerText.Replace("¥ ","").Replace(",","").Trim();
            return price;

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
            HAP.HtmlNode descElem = Document.SelectSingleNode("//div[@id='idTab1']");
            string desc = descElem.InnerHtml;
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
            HAP.HtmlNodeCollection imageTags = Document.SelectNodes("//a[contains(@class, 'thickbox')]");
            if (imageTags != null)
            {
                foreach(HAP.HtmlNode imageTag in imageTags)
                {
                    string imgSrc = imageTag.GetAttributeValue("href", "");
                    uri = new Uri(imgSrc);
                    dr = prodImages.NewRow();
                    dr["url"] = imgSrc;
                    prodImages.Rows.Add(dr);

                }

                return prodImages;
            }
            return null;
        }


        public override CategoryTable getCategoryPath()
        {
            HAP.HtmlNodeCollection breadcrumbs = Document.SelectNodes("//div[@class='breadcrumb']/a");
            int i = 0;
            string[] breadCrumbPath = new string[breadcrumbs.Count-1];
            foreach (HAP.HtmlNode breadcrumb  in breadcrumbs)
            {
                i++;
                if (i == 1)
                    continue;
                else
                    breadCrumbPath[i - 2] = breadcrumb.InnerText;
            }
            catPath = String.Join (@"///" , breadCrumbPath);
            CategoryTable categoryPathTable = new CategoryTable();
            DataRow categoryPath = categoryPathTable.NewRow();
            categoryPath["language_id"] = "1";
            categoryPath["category_path"] = catPath;
            categoryPathTable.Rows.Add(categoryPath);
            categoryPath = categoryPathTable.NewRow();
            categoryPath["language_id"] = "2";
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

            OptionTable[] retVal = new OptionTable[2];
            retVal[0] = ScrapOptions();
            retVal[1] = retVal[0];

            return retVal;
        }

        public override string getStatus()
        {
            return "0";
        }
        private OptionTable ScrapOptions()
        {
            OptionTable dtColor = new OptionTable();
            DataRow drColor;
            HAP.HtmlNodeCollection options= Document.SelectNodes("//select[@id='group_3']/option");
            if (options != null)
            {
                foreach(HAP.HtmlNode option in options)
                {
                    drColor = dtColor.NewRow();
                    drColor["option_name"] = "Option";
                    drColor["required"] = 1;
                    drColor["option_value"] = option.InnerText;
                    drColor["price"] = 0; 
                    dtColor.Rows.Add(drColor);
                }
            }
            return dtColor;
        }


    }

}
