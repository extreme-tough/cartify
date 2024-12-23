using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Aiplib;

namespace Aiplib
{

    public class ProfileConfig
    {
        IniFile configFile;
        private string _IMPORT_TO_CATEGORY="(Default)";
        private string _USERNAME = "";
        private string _PASSWORD = "";
        private bool _UPDATE_IN_LOOP;
        public ProfileConfig(string profileFile)
        {
            configFile = new IniFile(profileFile);
            _IMPORT_TO_CATEGORY = configFile.IniReadValue("profile", "IMPORT_TO_CATEGORY");
            _USERNAME = configFile.IniReadValue("profile", "USERNAME");
            _PASSWORD = configFile.IniReadValue("profile", "PASSWORD");
            _UPDATE_IN_LOOP = configFile.IniReadValue("profile", "_UPDATE_IN_LOOP").Trim() == "" ? false : bool.Parse( configFile.IniReadValue("profile", "_UPDATE_IN_LOOP"));
        }

        public string NAME
        {
            get
            {
                return configFile.IniReadValue("profile", "NAME");
            }
        }

        public string HOST
        {
            get
            {
                return configFile.IniReadValue("profile", "HOST");
            }
        }

        public string HOME_PAGE
        {
            get
            {
                return configFile.IniReadValue("profile", "HOME_PAGE");
            }
        }
        public int START_PAGE
        {
            get
            {
                return int.Parse(configFile.IniReadValue("profile", "START_PAGE"));
            }
        }
        public int PAGE_INCREMENTER
        {
            get
            {
                return int.Parse(configFile.IniReadValue("profile", "PAGE_INCREMENTER"));
            }
        }
        public float STOCK_MULTIPLIER
        {
            get
            {
                return int.Parse(configFile.IniReadValue("profile", "STOCK_MULTIPLIER"));
            }
        }
        public float PRICE_MULTIPLIER
        {            
            get
            {
                return float.Parse(configFile.IniReadValue("profile", "PRICE_MULTIPLIER"));
            }
        }
        public int DEFAULT_STOCK
        {
            get
            {
                return int.Parse(configFile.IniReadValue("profile", "DEFAULT_STOCK"));
            }
        }
        public string OUTOFSTOCK_STATUS
        {
            get
            {
                return configFile.IniReadValue("profile", "OUTOFSTOCK_STATUS");
            }
        }

        public string LOCAL_IMAGE_PATH
        {
            get
            {
                return configFile.IniReadValue("profile", "LOCAL_IMAGE_PATH");
            }
        }
        public string SERVER_VERSION
        {
            get
            {
                return configFile.IniReadValue("profile", "SERVER_VERSION");
            }
        }

        public string FEED_TYPE
        {
            get
            {
                return configFile.IniReadValue("profile", "FEED_TYPE");
            }
        }
        public int HEADER_ROWS
        {
            get
            {
                return int.Parse( configFile.IniReadValue("profile", "HEADER_ROWS"));
            }
        }
        public string SUBTRACT_STOCK
        {
            get
            {
                return configFile.IniReadValue("profile", "SUBTRACT_STOCK");
            }
        }
        public int LANGUAGE_ID
        {
            get
            {
                return int.Parse("0" + configFile.IniReadValue("profile", "LANGUAGE_ID"));
            }
        }
        public string DB_PREFIX
        {
            get
            {
                return configFile.IniReadValue("profile", "DB_PREFIX");
            }
        }
        public string TAX_CLASS
        {
            get
            {
                return configFile.IniReadValue("profile", "TAX_CLASS");
            }
        }
        public int CATEGORY_ROOT
        {
            get
            {
                return int.Parse(configFile.IniReadValue("profile", "CATEGORY_ROOT"));
            }
        }
        public string LANGUAGES
        {
            get
            {
                return configFile.IniReadValue("profile", "LANGUAGES");
            }
        }
        public string GetOptionName(string OptionKey)
        {
            return configFile.IniReadValue("profile", OptionKey);
        }
        public string OPTION_TYPE
        {
            get
            {
                return configFile.IniReadValue("profile", "OPTION_TYPE");
            }
        }
        public string CUSTOMER_GROUPS
        {
            get
            {
                return configFile.IniReadValue("profile", "CUSTOMER_GROUPS");
            }
        }
        public bool SEO_URL
        {
            get
            {
                if (configFile.IniReadValue("profile", "SEO_URL") == "0")
                    return false;
                else
                    return true;
            }
        }
        public string ATTRIBUTE_GROUP_ID
        {
            get
            {
                return configFile.IniReadValue("profile", "ATTRIBUTE_GROUP_ID");
            }
        }
        public bool DOWNLOAD_ENABLED
        {
            get
            {
                if (configFile.IniReadValue("profile", "DOWNLOAD_ENABLED") == "0")
                    return false;
                else
                    return true;
            }
        }
        public bool SKIP_EMPTY_TITLE
        {
            get
            {
                if (configFile.IniReadValue("profile", "SKIP_EMPTY_TITLE") == "0")
                    return false;
                else
                    return true;
            }
        }
        public bool SKIP_EMPTY_IMAGE
        {
            get
            {
                if (configFile.IniReadValue("profile", "SKIP_EMPTY_IMAGE") == "0")
                    return false;
                else
                    return true;
            }
        }
        public bool SKIP_EMPTY_MODEL
        {
            get
            {
                if (configFile.IniReadValue("profile", "SKIP_EMPTY_MODEL") == "0")
                    return false;
                else
                    return true;
            }
        }
        public string CURRENCY
        {
            get
            {
                return configFile.IniReadValue("profile", "CURRENCY");
            }
        }
        public bool PRICEPREFIX
        {
            get
            {
                if (configFile.IniReadValue("profile", "PRICEPREFIX") == "0")
                    return false;
                else
                    return true;
            }
        }
        public string DEFAULT_IMPORT_TYPE
        {
            get
            {
                return configFile.IniReadValue("profile", "DEFAULT_IMPORT_TYPE");
            }
        }
        public bool SEND_HEADERS
        {
            get
            {
                if (configFile.IniReadValue("profile", "SEND_HEADERS") == "0")
                    return false;
                else
                    return true;
            }
        }
        public string SHEET_TO_PROCESS
        {
            get
            {
                return configFile.IniReadValue("profile", "SHEET_TO_PROCESS");
            }
        }
        public string WEIGHT_CLASS_ID
        {
            get
            {
                return configFile.IniReadValue("profile", "WEIGHT_CLASS_ID");
            }
        }

        public string LENGTH_CLASS_ID
        {
            get
            {
                return configFile.IniReadValue("profile", "LENGTH_CLASS_ID");
            }
        }

        public string DECIMAL_SEPARATOR
        {
            get
            {
                return configFile.IniReadValue("profile", "DECIMAL_SEPARATOR");
            }
        }
        public string GROUPED_ROWS
        {
            get
            {
                return configFile.IniReadValue("profile", "GROUPED_ROWS");
            }
        }
        public string GROUP_COLUMN
        {
            get
            {
                return configFile.IniReadValue("profile", "GROUP_COLUMN");
            }
        }
        public string DOWNLOAD_COMPONENT
        {
            get
            {
                return configFile.IniReadValue("profile", "DOWNLOAD_COMPONENT");
            }
        }

        public string LOGIN_URL
        {
            get
            {
                return configFile.IniReadValue("profile", "LOGIN_URL");
            }
        }


        public string USERNAME
        {
            get
            {
                return _USERNAME;
            }

            set
            {
                _USERNAME = value;                
            }
        }

        public string PASSWORD
        {
            get
            {
                return _PASSWORD;
            }

            set
            {
                _PASSWORD = value;                
            }
        }

        public bool RESUME_TRIM_URL_PARAMETERS
        {
            get
            {
                if (configFile.IniReadValue("profile", "RESUME_TRIM_URL_PARAMETERS") == "0")
                    return false;
                else
                    return true;
            }
        }

        public string EDIT_EXCLUDE
        {
            get
            {
                return configFile.IniReadValue("profile", "EDIT_EXCLUDE");
            }
        }

        public bool SELECTED_FIELD_UPDATE
        {
            get
            {
                if (configFile.IniReadValue("profile", "SELECTED_FIELD_UPDATE") == "0")
                    return false;
                else
                    return true;
            }
        }
        public string SELECTED_FIELD_LIST
        {
            get
            {
                return configFile.IniReadValue("profile", "SELECTED_FIELD_LIST");
            }
        }

        public string FIELD_DEFAULT_VALUES
        {
            get
            {
                return configFile.IniReadValue("profile", "FIELD_DEFAULT_VALUES");
            }
        }

        public string QUICK_UPDATE_FIELDS
        {
            get
            {
                return configFile.IniReadValue("profile", "QUICK_UPDATE_FIELDS");
            }
        }

        public string KEY_FIELD
        {
            get
            {
                return configFile.IniReadValue("profile", "KEY_FIELD");
            }
        }
        public string XLS_KEY_COLUMN
        {
            get
            {
                return configFile.IniReadValue("profile", "XLS_KEY_COLUMN");
            }
        }

        public string FEED_LOCATION
        {
            get
            {
                return configFile.IniReadValue("profile", "FEED_LOCATION");
            }
        }

        public string FEED_DELIMITER
        {
            get
            {
                return configFile.IniReadValue("profile", "FEED_DELIMITER");
            }
        }

        public bool USE_EQUAL_OPTION
        {
            get
            {
                if (configFile.IniReadValue("profile", "USE_EQUAL_OPTION") == "0")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public string IMPORT_TO_CATEGORY
        {
            get
            {                
                return _IMPORT_TO_CATEGORY;             
            }
            set 
            {
                _IMPORT_TO_CATEGORY = value;                
            }
        }

        public string USER_INPUT_NAME
        {
            get
            {
                return configFile.IniReadValue("profile", "USER_INPUT_NAME");
            }
        }

        public string FEED_URL
        {
            get
            {
                return configFile.IniReadValue("profile", "FEED_URL");
            }
        }
        public string PASSWORD_INPUT_NAME
        {
            get
            {
                return configFile.IniReadValue("profile", "PASSWORD_INPUT_NAME");
            }
        }

        public string LOGIN_BUTTON
        {
            get
            {
                return configFile.IniReadValue("profile", "LOGIN_BUTTON");
            }
        }
        public string LOGGEDIN_URL
        {
            get
            {
                return configFile.IniReadValue("profile", "LOGGEDIN_URL");
            }
        }
        public int USER_INPUT_INDEX
        {
            get
            {
                return int.Parse( configFile.IniReadValue("profile", "USER_INPUT_INDEX"));
            }
        }
        public string USER_INPUT_METHOD
        {
            get
            {
                return configFile.IniReadValue("profile", "USER_INPUT_METHOD");
            }
        }
        public int PASSWORD_INPUT_INDEX
        {
            get
            {
                return int.Parse(configFile.IniReadValue("profile", "PASSWORD_INPUT_INDEX"));
            }
        }
        public string PASSWORD_INPUT_METHOD
        {
            get
            {
                return configFile.IniReadValue("profile", "PASSWORD_INPUT_METHOD");
            }
        }

        public string POST_INSERT_QUERY
        {
            get
            {
                return configFile.IniReadValue("profile", "POST_INSERT_QUERY");
            }
        }

        public bool IMAGE_REMOVE_QUERYSTRING
        {
            get
            {
                if (configFile.IniReadValue("profile", "IMAGE_REMOVE_QUERYSTRING") == "0")
                    return false;
                else
                    return true;
            }
        }

        public bool DELETE_NOSTOCK_PRODUCTS
        {
            get
            {
                if (configFile.IniReadValue("profile", "DELETE_NOSTOCK_PRODUCTS") == "0")
                    return false;
                else
                    return true;
            }
        }

        public bool DELETE_404_PRODUCTS
        {
            get
            {
                if (configFile.IniReadValue("profile", "DELETE_404_PRODUCTS") == "0")
                    return false;
                else
                    return true;
            }
        }

        public bool UPDATE_IN_LOOP
        {
            get
            {
                return _UPDATE_IN_LOOP;
            }

            set
            {
                _UPDATE_IN_LOOP = value;
            }
        }

        public string CONNECTION_STRING
        {
            get
            {
                return configFile.IniReadValue("profile", "CONNECTION_STRING");
            }
        }

        public string SERVER_APPLICATION
        {
            get
            {
                string serverApp = configFile.IniReadValue("profile", "SERVER_APPLICATION");
                return serverApp != "" ? serverApp : "Opencart";
            }
        }
        public string SERVER_FTP_HOSTNAME
        {
            get
            {
                return configFile.IniReadValue("profile", "SERVER_FTP_HOSTNAME");
            }
        }
        public string SERVER_FTP_USER
        {
            get
            {
                return configFile.IniReadValue("profile", "SERVER_FTP_USER");
            }
        }
        public string SERVER_FTP_PASSWORD
        {
            get
            {
                return configFile.IniReadValue("profile", "SERVER_FTP_PASSWORD");
            }
        }
        public string SERVER_FTP_PATH
        {
            get
            {
                return configFile.IniReadValue("profile", "SERVER_FTP_PATH");
            }
        }
        public string SERVER_FTP_PARAM
        {
            get
            {
                return configFile.IniReadValue("profile", "SERVER_FTP_PARAM");
            }
        }

        public string STORE_URL
        {
            get
            {
                return configFile.IniReadValue("profile", "STORE_URL");
            }
        }
        public string SERVER_IMAGE_PATH
        {
            get
            {
                return configFile.IniReadValue("profile", "SERVER_IMAGE_PATH");
            }
        }
        public string DELELE_LOCAL
        {
            get
            {
                return configFile.IniReadValue("profile", "DELELE_LOCAL");
            }
        }
        public string USER_AGENT
        {
            get
            {
                return configFile.IniReadValue("profile", "USER_AGENT");
            }
        }
        public string SIMULATE
        {
            get
            {
                return configFile.IniReadValue("profile", "SIMULATE");
            }
        }
        public int MAX_PRODUCTS_PER_CATEGORY
        {
            get
            {
                return int.Parse("0" + configFile.IniReadValue("profile", "MAX_PRODUCTS_PER_CATEGORY"));
            }
        }
        public int MAX_DOWNLOAD_TRIES
        {
            get
            {
                return int.Parse("0" + configFile.IniReadValue("profile", "MAX_DOWNLOAD_TRIES"));
            }
        }
        public int COMMAND_TIMEOUT
        {
            get
            {
                return int.Parse("0" + configFile.IniReadValue("profile", "COMMAND_TIMEOUT"));
            }
        }

        public string VERBOSE_LOG
        {
            get
            {
                return configFile.IniReadValue("profile", "VERBOSE_LOG");
            }
        }
        public void SaveSettings()
        {
            configFile.IniWriteValue("profile", "PASSWORD", _PASSWORD);
            configFile.IniWriteValue("profile", "USERNAME", _USERNAME);
            configFile.IniWriteValue("profile", "IMPORT_TO_CATEGORY", _IMPORT_TO_CATEGORY);
            configFile.IniWriteValue("profile", "_UPDATE_IN_LOOP", _UPDATE_IN_LOOP.ToString());
        }



    }
}
