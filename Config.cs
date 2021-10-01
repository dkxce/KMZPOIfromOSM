using System;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using ProtoBuf;

namespace KMZPOIfromOSM
{
    [ProtoContract]
    public class Config
    {
        [ProtoMember(1)]
        public string importFileName = "";
        [ProtoMember(2)]
        public string dictionaryFileName = AppDomain.CurrentDomain.BaseDirectory.Trim('\\')+@"\OSM\dictionary.json";
        [ProtoMember(3)]
        public string catalogFileName = AppDomain.CurrentDomain.BaseDirectory.Trim('\\') + @"\OSM\catalog.json";        
        [ProtoMember(4)]
        public string iconsFileName = AppDomain.CurrentDomain.BaseDirectory.Trim('\\') + @"\mapicons\poi_marker.zip";
        [ProtoMember(5)]
        public int hierarchyIndex = -1;
        [ProtoMember(6)]
        public byte saveNoCategory = 2; // 0 - Don Not Save, 1 - NoCategory, 2 - NoCategory and No Values But Keys, 3 - NoCategory and No Values But Keys List
        public static string[] saveNoCategoryList = new string[] { "Don't Save", "NoCategory", "NoCategory and NoValuesButKeys", "NoCategory and No Values But Keys List" };
        [ProtoMember(7)]
        public int updateStatusEvery = 3;

        [ProtoMember(8)]
        public bool onlyHasTags = true;
        [ProtoMember(9)]
        public bool onlyHasName = true;
        [ProtoMember(10)]
        public bool fullCatName = true;
        [ProtoMember(11)]
        public Dictionary<string, string> onlyWithTags = new Dictionary<string, string>();
        [ProtoMember(12)]
        public Dictionary<string, string> onlyOneOfTags = new Dictionary<string, string>();
        [ProtoMember(13)]
        public DateTime onlyMdfAfter = DateTime.MinValue;
        [ProtoMember(14)]
        public DateTime onlyMdfBefore = DateTime.MaxValue;
        [ProtoMember(15)]
        public List<string> onlyOfUser = new List<string>();
        [ProtoMember(16)]
        public List<int> onlyVersion = new List<int>();
        [ProtoMember(17)]
        public double[] onlyInBox = null; // min lat, min Lon, max lat, lax lon
        [ProtoMember(18)]
        public string onlyInPolygon = null;
        [ProtoMember(19)]
        public string onlybyRoute = null;
        [ProtoMember(20)]
        public int onlybyRtDist = 1000;
        [ProtoMember(21)]
        public int onlybyRLOR = 0; // 0 - [A] All Placemarks in Route Buffer (Left & Right) // 1 - [R] Placemarks by Right side only // 2 - [L] Placemarks by Left side only
        public static string[] onlybyRLORList = new string[] { "[A] All Placemarks in Route Buffer (Left & Right)", "[R] Placemarks by Right side only", "[L] Placemarks by Left side only" };
        [ProtoMember(22)]
        public bool onlyOneCategory = true;
        [ProtoMember(23)]
        public bool onlyOneButValues = true;
        [ProtoMember(24)]
        public bool onlyAllTags = false;
        [ProtoMember(25)]
        public string hasText = null;
        public Regex hasTextRegex = null;



        public PointF[] _in_polygon = null;
        public PointF[] _in_route = null;
        public OSMDictionary osmDict = new OSMDictionary();
        public OSMCatalog osmCat = new OSMCatalog();
        public OSMCatalogHierarchyList osmHier = new OSMCatalogHierarchyList();
        public List<string> iconList = new List<string>(new string[] { "noicon", "nocategory", "novalues" });
        public long noValueButKeyCatsCount = 0;
        public byte currentStatus = 0; // 0 - idle, 1 - read, 2 - read ok, 3 - read bad, 4 - ask for exit, 5 - exited, 6 - read stop
        public long total_nodes = 0;
        public long added_nodes = 0;

        public string tmpDir = AppDomain.CurrentDomain.BaseDirectory.Trim('\\') + @"\TMP_KMZPOIfromOSM\";
        public string imgDir = AppDomain.CurrentDomain.BaseDirectory.Trim('\\') + @"\TMP_KMZPOIfromOSM\images\";

        public void Save(string fileName)
        {
            string gdn = Path.GetDirectoryName(fileName);
            if ((gdn == "") || (gdn == fileName) || (!fileName.Contains("\\")))
                fileName = AppDomain.CurrentDomain.BaseDirectory.Trim('\\') + @"\" + fileName;
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            ProtoBuf.Serializer.Serialize<Config>(fs, this);
            fs.Close();
        }

        public static Config Load(string fileName, bool emptyIfNoFile)
        {
            Config result = new Config();
            string gdn = Path.GetDirectoryName(fileName);
            if ((gdn == "") || (gdn == fileName) || (!fileName.Contains("\\")))
                fileName = AppDomain.CurrentDomain.BaseDirectory.Trim('\\') + @"\" + fileName;
            if (!File.Exists(fileName))
            {
                if (emptyIfNoFile)
                    return result;
                else
                    return null;
            };
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            result = ProtoBuf.Serializer.Deserialize<Config>(fs);
            fs.Close();

            if (!File.Exists(result.importFileName)) result.importFileName = "";
            if (!File.Exists(result.dictionaryFileName)) result.dictionaryFileName = "";
            if (!File.Exists(result.catalogFileName)) result.catalogFileName = "";
            if (!File.Exists(result.iconsFileName)) result.iconsFileName = "";
            if (!File.Exists(result.onlyInPolygon)) result.onlyInPolygon = "";
            if (!File.Exists(result.onlybyRoute)) result.onlybyRoute = "";
            

            return result;
        }

        public event EventHandler onReloadHierarchy;
        public void ReloadHierarchy()
        {
            ReloadHierarchy(false);
        }
        public void ReloadHierarchy(bool onlyBasic)
        {
            osmDict = new OSMDictionary();
            if ((!String.IsNullOrEmpty(dictionaryFileName)) && (File.Exists(dictionaryFileName)))
                { try { osmDict = OSMDictionary.ReadFromFile(dictionaryFileName); } catch { }; };
            osmCat = new OSMCatalog();
            if ((!String.IsNullOrEmpty(catalogFileName)) && (File.Exists(catalogFileName)))
                { try { osmCat = OSMCatalog.ReadFromFile(catalogFileName); } catch { }; };
            osmCat.dict = osmDict;
            osmHier = osmCat.GetHierarchy(hierarchyIndex, onlyBasic, saveNoCategory);
            iconList = new List<string>(new string[] { "noicon", "nocategory", "novalues" });
            noValueButKeyCatsCount = 0;
            currentStatus = 0;
            total_nodes = 0;
            added_nodes = 0;
            _in_route = null;
            _in_polygon = null;

            if (onReloadHierarchy != null)
                onReloadHierarchy(this, null);
        }

        public event EventHandler onReloadProperties;
        public void ReloadProperties()
        {
            if (onReloadProperties != null)
                onReloadProperties(this, null);
        }

        public void PrepareForRead()
        {            
            ReloadHierarchy(true);
            LoadPolygonsAndRoutes();
        }

        private void LoadPolygonsAndRoutes()
        {
            if((!String.IsNullOrEmpty(this.onlyInPolygon)) && (File.Exists(this.onlyInPolygon)))
            {
                try
                {
                    string tof = "";
                    this._in_polygon = UTILS.loadpolyfull(this.onlyInPolygon, out tof);
                }
                catch
                {
                    this._in_polygon = null;
                };
            };
            if ((!String.IsNullOrEmpty(this.onlybyRoute)) && (File.Exists(this.onlybyRoute)))
            {
                try
                {
                    string tof = "";
                    this._in_route = UTILS.loadroute(this.onlybyRoute, out tof);
                }
                catch
                {
                    this._in_route = null;
                };
            };
        }
    }
}
