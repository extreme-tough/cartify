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
using EntityLib;

namespace laroygroup.com
{
    public class Importer : Parser
    {
        HAP.HtmlNodeCollection Nodes;
        HAP.HtmlNode aNode;
        HtmlAgilityPack.HtmlNode root;
        string itemURL, tempURL, catPath;
        dynamic jsonReturn;
        string nextCatURL = "";
        double first_price = 0;
        string MainImage, optionTitle, optionHeading;
        List<string> optionAttruibuteNames, optionAttruibuteLabels;
        Dictionary<string, string> propertyCollection = new Dictionary<string, string>();
        Dictionary<string, string> dataCollection = new Dictionary<string, string>();
        Dictionary<string, Dictionary<string, float>> option_list;
        string price, description, SKU, ISBN, rISBN, MPN, Location, Construction, Origin, refField;
        HAP.HtmlDocument doc;
        WebBrowser thisBrowser;
        string currentCat;
        List<string> related;
        string Title, Model, rModel;
        int TotalStock;
        int packSize;

        string[] lookup;
        string[] urlParts;
        List<string> optionData;
        Dictionary<string, List<string>> postOptionCmd;
        Dictionary<string, Dictionary<string, float>> options;
        string colorCode;
        Dictionary<string, string> abbrevs = new Dictionary<string, string>();
        public Importer()
        {

        }
        public string URL
        {
            set
            {
                itemURL = value;
                urlParts = itemURL.Split(new string[] { "#" }, StringSplitOptions.None);
                if (urlParts.Length > 1)
                    colorCode = urlParts[1];
                else
                    colorCode = "";
            }
        }

        public string CategoryPath
        {
            set
            {
                catPath = catPath;
            }
        }

        public string FeedFile
        {
            set
            {
            }
        }

        public Dictionary<string, string> PropertyBag
        {
            set
            {
                propertyCollection = value;
            }
        }

        public HtmlAgilityPack.HtmlNode Document
        {
            set
            {
                this.root = value;
            }
        }

        public WebBrowser Browser
        {
            set
            {
                this.thisBrowser = value;
            }
        }
        public void SetFeedData(string[] data, bool LastLine)
        {

        }

        public void SetFeedLines(List<string[]> data, bool LastLine)
        {


        }
        public void SetFeedData(Dictionary<string, string> data, bool LastLine)
        {

        }

        public void SetFeedData(DataSet ds)
        {

        }

        public DataSet getAdditionalTableData()
        {
            return null;
        }

        public void parseFeedLine()
        {

        }
        public List<Category> getCategoryURLs()
        {
            string className, catLink;
            Category objCategory = new Category();
            List<Category> urls = new List<Category>();            
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hokken-kennels-benches/hokken"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hokken-kennels-benches/kennels"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hokken-kennels-benches/benches"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/onderweg-buitenleven/automateriaal"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/onderweg-buitenleven/fietsmanden-karren"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/onderweg-buitenleven/transporters"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/onderweg-buitenleven/safety-reflecting"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/onderweg-buitenleven/hondensport"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/onderweg-buitenleven/draagtassen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/onderweg-buitenleven/anti-trektuigen-muilbanden"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/onderweg-buitenleven/andere"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/manden-kussens-dekens/ovalen-kussens"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/manden-kussens-dekens/rechthoekige-kussens"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/manden-kussens-dekens/matten-dekens"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/manden-kussens-dekens/manden"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/manden-kussens-dekens/iglo-s-en-huisjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/manden-kussens-dekens/luxe-manden-kussens"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hondenkleding-mode/jasjes-sweaters-t-shirts"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hondenkleding-mode/beschermende-kledij"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/broekjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/plasdoeken-toiletten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/anti-trektuigen-muilbanden"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/oog-oor-tandverzorging"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/reinigingsmiddelen-ontgeurders-en-reppers"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/poepzakjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/kammen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/borstels"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/tondeuses"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/scharen-messen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/shampoo-parfums"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/anti-insecten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hygiëne-verzorging/andere"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/eetpotten-drinkfonteinen-voederreservoirs/eetpotten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/eetpotten-drinkfonteinen-voederreservoirs/drinkfonteinen-voederreservoirs"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/speelgoed-puzzels/latex-vinyl-rubber-speeltjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/speelgoed-puzzels/pluche-knuffel-speeltjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/speelgoed-puzzels/snack-educatief-speelgoed"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/speelgoed-puzzels/apporteer-speelgoed"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/speelgoed-puzzels/kauw-trek-speelgoed"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/speelgoed-puzzels/kong"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/snacks-beloningen/kauwbotten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/snacks-beloningen/gevulde-snacks"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/snacks-beloningen/vleessnacks"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/snacks-beloningen/halfzachte-snacks"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/snacks-beloningen/biscuits"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hals-leibanden/trainingslijnen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hals-leibanden/kunst-leder-hals-leibanden"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hals-leibanden/strop-hals-leibanden"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hals-leibanden/nylon-basic-hals-leibanden"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hals-leibanden/nylon-premium-hals-leibanden"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hals-leibanden/safety-reflecting"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hals-leibanden/puppy-en-hondentuigen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hals-leibanden/ketting-hals-en-leibandens"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/hals-leibanden/flexi-rollijnen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/voeding/eukanuba/puppy-junior"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/voeding/eukanuba/adult"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/voeding/eukanuba/mature-senior"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/voeding/eukanuba/daily-care"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/voeding/eukanuba/breed-specific"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/voeding/eukanuba-evd"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/voeding/eukanuba-breeder"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/voeding/purina-treats"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/voeding/deco"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/hond/voeding/aanvullende-voeding"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/kattenbakken-kattenbakvulling/kattenbakken"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/kattenbakken-kattenbakvulling/kattenbakvulling"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/kattenbakken-kattenbakvulling/filters-en-ontgeurders"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/kattenbakken-kattenbakvulling/schepjes-zakken"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/krabpalen-meubels/krabpaal-<100cm"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/krabpalen-meubels/krabpaal->100cm"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/krabpalen-meubels/luxe-plafond-krabpalen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/krabpalen-meubels/krabtonnen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/krabpalen-meubels/krabstammen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/krabpalen-meubels/krabplanken-krabmatten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/krabpalen-meubels/krabpaal-onderdelen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/eetpotten-drinkfonteinen-voederreservoirs/eetpotten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/eetpotten-drinkfonteinen-voederreservoirs/drinkfonteinen-voederreservoirs"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/hygiëne-verzorging/kammen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/hygiëne-verzorging/borstels"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/hygiëne-verzorging/reinigingsmiddelen-ontgeurders"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/hygiëne-verzorging/andere"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/manden-kussentjes-huisjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/kattenluikjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/speelgoed/muizen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/speelgoed/balletjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/speelgoed/natural"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/speelgoed/actie"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/speelgoed/andere"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/speelgoed/kong"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/snacks/semi-moist"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/snacks/droge-snacks"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/halsbanden-en-harnassen/halsbanden"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/halsbanden-en-harnassen/harnassen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/voeding/eukanuba"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/voeding/iams"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/voeding/eukanuba-evd"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/voeding/friskies"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/voeding/deco"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/kat/vervoer"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/hokken-parken/buiten-hokken"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/hokken-parken/binnen-hokken"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/voeding-snacks/hoofdvoeding"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/voeding-snacks/hooi"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/voeding-snacks/sticks"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/voeding-snacks/snacks-met-kruiden-fruit-groenten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/bodembedekking"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/verzorging-hygiëne/chinchillazand"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/verzorging-hygiëne/vitamines-voedingssuplementen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/verzorging-hygiëne/ontgeurders-toiletten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/verzorging-hygiëne/kammen-borstels"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/speelgoed-huisjes/huisjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/speelgoed-huisjes/hamsterballen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/speelgoed-huisjes/speelgoed"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/speelgoed-huisjes/tunnels-hangmatten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/eetpotten-drinkflessen/eetpotten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/eetpotten-drinkflessen/drinkflessen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/transportkooien"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/harnassen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/knaagdier/vallen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/kip/kippenhokken"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/kip/voer/onderhoudsvoeder"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/kip/eetbakken-drinksilos"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/kip/verzorging"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/kip/warmtelampen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/duif/behuizing"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/duif/voer/granen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/duif/voer/onderhoudsvoeder"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/duif/vitamines-mineralen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/duif/verzorging"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/duif/eetbakken-drinksilos"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/duif/nestmateriaal"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/duif/transportmand"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/winter/wintervoer"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/winter/winterhuisjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/winter/eetbakken-drinksilos"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/winter/nestkastjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/kooien-volières/kooien"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/kooien-volières/volières"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/kooien-volières/papegaaikooien"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/kooien-volières/zitstokken"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/voer-snacks/hoofdvoeding"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/voer-snacks/eivoer"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/voer-snacks/zachtvoer"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/voer-snacks/specialiteiten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/voer-snacks/snacks"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/voer-snacks/supplementen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/voer-snacks/enkelvoudige-granen-zaden"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/verzorging-hygiëne-insecticides/vitamines-mineralen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/verzorging-hygiëne-insecticides/insecticide"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/verzorging-hygiëne-insecticides/badhuisjes-schoteltjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/speelgoed"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/eetpotjes-drinkflesjes/eetpotjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/eetpotjes-drinkflesjes/drinkflesjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/bodembedekking"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/kweekmateriaal/nestkastjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/kweekmateriaal/nestmateriaal"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/kweekmateriaal/kweekkooien"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vogels/vangnetjes-ringen-vogeldoosjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/vijver/uv-filters-uv-lampen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/vijver/pompen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/vijver/waterverzorging"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/vijver/voer"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/vijver/andere"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/vijver/medicijnen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/aquaria/viskommen-kleine-aquaria"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/aquaria/aquaria-meubels"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/aquaria/aquarium-onderdelen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/decoratie-bodembedekking/natuurlijke-elementen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/decoratie-bodembedekking/kunstplanten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/decoratie-bodembedekking/kunststof-ornamenten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/decoratie-bodembedekking/led-techniek"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/decoratie-bodembedekking/achterwanden"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/decoratie-bodembedekking/bodembedekking"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/visvoer/koudwatervissen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/visvoer/zoetwatervissen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/visvoer/zeewatervissen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/visvoer/voederautomaten-vakantievoer"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/plantverzorging/co2-gamma"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/plantverzorging/aquascaping"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/filtering/buitenfilters"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/filtering/binnenfilters"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/filtering/filterpatronen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/filtering/filtermaterialen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/filtering/luchtpompfilters"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/filtering/osmose"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verlichting/lichtkappen-en-armaturen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verlichting/lampen-juwel-aquaria"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verlichting/buislampen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verlichting/andere-lampen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verlichting/reflectoren"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verlichting/led-verlichting"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verzorging-onderhoud/waterbereidingsmiddelen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verzorging-onderhoud/algenhulp"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verzorging-onderhoud/zeewater-onderhoud"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verzorging-onderhoud/watertesters"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verzorging-onderhoud/filterbacteriën"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verzorging-onderhoud/medicijnen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verzorging-onderhoud/onderhoud"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verzorging-onderhoud/vuilklokken-en-stofzuigers"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/zuurstof/luchtpompen-luchtstenen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/zuurstof/oxydator"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/zuurstof/onderdelen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verwarming/verwarmers"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verwarming/externe-verwarmers"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/verwarming/thermometers"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/pompen/circulatiepompen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/pompen/opvoerpompen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/pompen/aquariumslang-zuignappen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/schepnetjes-transportzakjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/vissen/onderdelen-andere"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/terraria/terraria"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/terraria/schildpadbakjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/terraria/toebehoren"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/bodembedekking"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/decoratie/kunststof-decoratie"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/decoratie/natuurlijke-decoratie"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/decoratie/kunstplanten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/eet-en-drinkbakjes"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/voer/schildpadden"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/voer/hagedissen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/voer/vitamines-en-supplementen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/voer/voederpincetten"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/verwarming-vochtigheid/warmtelampen-en-lampenhouders"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/verwarming-vochtigheid/bodemverwarming"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/verwarming-vochtigheid/thermometers-thermostaten-en-hygrometers"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/verwarming-vochtigheid/vernevelaars"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/verlichting/spots-dag-en-nachtlampen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/verlichting/tl-lampen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/verzorging/waterbereiding"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/reptielen/verzorging/reinigingsmiddelen"); urls.Add(objCategory);
            objCategory = new Category(); objCategory.Add("", "https://www.laroygroup.com/nl/products/andere/overige"); urls.Add(objCategory);


            return urls;
        }

        public List<int> getCategoryIDs()
        {
            List<int> CatIDs = new List<int>();
            return CatIDs;
        }

        public string buildCategoryURL(string catURL, int page)
        {
            currentCat = catURL;
            if (page > 0)
                return catURL + "?page=" + page;
            else
                return catURL;
        }

        private string DownloadFile(string url)
        {
            string filename;
            WebClient client;
            client = new System.Net.WebClient();
            client.Encoding = Encoding.UTF8;
            filename = System.IO.Path.GetTempFileName();
            //string data = client.DownloadString(url);
            client.DownloadFile(url, filename);

            return filename;
        }

        public List<string> getItemURLs()
        {
            List<string> urls = new List<string>();
            string TopCat = currentCat;

            string href = "";

            HAP.HtmlNodeCollection breadcumbs = root.SelectNodes("//nav[@class='breadcrumb']/a");
            int iter = 1;
            catPath = "";
            if (breadcumbs == null) return urls;
            foreach (HAP.HtmlNode aNode in breadcumbs)
            {
                if (aNode.InnerText != "Home" && aNode.InnerText != "Products")
                    catPath = catPath + aNode.InnerText + @"///";

                if (iter == breadcumbs.Count)
                {
                    catPath = catPath + aNode.NextSibling.NextSibling.NextSibling.InnerText.Trim();
                }
                iter++;
            }

            HAP.HtmlNodeCollection prodLinks = root.SelectNodes("//a[@class='linkedImage']");
            if (prodLinks != null)
            {
                foreach (HAP.HtmlNode aNode in prodLinks)
                {
                    href = "https://www.laroygroup.com" + aNode.GetAttributeValue("href", "");
                    if (aNode.GetAttributeValue("href", "") != "#")
                        if (!urls.Contains(href))
                            urls.Add(href);
                }
            }
            return urls;
        }

        public List<string> getAdditionalItems()
        {
            List<string> retVal = new List<string>();
            return retVal;
        }


        public string[] getTitles()
        {
            HAP.HtmlNode aNode;
            string[] titles = new string[1];
            try
            {
                aNode = root.SelectNodes("//h1")[1];
            }
            catch (Exception ex)
            {
                return titles;
            }
            if (aNode == null)
                return titles;
            Title = aNode.InnerText;

            titles[0] = Title;

            optionAttruibuteNames = new List<string>();
            optionAttruibuteLabels = new List<string>();

            return titles;
        }

        public string getModel()
        {
            rModel = root.SelectNodes("//h1")[1].GetAttributeValue("data-product-id", "");
            return rModel;
        }

        public string getRefField()
        {
            return rModel;
        }

        public string getUPC()
        {
            return "";
        }

        public string getSKU()
        {
            return "";
        }

        public string getMPN()
        {
            return "";
        }

        public string getEAN()
        {
            return "";
        }

        public string getISBN()
        {
            return "";
        }

        public string getJAN()
        {
            return "";
        }

        public string getLocation()
        {
            return "";
        }

        public string getWeight()
        {
            return "0";
        }
        public string getLength()
        {
            return "0";
        }
        public string getWidth()
        {
            return "0";
        }
        public string getHeight()
        {
            return "0";
        }
        public string getStatus()
        {
            return "0";
        }
        public string getPrice()
        {
            first_price = 0;
            options = ScrapOptions();
            price = first_price.ToString();
            return price;
        }

        public Dictionary<string, string> getSpecial()
        {
            Dictionary<string, string> special = new Dictionary<string, string>();
            return special;
        }

        public Dictionary<string, string>[] getSpecialPrices(string[] customerGroups)
        {
            return new Dictionary<string, string>[0];
        }

        public string getManufacturer()
        {
            string brand = root.SelectNodes("//td[@class='desc']")[0].NextSibling.NextSibling.InnerText.Trim();
            return brand;
        }


        public string[] getDescriptions()
        {
            string[] desc = new string[1];
            if (root.SelectSingleNode("//td[@class='fontScalingMobile']") != null)
                desc[0] = root.SelectSingleNode("//td[@class='fontScalingMobile']").InnerText;
            else
                desc[0] = "";
            return desc;
        }

        public string[] getKeywords()
        {
            string[] keywords = new string[0];
            return keywords;
        }
        public string getMainImage()
        {
            aNode = root.SelectNodes("//a[@class= 'fancybox']")[0];
            string src = "https://www.laroygroup.com" + aNode.GetAttributeValue("href", "");
            return src;
        }

        public string[] getOtherImages()
        {
            List<string> OtherImages = new List<string>();
            int i = 0;
            Nodes = root.SelectNodes("//a[@class= 'fancybox']");
            if (Nodes == null) return OtherImages.ToArray();
            foreach (HAP.HtmlNode aNode in Nodes)
            {
                if (i >= 3)
                {
                    string src = "https://www.laroygroup.com" + aNode.GetAttributeValue("href", "");
                    if(!src.EndsWith("/"))
                       OtherImages.Add(src);
                }
                i++;
            }
            return OtherImages.ToArray();
        }

        public List<string[]> getCategoryPath()
        {
            List<string[]> categoryPaths = new List<string[]>();
            string[] catItem = new string[1];
            catItem[0] = catPath;
            categoryPaths.Add(catItem);
            return categoryPaths;
        }


        public string getStock()
        {
            return TotalStock.ToString();
        }

        public string getStockStatus()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string>[] getAttributes()
        {
            Dictionary<string, string>[] attributes = new Dictionary<string, string>[0];
            return attributes;
        }



        public List<string> getRelated()
        {
            return related;
        }

        public Dictionary<string, Dictionary<string, float>> getOptions()
        {
            return options;
        }

        private Dictionary<string, Dictionary<string, float>> ScrapOptions()
        {
            //string[][] option_list = new string[][] { };
            HAP.HtmlNodeCollection tdNodes;
            optionTitle = "";
            Dictionary<string, float> optionItems = new Dictionary<string, float>();


            double optionPrice;
            Nodes = root.SelectNodes("//div[@class='variants clearfix']/table/tbody/tr");
            if (Nodes != null)
            {
                optionItems = new Dictionary<string, float>();
                //Item is not multipack item but product with options
                foreach (HAP.HtmlNode aNode in Nodes)
                {

                    tdNodes = aNode.SelectNodes("td");
                    string optionValue="";
                    string articleID;
                    if (tdNodes.Count == 4)
                    {
                        optionPrice = 0;
                        optionValue = tdNodes[2].InnerText.Trim();
                        articleID = tdNodes[1].SelectSingleNode("small").NextSibling.InnerText;
                    }
                    else if(tdNodes.Count == 3) {
                        optionPrice = 0;
                        optionValue =  tdNodes[1].InnerText.Trim();
                        articleID = tdNodes[0].SelectSingleNode("small").NextSibling.InnerText;
                    }
                    else
                    { 
                        articleID = tdNodes[5].SelectSingleNode("div/span").GetAttributeValue("data-artcd", "");
                        optionValue = tdNodes[2].InnerText.Trim();

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.laroygroup.com/frontend/ajax.php");
                        request.UserAgent = "Mozilla /5.0 (compatible; Rigor/1.0.0; http://rigor.com)";
                        request.Accept = "*/*";
                        request.Host = "www.laroygroup.com";
                        request.Headers.Add("cookie", thisBrowser.Document.Cookie);
                        //string a = ;
                        /* CookieContainer cookieJar = new CookieContainer();
                        cookieJar.SetCookies(thisBrowser.Document.Url, thisBrowser.Document.Cookie.Replace(';', ';'));
                        request.CookieContainer = cookieJar;*/


                        request.Method = "POST";
                        string postData = "fork%5Bmodule%5D=products&fork%5Baction%5D=get_price&fork%5Blanguage%5D=nl&artcd=" + articleID;
                        byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = byteArray.Length;
                        // Get the request stream.
                        Stream dataStream = request.GetRequestStream();
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();
                        try
                        {
                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                            Stream receiveStream = response.GetResponseStream();
                            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                            string output = readStream.ReadToEnd();

                            jsonReturn = JsonConvert.DeserializeObject(output);
                            optionPrice = jsonReturn.data.single_item_bruto.Value;
                        }
                        catch (Exception ex)
                        {
                            optionPrice = 0;
                        }
                    }
                        
                    if (first_price == 0)
                    {
                        first_price = optionPrice;
                        if (!optionItems.ContainsKey(optionValue))
                            optionItems.Add(optionValue, 0);
                    }
                    else
                    {
                        if (!optionItems.ContainsKey(optionValue))
                            optionItems.Add(optionValue, (float) (optionPrice- first_price ));
                    }
                    
                }
            }

            options = new Dictionary<string, Dictionary<string, float>>();
            options.Add("Variants", optionItems);
            return options;
        }

        public string[] getTags()
        {
            string[] keywords = new string[0];
            return keywords;
        }

        public Dictionary<string, string> getAdditionalFields()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();
            return retVal;
        }

        public Dictionary<string, List<string>> getPostOptionSQLCommands()
        {

            return postOptionCmd;
        }

        public List<string> getOptionAttributeLabels()
        {
            return optionAttruibuteLabels;
        }
        public List<string> getOptionAttributeNames()
        {

            return optionAttruibuteNames;
        }



    }

}
