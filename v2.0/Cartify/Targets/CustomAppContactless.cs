using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace Cartify
{
    public static class Extensions
    {
        public static bool find<T>(this T[] array, T target)
        {
            return Array.FindIndex(array, x => x.Equals(target)) != -1;
        }
    }

    public class CustomAppContactless : DbWrapper
    {
        /// <summary>
        /// Creates a new database wrapper object that wraps around
        /// the users table.
        /// </summary>
        /// <param name="svr">The name of the server</param>
        /// <param name="db">The database catalog to use</param>
        /// <param name="user">The user name</param>
        /// <param name="pass">The user password</param>
        public CustomAppContactless()
        {

        }


        public override void DBInit()
        {
            DataRow row = GetRecord("SHOW COLUMNS FROM `" + DB_PREFIX + "items` LIKE 'reffield'");
            if (row == null)
            {
                Execute("ALTER TABLE `" + DB_PREFIX + "items` ADD reffield VARCHAR(64)");
            }
            row = GetRecord("SHOW COLUMNS FROM `" + DB_PREFIX + "items` LIKE 'source_url'");
            if (row == null)
            {
                Execute("ALTER TABLE `" + DB_PREFIX + "items` ADD source_url TEXT");
            }
            row = GetRecord("SHOW COLUMNS FROM `" + DB_PREFIX + "items` LIKE 'imported'");
            if (row == null)
            {
                Execute("ALTER TABLE `" + DB_PREFIX + "items` ADD imported BOOL DEFAULT 0");
            }
            row = GetRecord("SHOW COLUMNS FROM `" + DB_PREFIX + "items` LIKE 'source'");
            if (row == null)
            {
                Execute("ALTER TABLE `" + DB_PREFIX + "items` ADD source VARCHAR(60)");
            }
        }


        public override DataTable getImportedProducts(string name, string model, string price, string quantity, string status, string category_id, string brand_id, string source, string lastID)
        {
            string sql = "SELECT id as product_id, reffield as model,name,price, stock as quantity,status,source_url FROM " + DB_PREFIX + "items ";
               

            sql += " WHERE imported='1' and source='" + source + "' ";

            /*if (name != "")
                sql += " AND pd.name LIKE '" + name + "%'";
            if (model != "")
                sql += " AND p.model LIKE '" + model + "%'";
            if (price != "")
                sql += " AND p.price LIKE '" + model + "%'";
            if (quantity != "")
                sql += " AND p.price = '" + quantity + "'";
            if (status != "")
                sql += " AND p.status = '" + status + "'";
            if (brand_id != "")
                sql += " AND p.manufacturer_id= '" + brand_id + "'";
            if (lastID != "")
                sql += " AND p.product_id <= '" + lastID + "'";*/
            sql += " ORDER BY id DESC";
            return GetTable(sql);
        }


        public override void DeleteProduct(int product_id)
        {
            string idToDelete = product_id.ToString();
            Execute("DELETE FROM " + DB_PREFIX + "items WHERE id = '" + idToDelete + "'");
        }

 
        public override DataRow getAdminUser()
        {
            return null;
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
            return GetValue("SELECT id FROM " + DB_PREFIX + "items WHERE reffield='" + model.Replace("'", "''") + "'");
        }

        public override int getTotalImportedProducts(string source)
        {
            return GetValue("SELECT count(id) FROM " + DB_PREFIX + "items WHERE source='" + source +"' AND (imported='1' or source_url!='')");
        }

        public override int addProduct(Dictionary<string, dynamic> data)
        {
            string name="", title;
            float price;

            Dictionary<int, dynamic> product_description = data["product_description"];
            foreach (int key in product_description.Keys)
            {
                name = product_description[key]["name"].Replace("\\'", "'").Replace("'", "''");                
            }

            string catjson = "";
            string category_id, sub_category_id, child_sub_category_id;

            category_id = data["categories"].Count >= 1 ? data["categories"][0].ToString() : "0";

            if (data["categories"].Count >= 2)
                category_id  = sub_category_id = data["categories"][1].ToString();
            else
                sub_category_id = "0";

            child_sub_category_id = data["categories"].Count >= 3 ? data["categories"][2].ToString() : "0";


            switch (data["categories"].Count)
            {
                case 1:
                    catjson = "[{\"id\":\"" + category_id + "\",\"position\":1},{\"id\":\"" + sub_category_id + "\",\"position\":2}]";
                    break;
                case 2:
                    catjson = "[{\"id\":\"" + category_id + "\",\"position\":1},{\"id\":\""+ sub_category_id + "\",\"position\":2}]";
                    break;
                case 3:
                    catjson = "[{\"id\":\"" + category_id + "\",\"position\":1},{\"id\":\"" + sub_category_id + "\",\"position\":2}]";
                    break;

            }

            string sql = "INSERT INTO " + DB_PREFIX
                + "items SET reffield = '" + data["reffield"].Replace("'", "''")
                + "', name = '" + name
                + "', description = '" + name
                + "', category_ids = '" + catjson
                + "', category_id = '" + category_id
                + "', sub_category_id = '" + sub_category_id
                + "', child_sub_category_id = '" + child_sub_category_id
                + "', price = '" + data["price"] 
                + "', image = '" + data["image"].Replace("'", "''")
                + "', images = '[]',choice_options= '[]',attributes = '[]',add_ons = '[]',variations = '[]"
                + "', available_time_starts = '00:00:00',available_time_ends = '23:00:00"
                + "', status = '1', module_id='1',stock='"  + data["quantity"]
                + "', source_url='" + data["source_url"] + "',imported='1"
                + "', source = '" + data["source"]
                + "', created_at = NOW(), updated_at= NOW()";
            int product_id = InsertRecord(sql, true);
           
            return product_id;
        }

        public override int editProduct(int product_id, Dictionary<string, dynamic> data, string ExcludeList)
        {
            string name = "", title;
            float price;

            Dictionary<int, dynamic> product_description = data["product_description"];
            foreach (int key in product_description.Keys)
            {
                name = product_description[key]["name"].Replace("\\'", "'").Replace("'", "''");
            }

            string catjson = "";
            string category_id, sub_category_id, child_sub_category_id;

            category_id = data["categories"].Count >= 1 ? data["categories"][0].ToString() : "0";

            if (data["categories"].Count >= 2)
                category_id = sub_category_id = data["categories"][1].ToString();
            else
                sub_category_id = "0";

            child_sub_category_id = data["categories"].Count >= 3 ? data["categories"][2].ToString() : "0";


            switch (data["categories"].Count)
            {
                case 1:
                    catjson = "[{\"id\":\"" + category_id + "\",\"position\":1},{\"id\":\"" + sub_category_id + "\",\"position\":2}]";
                    break;
                case 2:
                    catjson = "[{\"id\":\"" + category_id + "\",\"position\":1},{\"id\":\"" + sub_category_id + "\",\"position\":2}]";
                    break;
                case 3:
                    catjson = "[{\"id\":\"" + category_id + "\",\"position\":1},{\"id\":\"" + sub_category_id + "\",\"position\":2}]";
                    break;

            }

            string sql = "UPDATE " + DB_PREFIX
                + "items SET reffield = '" + data["reffield"].Replace("'", "''")
                + "', name = '" + name
                + "', description = '" + name
                + "', category_ids = '" + catjson
                + "', category_id = '" + category_id
                + "', sub_category_id = '" + sub_category_id
                + "', child_sub_category_id = '" + child_sub_category_id
                + "', price = '" + data["price"]
                + "', image = '" + data["image"].Replace("'", "''")
                + "', images = '[]',choice_options= '[]',attributes = '[]',add_ons = '[]',variations = '[]"
                + "', available_time_starts = '00:00:00',available_time_ends = '23:00:00"
                + "', status = '1', module_id='1',stock='" + data["quantity"]
                + "', source_url='" + data["source_url"] + "',imported='1"
                + "', updated_at= NOW() WHERE id='" + product_id +"'";

            Execute(sql);
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
            string SQL,key;
            int keyCount;
            foreach (string tableName in fieldCollection.Keys)
            {
                if (tableName == "product_option") continue;
                string[] fieldValues = fieldCollection[tableName].Split(new string[] { "," }, StringSplitOptions.None);
                SQL = "UPDATE " + DB_PREFIX + tableName + " SET";
                int counter = 1;
                foreach (string field in fieldValues)
                {
                    if (field == "stock")
                        key = "quantity";
                    else
                        key = field;

                    keyCount = fieldValues.Length;
                    if (counter == keyCount)
                        SQL = SQL + " " + field.Replace("'", "''") + " = '" + data[key] + "'";
                    else
                        SQL = SQL + " " + field.Replace("'", "''") + " = '" + data[key] + "',";
                    counter++;


                }
                SQL = SQL + " WHERE id = '" + product_id + "'";
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
            return GetValue("SELECT id FROM " + DB_PREFIX + "items WHERE reffield='" + reffield + "'");
        }


        public override int getOption(string name)
        {
            return 0;
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
            string sql = "SELECT id as category_id,name as catname, name,parent_id,priority as sort_order " + DB_PREFIX + "FROM categories ORDER BY id;";
            DataTable tempCatList =  GetTable(sql);
            int rowNo = 0;
            foreach(DataRow dr in tempCatList.Rows)
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
            string sql = "SELECT id as attribute_id, 0 as attribute_group_id, '1' as language_id,name " + DB_PREFIX + " FROM `attributes`";
            return GetTable(sql);
        }

        public override int addCategory(int parentId, int top, string keyword, Dictionary<string, Dictionary<string, string>> name_array, string fullPath = "")
        {
            string name;
            int position;
            string[] catParts = fullPath.Split(new string[] { "///" }, StringSplitOptions.None);
            position = Array.IndexOf(catParts,keyword);
            



            string sql = "INSERT INTO " + DB_PREFIX + "categories SET parent_id = '" + parentId.ToString()
            + "', `name` = '" + name_array["1"]["name"].Replace("'","''").ToString()
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

        //write a function to encrypt string    


    }

    
}
