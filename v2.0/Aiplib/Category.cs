using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiplib
{
    public class Category
    {
        private string l_URL;
        private Hashtable URLList;

        public string URL
        {
            get { return l_URL; }
        }

        private string l_CategoryPath;

        public string CategoryPath
        {
            get { return l_CategoryPath; }
        }

        public Category(){
            URLList = new Hashtable();
        }
        public void Add(string CategoryPath,string URL)
        {
            if (!URLList.ContainsKey(CategoryPath)){
                l_URL = URL;
                l_CategoryPath = CategoryPath;
            }
        }
    }

    public class TargetCategory
    {
        private string l_categoryID;
        private string l_categoryPath;

        public string CategoryPath
        {
            get { return l_categoryPath; }
            set { l_categoryPath = value; }
        }

        public string CategoryID
        {
            get { return l_categoryID; }
            set { l_categoryID = value; }
        }

        public override string ToString()
        {
            return l_categoryPath;
        }
    }
}
