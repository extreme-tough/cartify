using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace AllImporterPro
{
    public class DbWrapper
    {
        private string connStr;
        private bool isConnected;

        /// <summary>
        /// Creates a new database wrapper object that wraps around
        /// the users table.
        /// </summary>
        /// <param name="svr">The name of the server</param>
        /// <param name="db">The database catalog to use</param>
        /// <param name="user">The user name</param>
        /// <param name="pass">The user password</param>
        public DbWrapper()
        {
            string ConnectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            this.connStr = ConnectionString;

            try
            {
                sqlConn = new MySqlConnection(this.connStr);
            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Error connecting you to " +
                    "the my sql server. Internal error message: " + excp.Message, excp);
                throw myExcp;
            }

            this.isConnected = false;
            Connect();
        }


        /// <summary>
        /// Opens the connection to the SQL database.
        /// </summary>
        private void Connect()
        {
            bool success = true;

            if (this.isConnected == false)
            {
                try
                {
                    this.sqlConn.Open();
                }
                catch (Exception excp)
                {
                    this.isConnected = false;
                    success = false;
                    Exception myException = new Exception("Error opening connection" +
                        " to the sql server. Error: " + excp.Message, excp);

                    throw myException;
                }

                if (success)
                {
                    this.isConnected = true;
                }
            }
        }

        /// <summary>
        /// Closes the connection to the sql connection.
        /// </summary>
        public void Disconnect()
        {
            if (this.isConnected)
            {
                this.sqlConn.Close();
            }
        }

        /// <summary>
        /// Gets the current state (boolean) of the connection.
        /// True for open, false for closed.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
        }

        private int InsertRecord(string query, bool returnKey)
        {
            object oResult;
            MySqlCommand command = new MySqlCommand(query, this.sqlConn);
            command.ExecuteNonQuery();
            if (returnKey)
            {
                command.CommandText = "SELECT last_insert_id()";
                oResult = command.ExecuteScalar();
                return Convert.ToInt32(oResult);
            }
            else
                return 0;
        }

        private void Execute(string query)
        {
            MySqlCommand command = new MySqlCommand(query, this.sqlConn);
            command.ExecuteNonQuery();           
        }

        private int GetValue(string query)
        {
            object oResult;
            MySqlCommand command = new MySqlCommand(query, this.sqlConn);
            try
            {
                oResult = command.ExecuteScalar();
                return Convert.ToInt32(oResult);

            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not add user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }
        }


        /// <summary>
        /// Adds a user into the database
        /// </summary>
        /// <param name="username">The user login</param>
        /// <param name="password">The user password</param>
        public int getManufacturer(string manufacturer)
        {
            return GetValue("SELECT manufacturer_id FROM " +  ImportConfig.DB_PREFIX + "manufacturer WHERE name='" + manufacturer + "'");
        }



        public int addManufacturer(Dictionary<string,dynamic> data) 
        {		    
            int manufacturer_id;
            manufacturer_id = InsertRecord("INSERT INTO " + ImportConfig.DB_PREFIX + "manufacturer SET name = '"  + data["name"]  + "', sort_order = '" + data["sort_order"] + "'",true);
		
		    if (data.ContainsKey("manufacturer_store") )
            {
			    foreach (string store_id in data["manufacturer_store"]) {
				    InsertRecord("INSERT INTO " +  ImportConfig.DB_PREFIX + "manufacturer_to_store SET manufacturer_id = '" + manufacturer_id + "', store_id = '"  + store_id + "'",false);
			    }
		    }
				
		    if (data["keyword"]!="") {
			    InsertRecord("INSERT INTO " +  ImportConfig.DB_PREFIX + "url_alias SET query = 'manufacturer_id="  +  manufacturer_id + "', keyword = '" + data["keyword"] + "'",false);
		    }
		
		    return manufacturer_id;
	    }

        public int getProductByModel(string model) {		
		    return GetValue("SELECT product_id FROM " + ImportConfig.DB_PREFIX + "product WHERE model='" + model.Replace("'","''") + "'");
	    }

        public int addProduct(Dictionary<string, dynamic> data)
        {
            int product_id = InsertRecord("INSERT INTO " + ImportConfig.DB_PREFIX
                + "product SET model = '" + data["model"].Replace("'", "''")
                + "', reffield = '" + data["reffield"]                
                + "', sku = '" + data["sku"] 
                + "', upc = '" + data["upc"]
                + "', ean = '" + data["ean"]
                + "', jan = '" + data["jan"]
                + "', isbn = '" + data["isbn"].Replace("'", "''")
                + "', mpn = '" + data["mpn"] 
                + "', location = '" + data["location"] 
                + "', quantity = '" + data["quantity"] 
                + "', minimum = '" + data["minimum"] 
                + "', subtract = '" + data["subtract"] 
                + "', stock_status_id = '" + data["stock_status_id"] 
                + "', date_available = NOW()"
                + ", manufacturer_id = '" + data["manufacturer_id"]
                + "', shipping = '" + data["shipping"] 
                + "', price = '" + data["price"] 
                + "', points = '" + data["points"] 
                + "', weight = '" + data["weight"] 
                + "', weight_class_id = '" + data["weight_class_id"] 
                + "', length = '" + data["length"] 
                + "', width = '" + data["width"]
                + "', image = '" + data["image"]
                + "', height = '" + data["height"] 
                + "', length_class_id = '" + data["length_class_id"] 
                + "', status = '" + data["status"] 
                + "', tax_class_id = '" + data["tax_class_id"]
                + "', sort_order = '" + data["sort_order"] 
                + "', date_added = NOW(), date_modified = NOW()",true);


            Dictionary<string, string>[] xtraImages = data["product_image"];
            foreach( Dictionary<string, string> xtraImage in xtraImages)
                if (xtraImage != null)
                {
                    InsertRecord("INSERT INTO " + ImportConfig.DB_PREFIX + "product_image SET product_id = '" + product_id
                        + "', image = '" + xtraImage["image"] + "', sort_order = '" + xtraImage["sort_order"] + "'", false);
                }
            Dictionary<string, dynamic> product_description = data["product_description"];
            foreach (string key in product_description.Keys)
            {
                InsertRecord("INSERT INTO " + ImportConfig.DB_PREFIX + "product_description SET product_id = '" + product_id + "', language_id = '" + key
                    + "', name = '" + product_description[key]["name"].Replace("'", "''")
                    + "', meta_keyword = '" + product_description[key]["meta_keyword"]
                    + "', meta_title = '" + product_description[key]["meta_title"].Replace("'", "''") 
                    + "', meta_description = '"  + product_description[key]["meta_description"]
                    +  "', description = '" +  product_description[key]["description"].Replace("'","''")
                    + "', tag = '" + product_description[key]["tag"]  + "'",false);
            }

            foreach (string key in data["product_store"])
            {
                InsertRecord("INSERT INTO " + ImportConfig.DB_PREFIX + "product_to_store SET product_id = '"  + product_id + "', store_id = '" + key + "'",false);
            }
            
            if (data["keyword"]!="")
                InsertRecord("INSERT INTO " + ImportConfig.DB_PREFIX + "url_alias SET query = 'product_id=" + product_id + "', keyword = '" + data["keyword"] + "'",false);

            foreach (int related_id in data["product_related"])
            {
                Execute("DELETE FROM " + ImportConfig.DB_PREFIX + "product_related WHERE product_id = '" + product_id + "' AND related_id = '" + related_id + "'");
				InsertRecord("INSERT INTO " + ImportConfig.DB_PREFIX + "product_related SET product_id = '"  + product_id +"', related_id = '" + related_id + "'",false);
                Execute("DELETE FROM " + ImportConfig.DB_PREFIX + "product_related WHERE product_id = '" + related_id + "' AND related_id = '" + product_id + "'");
                InsertRecord("INSERT INTO " + ImportConfig.DB_PREFIX + "product_related SET product_id = '" + related_id + "', related_id = '" + product_id + "'", false);
            }

            List<Dictionary<string, dynamic>> option_data = data["product_option"];
            int product_option_id;
            foreach (Dictionary<string, dynamic> product_option in option_data)
            {
                product_option_id = InsertRecord("INSERT INTO " + ImportConfig.DB_PREFIX + "product_option SET product_id = '" + product_id +
                    "', option_id = '" + product_option["option_id"] + "', required = '" + product_option["required"] + "'",true);
                foreach (Dictionary<string,dynamic> product_option_value in product_option["product_option_value"])
                {
                    InsertRecord("INSERT INTO "  + ImportConfig.DB_PREFIX + "product_option_value SET product_option_id = '"  + product_option_id 
                        + "', product_id = '"  + product_id  
                        +  "', option_id = '" + product_option["option_id"] 
                        + "', option_value_id = '" + product_option_value["option_value_id"] 
                        +  "', quantity = '" + product_option_value["quantity"] 
                        + "', subtract = '" + product_option_value["subtract"] 
                        + "', price = '" + product_option_value["price"] 
                        +  "', price_prefix = '" + product_option_value["price_prefix"] 
                        + "', points = '"  + product_option_value["points"] 
                        + "', points_prefix = '" + product_option_value["points_prefix"] 
                        + "', weight = '"  + product_option_value["weight"]
                        + "', weight_prefix = '" + product_option_value["weight_prefix"] + "'", false);
                }
            }
            return product_id;
        }

        public int getProductByRefField(string reffield) 
        {		
		    return GetValue("SELECT product_id FROM " + ImportConfig.DB_PREFIX + "product WHERE reffield='" + reffield + "'");		    
	    }

        
	    public int getOption(string name) {
		    return GetValue("SELECT o.option_id FROM `" + ImportConfig.DB_PREFIX + "option` o INNER JOIN " + ImportConfig.DB_PREFIX + "option_description od "
			    + " ON o.option_id = od.option_id WHERE name='"  + name + "'");	
	    }

        public int getOptionValue (int option_id, string name) {
		    return GetValue("SELECT ov.option_value_id FROM " + ImportConfig.DB_PREFIX + "option_value ov INNER JOIN " + ImportConfig.DB_PREFIX + "option_value_description ovd "
                + " ON ov.option_value_id = ovd.option_value_id WHERE name='" + name.Replace("'", "''") + "' AND ov.option_id='" + option_id + "'");
	    }

        public int addOptionValue (int option_id,Dictionary<string, string> name_array) {
		    int lastId  = InsertRecord("INSERT INTO " + ImportConfig.DB_PREFIX + "option_value SET option_id='" + option_id + "'",true);
		    
            foreach(string key in name_array.Keys) {
                InsertRecord("INSERT INTO " + ImportConfig.DB_PREFIX + "option_value_description SET option_value_id='" + lastId + "',language_id='" + key + "', option_id='" + option_id + "', name='" + name_array[key].Replace("'", "''") + "'", false);
		    }		
		    Execute("UPDATE " + ImportConfig.DB_PREFIX + "option_value SET sort_order='" + lastId + "' WHERE option_value_id='"  + lastId +"'");
		    return lastId;
	    }
    }
}
