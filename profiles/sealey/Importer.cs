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
using System.Text.RegularExpressions;

namespace sealey
{
    public class Importer : Parser
    {
        HAP.HtmlNodeCollection Nodes;
        HAP.HtmlNode aNode;
        HtmlAgilityPack.HtmlNode root;
        string itemURL;
        Dictionary<string,string> propertyCollection = new Dictionary<string,string>();
        string[] feedLine = new string[0];
        string MainImage="";
        int currentPage;
        string SKU, Location, Size, Title, Price, CommonName, Origin, HRef, Category, Model, Manufacturer, weight,DateAdded, EAN, prodCategory, Stock;
        string description;
        string supp;
        string catA, catB, catC, catD;
        string length, width, height;
        List<string> moreImages;
        Dictionary<string, string> attributes;
        DataTable documents;
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

        public void SetFeedData(DataSet ds)
        {
            DataRow dr = ds.Tables["Export_models_select_files_xls$"].Rows[0];
            Model = dr["Model No"].ToString();
            Title = dr["PLDescription"].ToString();
            Price = "0";
            Manufacturer = "Sealey";
            EAN = dr["Barcode"].ToString();
            description = dr["Web Text"].ToString().Replace("\n","</br>") + "</br>Warranty Years : " + dr["Warranty years"].ToString();
            DateAdded = dr["Incept date"].ToString();
            prodCategory = @"Sealey_Import///" + dr["Category Name"].ToString() + @"///" + dr["Cat Group"].ToString() + @"///" + dr["Micro Group"].ToString();
            weight = dr["Prod Weight"].ToString();
            if (dr["Stock Status"].ToString() == "Zero" || dr["Stock Status"].ToString() == "")
                Stock = "0";
            else
            {
                string[] stockParts = dr["Stock Status"].ToString().Split(new string[] { " " }, StringSplitOptions.None);

                Stock = int.Parse(stockParts[0]).ToString();
            }
            supp = dr["Supplementary Items"].ToString();
            int attSize = ds.Tables["Export_models_Tspecs$"].Rows.Count;
            Boolean insideNormalTable = false;
            Boolean insideSpecTable = false;

            StringBuilder subDesc = new StringBuilder("<table class=\"contentsTable\">\n");
            foreach (DataRow attRow in ds.Tables["Export_models_Tspecs$"].Rows)
            {
                if (attRow["Table Name"].ToString() != "")
                {
                    if (insideNormalTable)
                    {
                        subDesc.Append("</table></br>\n");
                        insideNormalTable = false;
                    }
                    if (!insideSpecTable)
                    {
                        subDesc.Append("<h3>" + attRow["Table Name"].ToString().Replace(" Table:", "") + "</h3></br>\n");
                        subDesc.Append("<table class=\"contentsTable\">\n");
                        subDesc.Append("<tr><td width=\"30%\">" + attRow["Attrib Name"].ToString() + "</td><td>" + attRow["Attrib Value"].ToString() + "</td></tr>\n");
                        insideSpecTable = true;
                    }
                    else
                    {
                        subDesc.Append("<tr><td width=\"30%\">" + attRow["Attrib Name"].ToString() + "</td><td>" + attRow["Attrib Value"].ToString() + "</td></tr>\n");
                    }
                }
                else
                {
                    if (insideSpecTable){
                        subDesc.Append("</table></br>\n");
                        subDesc.Append("<table class=\"contentsTable\"></br>\n");
                        insideSpecTable = false;
                    }
                    subDesc.Append("<tr><td width=\"30%\">" + attRow["Attrib Name"].ToString() + "</td><td>" + attRow["Attrib Value"].ToString() + "</td></tr>\n");
                    if (!insideSpecTable)
                        insideNormalTable = true;
                }
            }
            if (insideNormalTable || insideSpecTable)
                subDesc.Append("</table></br>\n");

            description = description + @"</br></br>\n" + subDesc;

            dr = ds.Tables["Export_models_attributes$"].Rows[0];
            height = dr["Height"].ToString();
            width = dr["Width"].ToString();
            length = dr["Depth"].ToString();
            moreImages = new List<string>();
            documents = new DataTable("product_documents");
            documents.Columns.Add("file",typeof(string));
            documents.Columns.Add("type",typeof(string));
            DataRow docRow;

            foreach (DataRow fileRow in ds.Tables["Export_Models_Images_videos$"].Rows)
            {
                if (fileRow["File Type"].ToString()=="Web Main Picture")
                {
                    MainImage = "Sealey/WEBMain/" + Regex.Replace(fileRow["File Name"].ToString(), ".PNG", ".jpg", RegexOptions.IgnoreCase);
                }
                
                if (fileRow["File Type"].ToString() == "Web Image")
                {
                    moreImages.Add("Sealey/WEBImage/" + Regex.Replace(fileRow["File Name"].ToString(), ".PNG", ".jpg", RegexOptions.IgnoreCase));
                }

                if (fileRow["File Type"].ToString() == "Instructions")
                {
                    docRow = documents.NewRow();
                    docRow["file"] = "Sealey/PDFs/Instructions/" + fileRow["File Name"].ToString().Replace(".PDF",".pdf");
                    docRow["type"] = "Instructions";
                    documents.Rows.Add(docRow);
                }
                if (fileRow["File Type"].ToString() == "Material Safety Data Sheet")
                {
                    docRow = documents.NewRow();
                    docRow["file"] = "Sealey/PDFs/MSDS/" + fileRow["File Name"].ToString().Replace(".PDF", ".pdf");
                    docRow["type"] = "MSDS";
                    documents.Rows.Add(docRow);
                }
                if (fileRow["File Type"].ToString() == "Parts Diagrams")
                {
                    docRow = documents.NewRow();
                    docRow["file"] = "Sealey/PDFs/Parts/" + fileRow["File Name"].ToString().Replace(".PDF", ".pdf");
                    docRow["type"] = "Parts";
                    documents.Rows.Add(docRow);
                }
            }
        }

        public void SetFeedData(Dictionary<string, string> data, bool LastLine)
        {
           

        }

        public void SetFeedData(string[] data, bool LastLine)
        {
           
        }

        public void SetFeedLine(Dictionary<string, string> data, bool LastLine)
        {
            

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
            return EAN;
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
            string[] OtherImages = new string[moreImages.Count];
            foreach (string img in moreImages)
            {
                OtherImages[i] = img;
                i++;
            }
            return OtherImages;
        }

        public List<string[]> getCategoryPath()
        {
            List<string[]> categoryPaths = new List<string[]>();
            string[] catItem = new string[1];
            catItem[0] = prodCategory;
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
            return Stock;
        }

        public string getStockStatus()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string>[] getAttributes()
        {
            Dictionary<string, string>[] att= new Dictionary<string, string>[1];
            att[0] = attributes;
            return att;
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
            retVal.Add("supplementary", supp);
            retVal.Add("supplier_id", "2");
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

        public DataSet getAdditionalTableData()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(documents);
            return ds;
        }
    }
}
