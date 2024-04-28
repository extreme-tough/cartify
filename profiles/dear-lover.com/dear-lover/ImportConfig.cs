using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AllImporterPro
{
    public static class ImportConfig
    {
        public const int START_PAGE = 1;
        public const int PAGE_INCREMENTER  = 1;
        public const int DEFAULT_STOCK = 125;
        public const string OUTOFSTOCK_STATUS = "5";
        public const string OPTION_NAME = "Size";
        public const float PRICE_MULTIPLIER = 0;
        public const float STOCK_MULTIPLIER = (float) 0.7;
        public const string DB_PREFIX = "";
        public const string SUBTRACT_STOCK = "1";
        public const string  TAX_CLASS = "0";
        public const string LANGUAGE_ID = "1";
        public const bool DOWNLOAD_ENABLED = false;     
        public const string  OPTION_TYPE = "select";
    }
}
