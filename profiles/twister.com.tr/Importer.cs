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

namespace twister.com.tr
{
    public class Importer : Parser
    {
        dynamic productJSON,productDetailJSON;
        string productData;
        List<string> optionAttruibuteNames, optionAttruibuteLabels;
        Dictionary<string, string> propertyCollection = new Dictionary<string, string>();
        WebBrowser thisBrowser;
        string currentCat;
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
            return urls;
        }


        public override string buildCategoryURL(string catURL, int page)
        {
            currentCat = catURL;
            switch (currentCat)
            {
                case "tayt":
                    catURL = "103";
                    break;
                case "kadin-saat":
                    catURL = "123";
                    break;
                case "buyuk-beden":
                    catURL = "115";
                    break;
                case "jean2":
                    catURL = "120";
                    break;
                case "jean1":
                    catURL = "119";
                    break;
                case "t-shirt":
                    catURL = "118";
                    break;
                case "super-firsat":
                    catURL = "99";
                    break;
                case "takim1":
                    catURL = "106";
                    break;
            }
            

            catURL = "https://twister.com.tr/api/product/GetProductList?FilterJson={\"CategoryIdList\":["
                + catURL + "],\"BrandIdList\":[],\"SupplierIdList\":[],\"TagIdList\":[],\"TagId\":-1,\"FilterObject\":[],\"MinStockAmount\":-1,\"IsShowcaseProduct\":-1,\"IsOpportunityProduct\":-1,\"FastShipping\":-1,\"IsNewProduct\":-1,\"IsDiscountedProduct\":-1,\"IsShippingFree\":-1,\"IsProductCombine\":-1,\"MinPrice\":0,\"MaxPrice\":0,\"SearchKeyword\":\"\",\"StrProductIds\":\"\",\"IsSimilarProduct\":false,\"RelatedProductId\":0,\"ProductKeyword\":\"\",\"PageContentId\":0,\"StrProductIDNotEqual\":\"\",\"IsVariantList\":-1,\"IsVideoProduct\":-1,\"ShowBlokVideo\":-1,\"VideoSetting\":{\"ShowProductVideo\":-1,\"AutoPlayVideo\":-1},\"ShowList\":1,\"VisibleImageCount\":6,\"ShowCounterProduct\":-1}&PagingJson={\"PageItemCount\":0,\"PageNumber\":" +
                page.ToString() +",\"OrderBy\":\"KATEGORISIRA\",\"OrderDirection\":\"ASC\"}&CreateFilter=false";
            return catURL;
        }

        public override List<string> getItemURLs()
        {
            List<string> urls = new List<string>();
            string TopCat = currentCat;

            string href = "";
            
            dynamic itemsObj = JsonConvert.DeserializeObject(Document.InnerText);

            
            int iter = 1;
            foreach (dynamic item in itemsObj.products)
            {
                urls.Add("https://twister.com.tr" + item.url.Value);
                iter++;
            }

            return urls;
        }


        public override Dictionary<int,string> getTitles()
        {

            int startPos = Document.InnerHtml.IndexOf("<script type=\"application/ld+json\">");
            int endPos = Document.InnerHtml.IndexOf("</script>", startPos);
            productData = Document.InnerHtml.Substring(startPos + "<script type=\"application/ld+json\">".Length, endPos - startPos- 35);
            productJSON = JsonConvert.DeserializeObject(productData);

            startPos = Document.InnerHtml.IndexOf("productDetailModel = ");
            startPos = startPos + ("productDetailModel = ").Length;
            endPos = Document.InnerHtml.IndexOf("};", startPos);
            productData = Document.InnerHtml.Substring(startPos, endPos - startPos + 1);
            productDetailJSON = JsonConvert.DeserializeObject(productData);

            string TitleText="";
            try
            {
                //aNode = Document.SelectSingleNode("h1");
                TitleText = productJSON.name.ToString();
            }
            catch (Exception ex)
            {
                return Titles;
            }

            Titles.Clear();
            foreach (string language in Languages)
            {
                Titles.Add(int.Parse(language), TitleText);
            }
            optionAttruibuteNames = new List<string>();
            optionAttruibuteLabels = new List<string>();

            prodImages = new ImageTable();

            return Titles;
        }

        public override string getModel()
        {
            // Model = Document.SelectSingleNode("//span[@id='divUrunKodu']").InnerText.Trim().Replace(")","").Replace("(","");
            Model = productJSON.sku.ToString();
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getStatus()
        {
            if (productJSON.offers.availability.ToString() == "InStock")
                return "1";
            else
                return "0";
        }


        public override string getStock()
        {
            if (productJSON.offers.availability.ToString() == "InStock")
                return "99";
            else
                return "0";
        }

        public override string getPrice()
        {
            options = new OptionTable[Languages.Length];
            options[0] = ScrapOptions();
            if (options.Length>1)
                options[1] = (OptionTable)options[0].Copy();
            if (options.Length > 2)
                options[2] = (OptionTable)options[0].Copy();


            string price = Document.SelectNodes("//span[@class='spanFiyat']")[0].InnerText;
            price= price.Replace(".", "").Replace(",", ".").Replace("₺", "").Trim();
            return price;

        }

        public override SpecialTable getSpecial()
        {
            SpecialTable special = new SpecialTable();
            DataRow dr = special.NewRow();

            string price = Document.SelectNodes("//span[@class='spanFiyat']")[1].InnerText;
            price = price.Replace(".", "").Replace(",", ".").Replace("₺", "").Trim();
            if (price != "")
            {
                price = price.Replace(".", "").Replace(",", ".").Replace(" TL", "").Trim();
                dr["customer_group_id"] = "1";
                dr["price"] = price;
                special.Rows.Add(dr);
            }
            return special;
        }

        public override string getManufacturer()
        {
            //string brand = Document.SelectSingleNode("//span[@class='right_line Marka']").InnerText.Trim();
            string brand = productJSON.brand.name.ToString();
            return brand;
        }


        public override Dictionary<int,string> getDescriptions()
        {
            //string desc = Document.SelectNodes("//div[@class='urunTabAlt']")[0].InnerHtml;            
            string desc = productJSON.description.ToString();
            //desc = "";
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
            foreach (dynamic image in productJSON.image)
            {
                src = image;
                uri = new Uri(src);
                dr = prodImages.NewRow();
                dr["url"] = src;
                dr["image_name"] = Model + "_" + i.ToString() + System.IO.Path.GetExtension(uri.LocalPath); ;
                prodImages.Rows.Add(dr);
                i++;
            }

            

            return prodImages;
        }


        public override CategoryTable getCategoryPath()
        {
            CategoryTable categoryPathTable = new CategoryTable();
            string catPath = "";
            int i = 0;
            foreach (HAP.HtmlNode categoryItem in Document.SelectNodes("//a[@itemprop='item']"))
            {
                if (i == Document.SelectNodes("//a[@itemprop='item']").Count - 1) break;
                if (i > 0)
                {
                    catPath = catPath + categoryItem.InnerText.Trim() + "///";
                }
                i++;
            }
            if (catPath.Length != 0)
                catPath = catPath.Substring(0, catPath.Length - 3);


            DataRow categoryPath = categoryPathTable.NewRow();
            categoryPath["language_id"] = "1";
            categoryPath["category_path"] = catPath;
            categoryPathTable.Rows.Add(categoryPath);

            return categoryPathTable;
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
            string url = "";
            dynamic colors; DataRow drSize, drColor;
            OptionTable dtColor = new OptionTable();
            OptionTable dtSize = new OptionTable();
            string colorname;
            bool optionsFound = false;
            productStatus = true;

            if (productDetailJSON.productVariantData != null)
            {
                foreach (dynamic variant in productDetailJSON.productVariantData)
                {
                    if (variant.ekSecenekTipiTanim == "Beden Seç")
                    {
                        if (variant.stokAdedi == "1")
                        {
                            drSize = dtSize.NewRow();
                            drSize["option_name"] = "Beden Seç";
                            drSize["required"] = 1;
                            drSize["option_value"] = variant.tanim;
                            drSize["price"] = 0;
                            drSize["option_image"] = "";
                            drSize["parent_option_value"] = "";
                            drSize["quantity"] = "99";
                            dtSize.Rows.Add(drSize);
                        }
                    }
                    else
                    {
                        if (variant.stokAdedi == "1")
                        {
                            drColor = dtColor.NewRow();

                            drColor["option_name"] = variant.ekSecenekTipiTanim;
                            drColor["required"] = 1;
                            drColor["option_value"] = variant.tanim;
                            drColor["price"] = 0;
                            drColor["quantity"] = "99";
                            dtColor.Rows.Add(drColor);
                            break;
                        }
                    }
                }



                dtColor.Merge(dtSize);
            
                if (dtColor.Rows.Count == 0 )
                productStatus = false;
            }
            return dtColor;
        }


    }

}
