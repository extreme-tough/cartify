using System.Security.Cryptography;
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

namespace meesho.com
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
        double priceNumeric;
        public Importer()
        {
        }

        
        public override List<Category> getCategoryURLs()
        {
            List<Category> urls = new List<Category>();
            string TopCat="",subCat="";
            foreach (HAP.HtmlNode topMenuNode in Document.SelectSingleNode("//div[@class='S_NavBarDesktop-sc-1cq484b-0 eaZXuq nav-head-container']").ChildNodes)
            {
                string className = topMenuNode.GetAttributeValue("class", "");
                if (className == "NavBarDesktop__Subtitle-sc-1049n7y-0 cXxWzL")
                {
                    //Top level menu item
                    TopCat = topMenuNode.InnerText.Trim();
                    if (TopCat.StartsWith("All ")) continue;
                }
                if (className== "NavBarDesktop__StyledLevel3-sc-1049n7y-1 jBpa-Dp")
                {
                    //Panel of top menu drop down
                    HAP.HtmlNodeCollection topmenuSections = topMenuNode.ChildNodes[0].ChildNodes;
                    foreach (HAP.HtmlNode topmenuSection in topmenuSections) { 
                        foreach(HAP.HtmlNode menuItem in topmenuSection.ChildNodes)
                        {
                            if (menuItem.GetAttributeValue("class","")== "sub-list-title")
                            {
                                subCat = menuItem.InnerText.Trim();
                            }
                            if (menuItem.GetAttributeValue("class","")== "sub-list-item pointer")
                            {
                                string childCat = menuItem.InnerText.Trim();
                                if (!childCat.StartsWith("All ") && (childCat!="View All"))
                                {
                                    Category objCategory = new Category();
                                    objCategory = new Category(); objCategory.Add(TopCat + "///" + subCat + "///" + childCat, "https://www.meesho.com" + menuItem.GetAttributeValue("href", "")); urls.Add(objCategory);
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
            currentCat = catURL;
            catURL= catURL + "?page=" + page.ToString();
            return catURL;
        }

        public override List<string> getItemURLs()
        {
            List<string> urls = new List<string>();

            HAP.HtmlNodeCollection itemNodes = Document.SelectNodes("//div[@class='sc-dkzDqf ProductList__GridCol-sc-8lnc8o-0 kmfTGq dXXltq']");
            foreach(HAP.HtmlNode node in itemNodes)
            {
                if (node.ChildNodes[0].GetAttributeValue("href", "")!="")
                {
                    string[] urlParts = node.ChildNodes[0].GetAttributeValue("href", "").Split(new string[] { @"/" }, StringSplitOptions.None);
                    string productID = urlParts[urlParts.Length - 1];
                    urls.Add("https://www.meesho.com/api/v1/product/" + productID);
                }
            }

            return urls;
        }


        public override Dictionary<int,string> getTitles()
        {

            string productData = Document.InnerText;
            productJSON = JsonConvert.DeserializeObject(productData);

            parseImages();            
            
            string TitleText="";
            TitleText = productJSON.payload.name;

            /* HAP.HtmlNode aNode;
            try
            {
                aNode = Document.SelectNodes("//span[@class='Text__StyledText-sc-oo0kvp-0 feWUdl']")[0];                
                TitleText = aNode.InnerText;
            }
            catch (Exception ex)
            {
                return Titles;
            }
            if (aNode == null)
                return Titles; */

            Titles.Clear();
            foreach (string language in Languages)
            {
                Titles.Add(int.Parse(language), TitleText);
            }

            return Titles;
        }



        public override string getModel()
        {
            Model = productJSON.payload.original_product_id;
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getStatus()
        {
            if (productJSON.payload.in_stock == "true")
                return "1";
            else
                return "0";
        }

        public override string getPrice()
        {
            string price = productJSON.payload.original_price;
            return price;

        }

        public override SpecialTable getSpecial()
        {
            SpecialTable special = new SpecialTable();
            DataRow dr = special.NewRow();
            dr["customer_group_id"] = "1";
            dr["price"] = productJSON.payload.original_price;
            special.Rows.Add(dr);
            return special;
        }

        public override string getManufacturer()
        {
            //string brand = Document.SelectSingleNode("//span[@class='Text__StyledText-sc-oo0kvp-0 dkoszY ShopCardstyled__ShopName-sc-du9pku-6 gDqEux ShopCardstyled__ShopName-sc-du9pku-6 gDqEux']").InnerText.Trim();
            string brand = productJSON.payload.supplier_name;
            return brand;
        }


        public override Dictionary<int,string> getDescriptions()
        {
            string desc;
            try {
                // desc = Document.SelectNodes("//div[@class='pr-in-dt-cn']")[0].InnerHtml;
                desc = productJSON.payload.description.ToString().Replace("\n", "<br>");
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

            
            
            Model = productJSON.payload.product_id;
            string src; Uri uri;
            DataRow dr;
            int i = 0;
            prodImages = new ImageTable();

            foreach (dynamic image in productJSON.payload.images)
            {
                src =  image;
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
            ScrapOptions();
            return options;
        }

        private OptionTable ScrapOptions()
        {
            // string url= "https://api.trendyol.com/webbrowsinggw/api/productGroup/" + productJSON.product.productGroupId.Value + "?storefrontId=1&culture=tr-TR";
            OptionTable dtSize = new OptionTable();

            foreach (string option in productJSON.payload.variations)
            {
                if (option != "Free Size")
                {
                    DataRow drSize = dtSize.NewRow();
                    drSize["option_name"] = "Size";
                    drSize["required"] = 1;
                    drSize["option_value"] = option.Trim();
                    drSize["price"] = 0;
                    drSize["option_image"] = "";
                    drSize["parent_option_value"] = "";
                    drSize["quantity"] = "99";
                    
                    dtSize.Rows.Add(drSize);
                }
            }
            return dtSize;
        }

        public override List<string> getRelated()
        {
            List<string> related = new List<string>();
            foreach (dynamic duplicate_product in productJSON.payload.duplicate_products)
            {
                related.Add(duplicate_product.id.ToString());
            }
            return related;
        }

    }

}
