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

namespace trendyol.com
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
            currentCat = catURL;
            catURL =catURL.Replace(",","%2C");
            //catURL =  "https://api.trendyol.com/websearchgw/v2/api/infinite-scroll/" + catURL + (catURL.Contains("?") ? "&": "?") + "pi=" + page.ToString() + "&storefrontId=1&culture=tr-TR&userGenderId=1&pId=0&scoringAlgorithmId=2&categoryRelevancyEnabled=false&isLegalRequirementConfirmed=False&searchStrategyType=DEFAULT";
            catURL = "https://public.trendyol.com/discovery-web-searchgw-service/v2/api/infinite-scroll/" + catURL + (catURL.Contains("?") ? "&" : "?") + "pi=" + page.ToString() + "&storefrontId=1&culture=tr-TR&userGenderId=1&pId=wx5TNZbqLU&scoringAlgorithmId=2&categoryRelevancyEnabled=false&isLegalRequirementConfirmed=false&searchStrategyType=DEFAULT&productStampType=TypeA&searchTestTypeAbValue=B";
            return catURL;
        }

        public override List<string> getItemURLs()
        {
            List<string> urls = new List<string>();
            string TopCat = currentCat;

            string href = "";
            
            dynamic itemsObj = JsonConvert.DeserializeObject(Document.InnerText);
            Newtonsoft.Json.Linq.JObject itemsObj1 =  Newtonsoft.Json.Linq.JObject.Parse(Document.InnerText);


            int iter = 1;
            foreach (dynamic item in itemsObj.result.products)
            {
                urls.Add("https://www.trendyol.com" + item.url.Value);
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
                aNode = Document.SelectNodes("//div[@class='pr-in-cn']")[0].ChildNodes[0].ChildNodes[0];
                if (aNode.SelectSingleNode("a") != null)
                    TitleText = aNode.SelectSingleNode("a").InnerText + aNode.SelectSingleNode("span").InnerText;
                else if (aNode.SelectSingleNode("font") != null)
                    TitleText = aNode.SelectSingleNode("font").InnerText + aNode.SelectSingleNode("span").InnerText;
                else
                    TitleText = aNode.InnerText;
            }
            catch 
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
            optionAttruibuteNames = new List<string>();
            optionAttruibuteLabels = new List<string>();

            int startPos = Document.InnerHtml.IndexOf("window.__PRODUCT_DETAIL_APP_INITIAL_STATE__=");
            if (startPos == -1)
            {
                startPos = Document.InnerHtml.IndexOf("window.__PRODUCT_DETAIL_APP_INITIAL_STATE__ = ");
                startPos = startPos + "window.__PRODUCT_DETAIL_APP_INITIAL_STATE__ = ".Length;
            }
            else {
                startPos = startPos + "window.__PRODUCT_DETAIL_APP_INITIAL_STATE__=".Length;
            }
            int endPos = Document.InnerHtml.IndexOf("}};", startPos);
            productData = Document.InnerHtml.Substring(startPos, endPos - startPos + 2);
            productJSON = JsonConvert.DeserializeObject(productData);
            prodImages = new ImageTable();

            WebClient wc = new WebClient();
            string marketData = wc.DownloadString("https://public.trendyol.com/discovery-storefrontmarketing-webmarketinggw-service/v1/product-detail/monokido/tigo-tribe-purple-sweatshirt-p-143578048?culture=tr-TR&storefrontId=1");
            dynamic marketDataJSON = JsonConvert.DeserializeObject(marketData);
            string rawBreadCrumb = marketDataJSON.result.partials.breadcrumb.html.Value;
            MatchCollection anchors = Regex.Matches(rawBreadCrumb, "(?<=<a[^>]*>).*?(?=</a>)", RegexOptions.IgnoreCase);
            List<string> newlist = new List<string>();
            foreach(Match match in anchors)
            {
                Match result = Regex.Match(match.Value, "(?<=<span[^>]*>).*?(?=</span>)", RegexOptions.IgnoreCase);
                newlist.Add(result.Value);
            }
            catPath = String.Join("///", newlist.ToArray());

            return Titles;
        }

        public override string getModel()
        {
            Model = productJSON.product.id;
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getStatus()
        {
            if (Document.InnerHtml.Contains("<button class=\"pr-in-btn add-to-bs so\">Tükendi</button>"))
                productStatus = false;
            if (productStatus)
                return "1";
            else
                return "0";
        }

        public override string getPrice()
        {
            parseImages();

            if (productJSON == null)
            {
                int startPos = Document.InnerHtml.IndexOf("window.__PRODUCT_DETAIL_APP_INITIAL_STATE__ = ");
                if (startPos == -1)
                {
                    startPos = Document.InnerHtml.IndexOf("window.__PRODUCT_DETAIL_APP_INITIAL_STATE__=");
                    startPos = startPos + "window.__PRODUCT_DETAIL_APP_INITIAL_STATE__=".Length;
                } else
                {
                    startPos = startPos + "window.__PRODUCT_DETAIL_APP_INITIAL_STATE__ = ".Length;
                }

                int endPos = Document.InnerHtml.IndexOf("}};", startPos);
                productData = Document.InnerHtml.Substring(startPos, endPos - startPos + 2);
                productJSON = JsonConvert.DeserializeObject(productData);
            }
            // Below login changed to get price as max price based on discussion from 23rd Jun 2022
            double maxPrice = 0;
            if (productJSON.product.price.discountedPrice.value.Value > maxPrice)
                maxPrice = productJSON.product.price.discountedPrice.value.Value;
            if (productJSON.product.price.sellingPrice.value.Value > maxPrice)
                maxPrice = productJSON.product.price.sellingPrice.value.Value;
            if (productJSON.product.price.originalPrice.value.Value> maxPrice)
                maxPrice = productJSON.product.price.originalPrice.value.Value;
            string price = maxPrice.ToString();
            /* price= price.Replace(".", "").Replace(",", ".").Replace(" TL","").Trim();
            productPrice = float.Parse(price);*/
            options = new OptionTable[Languages.Length];
            options[0] = ScrapOptions();
            options[1] = (OptionTable)options[0].Copy();
            // options[2] = (OptionTable)options[0].Copy();

            return price;

        }

        public override SpecialTable getSpecial()
        {
            SpecialTable special = new SpecialTable();
            DataRow dr = special.NewRow();

            // Below login changed to get price as max price based on discussion from 23rd Jun 2022
            double discountPrice = float.MaxValue;
            if (productJSON.product.price.discountedPrice.value.Value < discountPrice)
                discountPrice = productJSON.product.price.discountedPrice.value.Value;
            if (productJSON.product.price.sellingPrice.value.Value < discountPrice)
                discountPrice = productJSON.product.price.sellingPrice.value.Value;
            if (productJSON.product.price.originalPrice.value.Value < discountPrice)
                discountPrice = productJSON.product.price.originalPrice.value.Value;

            string price = discountPrice.ToString();
            // price = price.Replace(".", "").Replace(",", ".").Replace(" TL", "").Trim();
            dr["customer_group_id"] = "1";
            dr["price"] = price;
            special.Rows.Add(dr);
            return special;
        }

        public override string getManufacturer()
        {
            string brand = productJSON.product.brand.name;
            return brand;
        }


        public override Dictionary<int,string> getDescriptions()
        {
            string desc;
            try {
                // desc = Document.SelectNodes("//div[@class='pr-in-dt-cn']")[0].InnerHtml;
                desc = Document.SelectNodes("//section[@class='details-section']")[0].InnerHtml;
            }
            catch
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

            foreach (dynamic image in productJSON.product.images)
            {
                src = "https://cdn.dsmcdn.com/mnresize/415/622" + image;
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
            
            return prodImages;
        }


        public override CategoryTable getCategoryPath()
        {
            CategoryTable categoryPathTable = new CategoryTable();
            /* string catPath = "";
            foreach (dynamic categoryItem in productJSON.product.breadcrumb.items)
            {
                if (categoryItem.type != "BRAND")
                {
                    catPath = catPath + categoryItem.name + "///";
                }
            }
            if (catPath.Length != 0)
                catPath = catPath.Substring(0, catPath.Length - 3);
            */

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
            foreach(dynamic attribute in productJSON.product.attributes)
            {
                foreach(string language in Languages)
                {
                    DataRow dr = retVal.NewRow();
                    dr["language_id"] = language;
                    dr["name"] = attribute.key.name;
                    dr["value"] = attribute.value.name;
                    retVal.Rows.Add(dr);
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
            // string url= "https://api.trendyol.com/webbrowsinggw/api/productGroup/" + productJSON.product.productGroupId.Value + "?storefrontId=1&culture=tr-TR";
            string url = "https://public.trendyol.com/discovery-web-productgw-service/api/productGroup/" + productJSON.product.productGroupId.Value + "?storefrontId=1&culture=tr-TR";

            string groupData= Functions.DownloadFile(url);
            dynamic groupJSON = JsonConvert.DeserializeObject(groupData);
            dynamic colors; DataRow drSize, drColor;
            OptionTable dtColor = new OptionTable();
            OptionTable dtSize = new OptionTable();
            string colorname;
            bool optionsFound = false;
            productStatus = true;
            if (groupJSON.result.slicingAttributes.Count >= 1)
            {
                colors = groupJSON.result.slicingAttributes[0].attributes;
                //Go through each color images
                foreach (dynamic color in colors)
                {
                    optionsFound = true;
                    colorname = color.name;
                    //Get the product url of the color item
                    string other_url = "https://www.trendyol.com" + color.contents[0].url;

                    drColor = dtColor.NewRow();
                    drSize = dtColor.NewRow();
               
                    drColor["option_name"] = groupJSON.result.slicingAttributes[0].displayName;
                    drColor["required"] = 1;
                    drColor["option_value"] = colorname;
                    drColor["price"] = 0;
                    

                    if (this.URL == other_url)
                    {

                        string attName = "";
                        foreach (dynamic variant in productJSON.product.variants)
                        {
                            attName = variant.attributeName;
                        }

                        //It is current product
                        foreach (dynamic variant in productJSON.product.allVariants)
                        {
                            if (variant.inStock.ToString() != "False" && attName != "")
                            {
                                drSize = dtSize.NewRow();
                                drSize["option_name"] = attName;
                                drSize["required"] = 1;
                                drSize["option_value"] = variant.value;

                                /* float optionPrice = float.Parse(variant.price.discountedPrice.value.ToString());
                                if (optionPrice > productPrice)
                                {
                                    drSize["price"] = (optionPrice - productPrice).ToString();
                                    drSize["price_prefix"] = "+";
                                }
                                else if (optionPrice < productPrice)
                                {
                                    drSize["price"] = (productPrice - optionPrice).ToString();
                                    drSize["price_prefix"] = "-";
                                }
                                else
                                {
                                    drSize["price"] = "0";
                                } */

                                // drSize["price"] = variant.price.discountedPrice.value.ToString();
                                drSize["price_prefix"] = "+";
                                drSize["price"] = "0";

                                if (variant.isRunningOut=="True")
                                    drSize["quantity"] = "99";
                                else
                                    drSize["quantity"] = "0";
                                drSize["option_image"] = "";
                                drSize["parent_option_value"] = colorname;
                                drSize["parent_option_name"] = groupJSON.result.slicingAttributes[0].displayName;
                                dtSize.Rows.Add(drSize);
                            }
                        }
                        drColor["option_image"] = prodImages.Rows.Count>0 ? prodImages.Rows[0]["url"].ToString() : "";
                        drColor["option_image_name"] = prodImages.Rows.Count > 0 ? prodImages.Rows[0]["image_name"].ToString() : "";
                        dtColor.Rows.Add(drColor);
                    }
                    else
                    {
                        string otherProd = Functions.DownloadFile(other_url);
                        int startPos = otherProd.IndexOf("window.__PRODUCT_DETAIL_APP_INITIAL_STATE__=");
                        if (startPos == -1)
                        {
                            startPos = otherProd.IndexOf("window.__PRODUCT_DETAIL_APP_INITIAL_STATE__ =");
                            startPos = startPos + "window.__PRODUCT_DETAIL_APP_INITIAL_STATE__ =".Length;
                        }
                        else {
                            startPos = startPos + "window.__PRODUCT_DETAIL_APP_INITIAL_STATE__=".Length;
                        }
                        int endPos = otherProd.IndexOf("}};", startPos);
                        string otherProdData = otherProd.Substring(startPos, endPos - startPos + 2);
                        dynamic otherProdJSON = JsonConvert.DeserializeObject(otherProdData);
                        string imgsrc = "https://cdn.dsmcdn.com/mnresize/415/622" + otherProdJSON.product.images[0];

                        string attName = "",attValue ="";
                        foreach (dynamic variant in otherProdJSON.product.variants)
                        {
                            attName = variant.attributeName;                            
                        }

                        foreach (dynamic variant in otherProdJSON.product.allVariants)
                        {
                            if (variant.inStock.ToString() != "False" && attName != "")
                            {
                                drSize = dtSize.NewRow();
                                drSize["option_name"] = attName;
                                drSize["required"] = 1;
                                drSize["option_value"] = variant.value;

                                /* float optionPrice = float.Parse(variant.price.discountedPrice.value.ToString());
                                if (optionPrice > productPrice)
                                {
                                    drSize["price"] = (optionPrice - productPrice).ToString();
                                    drSize["price_prefix"] = "+";
                                }
                                else if (optionPrice < productPrice)
                                {
                                    drSize["price"] = (productPrice - optionPrice).ToString();
                                    drSize["price_prefix"] = "-";
                                }
                                else
                                {
                                    drSize["price"] = "0";
                                } */

                                // drSize["price"] = variant.price.discountedPrice.value.ToString();
                                drSize["price_prefix"] = "+";
                                drSize["price"] = "0";
                                drSize["quantity"] = "99";
                                drSize["option_image"] = "";
                                drSize["parent_option_value"] = colorname;
                                drSize["parent_option_name"] = groupJSON.result.slicingAttributes[0].displayName;
                                dtSize.Rows.Add(drSize);
                            }
                        }
                        string src; Uri uri;
                        uri = new Uri(imgsrc);

                        drColor["option_image"] = imgsrc;
                        drColor["option_image_name"] = otherProdJSON.product.id + "_0".ToString() + System.IO.Path.GetExtension(uri.LocalPath);

                        int i = 0;
                        DataRow dr;
                        foreach (dynamic image in otherProdJSON.product.images)
                        {
                            src = "https://cdn.dsmcdn.com/mnresize/415/622" + image;
                            uri = new Uri(src);
                            dr = prodImages.NewRow();
                            dr["url"] = src;
                            dr["image_name"] = otherProdJSON.product.id + "_" + i.ToString() + System.IO.Path.GetExtension(uri.LocalPath);
                            prodImages.Rows.Add(dr);
                            i++;
                        }

                        dtColor.Rows.Add(drColor);
                    }


                }
            } else
            {
                string attName="";
                //No color present
                foreach (dynamic variant in productJSON.product.variants)
                {
                    attName = variant.attributeName;
                    break;
                }

                foreach (dynamic variant in productJSON.product.allVariants)
                {
                    if (variant.stock != 0 && variant.attributeName!="")
                    {
                        drSize = dtSize.NewRow();
                        drSize["option_name"] = attName;
                        drSize["required"] = 1;
                        drSize["option_value"] = variant.value;
                        drSize["price"] = 0;
                        drSize["option_image"] = "";
                        drSize["parent_option_value"] = "";
                        if (variant.isRunningOut == "True")
                            drSize["quantity"] = "99";
                        else
                            drSize["quantity"] = "0";
                        dtSize.Rows.Add(drSize);
                    }
                }
            }

            dtColor.Merge(dtSize);

            if (dtColor.Rows.Count == 0 && (optionsFound))
                productStatus = false;
            return dtColor;
        }


    }

}
