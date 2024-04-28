using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Cartify
{
    public class WooCommerce : DbWrapper
    {
        /// <summary>
        /// Creates a new database wrapper object that wraps around
        /// the users table.
        /// </summary>
        /// <param name="svr">The name of the server</param>
        /// <param name="db">The database catalog to use</param>
        /// <param name="user">The user name</param>
        /// <param name="pass">The user password</param>
        public WooCommerce()
        {

        }


        public override void DBInit()
        {
            DataRow row = GetRecord("SHOW COLUMNS FROM `" + DB_PREFIX + "posts` LIKE 'reffield'");
            if (row == null)
            {
                Execute("ALTER TABLE `" + DB_PREFIX + "posts` ADD reffield VARCHAR(64)");                
            }
            row = GetRecord("SHOW COLUMNS FROM `" + DB_PREFIX + "posts` LIKE 'source_url'");
            if (row == null)
            {
                Execute("ALTER TABLE `" + DB_PREFIX + "posts` ADD source_url TEXT");
            }
            row = GetRecord("SHOW COLUMNS FROM `" + DB_PREFIX + "posts` LIKE 'imported'");
            if (row == null)
            {
                Execute("ALTER TABLE `" + DB_PREFIX + "posts` ADD imported BOOL DEFAULT 0");
            }
        }


        public override DataTable getImportedProducts(string name, string model, string price, string quantity, string status, string category_id, string brand_id, string lastID)
        {            
            return null;
        }


        public override void DeleteProduct(int product_id)
        {
            string idToDelete = product_id.ToString();
            Execute("DELETE FROM " + DB_PREFIX + "posts WHERE ID = '" + idToDelete + "'");
            Execute("DELETE FROM " + DB_PREFIX + "postmeta WHERE post_id = '" + idToDelete + "'");
        }

 
        public override DataRow getAdminUser()
        {
            return GetRecord("SELECT user_login as username,user_pass as password,'' as salt FROM " + DB_PREFIX + "users WHERE user_login='admin'");
        }

        public override int getManufacturer(string manufacturer)
        {
            return 0;
        }

        public override DataTable getManufacturers()
        {
            return null;
        }

        public override int addManufacturer(Dictionary<string, dynamic> data)
        {            
            return 0;
        }

        public override int getProductByModel(string model)
        {
            return GetValue("SELECT ID FROM " + DB_PREFIX + "posts WHERE reffield='" + model.Replace("'", "''") + "'");
        }

        public override int getTotalImportedProducts()
        {
            return GetValue("SELECT count(ID) FROM " + DB_PREFIX + "posts WHERE imported='1' or source_url!=''");
        }


        public override int addProduct(Dictionary<string, dynamic> data)
        {
            string name = "", desc="", title="";
            float price;

            Dictionary<int, dynamic> product_description = data["product_description"];
            foreach (int key in product_description.Keys)
            {
                name = product_description[key]["name"].Replace("\\'", "'").Replace("'", "''");
                title = name.GenerateSlug();
                desc = product_description[key]["description"].Replace("\\'", "'").Replace("'", "''");
            }

            int product_id = InsertRecord("INSERT INTO " + DB_PREFIX
                + "posts SET reffield = '" + data["reffield"]
                + "', post_author = '1', post_content = '" + desc
                + "', post_title = '" + name
                + "', post_status = 'publish', comment_status = 'closed',ping_status='closed'"
                + "', post_name = '" + title
                + "', post_parent = '0', menu_order  = '0', post_type= 'product', comment_count = '0', source_url='" + data["source_url"] + "',imported='1"
                + "', post_date = NOW(), post_date_gmt = UTC_TIMESTAMP()", true);

            string sale_price="";
            price = float.Parse(data["price"]);

            if (data["special_prices"] != null)
            {
                foreach (DataRow specialPrice in ((DataTable)(data["special_prices"])).Rows)
                {
                    if (float.Parse(specialPrice["price"].ToString()) != price)
                    {
                        sale_price = specialPrice["price"].ToString();
                    }
                }
            }

            name = Path.GetFileNameWithoutExtension(data["image"].ToString());
            title = name.GenerateSlug();

            int mainImageID = InsertRecord("INSERT INTO " + DB_PREFIX
               + "posts SET post_author = '1', post_content = '', post_title = '" + name
               + "', post_status = 'inherit', comment_status = 'open',ping_status='closed'"
               + "', post_name = '" + title
               + "', post_parent = '" + product_id.ToString()
               + "', menu_order  = '0', post_type= 'attachment', post_mime_type='image/png',comment_count = '0', post_date = NOW(), post_date_gmt = UTC_TIMESTAMP()", true);


            DataTable xtraImages = (DataTable)data["product_image"];
            string sql = "";
            foreach (DataRow xtraImage in xtraImages.Rows)
            if (xtraImage != null && Convert.ToBoolean(xtraImage["active"]))
            {
                name = Path.GetFileNameWithoutExtension(xtraImage["image"].ToString());
                title = name.GenerateSlug();
                Execute("INSERT INTO " + DB_PREFIX
                   + "posts SET post_author = '1', post_content = '', post_title = '" + name
                   + "', post_status = 'inherit', comment_status = 'open',ping_status='closed'"
                   + "', post_name = '" + title
                   + "', post_parent = '" + product_id.ToString()
                   + "', menu_order  = '0', post_type= 'attachment', post_mime_type='image/png',comment_count = '0', post_date = NOW(), post_date_gmt = UTC_TIMESTAMP()");
                }

            Execute("INSERT INTO " + DB_PREFIX + "postmeta SET post_id='" + product_id + "',meta_key='_sku',meta_value='" + data["sku"] + "'");
            Execute("INSERT INTO " + DB_PREFIX + "postmeta SET post_id='" + product_id + "',meta_key='_regular_price',meta_value='" + data["price"] + "'");
            if (float.Parse(sale_price)!=0)
                Execute("INSERT INTO " + DB_PREFIX + "postmeta SET post_id='" + product_id + "',meta_key='_sale_price',meta_value='" + sale_price + "'");
            Execute("INSERT INTO " + DB_PREFIX + "postmeta SET post_id='" + product_id + "',meta_key='_tax_status',meta_value='taxable'");
            Execute("INSERT INTO " + DB_PREFIX + "postmeta SET post_id='" + product_id + "',meta_key='_stock_status',meta_value='instock'");


          

           

            List<int> categories = data["categories"];

            foreach (int category_id in categories)
            {
                sql = sql + "INSERT INTO " + DB_PREFIX + "product_to_category SET product_id = '" + product_id + "', category_id = '" + category_id + "';";
            }

            if (data["seourl"] != null)
            {
                Dictionary<int, string> seoURL = (Dictionary<int, string>)data["seourl"];
                foreach (int language in seoURL.Keys)
                {
                    if (Version == "3.0")
                        sql = sql + "INSERT INTO " + DB_PREFIX + "seo_url SET query = 'product_id=" + product_id + "', language_id = '" + language + "', store_id = '0',  keyword = '" + seoURL[language] + "';";
                    else
                        sql = sql + "INSERT INTO " + DB_PREFIX + "url_alias SET query = 'product_id=" + product_id + "', keyword = '" + seoURL[language] + "';";
                }
            }
            if (data["product_related"] != null)
            {
                foreach (int related_id in data["product_related"])
                {
                    sql = sql + "DELETE FROM " + DB_PREFIX + "product_related WHERE product_id = '" + product_id + "' AND related_id = '" + related_id + "';";
                    if (product_id != 0 && related_id != 0)
                        sql = sql + "INSERT INTO " + DB_PREFIX + "product_related SET product_id = '" + product_id + "', related_id = '" + related_id + "';";
                    sql = sql + "DELETE FROM " + DB_PREFIX + "product_related WHERE product_id = '" + related_id + "' AND related_id = '" + product_id + "';";
                    if (product_id != 0 && related_id != 0)
                        sql = sql + "INSERT INTO " + DB_PREFIX + "product_related SET product_id = '" + related_id + "', related_id = '" + product_id + "';";
                }
            }
            File.WriteAllText("sql.txt", sql, Encoding.UTF32);
            Execute(sql);
            int product_option_id = 0;
            int lastOptionID = 0;
            if (data["product_option"] != null)
            {
                foreach (DataRow product_option in ((DataTable)(data["product_option"][0])).Rows)
                {
                    if (int.Parse(product_option["option_id"].ToString()) != lastOptionID)
                    {
                        product_option_id = InsertRecord("INSERT INTO " + DB_PREFIX + "product_option SET product_id = '"
                            + product_id + "', option_id = '" + product_option["option_id"] + "', value='', required = '" + product_option["required"].ToString() + "'", true);
                        if (product_option["parent_option_id"].ToString().Trim() != "0")
                            InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_parent SET product_id = '"
                            + product_id + "', product_option_id = '" + product_option_id + "', parent_option='" + product_option["parent_option_id"].ToString() + "'", true);
                    }
                    lastOptionID = int.Parse(product_option["option_id"].ToString());
                    DataRow option_value_record = GetRecord("SELECT product_option_value_id FROM " + DB_PREFIX + "product_option_value WHERE "
                        + "product_id = '" + product_id + "' AND option_value_id = '" + product_option["option_value_id"] + "'");
                    int product_option_value_id;
                    if (option_value_record == null)
                    {
                        //Product option value does not exists
                        product_option_value_id = InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_value "
                            + " SET product_option_id = '" + product_option_id.ToString()
                            + "', product_id = '" + product_id
                            + "', option_id = '" + product_option["option_id"]
                            + "', option_value_id = '" + product_option["option_value_id"]
                            + "', quantity = '" + product_option["quantity"]
                            + "', subtract = '" + product_option["subtract"]
                            + "', price = '" + product_option["price"]
                            + "', price_prefix = '" + product_option["price_prefix"]
                            + "', points = '" + product_option["points"]
                            + "', points_prefix = '" + product_option["points_prefix"]
                            + "', weight = '" + product_option["weight"]
                            + "', weight_prefix = '" + product_option["weight_prefix"] + "'", true);
                        if (product_option["option_image_name"].ToString().Trim() != "")
                            InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_value_image "
                            + " SET product_option_value_id = '" + product_option_value_id.ToString()
                            + "', product_id = '" + product_id
                            + "', option_image = '" + product_option["option_image_name"].ToString().Replace("'", "''") + "'", true);
                    }
                    else
                    {
                        //Product option value exists. This may be a duplicate or dependent option repeated with a different parent
                        product_option_value_id = int.Parse(option_value_record["product_option_value_id"].ToString());
                    }
                    if (product_option["parent_option_value"].ToString() != "")
                    {
                        DataRow option_value_description_row = GetRecord("SELECT * FROM " + DB_PREFIX + "option_value_description WHERE `name`='" + product_option["parent_option_value"] + "'");
                        int parent_option_value_id = 0;
                        if (option_value_description_row == null)
                        {
                            throw new Exception("Unexpected error : Parent option not found");
                        }
                        else
                        {
                            parent_option_value_id = int.Parse(option_value_description_row["option_value_id"].ToString());
                        }
                        //This is a dependent option and therefore add relationship
                        InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_value_to_option_value"
                        + " SET product_option_id = '" + product_option_id.ToString()
                        + "', product_option_value_id = '" + product_option_value_id
                        + "', option_value_id = '" + parent_option_value_id
                        + "', product_id = '" + product_id
                        + "';"
                        + " INSERT INTO " + DB_PREFIX + "product_option_value_to_option_value"
                        + " SET product_option_id = '" + product_option_id.ToString()
                        + "', product_option_value_id = '0', option_value_id = '" + parent_option_value_id
                        + "', product_id = '" + product_id
                        + "'", false);
                    }
                }
            }
            sql = "";
            DataTable attValues = data["product_attribute"];
            if (attValues != null)
            {
                foreach (DataRow product_attribute in attValues.Rows)
                {
                    sql = sql + "INSERT INTO " + DB_PREFIX + "product_attribute SET product_id = '" + product_id + "', attribute_id = '"
                        + product_attribute["attribute_id"] + "', language_id = '" + product_attribute["language_id"] + "', text = '" + product_attribute["value"].ToString().Replace("'", "''") + "';";
                }
            }

            DataSet other_tables = data["other_tables"];
            if (other_tables != null)
            {
                foreach (DataTable dt in other_tables.Tables)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string sqlFields = "";
                        foreach (DataColumn dc in dt.Columns)
                        {
                            sqlFields = sqlFields + "," + dc.ColumnName + " = '" + dr[dc.ColumnName] + "'";
                        }
                        //sqlFields = sqlFields.Substring(0, sqlFields.Length - 1);
                        sql = sql + "INSERT INTO " + DB_PREFIX + dt.TableName + " SET product_id = '" + product_id + "'" + sqlFields + ";";
                    }
                }
            }
            Execute(sql);
            return product_id;
        }

        public override int editProduct(int product_id, Dictionary<string, dynamic> data, string ExcludeList)
        {
            string name, title;
            string sql;
            string[] exludeFieldList = ExcludeList.Split(new string[] { "," }, StringSplitOptions.None);

            float price;
            sql = "";
            sql = sql + "DELETE FROM " + DB_PREFIX + "product_option_parent WHERE product_id = '" + product_id + "';";
            sql = sql + "DELETE FROM " + DB_PREFIX + "product_option_value_to_option_value WHERE product_id = '" + product_id + "';";
            try
            {
                Execute(sql);
            }
            catch (Exception)
            {
                //
            }
            sql = "";
            sql = sql + "DELETE FROM " + DB_PREFIX + "product_special WHERE product_id = '" + product_id + "';";
            sql = sql + "DELETE FROM " + DB_PREFIX + "product_option WHERE product_id = '" + product_id + "';";
            sql = sql + "DELETE FROM " + DB_PREFIX + "product_option_value WHERE product_id = '" + product_id + "';";
            sql = sql + "DELETE FROM " + DB_PREFIX + "product_attribute WHERE product_id = '" + product_id + "';";
            sql = sql + "UPDATE " + DB_PREFIX
                + "product SET model = '" + data["model"].Replace("'", "''") + "'"
                + ", reffield = '" + data["reffield"] + "'"
                + ", sku = '" + data["sku"] + "'"
                + (!exludeFieldList.Contains("upc") ? ", upc = '" + data["upc"] + "'" : "")
                + (!exludeFieldList.Contains("ean") ? ", ean = '" + data["ean"] + "'" : "")
                + ", jan = '" + data["jan"] + "'"
                + ", isbn = '" + data["isbn"].Replace("'", "''") + "'"
                + ", mpn = '" + data["mpn"] + "'"
                + ", location = '" + data["location"] + "'"
                + (!exludeFieldList.Contains("quantity") ? ", quantity = '" + data["quantity"] + "'" : "")
                + ", minimum = '" + data["minimum"] + "'"
                + ", subtract = '" + data["subtract"] + "'"
                + ", stock_status_id = '" + data["stock_status_id"] + "'"
                + ", date_available = NOW()"
                + (!exludeFieldList.Contains("manufacturer") ? ", manufacturer_id = '" + data["manufacturer_id"] + "'" : "")
                + ", shipping = '" + data["shipping"] + "'"
                + (!exludeFieldList.Contains("price") ? ", price = '" + data["price"] + "'" : "")
                + ", points = '" + data["points"] + "'"
                + (!exludeFieldList.Contains("weight") ? ", weight = '" + data["weight"] + "'" : "")
                + ", weight_class_id = '" + data["weight_class_id"] + "'"
                + (!exludeFieldList.Contains("length") ? ", length = '" + data["length"] + "'" : "")
                + (!exludeFieldList.Contains("width") ? ", width = '" + data["width"] + "'" : "")
                + (!exludeFieldList.Contains("image") ? ", image = '" + data["image"].Replace("'", "''") + "'" : "")
                + (!exludeFieldList.Contains("height") ? ", height = '" + data["height"] + "'" : "")
                + ", length_class_id = '" + data["length_class_id"] + "'"
                + (!exludeFieldList.Contains("status") ? ", status = '" + data["status"] + "'" : "")
                + ", tax_class_id = '" + data["tax_class_id"] + "'"
                + ", sort_order = '" + data["sort_order"] + "'"
                + ", source_url='" + data["source_url"] + "',imported='1'"
                + ", date_added = NOW(), date_modified = NOW() WHERE product_id = '" + product_id + "';";
            
            Execute(sql);
            price = float.Parse(data["price"]);
            //Application.Exit();
            if (!exludeFieldList.Contains("image"))
            {
                sql = "";
                sql = sql + "DELETE FROM " + DB_PREFIX + "product_image WHERE product_id = '" + product_id + "';";

                DataTable xtraImages = (DataTable)data["product_image"];
                foreach (DataRow xtraImage in xtraImages.Rows)
                    if (xtraImage != null && Convert.ToBoolean(xtraImage["active"]))
                    {
                        sql = sql + "INSERT INTO " + DB_PREFIX + "product_image SET product_id = '" + product_id
                            + "', image = '" + xtraImage["image_name"].ToString().Replace("'", "''") + "', sort_order = '1';";
                    }
                Execute(sql);
            }


            Dictionary<int, dynamic> product_description = data["product_description"];
            string[] langKeys = new string[product_description.Keys.Count];
            int j = 0;
            if (!exludeFieldList.Contains("description"))
            {
                sql = "";
                sql = sql + "DELETE FROM " + DB_PREFIX + "product_description WHERE product_id = '" + product_id + "';";

                foreach (int key in product_description.Keys)
                {

                    name = product_description[key]["name"].Replace("\\'", "'").Replace("'", "''");
                    title = product_description[key]["meta_title"].Replace("\\'", "'").Replace("'", "''");
                    if (Version == "2.0" || Version == "2.1" || Version == "2.2" || Version == "3.0")
                        sql = sql + "INSERT INTO " + DB_PREFIX + "product_description SET product_id = '" + product_id + "', language_id = '" + key
                            + "', name = '" + name
                            + "', meta_keyword = '" + product_description[key]["meta_keyword"].Replace("\\'", "'").Replace("'", "''")
                            + "', meta_title = '" + title
                            + "', meta_description = LEFT('" + product_description[key]["meta_description"].Replace("\\'", "'").Replace("'", "''")
                            + "',255), description = '" + product_description[key]["description"].Replace("\\'", "'").Replace("'", "''")
                            + "', tag = '" + product_description[key]["tag"].Replace("\\'", "'").Replace("'", "''") + "';";
                    else
                        sql = sql + "INSERT INTO " + DB_PREFIX + "product_description SET product_id = '" + product_id + "', language_id = '" + key
                            + "', name = '" + name
                            + "', meta_keyword = '" + product_description[key]["meta_keyword"].Replace("\\'", "'").Replace("'", "''")
                            + "', meta_description = LEFT('" + product_description[key]["meta_description"].Replace("\\'", "'").Replace("'", "''")
                            + "',255), description = '" + product_description[key]["description"].Replace("\\'", "'").Replace("'", "''")
                            + "', tag = '" + product_description[key]["tag"].Replace("\\'", "'").Replace("'", "''") + "';";
                }
                File.WriteAllText("sql.txt", sql, Encoding.UTF32);
                Execute(sql);
            }

            List<int> categories = data["categories"];
            if (!exludeFieldList.Contains("category"))
            {
                foreach (int category_id in categories)
                {
                    int val = GetValue("SELECT product_id FROM " + DB_PREFIX + "product_to_category WHERE  product_id = '" + product_id + "' AND category_id = '" + category_id + "'");
                    if (val == 0)
                        InsertRecord("INSERT INTO " + DB_PREFIX + "product_to_category SET product_id = '" + product_id + "', category_id = '" + category_id + "'", false);
                }
            }

            sql = "";
            if (data["special_prices"] != null)
            {
                foreach (DataRow specialPrice in ((DataTable)(data["special_prices"])).Rows)
                {
                    if (float.Parse(specialPrice["price"].ToString()) != price)
                    {
                        sql = sql + "INSERT INTO " + DB_PREFIX + "product_special SET product_id = '" + product_id
                            + "', customer_group_id = '" + specialPrice["customer_group_id"]
                            + "', priority = '" + specialPrice["priority"]
                            + "', price = '" + specialPrice["price"] + "';";
                    }
                }
            }
            Execute(sql);

            if (data["seourl"] != null)
            {
                Dictionary<int, string> seoURL = (Dictionary<int, string>)data["seourl"];
                foreach (int language in seoURL.Keys)
                {
                    if (Version == "3.0")
                        InsertRecord("UPDATE " + DB_PREFIX + "seo_url SET keyword = '" + seoURL[language] + "', language_id = '" + language + "', store_id = '0' WHERE query = 'product_id=" + product_id + "'", false);
                    else
                        InsertRecord("UPDATE " + DB_PREFIX + "url_alias SET keyword = '" + seoURL[language] + "' WHERE query = 'product_id=" + product_id + "'", false);
                }
            }

            if (data["seourl"] != null)


                if (data["product_related"] != null)
                {
                    foreach (int related_id in data["product_related"])
                    {
                        Execute("DELETE FROM " + DB_PREFIX + "product_related WHERE product_id = '" + product_id + "' AND related_id = '" + related_id + "'");
                        if (product_id != 0 && related_id != 0)
                            InsertRecord("INSERT INTO " + DB_PREFIX + "product_related SET product_id = '" + product_id + "', related_id = '" + related_id + "'", false);
                        Execute("DELETE FROM " + DB_PREFIX + "product_related WHERE product_id = '" + related_id + "' AND related_id = '" + product_id + "'");
                        if (product_id != 0 && related_id != 0)
                            InsertRecord("INSERT INTO " + DB_PREFIX + "product_related SET product_id = '" + related_id + "', related_id = '" + product_id + "'", false);
                    }
                }

            int product_option_id = 0;
            int lastOptionID = 0;
            if (data["product_option"] != null)
            {
                foreach (DataRow product_option in ((DataTable)(data["product_option"][0])).Rows)
                {
                    if (int.Parse(product_option["option_id"].ToString()) != lastOptionID)
                    {
                        product_option_id = InsertRecord("INSERT INTO " + DB_PREFIX + "product_option SET product_id = '"
                            + product_id + "', option_id = '" + product_option["option_id"] + "', value='', required = '" + product_option["required"].ToString() + "'", true);
                        if (product_option["parent_option_id"].ToString().Trim() != "0")
                            InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_parent SET product_id = '"
                                + product_id + "', product_option_id = '" + product_option_id + "', parent_option='" + product_option["parent_option_id"].ToString() + "'", true);
                    }
                    lastOptionID = int.Parse(product_option["option_id"].ToString());
                    DataRow option_value_record = GetRecord("SELECT product_option_value_id FROM " + DB_PREFIX + "product_option_value WHERE "
                        + "product_id = '" + product_id + "' AND option_value_id = '" + product_option["option_value_id"] + "'");
                    int product_option_value_id;
                    if (option_value_record == null)
                    {
                        //Product option value does not exists
                        product_option_value_id = InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_value "
                            + " SET product_option_id = '" + product_option_id.ToString()
                            + "', product_id = '" + product_id
                            + "', option_id = '" + product_option["option_id"]
                            + "', option_value_id = '" + product_option["option_value_id"]
                            + "', quantity = '" + product_option["quantity"]
                            + "', subtract = '" + product_option["subtract"]
                            + "', price = '" + product_option["price"]
                            + "', price_prefix = '" + product_option["price_prefix"]
                            + "', points = '" + product_option["points"]
                            + "', points_prefix = '" + product_option["points_prefix"]
                            + "', weight = '" + product_option["weight"]
                            + "', weight_prefix = '" + product_option["weight_prefix"] + "'", true);
                        if (product_option["option_image_name"].ToString().Trim() != "")
                            InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_value_image "
                            + " SET product_option_value_id = '" + product_option_value_id.ToString()
                            + "', product_id = '" + product_id
                            + "', option_image = '" + product_option["option_image_name"].ToString().Replace("'", "''") + "'", true);
                    }
                    else
                    {
                        //Product option value exists. This may be a duplicate or dependent option repeated with a different parent
                        product_option_value_id = int.Parse(option_value_record["product_option_value_id"].ToString());
                    }
                    if (product_option["parent_option_value"].ToString() != "")
                    {
                        DataRow option_value_description_row = GetRecord("SELECT * FROM " + DB_PREFIX + "option_value_description WHERE `name`='" + product_option["parent_option_value"] + "' AND option_id='" + product_option["parent_option_id"] + "'");
                        int parent_option_value_id = 0;
                        if (option_value_description_row == null)
                        {
                            throw new Exception("Unexpected error : Parent option not found");
                        }
                        else
                        {
                            parent_option_value_id = int.Parse(option_value_description_row["option_value_id"].ToString());
                        }
                        //This is a dependent option and therefore add relationship
                        Execute("INSERT INTO " + DB_PREFIX + "product_option_value_to_option_value"
                        + " SET product_option_id = '" + product_option_id.ToString()
                        + "', product_option_value_id = '" + product_option_value_id
                        + "', option_value_id = '" + parent_option_value_id
                        + "', product_id = '" + product_id
                        + "';"
                        + " INSERT INTO " + DB_PREFIX + "product_option_value_to_option_value"
                        + " SET product_option_id = '" + product_option_id.ToString()
                        + "', product_option_value_id = '0', option_value_id = '" + parent_option_value_id
                        + "', product_id = '" + product_id
                        + "'");
                    }
                }
            }
            sql = "";
            DataTable attValues = data["product_attribute"];
            if (attValues != null)
            {
                foreach (DataRow product_attribute in attValues.Rows)
                {
                    sql = sql + "INSERT INTO " + DB_PREFIX + "product_attribute SET product_id = '" + product_id + "', attribute_id = '"
                        + product_attribute["attribute_id"] + "', language_id = '" + product_attribute["language_id"] + "', text = '" + product_attribute["value"].ToString().Replace("'", "''") + "';";
                }
            }
            Execute(sql);

            DataSet other_tables = data["other_tables"];
            if (other_tables != null)
            {
                foreach (DataTable dt in other_tables.Tables)
                {
                    Execute("DELETE FROM " + DB_PREFIX + dt.TableName + " WHERE product_id = '" + product_id + "'");
                    foreach (DataRow dr in dt.Rows)
                    {
                        string sqlFields = "";
                        foreach (DataColumn dc in dt.Columns)
                        {
                            sqlFields = sqlFields + "," + dc.ColumnName + " = '" + dr[dc.ColumnName] + "'";
                        }
                        //sqlFields = sqlFields.Substring(0, sqlFields.Length - 1);
                        InsertRecord("INSERT INTO " + DB_PREFIX + dt.TableName + " SET product_id = '" + product_id + "'" + sqlFields, false);
                    }
                }
            }
            //Application.Exit();
            return product_id;
        }


        public override int quickUpdate(int product_id, Dictionary<string, dynamic> data)
        {
            string sql;

            float price;
            sql = "";
            sql = sql + "DELETE FROM " + DB_PREFIX + "product_special WHERE product_id = '" + product_id + "';";
            sql = sql + "DELETE FROM " + DB_PREFIX + "product_option WHERE product_id = '" + product_id + "';";
            sql = sql + "DELETE FROM " + DB_PREFIX + "product_option_value WHERE product_id = '" + product_id + "';";
            sql = sql + "UPDATE " + DB_PREFIX
                + "product SET  quantity = '" + data["quantity"] + "'"
                + ", price = '" + data["price"] + "'"
                + ", status = '" + data["status"] + "'"
                + ", date_modified = NOW() WHERE product_id = '" + product_id + "';";

            price = float.Parse(data["price"]);


            sql = "";
            if (data["special_prices"] != null)
            {
                foreach (DataRow specialPrice in ((DataTable)(data["special_prices"])).Rows)
                {
                    if (float.Parse(specialPrice["price"].ToString()) != price)
                    {
                        sql = sql + "INSERT INTO " + DB_PREFIX + "product_special SET product_id = '" + product_id
                            + "', customer_group_id = '" + specialPrice["customer_group_id"]
                            + "', priority = '" + specialPrice["priority"]
                            + "', price = '" + specialPrice["price"] + "';";
                    }
                }
            }
            Execute(sql);


            int product_option_id = 0;
            int lastOptionID = 0;
            if (data["product_option"] != null)
            {
                foreach (DataRow product_option in ((DataTable)(data["product_option"][0])).Rows)
                {
                    if (int.Parse(product_option["option_id"].ToString()) != lastOptionID)
                    {
                        product_option_id = InsertRecord("INSERT INTO " + DB_PREFIX + "product_option SET product_id = '"
                            + product_id + "', option_id = '" + product_option["option_id"] + "', value='', required = '" + product_option["required"].ToString() + "'", true);
                        if (product_option["parent_option_id"].ToString().Trim() != "0")
                            InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_parent SET product_id = '"
                                + product_id + "', product_option_id = '" + product_option_id + "', parent_option='" + product_option["parent_option_id"].ToString() + "'", true);
                    }
                    lastOptionID = int.Parse(product_option["option_id"].ToString());
                    DataRow option_value_record = GetRecord("SELECT product_option_value_id FROM " + DB_PREFIX + "product_option_value WHERE "
                        + "product_id = '" + product_id + "' AND option_value_id = '" + product_option["option_value_id"] + "'");
                    int product_option_value_id;
                    if (option_value_record == null)
                    {
                        //Product option value does not exists
                        product_option_value_id = InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_value "
                            + " SET product_option_id = '" + product_option_id.ToString()
                            + "', product_id = '" + product_id
                            + "', option_id = '" + product_option["option_id"]
                            + "', option_value_id = '" + product_option["option_value_id"]
                            + "', quantity = '" + product_option["quantity"]
                            + "', subtract = '" + product_option["subtract"]
                            + "', price = '" + product_option["price"]
                            + "', price_prefix = '" + product_option["price_prefix"]
                            + "', points = '" + product_option["points"]
                            + "', points_prefix = '" + product_option["points_prefix"]
                            + "', weight = '" + product_option["weight"]
                            + "', weight_prefix = '" + product_option["weight_prefix"] + "'", true);
                        if (product_option["option_image_name"].ToString().Trim() != "")
                            InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_value_image "
                            + " SET product_option_value_id = '" + product_option_value_id.ToString()
                            + "', product_id = '" + product_id
                            + "', option_image = '" + product_option["option_image_name"].ToString().Replace("'", "''") + "'", true);
                    }
                    else
                    {
                        //Product option value exists. This may be a duplicate or dependent option repeated with a different parent
                        product_option_value_id = int.Parse(option_value_record["product_option_value_id"].ToString());
                    }
                    if (product_option["parent_option_value"].ToString() != "")
                    {
                        DataRow option_value_description_row = GetRecord("SELECT * FROM " + DB_PREFIX + "option_value_description WHERE `name`='" + product_option["parent_option_value"] + "'");
                        int parent_option_value_id = 0;
                        if (option_value_description_row == null)
                        {
                            throw new Exception("Unexpected error : Parent option not found");
                        }
                        else
                        {
                            parent_option_value_id = int.Parse(option_value_description_row["option_value_id"].ToString());
                        }
                        //This is a dependent option and therefore add relationship
                        Execute("INSERT INTO " + DB_PREFIX + "product_option_value_to_option_value"
                        + " SET product_option_id = '" + product_option_id.ToString()
                        + "', product_option_value_id = '" + product_option_value_id
                        + "', option_value_id = '" + parent_option_value_id
                        + "', product_id = '" + product_id
                        + "';"
                        + " INSERT INTO " + DB_PREFIX + "product_option_value_to_option_value"
                        + " SET product_option_id = '" + product_option_id.ToString()
                        + "', product_option_value_id = '0', option_value_id = '" + parent_option_value_id
                        + "', product_id = '" + product_id
                        + "'");
                    }
                }
            }
            sql = "";

            return product_id;
        }



        public override void updateSelectedFields(Dictionary<string, string> fieldCollection, int product_id, Dictionary<string, dynamic> data)
        {
            string SQL;
            int keyCount;
            foreach (string tableName in fieldCollection.Keys)
            {
                if (tableName == "product_option") continue;
                string[] fieldValues = fieldCollection[tableName].Split(new string[] { "," }, StringSplitOptions.None);
                SQL = "UPDATE " + DB_PREFIX + tableName + " SET";
                int counter = 1;
                foreach (string field in fieldValues)
                {

                    keyCount = fieldValues.Length;
                    if (counter == keyCount)
                        SQL = SQL + " " + field.Replace("'", "''") + " = '" + data[field] + "'";
                    else
                        SQL = SQL + " " + field.Replace("'", "''") + " = '" + data[field] + "',";
                    counter++;


                }
                SQL = SQL + " WHERE product_id = '" + product_id + "'";
                Execute(SQL);
            }

            if (fieldCollection.ContainsKey("product_option"))
            {
                //Quick update the options
                SQL = "";
                SQL = SQL + "DELETE FROM " + DB_PREFIX + "product_option WHERE product_id = '" + product_id + "';";
                SQL = SQL + "DELETE FROM " + DB_PREFIX + "product_option_value WHERE product_id = '" + product_id + "';";
                SQL = SQL + "DELETE FROM " + DB_PREFIX + "product_option_parent WHERE product_id = '" + product_id + "';";
                SQL = SQL + "DELETE FROM " + DB_PREFIX + "product_option_value_image WHERE product_id = '" + product_id + "';";
                SQL = SQL + "DELETE FROM " + DB_PREFIX + "product_option_value_to_option_value WHERE product_id = '" + product_id + "';";
                Execute(SQL);
                if (data["product_option"] != null)
                {
                    int product_option_id = 0;
                    int lastOptionID = 0;
                    foreach (DataRow product_option in ((DataTable)(data["product_option"][0])).Rows)
                    {
                        if (int.Parse(product_option["option_id"].ToString()) != lastOptionID)
                        {
                            product_option_id = InsertRecord("INSERT INTO " + DB_PREFIX + "product_option SET product_id = '"
                                + product_id + "', option_id = '" + product_option["option_id"] + "', value='', required = '" + product_option["required"].ToString() + "'", true);
                            if (product_option["parent_option_id"].ToString().Trim() != "0")
                                InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_parent SET product_id = '"
                                    + product_id + "', product_option_id = '" + product_option_id + "', parent_option='" + product_option["parent_option_id"].ToString() + "'", true);
                        }
                        lastOptionID = int.Parse(product_option["option_id"].ToString());
                        DataRow option_value_record = GetRecord("SELECT product_option_value_id FROM " + DB_PREFIX + "product_option_value WHERE "
                            + "product_id = '" + product_id + "' AND option_value_id = '" + product_option["option_value_id"] + "'");
                        int product_option_value_id;
                        if (option_value_record == null)
                        {
                            //Product option value does not exists
                            product_option_value_id = InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_value "
                                + " SET product_option_id = '" + product_option_id.ToString()
                                + "', product_id = '" + product_id
                                + "', option_id = '" + product_option["option_id"]
                                + "', option_value_id = '" + product_option["option_value_id"]
                                + "', quantity = '" + product_option["quantity"]
                                + "', subtract = '" + product_option["subtract"]
                                + "', price = '" + product_option["price"]
                                + "', price_prefix = '" + product_option["price_prefix"]
                                + "', points = '" + product_option["points"]
                                + "', points_prefix = '" + product_option["points_prefix"]
                                + "', weight = '" + product_option["weight"]
                                + "', weight_prefix = '" + product_option["weight_prefix"] + "'", true);
                            if (product_option["option_image_name"].ToString().Trim() != "")
                                InsertRecord("INSERT INTO " + DB_PREFIX + "product_option_value_image "
                                + " SET product_option_value_id = '" + product_option_value_id.ToString()
                                + "', product_id = '" + product_id
                                + "', option_image = '" + product_option["option_image_name"].ToString().Replace("'", "''") + "'", true);
                        }
                        else
                        {
                            //Product option value exists. This may be a duplicate or dependent option repeated with a different parent
                            product_option_value_id = int.Parse(option_value_record["product_option_value_id"].ToString());
                        }
                        if (product_option["parent_option_value"].ToString() != "")
                        {
                            DataRow option_value_description_row = GetRecord("SELECT * FROM " + DB_PREFIX + "option_value_description WHERE `name`='" + product_option["parent_option_value"] + "'");
                            int parent_option_value_id = 0;
                            if (option_value_description_row == null)
                            {
                                throw new Exception("Unexpected error : Parent option not found");
                            }
                            else
                            {
                                parent_option_value_id = int.Parse(option_value_description_row["option_value_id"].ToString());
                            }
                            //This is a dependent option and therefore add relationship
                            Execute("INSERT INTO " + DB_PREFIX + "product_option_value_to_option_value"
                            + " SET product_option_id = '" + product_option_id.ToString()
                            + "', product_option_value_id = '" + product_option_value_id
                            + "', option_value_id = '" + parent_option_value_id
                            + "', product_id = '" + product_id
                            + "';"
                            + " INSERT INTO " + DB_PREFIX + "product_option_value_to_option_value"
                            + " SET product_option_id = '" + product_option_id.ToString()
                            + "', product_option_value_id = '0', option_value_id = '" + parent_option_value_id
                            + "', product_id = '" + product_id
                            + "'");
                        }
                    }
                }
            }
        }
        public override void updateAdditionalFields(int product_id, Dictionary<string, string> data)
        {
            string SQL = "UPDATE " + DB_PREFIX + "product SET";
            int keyCount = data.Keys.Count;
            int counter = 1;
            foreach (string key in data.Keys)
            {
                if (counter == keyCount)
                    SQL = SQL + " " + key + " = '" + data[key] + "'";
                else
                    SQL = SQL + " " + key + " = '" + data[key] + "',";
                counter++;
            }
            SQL = SQL + " WHERE product_id = '" + product_id + "'";
            Execute(SQL);
        }
        public override int getProductByRefField(string reffield)
        {
            return GetValue("SELECT product_id FROM " + DB_PREFIX + "product WHERE reffield='" + reffield + "'");
        }


        public override int getOption(string name)
        {
            return GetValue("SELECT o.option_id FROM `" + DB_PREFIX + "option` o INNER JOIN " + DB_PREFIX + "option_description od "
                + " ON o.option_id = od.option_id WHERE name='" + name + "'");
        }


        public override int getOptionValue(int option_id, string name)
        {
            return GetValue("SELECT ov.option_value_id FROM " + DB_PREFIX + "option_value ov INNER JOIN " + DB_PREFIX + "option_value_description ovd "
                + " ON ov.option_value_id = ovd.option_value_id WHERE name='" + name.Replace("'", "''") + "' AND ov.option_id='" + option_id + "'");
        }

        public override int addOption(string[] name, string[] languages)
        {
            int option_id = InsertRecord("INSERT INTO " + DB_PREFIX + "option SET type='select',sort_order='0'", true);
            for (int i = 0; i < languages.Length; i++)
            {
                InsertRecord("INSERT INTO " + DB_PREFIX + "option_description SET "
                    + " option_id='" + option_id.ToString()
                    + "',name='" + name[i] + "',language_id='" + languages[i] + "'", true);
            }
            return option_id;
        }

        public override int addOptionValue(int option_id, string[] optionValueName, string[] languages)
        {
            int lastId = InsertRecord("INSERT INTO " + DB_PREFIX + "option_value SET option_id='" + option_id + "',image='',sort_order='1'", true);

            for (int i = 0; i < languages.Length; i++)
            {
                InsertRecord("INSERT INTO " + DB_PREFIX + "option_value_description "
                    + " SET option_value_id='" + lastId + "',language_id='" + languages[i]
                    + "', option_id='" + option_id + "', name='" + optionValueName[i].Replace("'", "''") + "'", false);
            }
            Execute("UPDATE " + DB_PREFIX + "option_value SET sort_order='" + lastId + "' WHERE option_value_id='" + lastId + "'");
            return lastId;
        }
        public override DataTable getCategories()
        {
            string catPath;
            string sql = "select wtt.term_id as category_id,wt.name as catname,name,parent as parent_id,0 as sort_order from "  
                + DB_PREFIX + "term_taxonomy wtt inner join "+ DB_PREFIX
                + "terms wt on wtt.term_id = wt.term_id where taxonomy = 'product_cat'  and name<>'Uncategorized' order by wtt.term_id; ";

            DataTable tempCatList = GetTable(sql);
            int rowNo = 0;
            foreach (DataRow dr in tempCatList.Rows)
            {
                catPath = dr["catname"].ToString();
                if (dr["parent_id"].ToString() != "0")
                {
                    DataRow[] rowsSub = tempCatList.Select("category_id=" + dr["parent_id"].ToString());
                    if (rowsSub.Length == 1)
                    {
                        catPath = rowsSub[0]["catname"].ToString() + "&nbsp;&nbsp;&gt;&nbsp;&nbsp;" + catPath;
                        if (rowsSub[0]["parent_id"].ToString() != "0")
                        {
                            DataRow[] rowsChild = tempCatList.Select("category_id=" + rowsSub[0]["parent_id"].ToString());
                            if (rowsChild.Length == 1)
                            {
                                catPath = rowsChild[0]["catname"].ToString() + "&nbsp;&nbsp;&gt;&nbsp;&nbsp;" + catPath;
                            }
                        }
                    }
                }
                tempCatList.Rows[rowNo]["name"] = catPath;
                rowNo++;
            }
            tempCatList.Columns.Remove("catname");
            return tempCatList;
        }

        public override DataTable getCategoriesProper()
        {
            string sql = "SELECT cp.category_id AS category_id, GROUP_CONCAT(cd1.name ORDER BY cp.level SEPARATOR '  >  ') AS name, c1.parent_id, c1.sort_order FROM " +
                DB_PREFIX + "category_path cp LEFT JOIN " + DB_PREFIX + "category c1 ON (cp.category_id = c1.category_id) LEFT JOIN "
                + DB_PREFIX + "category c2 ON (cp.path_id = c2.category_id) LEFT JOIN " + DB_PREFIX +
                "category_description cd1 ON (cp.path_id = cd1.category_id) LEFT JOIN " + DB_PREFIX +
                "category_description cd2 ON (cp.category_id = cd2.category_id) WHERE cd1.language_id = '" +
                LANGUAGE_ID + "' AND cd2.language_id = '" + LANGUAGE_ID + "' GROUP BY cp.category_id";
            return GetTable(sql);
        }
        public override DataTable getAttributes(string attribute_group_id)
        {
            return null;
        }
        public override int addCategory(int parentId, int top, string keyword, Dictionary<string, Dictionary<string, string>> name_array, string fullPath = "")
        {
            string name;
            int position;
            string[] catParts = fullPath.Split(new string[] { "///" }, StringSplitOptions.None);
            position = Array.IndexOf(catParts, keyword);




            string sql = "INSERT INTO " + DB_PREFIX + "categories SET parent_id = '" + parentId.ToString()
            + "', `name` = '" + name_array["1"]["name"].ToString()
            + "', `position` = '" + position.ToString() + "', priority = '0', module_id='1', status = '1', updated_at = NOW(), created_at = NOW()";
            int category_id = InsertRecord(sql, true);

            return category_id;
        }

        public override int addAttribute(string attribute_group_id, int sort_order, Dictionary<string, string> attItem)
        {
            string att_name;
            int lastKey = InsertRecord("INSERT INTO " + DB_PREFIX + "attribute SET attribute_group_id = '" + attribute_group_id + "', sort_order = '" + sort_order.ToString() + "'", true);
            foreach (string key in attItem.Keys)
            {
                att_name = key.Replace("\\'", "'").Replace("'", "''");
                InsertRecord("INSERT INTO " + DB_PREFIX + "attribute_description SET attribute_id = '" + lastKey + "', language_id = '" + key + "', name = '" + attItem[key] + "'", false);
            }
            return lastKey;
        }
    }
}
