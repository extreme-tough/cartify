using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Aiplib;
namespace ParserFactory
{
    public static class Config
    {

        public static string ReadValue(string profile,string key)
        {
            IniFile ProfileConfigFile;
            ProfileConfigFile = new IniFile( profile + ".ini");
            string retValue = ProfileConfigFile.IniReadValue("profile", key);
            return retValue;
        }
    }

}
