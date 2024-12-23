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
using System.Data.SqlClient;

namespace Cartify
{
    public abstract class DbWrapper
    {
        private MySqlConnection sqlConn;
        private string connStr;
        private bool isConnected;
        public string DB_PREFIX ;
        public int LANGUAGE_ID = 1;
        public int CommandTimeout = 2147480;
        public string Version;

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

        }

        public void Open(string ConnectionString)
        {
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
            
            MySqlCommand command = new MySqlCommand("set net_write_timeout=99999; set net_read_timeout=99999", this.sqlConn);
            command.ExecuteNonQuery();
            command.Dispose();  //added by Rathika
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

        protected int InsertRecord(string query, bool returnKey)
        {
            object oResult;
            //added by Rathika
            //this.sqlConn.Close();
            if (this.sqlConn.State == ConnectionState.Closed)
            {
                this.sqlConn.Open();
            }
            ////////////////            
            MySqlCommand command = new MySqlCommand(query, this.sqlConn);
            command.CommandTimeout = CommandTimeout;
            int i = 0;
            while (true)
            {
                try
                {
                    command.ExecuteNonQuery();
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Connection must be valid and open.")
                        sqlConn.Open();
                    i++;
                    if (i > 10)
                        throw ex;
                    continue;
                }
            }
            if (returnKey)
            {
                command.CommandText = "SELECT last_insert_id()";
                oResult = command.ExecuteScalar();
                return Convert.ToInt32(oResult);
            }
            else
                return 0;
        }

        public void Execute(string query)
        {
            if (query == "") return;
            MySqlCommand command = new MySqlCommand(query, this.sqlConn);
            command.CommandTimeout = CommandTimeout;
            command.ExecuteNonQuery();
            command.Dispose();  //added by Rathika
        }

        protected int GetValue(string query)
        {
            object oResult;
            //this.sqlConn.Close();
            //added by Rathika
            if (this.sqlConn.State == ConnectionState.Closed)
            {
                this.sqlConn.Open();
            }
            /////////////
            MySqlCommand command = new MySqlCommand(query, this.sqlConn);
            command.CommandTimeout = CommandTimeout;
            try
            {
                oResult = command.ExecuteScalar();
                return Convert.ToInt32(oResult);

            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }
        }

        protected DataTable GetTable(string query)
        {
            //added by Rathika
            //this.sqlConn.Close();
            if (this.sqlConn.State == ConnectionState.Closed)
            {
                this.sqlConn.Open();
            }
            ///////
            MySqlCommand command = new MySqlCommand();//("set net_write_timeout=9999999999; set net_read_timeout=9999999999", this.sqlConn);
            //command.ExecuteNonQuery();
            command.CommandText = query;
            command.Connection = this.sqlConn;
            command.CommandTimeout = CommandTimeout;
            try
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter adp = new MySqlDataAdapter(command);
                int i = 0;
                while (i < 5)
                {
                    i++;
                    try
                    {
                        adp.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Fatal"))
                            continue;
                    }
                    break;
                }

                return dt;

            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not add user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }
        }

        protected DataRow GetRecord(string query)
        {
            //added by Rathika
            //this.sqlConn.Close();
            if (this.sqlConn.State == ConnectionState.Closed)
            {
                this.sqlConn.Open();
            }
            ///////
            MySqlCommand command = new MySqlCommand();//("set net_write_timeout=9999999999; set net_read_timeout=9999999999", this.sqlConn);
            //command.ExecuteNonQuery();
            command.Connection = this.sqlConn;

            command.CommandText = query;            
            command.CommandTimeout = CommandTimeout;
            try
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter adp = new MySqlDataAdapter(command);
                int i = 0;
                while (i < 5)
                {
                    i++;
                    try
                    {
                        adp.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Fatal"))
                            continue;
                    }
                    break;
                }
                if (dt.Rows.Count > 0)
                    return dt.Rows[0];
                else
                    return null;

            }
            catch (Exception excp)
            {
                Exception myExcp = new Exception("Could not add user. Error: " +
                    excp.Message, excp);
                throw (myExcp);
            }
        }


        public abstract void DBInit();             

        public abstract DataTable getImportedProducts(string name, string model, string price, string quantity, string status, string category_id, string brand_id, string source,string lastID);

        public abstract void DeleteProduct(int product_id);

        public abstract DataRow getAdminUser();

        public abstract int getManufacturer(string manufacturer);

        public abstract DataTable getManufacturers();

        public abstract int addManufacturer(Dictionary<string, dynamic> data);

        public abstract int getProductByModel(string model);

        public abstract int getTotalImportedProducts(string source);

        public abstract int addProduct(Dictionary<string, dynamic> data);

        public abstract int editProduct(int product_id, Dictionary<string, dynamic> data, string ExcludeList);
        public abstract int quickUpdate(int product_id, Dictionary<string, dynamic> data);
        public abstract void updateSelectedFields(Dictionary<string, string> fieldCollection, int product_id, Dictionary<string, dynamic> data);
        public abstract void updateAdditionalFields(int product_id, Dictionary<string, string> data);
        public abstract int getProductByRefField(string reffield);
        public abstract int getOption(string name);
        public abstract int getOptionValue(int option_id, string name);
        public abstract int addOption(string[] name, string[] languages);
        public abstract int addOptionValue(int option_id, string[] optionValueName, string[] languages);
        public abstract DataTable getCategories();
        public abstract DataTable getCategoriesProper();
        public abstract DataTable getAttributes(string attribute_group_id);
        public abstract int addCategory(int parentId, int top, string keyword, Dictionary<string, Dictionary<string, string>> name_array,string fullPath="");

        public abstract int addAttribute(string attribute_group_id, int sort_order, Dictionary<string, string> attItem);
    }
}
