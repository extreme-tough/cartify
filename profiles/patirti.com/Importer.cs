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
using System.Text.RegularExpressions;

namespace patirti.com
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
        Dictionary<string, string> abbrevs = new Dictionary<string, string>();
        ImageTable prodImages;
        bool productStatus = true;
        float productPrice;
        string catPath;
        public Importer()
        {
        }

        /*
        public override List<Category> getCategoryURLs()
        {
            Category objCategory = new Category();
            List<Category> urls = new List<Category>();            
            objCategory = new Category(); objCategory.Add("", "kadin-giyim-x-g1-c82"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kadin-ayakkabi-x-g1-c114"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kadin-aksesuar-x-g1-c27"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kadin-ic-giyim-x-g1-c64"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kozmetik-x-c89"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kozmetik-x-c89"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kadin-spor-outdoor-x-g1-c104593"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "erkek-giyim-x-g2-c82"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "erkek+ayakkabi"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "erkek+kozmetik"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "erkek+aksesuar"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "erkek+ic-giyim"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "erkek+spor-outdoor"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "elektronik"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "erkek?q=l%C3%BCks"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "bebek-giyim"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "cocuk+bebek-bezi"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "cocuk+bebek-mamalari"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "cocuk+islak-mendil"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "cocuk+giyim?yasgrubu=1-2"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "cocuk+giyim?yasgrubu=2-2"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "cocuk+annebebek-bakim"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "oyuncak"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "cocuk-gerecleri"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "beslenme-emzirme"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "sofra--mutfak"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "banyo"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "ev-tekstili"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "mobilya"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "ev-dekorasyon"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "spor-aletleri"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kitap--muzik--hobi"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kirtasiye-ofis-malzemeleri"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "otomobil-ve-motosiklet"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "yapi-market"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "ev-bakim-ve-temizlik"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "tum--urunler?q=G%c4%b1da%20%c3%9cr%c3%bcnleri%20%c4%b0%c3%a7ecek%20%c3%9cr%c3%bcnleri"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kozmetik"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "saglik"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "pet-shop-urunleri"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "tum--urunler?q=Natracare%20Logona%20Sonett%20The%20LifeCo%20Sante%20Desert%20Essence%20Kiss%20My%20Face%20Aksu%20Vital%20Shiffa%20Home%20Softem%20Cotoneve%20Otac%c4%b1%20Moos%20Seventh%20Generation%20Do%c4%9fal%20Tutku%20Baktat%20Humm%20Organic"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "annebebek-bakim"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "makyaj"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "parfum-ve-deodorant"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "cilt-bakimi"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "tum--urunler?q=Sa%c3%a7%20Bak%c4%b1m%c4%b1%20Sa%c3%a7%20%c5%9eekillendirici"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "tiras-agda-epilasyon"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kozmetik"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kadin+ayakkabi"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "erkek+ayakkabi"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "erkek+canta"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "cocuk+ayakkabi"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "cocuk+canta"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "tum--urunler?q=Ayakkab%C4%B1%20%C3%87anta+l%C3%BCks"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kadin+aksesuar"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "taki"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "erkek+aksesuar"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "cocuk+aksesuar"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "kucuk-ev-aletleri"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "teknoloji-urunleri"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "telefon"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "bilgisayar-tablet"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "oyuncu-bilgisayari"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "tv-goruntu-ses-sistemleri"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "oyun-ve-oyun-konsollari"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "foto-ve-kamera"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "beyaz-esya"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "elektronik-kisisel-bakim"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "elektronik"); urls.Add(objCategory);
            return urls;
        }*/


        public override string buildCategoryURL(string catURL, int page)
        {            
            //catURL =  "https://api.trendyol.com/websearchgw/v2/api/infinite-scroll/" + catURL + (catURL.Contains("?") ? "&": "?") + "pi=" + page.ToString() + "&storefrontId=1&culture=tr-TR&userGenderId=1&pId=0&scoringAlgorithmId=2&categoryRelevancyEnabled=false&isLegalRequirementConfirmed=False&searchStrategyType=DEFAULT";
            catURL = catURL + "?pagenumber=" + page.ToString();
            return catURL;
        }

        public override List<string> getItemURLs()
        {
            List<string> urls = new List<string>();
            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//div[@class='fl col-12 productItemImage']/a");
            if (nodes == null) return urls;
            foreach (HAP.HtmlNode item in nodes)
            {
                urls.Add("https://www.patirti.com" + item.GetAttributeValue("href", "").Replace("&amp;", "&") );
            }

            return urls;
        }


        public override Dictionary<int,string> getTitles()
        {
            string TitleText="";
            HAP.HtmlNode aNode;
            try
            {
                aNode = Document.SelectSingleNode("//h1");
                TitleText = aNode.InnerText.Trim();                
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
                Titles.Add(int.Parse(language), TitleText);
            }

            return Titles;
        }

        public override string getModel()
        {
            HAP.HtmlNode aNode = Document.SelectSingleNode("//span[@itemprop='sku']");
            Model = aNode.InnerText.Trim();
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getStatus()
        {
            //TODO: Yet to do
            if (Document.InnerHtml.Contains("<button class=\"pr-in-btn add-to-bs so\">Tükendi</button>"))
                productStatus = false;
            if (productStatus)
                return "1";
            else
                return "0";
        }

        public override string getPrice()
        {
            HAP.HtmlNode aNode = Document.SelectSingleNode("//div[@class='currentPrice']");
            string price;
            if (aNode != null)
                price = aNode.InnerText.Split(new char[] { Convert.ToChar(8378) }, StringSplitOptions.None)[0].Trim();
            else
            {
                aNode = Document.SelectSingleNode("//div[@class='addPriceDiscount']");
                price = aNode.SelectSingleNode("div/span").InnerText.Split(new char[] { Convert.ToChar(8378) }, StringSplitOptions.None)[0].Trim(); 

            }
            // productPrice = float.Parse(price);
            options = new OptionTable[Languages.Length];
            options[0] = ScrapOptions();
            options[1] = (OptionTable)options[0].Copy();
                // options[2] = (OptionTable)options[0].Copy();
            
            return price;

        }

        public override SpecialTable getSpecial()
        {
            SpecialTable special = new SpecialTable();
            /*DataRow dr = special.NewRow();
            string price = productJSON.product.price.discountedPrice.text;
            price = price.Replace(".", "").Replace(",", ".").Replace(" TL", "").Trim();
            dr["customer_group_id"] = "1";
            dr["price"] = price;
            special.Rows.Add(dr); */
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
                // desc = Document.SelectNodes("//div[@class='pr-in-dt-cn']")[0].InnerHtml;
                desc = Document.SelectNodes("//div[@id='productDescription']")[0].InnerHtml;
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
            string src; Uri uri;
            DataRow dr;
            int i = 0;

            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//div[@class='box col-10 col-sm-12 productImage']/ul/li");
            prodImages = new ImageTable();

            foreach (HAP.HtmlNode li in nodes)
            {
                
                src =  li.SelectSingleNode("a").GetAttributeValue("href", "");
                uri = new Uri(src);
                dr = prodImages.NewRow();
                dr["url"] = src;
                dr["image_name"] = Model + "_" + i.ToString() + System.IO.Path.GetExtension(uri.LocalPath); ;
                prodImages.Rows.Add(dr);
                
                i++;
            }
        }

        public override ImageTable getImages()
        {
            parseImages();
            return prodImages;
        }


        public override CategoryTable getCategoryPath()
        {
            CategoryTable categoryPathTable = new CategoryTable();
            string catPath = "";

            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//li[@class='breadcrumb-item']");

            foreach (HAP.HtmlNode li in nodes)
            {
                if (li.InnerText.Trim() != "Ana Sayfa")
                {
                    catPath = catPath + li.InnerText.Trim()+ "///";
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

        private OptionTable ScrapOptions()
        {

            HAP.HtmlNodeCollection nodes = Document.SelectNodes("//label[@class='custom-control custom-radio ']");
            OptionTable dtColor = new OptionTable();
            OptionTable dtSize = new OptionTable();
            DataRow drSize;
            string size;
            if (nodes==null)
            {
                nodes = Document.SelectNodes("//label[@class='custom-control custom-radio SelectRadio']");                
            }
            foreach (HAP.HtmlNode node in nodes)
            {
                string strClass = node.ParentNode.ParentNode.GetAttributeValue("class", "");
                size = node.InnerText.Trim();

                if (strClass == "Item ")
                {
                    drSize = dtSize.NewRow();
                    drSize["option_name"] = "Beden";
                    drSize["required"] = 1;
                    drSize["option_value"] = size;

                    drSize["price_prefix"] = "+";
                    drSize["price"] = "0";
                    drSize["quantity"] = "99";
                    drSize["option_image"] = "";

                    dtSize.Rows.Add(drSize);
                }
            }
            return dtSize;
        }
    }

}
