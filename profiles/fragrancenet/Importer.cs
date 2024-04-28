using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Data;
using ParserFactory;
using HAP = HtmlAgilityPack;
using EntityLib;

namespace fragrancenet
{
    public class Importer : Parser
    {

        Dictionary<string,string> propertyCollection = new Dictionary<string,string>();
        string[] feedLine = new string[0];
        string MainImage;
        string Title, Price,  Category, Manufacturer;
        string description;
        string Stock;
        string sku;
        string weight,length,width,height;

        Dictionary<string, string> catList = new Dictionary<string, string>();
        public Importer()
        {            

        }


        public override void SetFeedData(Dictionary<string, string> data)
        {
            // string description1 = "", description2 = "", description3="";
            string itemType, gender;
            data.TryGetValue("UPC", out sku);
            data.TryGetValue("UPC", out Model);
            data.TryGetValue("Designer", out Manufacturer);
            data.TryGetValue("Product Description", out description);
            // data.TryGetValue("Year Introduced", out description2);
            // data.TryGetValue("Recommended Use", out description3);
            //description = "";
            //description += "<strong>Fragrance Notes</strong> : " + description1  + "</br>";
            //description += "<strong>Year Introduced</strong> : " + description2 + " </br>";
            //description += "<strong>Recommended Use</strong> : " + description3 + " </br>";
            data.TryGetValue("Name", out Title);
            data.TryGetValue("Quantity", out Stock);
            data.TryGetValue("FNET Wholesale Price", out Price);
            data.TryGetValue("Item Type", out itemType);
            data.TryGetValue("Gender", out gender);
            Category = itemType + "///" + gender;
            data.TryGetValue("Image Large", out MainImage);
            data.TryGetValue("Weight", out weight);
            data.TryGetValue("L", out length);
            data.TryGetValue("W", out width);
            data.TryGetValue("H", out height);
        }


        public override Dictionary<int, string> getTitles()
        {

            Titles = new Dictionary<int, string>();
            foreach (string language in Languages)
            {
                Titles.Add(int.Parse(language), Title);
            }

            return Titles;
        }

        public override string getModel()
        {
            return Model;
        }

        public override string getSKU()
        {
            return sku;
        }

        public override string getStatus()
        {
            return "1";
        }

        public override string getWeight()
        {
            return weight;
        }

        public override string getLength()
        {
            return length;
        }

        public override string getWidth()
        {
            return width;
        }

        public override string getHeight()
        {
            return height;
        }


        public override string getPrice()
        {
            return Price;
        }


        public override string getManufacturer()
        {
            return Manufacturer;
        }


        public override Dictionary<int, string> getDescriptions()
        {
            Descriptions = new Dictionary<int, string>();
            foreach (string language in Languages)
            {
                Descriptions.Add(int.Parse(language), description);
            }
            return Descriptions;
        }

        public override ImageTable getImages()
        {
            Uri uri;
            DataRow dr;
            ImageTable prodImages = new ImageTable();

            uri = new Uri(MainImage);
            dr = prodImages.NewRow();
            dr["url"] = MainImage;
            dr["image_name"] =   System.IO.Path.GetFileName(uri.LocalPath); 
            prodImages.Rows.Add(dr);
            
            return prodImages;
        }


        public override CategoryTable getCategoryPath()
        {
            CategoryTable categoryPathTable = new CategoryTable();


            DataRow categoryPath = categoryPathTable.NewRow();
            categoryPath["language_id"] = "1";
            categoryPath["category_path"] =Category;
            categoryPathTable.Rows.Add(categoryPath);

            return categoryPathTable;
        }


        public override string getStock()
        {
            return Stock;
        }



        public override string getRefField()
        {
            return Model;
        }

    }
}
