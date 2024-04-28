using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using HtmlAgilityPack;
using Microsoft.Win32;
namespace Aiplib
{
    public static class Functions
    {

        public static string GetRegValue(string Profile, string RegKey)
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Cartify\\" + Profile))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue(RegKey);
                        if (o != null)
                        {
                            return o.ToString();
                            //do what you like with version
                        }
                    }
                }
            }
            catch (Exception )  //just for demonstration...it's always best to handle specific exceptions
            {
                return "";
            }
            return "";
        }


        public static void SetRegValue(string Profile, string RegKey, string value)
        {

            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Cartify\\" + Profile, true);

                if (key != null)
                {

                    key.SetValue(RegKey, value);
                }
                else
                {
                    RegistryKey subkey = Registry.LocalMachine.CreateSubKey("Software\\Cartify\\" + Profile);
                    subkey.SetValue(RegKey, value);
                }

            }
            catch (Exception)  //just for demonstration...it's always best to handle specific exceptions
            {
            }

        }

        public static string StripHtmlTags(string html)
        {
            if (String.IsNullOrEmpty(html)) return "";
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return HttpUtility.HtmlDecode(doc.DocumentNode.InnerText);
        }


        public static string DownloadFile(string url)
        {
            WebClient client;
            client = new System.Net.WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:43.0) Gecko/20100101 Firefox/43.0");
            string host;
            Uri linkToUse = new Uri(url);
            host = linkToUse.Host;
            client.Headers.Add("Host", host);
            client.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");                        
            return client.DownloadString(url); 
        }

    }
}
