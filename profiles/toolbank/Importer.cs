using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using ParserFactory;
using HAP = HtmlAgilityPack;
using EntityLib;

namespace toolbank
{
    public class Importer : Parser
    {
        HAP.HtmlNodeCollection Nodes;
        HAP.HtmlNode aNode;
        HtmlAgilityPack.HtmlNode root;
        string itemURL;
        Dictionary<string,string> propertyCollection = new Dictionary<string,string>();
        string[] feedLine = new string[0];
        string MainImage;
        int currentPage;
        string SKU, Location, Size, Title, Price, CommonName, Origin, HRef, Category, Model, Manufacturer, weight,DateAdded;
        string description;
        string catA, catB, catC, catD;
        string length, width, height;
        
        Dictionary<string, string> catList = new Dictionary<string, string>();
        public Importer()
        {            

        }
        ~Importer()
        {
            

        }

        public string CategoryPath
        {
            set
            {
                
            }
        }

        public string URL 
        {            
            set
            {
                itemURL = value;
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
                propertyCollection  = value;
            }
        }

        public HtmlAgilityPack.HtmlNode Document
        {
            set
            {
                this.root = value;
            }
        }

        public List<Category> getCategoryURLs()
        {
            List<Category> urls = new List<Category>();
            return urls;
        }

        public void SetFeedLines(List<string[]> data, bool LastLine)
        {


        }

        public void SetFeedLine(string[] data, bool LastLine)
        {
            
        }

        public void SetFeedLine(Dictionary<string, string> data, bool LastLine)
        {
            Model = data["﻿StockCode"];
            Title = data["Product_Name"];
            Price = data["CurrentListPrice"];
            Manufacturer = data["Brand_Name"];
            weight = data["Weight"];
            length = data["Dimension1"];
            width = data["Dimension2"];
            height = data["Dimension3"];
            description = data["ProductDescription"];
            catA = data["ClassAName"];
            catB = data["ClassBName"];
            catC = data["ClassCName"];
            catD = data["ClassDName"];
            MainImage = "Toolbank/" + data["ImageRef"] + ".JPG";
        }
        
        public string buildCategoryURL(string catURL, int page)
        {
            return "";
        }

        public List<string> getItemURLs()
        {
            return null;
        }

        public string[] getTitles()
        {
            string[] titles = new string[1];
            titles[0] = Title;
            return titles;
        }

        public string getModel()
        {
            return Model;
        }

        public string getUPC()
        {
            return "";
        }

        public string getSKU()
        {
            return "";
        }
        public string getJAN()
        {
            return "";
        }
        public string getMPN()
        {
            return "";
        }

        public string getEAN()
        {
            return "";
        }

        public string getISBN()
        {
            return "";
        }

        public string getWeight()
        {
            return weight;
        }
        public string getLength()
        {
            return length;
        }
        public string getWidth()
        {
            return width;
        }
        public string getHeight()
        {
            return height;
        }

        public string getStatus()
        {
            return "1";
        }

        public string getLocation()
        {
            return "";
        }

        public string getPrice()
        {
            return Price;
        }

        public Dictionary<string, string> getSpecial()
        {            
            return null;
        }

        public Dictionary<string, string>[] getSpecialPrices(string[] customerGroups)
        {            
            return null;
        }

        public List<string> getAdditionalItems()
        {
            return null;
        }

        public string getManufacturer()
        {
            return Manufacturer;
        }

        public string[] getKeywords()
        {
            string[] keywords = new string[0];
            return keywords;
        }

        public string[] getTags()
        {
            string[] keywords = new string[0];
            return keywords;
        }
        
        public string[] getDescriptions()
        {
            
            
            string[] desc = new string[1];
            desc[0] = description;
            return desc;
        }

        public string getMainImage()
        {
            return MainImage;
        }

        public string[] getOtherImages()
        {
            string ImageURL; int i = 0, j = 0;
            string[] OtherImages = new string[0];
            return OtherImages;
        }

        public List<string[]> getCategoryPath()
        {
            List<string[]> categoryPaths = new List<string[]>();
            string[] catItem = new string[1];
            string cat="";
            if (catA != "")
                cat = cat + catA;
            if (catB != "")
                cat = cat + @"///" + catB;
            if (catC != "")
                cat = cat + @"///" + catC;
            if (catD != "")
                cat = cat + @"///" + catD;
            catItem[0] = cat;
            categoryPaths.Add(catItem);
            return categoryPaths;
        }

        public List<int> getCategoryIDs()
        {
            List<int> CatIDs = new List<int>();
            if (!catList.ContainsKey(Category)) return CatIDs;
            string catIDList = catList[Category];
            foreach (string cat in catIDList.Split(new string[] { "," }, StringSplitOptions.None))
            {
                if (cat != "")
                    CatIDs.Add(int.Parse(cat));
            }            
            return CatIDs;
        }

        public string getStock()
        {
            throw new NotImplementedException();
        }

        public string getStockStatus()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string>[] getAttributes()
        {
            Dictionary<string, string>[] attributes= new Dictionary<string, string>[0];
            return attributes;
        }

        public string getRefField()
        {
            return Model;
        }


        public Dictionary<string, Dictionary<string, float>> getOptions()
        {
            //string[][] option_list = new string[][] { };
            float priceOpt;
            Dictionary<string, Dictionary<string, float>> option_list = new Dictionary<string, Dictionary<string, float>>();
            //Tuple<string, string, float>[] option_list = new Tuple<string, string, float>[0];

            Dictionary<string, float> option_list_item = new Dictionary<string,float>();
           
            return option_list;
        }

        public Dictionary<string, string> getAdditionalFields()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            retVal.Add("supplier_id", "1");
            return retVal;
        }

        public List<string> getRelated()
        {
            List<string> retVal = new List<string>();
            return retVal;
        }
        public Dictionary<string, List<string>> getPostOptionSQLCommands()
        {
            return null;
        }
    }
}
