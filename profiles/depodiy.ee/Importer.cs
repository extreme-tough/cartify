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
using RestSharp;

namespace depodiy.ee
{
    public class Importer : Parser
    {
        dynamic productJSON;
        string productData;
        string width, length, height, weight;
        List<string> optionAttruibuteNames, optionAttruibuteLabels;
        Dictionary<string, string> propertyCollection = new Dictionary<string, string>();
        WebBrowser thisBrowser;
        int currentPage;
        string currentCat,currentCatURL;
        OptionTable[] options;
        Dictionary<string, string> abbrevs = new Dictionary<string, string>();
        ImageTable prodImages;
        dynamic productObj;
        bool productStatus = true;
        AttributeTable attTable = new AttributeTable();

        HashSet<string> categoryList = new HashSet<string>();
        public Importer()
        {
        }


        public void WalkNode(dynamic node)
        {
            if (node.subCategories == null) return;
            foreach(dynamic child in node.subCategories)
            {
                if (child.subCategories==null || child.subCategories.Count == 0)
                    categoryList.Add(child.id.Value);
                WalkNode(child);
            }
        }

        public override List<Category> getCategoryURLs()
        {

            var client = new RestClient("https://online.depo-diy.ee/graphql");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            // request.AddHeader("Cookie", "Depo.Customer=947c857e-887d-485d-a3e5-7949e3766cb7");
            // request.AddParameter("application/json", "{\"operationName\":\"categories\",\"variables\":{\"categoryId\":0},\"query\":\"query categories($cursorAfter: String, $cursorBefore: String, $categoryId: Int) {\\n  categories(\\n    after: $cursorAfter\\n    before: $cursorBefore\\n    where: {parentCategoryId: $categoryId}\\n  ) {\\n    edges {\\n      node {\\n        id\\n        name\\n        pictureUrl\\n        includeInTopMenu\\n        showOnHomepage\\n        parentCategory {\\n          id\\n          name\\n          __typename\\n        }\\n        subCategories {\\n          id\\n          name\\n          pictureUrl\\n          includeInTopMenu\\n          showOnHomepage\\n          subCategories {\\n            id\\n            name\\n            includeInTopMenu\\n            showOnHomepage\\n            subCategories {\\n              id\\n              name\\n              includeInTopMenu\\n              showOnHomepage\\n              subCategories {\\n                id\\n                name\\n                includeInTopMenu\\n                showOnHomepage\\n                subCategories {\\n                  id\\n                  name\\n                  includeInTopMenu\\n                  showOnHomepage\\n                  subCategories {\\n                    id\\n                    name\\n                    includeInTopMenu\\n                    showOnHomepage\\n                    __typename\\n                  }\\n                  __typename\\n                }\\n                __typename\\n              }\\n              __typename\\n            }\\n            __typename\\n          }\\n          __typename\\n        }\\n        __typename\\n      }\\n      __typename\\n    }\\n    __typename\\n  }\\n}\\n\"}", ParameterType.RequestBody);
            var body = @"{""operationName"":""categories"",""variables"":{""categoryId"":0},""query"":""query categories($cursorAfter: String, $cursorBefore: String, $categoryId: Int) {\n  categories(\n    after: $cursorAfter\n    before: $cursorBefore\n    where: {parentCategoryId: $categoryId}\n  ) {\n    edges {\n      node {\n        id\n        name\n        pictureUrl\n        includeInTopMenu\n        showOnHomepage\n        parentCategory {\n          id\n          name\n          __typename\n        }\n        subCategories {\n          id\n          name\n          pictureUrl\n          includeInTopMenu\n          showOnHomepage\n          subCategories {\n            id\n            name\n            includeInTopMenu\n            showOnHomepage\n            subCategories {\n              id\n              name\n              includeInTopMenu\n              showOnHomepage\n              subCategories {\n                id\n                name\n                includeInTopMenu\n                showOnHomepage\n                subCategories {\n                  id\n                  name\n                  includeInTopMenu\n                  showOnHomepage\n                  subCategories {\n                    id\n                    name\n                    includeInTopMenu\n                    showOnHomepage\n                    __typename\n                  }\n                  __typename\n                }\n                __typename\n              }\n              __typename\n            }\n            __typename\n          }\n          __typename\n        }\n        __typename\n      }\n      __typename\n    }\n    __typename\n  }\n}\n""}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            
            dynamic itemsObj = JsonConvert.DeserializeObject(response.Content);
            foreach (dynamic subCat in itemsObj.data.categories.edges[0].node.subCategories)
            {
                if (subCat.includeInTopMenu.Value == false)
                    continue;
                if (subCat.subCategories.Count == 0)
                    categoryList.Add(subCat.id.Value);
                WalkNode(subCat);
            }
                
            Category objCategory = new Category();
            List<Category> urls = new List<Category>();   

            foreach(string cat in categoryList)
            {
                objCategory = new Category(); objCategory.Add("", cat); urls.Add(objCategory);
            }
            
            return urls;
        }


        public override string buildCategoryURL(string catURL, int page)
        {
            currentPage = page;
            currentCatURL = catURL;
            return "https://online.depo-diy.ee/graphql";
        }

        public override List<string> getItemURLs()
        {
            int startJsonPage = 0;
            List<string> urls = new List<string>();
            if (currentPage == 0)
            {
                while (true)
                {
                    var client = new RestClient("https://online.depo-diy.ee/graphql");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("content-type", "application/json");
                    request.AddHeader("sec-fetch-dest", "empty");
                    // request.AddHeader("Cookie", "Depo.Customer=947c857e-887d-485d-a3e5-7949e3766cb7");
                    //request.AddParameter("application/json", "{\"operationName\":\"productsFromCategory\",\"variables\":{\"searchString\":\"\",\"start\":"+
                    //    startJsonPage + ",\"categoryId\":"
                    //    + currentCatURL + ",\"order\":[{\"attributeName\":\"YellowPrice\",\"direction\":\"ASC\"}],\"rows\":20},\"query\":\"query productsFromCategory($searchString: String, $order: [ProductSortModelInput], $facets: [FacetFilterInput], $categoryId: Int!, $rows: Int, $start: Int) {\\n  categories(where: {id: $categoryId}) {\\n    edges {\\n      node {\\n        categoryBreadcrumb {\\n          name\\n          id\\n          __typename\\n        }\\n        name\\n        attributes {\\n          displayInProductList\\n          name\\n          displayName\\n          useInFacets\\n          __typename\\n        }\\n        products(\\n          where: {searchString: $searchString}\\n          order_by: $order\\n          facets: $facets\\n          rows: $rows\\n          start: $start\\n        ) {\\n          facetsInfo {\\n            facets {\\n              name\\n              typeName\\n              values {\\n                count\\n                selected\\n                typeName\\n                stringValue\\n                decimalValue\\n                __typename\\n              }\\n              __typename\\n            }\\n            __typename\\n          }\\n          pageInfo {\\n            endCursor\\n            startCursor\\n            hasPreviousPage\\n            hasNextPage\\n            totalCount\\n            __typename\\n          }\\n          edges {\\n            node {\\n              id\\n              name\\n              yellowPrice\\n              yellowNormPrice\\n              yellowPriceWithVat\\n              orangePrice\\n              orangeNormPrice\\n              orangePriceWithVat\\n              normUnit\\n              yellowNormPriceWithVat\\n              orangeNormPriceWithVat\\n              sku\\n              specificationBarcodes\\n              specificationBrand\\n              thumbnailPictureUrl\\n              cardThumbnailPictureUrl\\n              orangePriceQuantity\\n              attributes\\n              unit\\n              unitConversion {\\n                factor\\n                fromUnit\\n                toUnit\\n                __typename\\n              }\\n              stockItems {\\n                locationId\\n                productId\\n                quantity\\n                locationName\\n                __typename\\n              }\\n              __typename\\n            }\\n            __typename\\n          }\\n          __typename\\n        }\\n        __typename\\n      }\\n      __typename\\n    }\\n    __typename\\n  }\\n}\\n\"}", ParameterType.RequestBody);
                    var body = @"{
                                " + "\n" +
                                                    @"  ""operationName"": ""productsFromCategory"",
                                " + "\n" +
                                                    @"  ""variables"": {
                                " + "\n" +
                                                    @"    ""searchString"": """",
                                " + "\n" +
                                                    @"    ""categoryId"": "+ currentCatURL + @",
                                " + "\n" +
                                                    @"    ""order"": null,
                                " + "\n" +
                                                    @"    ""rows"": 20,
                                " + "\n" +
                                                    @"    ""start"": " + startJsonPage + @"
                                " + "\n" +
                                                    @"  },
                                " + "\n" +
                                                    @"  ""query"": ""query productsFromCategory($searchString: String, $order: [ProductSortModelInput], $facets: [FacetFilterInput], $categoryId: Int!, $rows: Int, $start: Int) {\n  categories(where: {id: $categoryId}) {\n    edges {\n      node {\n        categoryBreadcrumb {\n          name\n          id\n          __typename\n        }\n        name\n        attributes {\n          displayInProductList\n          name\n          displayName\n          useInFacets\n          __typename\n        }\n        products(\n          where: {searchString: $searchString}\n          order_by: $order\n          facets: $facets\n          rows: $rows\n          start: $start\n        ) {\n          facetsInfo {\n            facets {\n              name\n              values {\n                count\n                selected\n                stringValue\n                decimalValue\n                __typename\n              }\n              __typename\n            }\n            __typename\n          }\n          pageInfo {\n            endCursor\n            startCursor\n            hasPreviousPage\n            hasNextPage\n            totalCount\n            __typename\n          }\n          edges {\n            node {\n              id\n              name\n              yellowPrice\n              yellowNormPrice\n              yellowPriceWithVat\n              orangePrice\n              orangeNormPrice\n              orangePriceWithVat\n              normUnit\n              yellowNormPriceWithVat\n              orangeNormPriceWithVat\n              specificationBarcodes\n              specificationBrand\n              thumbnailPictureUrl\n              cardThumbnailPictureUrl\n              orangePriceQuantity\n              attributes\n              unit\n              unitConversion {\n                factor\n                fromUnit\n                toUnit\n                __typename\n              }\n              stockItems {\n                locationId\n                productId\n                quantity\n                locationName\n                __typename\n              }\n              energyEfficiency\n              energyEfficiencyDocumentUrl\n              energyEfficiencyImageUrl\n              __typename\n            }\n            __typename\n          }\n          __typename\n        }\n        __typename\n      }\n      __typename\n    }\n    __typename\n  }\n}\n""
                                " + "\n" +
                    @"}";
                    body = "{\"operationName\":\"productsFromCategory\",\"variables\":{\"searchString\":\"\",\"categoryId\":" + currentCatURL +",\"order\":null,\"facets\":[],\"rows\":20,\"start\":" + startJsonPage + "},\"query\":\"query productsFromCategory($searchString: String, $order: [ProductSortModelInput], $facets: [FacetFilterInput], $categoryId: Int!, $rows: Int, $start: Int) {\n  categories(where: {id: $categoryId}) {\n    edges {\n      node {\n        categoryBreadcrumb {\n          name\n          id\n          __typename\n        }\n        name\n        minYellowPrice\n        minYellowPriceWithVat\n        maxYellowPrice\n        maxYellowPriceWithVat\n        showProducts\n        showFilters\n        attributes {\n          displayInProductList\n          name\n          isRange\n          displayName\n          useInFacets\n          __typename\n        }\n        products(\n          where: {searchString: $searchString}\n          order_by: $order\n          facets: $facets\n          rows: $rows\n          start: $start\n        ) {\n          facetsInfo {\n            facets {\n              name\n              values {\n                count\n                selected\n                value\n                __typename\n              }\n              __typename\n            }\n            __typename\n          }\n          pageInfo {\n            endCursor\n            startCursor\n            hasPreviousPage\n            hasNextPage\n            totalCount\n            __typename\n          }\n          edges {\n            node {\n              id\n              name\n              yellowPrice\n              yellowNormPrice\n              yellowPriceWithVat\n              orangePrice\n              orangeNormPrice\n              orangePriceWithVat\n              normUnit\n              yellowNormPriceWithVat\n              orangeNormPriceWithVat\n              specificationBarcodes\n              specificationBrand\n              thumbnailPictureUrl\n              cardThumbnailPictureUrl\n              orangePriceQuantity\n              attributes\n              unit\n              unitConversion {\n                factor\n                fromUnit\n                toUnit\n                __typename\n              }\n              stockItems {\n                locationId\n                productId\n                quantity\n                locationName\n                __typename\n              }\n              energyEfficiency\n              energyEfficiencyDocumentUrl\n              energyEfficiencyImageUrl\n              __typename\n            }\n            __typename\n          }\n          __typename\n        }\n        __typename\n      }\n      __typename\n    }\n    __typename\n  }\n}\n\"}";
                    request.AddParameter("application/json", body, ParameterType.RequestBody);


                    IRestResponse response = client.Execute(request);

                    dynamic itemsObj = JsonConvert.DeserializeObject(response.Content);
                    dynamic productData = itemsObj.data.categories.edges[0].node.products.edges;
                    if (productData.Count > 0)
                    {
                        foreach(dynamic product in productData)
                        {
                            urls.Add("https://online.depo-diy.ee/products/"+ currentCatURL + "#" + product.node.id.Value);
                        }
                    } else
                    {
                        break;
                    }

                    startJsonPage += 20;
                }
            }

            return urls;
        }


        public override Dictionary<int,string> getTitles()
        {



            string productId = URL.Split(new string[] { "#" }, StringSplitOptions.None).Last();

            var client = new RestClient("https://online.depo-diy.ee/graphql");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json");
            // request.AddHeader("Cookie", "Depo.Customer=947c857e-887d-485d-a3e5-7949e3766cb7");
            var body = @"{""operationName"":""product"",""variables"":{""productId"":"+ productId + @"},""query"":""query product($productId: Int!) {\n  product(productId: $productId) {\n    breadcrumb {\n      categoryBreadcrumb {\n        name\n        id\n        __typename\n      }\n      __typename\n    }\n    id\n    name\n    yellowPrice\n    yellowNormPrice\n    yellowPriceWithVat\n    orangePrice\n    orangeNormPrice\n    orangePriceWithVat\n    normUnit\n    yellowNormPriceWithVat\n    orangeNormPriceWithVat\n    specificationBarcodes\n    specificationBrand\n    largePictureUrls\n    bigPictureUrls\n    smallPictureUrls\n    orangePriceQuantity\n    cardThumbnailPictureUrl\n    unit\n    fullDescription\n    metaTitle\n    metaKeywords\n    metaDescription\n    metaSchema\n    unitConversion {\n      factor\n      fromUnit\n      toUnit\n      __typename\n    }\n    attributes\n    attributeDisplayNames\n    stockItems {\n      locationId\n      productId\n      quantity\n      locationName\n      __typename\n    }\n    energyEfficiency\n    energyEfficiencyDocumentUrl\n    energyEfficiencyImageUrl\n    __typename\n  }\n}\n""}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            productObj = JsonConvert.DeserializeObject(response.Content);


            string TitleText="";
            TitleText = productObj.data.product.name.Value;
            Titles.Clear();
            foreach (string language in Languages)
            {
                Titles.Add(int.Parse(language), TitleText);
            }

            attTable = parseAttributes();


            return Titles;
        }

        public override string getModel()
        {
            if (Model=="")   
                Model = productObj.data.product.id.Value;
            return Model;
        }

        public override string getRefField()
        {
            return Model;
        }

        public override string getStatus()
        {
            if (productObj.data.product.stockItems.Count > 0 && productObj.data.product.stockItems[0].quantity.Value > 0)
                return "1";
            else
                return "0";
        }

        public override string getPrice()
        {
            double price = productObj.data.product.yellowPriceWithVat.Value;
            string pricestr = price.ToString();
            return pricestr;

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
            string desc;
            try {
                desc = productObj.data.product.fullDescription;
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

            foreach (dynamic image in productObj.data.product.bigPictureUrls)
            {
                prodImages = new ImageTable();
                uri = new Uri(image.Value);
                dr = prodImages.NewRow();
                dr["url"] = image.Value;
                dr["image_name"] = System.IO.Path.GetFileName(uri.LocalPath);
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
            foreach (dynamic categoryItem in productObj.data.product.breadcrumb.categoryBreadcrumb)
            {
                catPath = catPath + categoryItem.name + "///";               
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
            if (productObj.data.product.stockItems.Count > 0 )
            {
                double stock;
                stock = productObj.data.product.stockItems[0].quantity.Value;

                int newstock;
                newstock = (int)stock;
                return newstock.ToString();
            } else
            {
                return "0";
            }
        }


        private  AttributeTable parseAttributes()
        {
            AttributeTable retVal = new AttributeTable();
            HashSet<string> uniqueKeys = new HashSet<string>();

            List<string> props = new List<string>();
            Model = "";
            foreach (dynamic attribute in productObj.data.product.attributeDisplayNames)
            {
                props.Add(attribute.Value.Value);
            }
            int i = 0;
            width = length = height = width = "0";

            foreach (dynamic attribute in productObj.data.product.attributes)
            {

                foreach (string language in Languages)
                {                    
                    switch (props[i].ToString())
                    {
                        case "Vöötkood":
                            string barcodes = attribute.Value.Value;
                            Model = barcodes.Split(new string[] { "," }, StringSplitOptions.None).First();
                            break;
                        case "Laius (mm)":
                            width = attribute.Value.Value;
                            break;
                        case "Pikkus (mm)":
                            length = attribute.Value.Value;
                            break;
                        case "Paksus (mm)":
                            height = attribute.Value.Value;
                            break;
                        case "Kaal":
                            weight = attribute.Value.Value;
                            break;
                        default:
                            DataRow dr = retVal.NewRow();
                            dr["language_id"] = language;
                            dr["name"] = props[i];
                            dr["value"] = attribute.Value.Value;
                            if (!uniqueKeys.Contains(props[i]))
                            {
                                retVal.Rows.Add(dr);
                                uniqueKeys.Add(props[i]);
                            }
                            break;
                    }                    
                }
                i++;
            }
            return retVal;
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

        public override AttributeTable getAttributes()
        {
            return attTable;
        }




        public override OptionTable[] getOptions()
        {
            
            return null;
        }

        private OptionTable ScrapOptions()
        {
            return null;
        }


    }

}
