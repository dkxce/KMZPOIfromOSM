using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace KMZPOIfromOSM
{
    public partial class KMZPOIMainForm : Form
    {
        public Config config = new Config();

        public bool in_progress = false;

        public KMZPOIMainForm()
        {
            InitializeComponent();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            Text += " " + fvi.FileVersion + " by " + fvi.CompanyName;

            LoadConfig();
        }

        public void LoadConfig()
        {
            config = Config.Load("OSM2SHP.cpb", true);
            config.onReloadHierarchy += new EventHandler(config_onReloadHierarchy);
            config.onReloadProperties += new EventHandler(config_onReloadProperties);
            config.ReloadProperties();
            config.ReloadHierarchy();
        }

        public void config_onReloadProperties(object sender, EventArgs e)
        {
            props.Items[0].SubItems[1].Text = Path.GetFileName(config.importFileName);
            props.Items[1].SubItems[1].Text = Path.GetFileName(config.dictionaryFileName);
            props.Items[2].SubItems[1].Text = Path.GetFileName(config.catalogFileName);
            props.Items[3].SubItems[1].Text = Path.GetFileName(config.iconsFileName);
            props.Items[4].SubItems[1].Text = config.hierarchyIndex.ToString();
            props.Items[5].SubItems[1].Text = Config.saveNoCategoryList[config.saveNoCategory];
            props.Items[6].SubItems[1].Text = config.updateStatusEvery.ToString();
            props.Items[7].SubItems[1].Text = config.onlyHasTags ? "Yes" : "No";
            props.Items[8].SubItems[1].Text = config.onlyHasName ? "Yes" : "No";
            props.Items[9].SubItems[1].Text = config.fullCatName ? "Yes" : "No";
            props.Items[17].SubItems[1].Text = Path.GetFileName(config.onlyInPolygon);
            props.Items[18].SubItems[1].Text = Path.GetFileName(config.onlybyRoute);
            props.Items[19].SubItems[1].Text = config.onlybyRtDist.ToString();
            props.Items[20].SubItems[1].Text = Config.onlybyRLORList[config.onlybyRLOR];
            props.Items[21].SubItems[1].Text = config.onlyOneCategory ? "No" : "Yes";
            props.Items[22].SubItems[1].Text = config.onlyOneButValues ? "No" : "Yes";
            props.Items[23].SubItems[1].Text = config.onlyAllTags ? "Yes" : "No";
            props.Items[24].SubItems[1].Text = config.hasText;


            {//10
                string tl = "";
                foreach (KeyValuePair<string, string> kvp in config.onlyWithTags)
                {
                    if (tl.Length > 0) tl += ",";
                    if ((kvp.Value == null) || (kvp.Value == ""))
                        tl += kvp.Key;
                    else
                        tl += String.Format("{0}={1}", kvp.Key, kvp.Value);
                };
                props.Items[10].SubItems[1].Text = tl;
            };
            {//11
                string tl = "";
                foreach (KeyValuePair<string, string> kvp in config.onlyOneOfTags)
                {
                    if (tl.Length > 0) tl += ",";
                    if ((kvp.Value == null) || (kvp.Value == ""))
                        tl += kvp.Key;
                    else
                        tl += String.Format("{0}={1}", kvp.Key, kvp.Value);
                };
                props.Items[11].SubItems[1].Text = tl;
            };
            {//12
                if(config.onlyMdfAfter != DateTime.MinValue)
                    props.Items[12].SubItems[1].Text = config.onlyMdfAfter.ToString("yyyy-MM-ddTHH:mm:ss");
            };
            {//13
                if (config.onlyMdfBefore != DateTime.MaxValue)
                    props.Items[13].SubItems[1].Text = config.onlyMdfBefore.ToString("yyyy-MM-ddTHH:mm:ss");
            };
            {//14
                string tl = "";
                foreach (string user in config.onlyOfUser)
                {
                    if (tl.Length > 0) tl += ","; 
                    tl += user;
                };
                props.Items[14].SubItems[1].Text = tl;
            };
            {//15
                string tl = "";
                foreach (int ver in config.onlyVersion)
                {
                    if (tl.Length > 0) tl += ",";
                    tl += ver.ToString();
                };
                props.Items[15].SubItems[1].Text = tl;
            };
            {//16
                string tl = "";
                if ((config.onlyInBox != null) && (config.onlyInBox.Length == 4))
                    tl = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3}", new object[] { config.onlyInBox[0], config.onlyInBox[1], config.onlyInBox[2], config.onlyInBox[3] });
                props.Items[16].SubItems[1].Text = tl;
            };
        }

        public void config_onReloadHierarchy(object sender, EventArgs e)
        {
            catView.Items.Clear();
            for (int i = 0; i < config.osmHier.Count; i++)
            {
                ListViewItem lvi = new ListViewItem(i.ToString());
                lvi.SubItems.Add(config.osmHier[i].id.ToString());
                string name = config.fullCatName ? config.osmHier[i].dict_fullname : config.osmHier[i].dict_name;
                string nodn = config.fullCatName ? config.osmHier[i].fullname : config.osmHier[i].name;
                if (nodn != name) name += " (" + nodn + ")";
                lvi.SubItems.Add(name);
                lvi.SubItems.Add("0");
                lvi.SubItems.Add("0");
                lvi.Checked = true;
                lvi.ToolTipText = config.osmHier[i].fullname;
                catView.Items.Add(lvi);
            };
            ttlTxt.Text = "Total " + config.osmHier.Count.ToString() + " categories";
            status.Text = "Idle";
        }

        public void SaveConfig()
        {
            config.Save("KMZPOIfromOSM.cpb");
        }

        private void KMZPOIMaonForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveConfig();
        }
       
        private void SetPropsEnables(bool enabled)
        {
            in_progress = !enabled;
            mnu2_Opening(this, new CancelEventArgs());
        }

        private void ReadFileData()
        {
            if (!File.Exists(config.importFileName))
            {
                MessageBox.Show("File Not Found", "Read Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            };

            string ext = Path.GetExtension(config.importFileName).ToLower();
            if (ext == ".pbf")
                ReadPBFData();

            if (ext == ".osm")
                ReadXMLData();
        }

        private void ReadPBFData()
        {            
            OSMPBFReader pbfr = new OSMPBFReader(config.importFileName);
            if (!pbfr.ValidHeader)
            {
                pbfr.Close();
                MessageBox.Show("Ivalid OSMHeader", "Read Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            };

            status3.Text = "";
            statusStrip2.Visible = false;
            status.Text = "Preparing Temporary Folders...";
            ttlTxt.Text = "...";
            Application.DoEvents();
            if(true)
            {
                try
                {
                    if(Directory.Exists(config.tmpDir))
                        Directory.Delete(config.tmpDir, true);
                    Directory.CreateDirectory(config.tmpDir);
                    Directory.CreateDirectory(config.imgDir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Create Temp Folder\r\n"+ex.Message, "Temp Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                };
            };

            config.PrepareForRead();
            config.currentStatus = 1; // read
            SetPropsEnables(false);

            config.hasTextRegex = null;
            try
            {
                if (!String.IsNullOrEmpty(config.hasText))
                    config.hasTextRegex = new Regex(config.hasText, RegexOptions.IgnoreCase);
            }
            catch { };

            long current_node = 0;
            DateTime lastStatusUpdate = DateTime.UtcNow;
            while (!pbfr.EndOfFile)
            {
                pbfr.ReadNext();

                UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length);
                lastStatusUpdate = DateTime.UtcNow;

                if (pbfr.HasOSMPrimitiveBlock)
                {
                    double[] bbox = new double[4];
                    if (pbfr.HasOSMHeader)
                        bbox = pbfr.OSMHeaderBlock.GetBBox;
                    if (pbfr.OSMPrimitiveBlock.primitivegroup.Count > 0)
                        for (int z = 0; z < pbfr.OSMPrimitiveBlock.primitivegroup.Count; z++)
                        {
                            if (pbfr.OSMPrimitiveBlock.primitivegroup[z].dense != null)
                            {
                                OSMPBFReader.DenseNodes dns = pbfr.OSMPrimitiveBlock.primitivegroup[z].dense;
                                if ((dns.id != null) && (dns.Count > 0))
                                {
                                    config.total_nodes += dns.Count;
                                    
                                    UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length);
                                    lastStatusUpdate = DateTime.UtcNow;
                                    
                                    KeyValuePair<int, OSMPBFReader.NodeInfo> kvp;
                                    try
                                    {
                                        while ((kvp = dns.Next).Key != -1)
                                        {
                                            if (ProcessNode(kvp.Value)) config.added_nodes++;
                                            current_node++;
                                            if ((current_node % 100) == 0)
                                                Application.DoEvents();
                                            if (DateTime.UtcNow.Subtract(lastStatusUpdate).TotalSeconds >= config.updateStatusEvery)
                                            {
                                                UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length);
                                                lastStatusUpdate = DateTime.UtcNow;
                                            };
                                        };
                                    }
                                    catch (Exception ex)
                                    {
                                        if (MessageBox.Show("Error:\r\n" + ex.Message + "\r\n\r\nContinue?", "Read File", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                                        {
                                            UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length, "Read File Error");
                                            pbfr.Close();
                                            config.currentStatus = 3; // read bad                                            
                                            SetPropsEnables(true);
                                            return;
                                        };
                                    };
                                    if (config.currentStatus == 5)
                                    {
                                        UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length, "File Read Stopped");
                                        pbfr.Close();
                                        config.currentStatus = 6; // read stop                                        
                                        SetPropsEnables(true);
                                        return;
                                    };
                                };                                
                            };
                            if ((pbfr.OSMPrimitiveBlock.primitivegroup[z].nodes != null) && (pbfr.OSMPrimitiveBlock.primitivegroup[z].nodes.Count > 0))
                            {
                                List<OSMPBFReader.Node> nodes = pbfr.OSMPrimitiveBlock.primitivegroup[z].nodes;
                                config.total_nodes += nodes.Count;

                                UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length);
                                lastStatusUpdate = DateTime.UtcNow;

                                for (int i = 0; i < nodes.Count; i++)
                                {
                                    try
                                    {
                                        if (ProcessNode(nodes[i].GetNode(pbfr.OSMPrimitiveBlock))) config.added_nodes++;
                                        current_node++;
                                        if ((current_node % 100) == 0)
                                            Application.DoEvents();
                                        if (DateTime.UtcNow.Subtract(lastStatusUpdate).TotalSeconds >= config.updateStatusEvery)
                                        {
                                            UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length);
                                            lastStatusUpdate = DateTime.UtcNow;
                                        };
                                    }
                                    catch (Exception ex)
                                    {
                                        if (MessageBox.Show("Error:\r\n" + ex.Message + "\r\n\r\nContinue?", "Read File", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                                        {
                                            UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length, "Read File Error");
                                            pbfr.Close();
                                            config.currentStatus = 3; // read bad                                            
                                            SetPropsEnables(true);
                                            return;
                                        };
                                    };
                                    if (config.currentStatus == 5)
                                    {
                                        UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length, "File Read Stopped");
                                        pbfr.Close();
                                        config.currentStatus = 6; // read stop                                        
                                        SetPropsEnables(true);
                                        return;
                                    };
                                };
                            };
                        };
                };
            };
            UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length, "File Read Completed");
            pbfr.Close();

            config.currentStatus = 2; // read ok
            SetPropsEnables(true);
        }

        private void ReadXMLData()
        {
            OSMXMLReader pbfr = new OSMXMLReader(config.importFileName);
            if (!pbfr.ValidHeader)
            {
                pbfr.Close();
                MessageBox.Show("Ivalid OSMHeader", "Read Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            };

            status3.Text = "";
            statusStrip2.Visible = false;
            status.Text = "Preparing Temporary Folders...";
            ttlTxt.Text = "...";
            Application.DoEvents();
            if (true)
            {
                try
                {
                    if (Directory.Exists(config.tmpDir))
                        Directory.Delete(config.tmpDir, true);
                    Directory.CreateDirectory(config.tmpDir);
                    Directory.CreateDirectory(config.imgDir);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Create Temp Folder\r\n" + ex.Message, "Temp Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                };
            };

            config.PrepareForRead();
            config.currentStatus = 1; // read
            SetPropsEnables(false);

            long current_node = 0;
            DateTime lastStatusUpdate = DateTime.UtcNow;
            UpdatePercentage(0, 0, 0, pbfr.Position, pbfr.Length);
            while (!pbfr.EndOfFile)
            {
                pbfr.ReadNext();
                if (pbfr.NodeInfo != null)
                {
                    config.total_nodes++;
                    try
                    {
                        if (ProcessNode(pbfr.NodeInfo)) config.added_nodes++;
                        current_node++;
                        if (current_node == 1)
                            UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length);
                        if ((current_node % 100) == 0)
                            Application.DoEvents();
                        if (DateTime.UtcNow.Subtract(lastStatusUpdate).TotalSeconds >= config.updateStatusEvery)
                        {
                            UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length);
                            lastStatusUpdate = DateTime.UtcNow;
                        };
                    }
                    catch (Exception ex)
                    {
                        if (MessageBox.Show("Error:\r\n" + ex.Message + "\r\n\r\nContinue?", "Read File", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                        {
                            UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length, "Read File Error");
                            pbfr.Close();
                            config.currentStatus = 3; // read bad                                            
                            SetPropsEnables(true);
                            return;
                        };
                    };
                    if (config.currentStatus == 5)
                    {
                        UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length, "File Read Stopped");
                        pbfr.Close();
                        config.currentStatus = 6; // read stop                                        
                        SetPropsEnables(true);
                        return;
                    };
                };
            };
            UpdatePercentage(current_node, config.added_nodes, config.total_nodes, pbfr.Position, pbfr.Length, "File Read Completed");           
            pbfr.Close();

            config.currentStatus = 2; // read ok
            SetPropsEnables(true);
        }

        private bool ProcessNode(OSMPBFReader.NodeInfo ni)
        {
            if (config.currentStatus == 5) return false;
            if (config.currentStatus == 4) { if (MessageBox.Show("Do you really want to stop reading file?", "Read File", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) { config.currentStatus = 5; return false; } else config.currentStatus = 1; };

            // ++filters
            if (!ApplyFilters(ni)) return false;
            // --filters

            Dictionary<string, string> translate_tags = new Dictionary<string, string>();
            OSMCatalogHierarchyList in_cats_by_one = new OSMCatalogHierarchyList();
            OSMCatalogHierarchyList in_cats_by_all = new OSMCatalogHierarchyList();
            List<string> no_cats = new List<string>();
            
            if ((config.osmCat.records != null) && (config.osmCat.Count > 0) && (ni.tags != null) && (ni.tags.Count > 0))
                for (int i = 0; i < config.osmCat.Count; i++)
                {
                    if (config.osmCat[i].tags.Count == 0) continue;

                    string all_tags = "";
                    int different_tags = config.osmCat[i].tags.Count;                    
                    foreach (KeyValuePair<string, string> nitag in ni.tags)                    
                    {
                        // FIND CATEGORIES
                        foreach (KeyValuePair<string, string> catag in config.osmCat[i].tags)
                        {
                            if ((nitag.Key == catag.Key) && (Regex.IsMatch(nitag.Value, @"^([\w\s\:\=]+)$")))
                            {
                                string no_in_cats = String.Format("{0}:{1}", nitag.Key, nitag.Value);
                                if (no_cats.IndexOf(no_in_cats) < 0) no_cats.Add(no_in_cats);
                                if (ni.icon == "noicon") ni.icon = nitag.Value.Replace(":", "_").Replace("=", "_");
                            };
                            if ((nitag.Key == catag.Key) && (nitag.Value == catag.Value))
                            {
                                if (all_tags.Length > 0) all_tags += ",";
                                all_tags += String.Format("{0}:{1}", nitag.Key, nitag.Value);
                                different_tags--;
                            };
                        };
                        // MAKE DICTIONARY
                        foreach (KeyValuePair<string, Dictionary<string, string>> ext in config.osmCat[i].moretags)
                        {
                            if (ext.Key == nitag.Key)
                            {
                                if (ni.icon == "noicon") ni.icon = config.osmCat[i].name;
                                string txtk = config.osmDict.Translate(nitag.Key);
                                if (ext.Value.ContainsKey("class"))
                                {
                                    string txtv = config.osmDict.TranslateClass(ext.Value["class"], nitag.Value);
                                    if (((txtk != nitag.Key) || (txtv != nitag.Value)) && (!translate_tags.ContainsKey(txtk)))
                                        translate_tags.Add(txtk, txtv);
                                };
                            };
                        };
                    };
                    if (different_tags < config.osmCat[i].tags.Count)
                    {
                        ni.icon = config.osmCat[i].name;
                        if (config.hierarchyIndex == 0)
                        {
                            if (different_tags > 0)
                            {
                                if (in_cats_by_one.IndexOf(all_tags) < 0)
                                    in_cats_by_one.Add(new OSMCatalogHierarchy(config.osmCat[i].id, all_tags, ni.icon));
                            }
                            else
                            {
                                if (in_cats_by_all.IndexOf(all_tags) < 0)
                                    in_cats_by_all.Add(new OSMCatalogHierarchy(config.osmCat[i].id, all_tags, ni.icon));
                            };
                        }
                        else
                        {
                            if (different_tags > 0)
                            {
                                if (config.onlyOneCategory)
                                {
                                    OSMCatalogHierarchy hel = config.osmCat.GetHierarchyElement(config.osmCat[i].id, config.hierarchyIndex);
                                    if (in_cats_by_one.IndexOf(hel.id) < 0) in_cats_by_one.Add(hel);
                                }
                                else
                                {
                                    OSMCatalogHierarchy[] hels = config.osmCat.GetHierarchyElements(config.osmCat[i].id, config.hierarchyIndex);
                                    foreach (OSMCatalogHierarchy hel in hels) if (in_cats_by_one.IndexOf(hel.id) < 0) in_cats_by_one.Add(hel);
                                };
                            }
                            else
                            {
                                if (config.onlyOneCategory)
                                {
                                    OSMCatalogHierarchy hel = config.osmCat.GetHierarchyElement(config.osmCat[i].id, config.hierarchyIndex);
                                    if (in_cats_by_all.IndexOf(hel.id) < 0) in_cats_by_all.Add(hel);
                                }
                                else
                                {
                                    OSMCatalogHierarchy[] hels = config.osmCat.GetHierarchyElements(config.osmCat[i].id, config.hierarchyIndex);
                                    foreach (OSMCatalogHierarchy hel in hels) if (in_cats_by_all.IndexOf(hel.id) < 0) in_cats_by_all.Add(hel);
                                };
                            };
                        };
                    };
                }; // END foreach osmCat
            
            // add translated tags
            foreach (KeyValuePair<string, string> kvp in translate_tags) if(!ni.tags.ContainsKey(kvp.Key)) ni.tags.Add(kvp.Key, kvp.Value);
            // in all tags is same
            if (config.onlyAllTags) in_cats_by_one.Clear();            

            if (in_cats_by_all.Count > 0)
            {
                if (config.onlyOneCategory) 
                    in_cats_by_one = in_cats_by_all;
                else
                    foreach (OSMCatalogHierarchy hel in in_cats_by_all) if (in_cats_by_one.IndexOf(hel.id) < 0) in_cats_by_one.Add(hel);
            };            

            if ((in_cats_by_one.Count == 0) && (config.saveNoCategory > 0))
            {
                // Save No Categories
                if ((config.saveNoCategory == 1) || (no_cats.Count == 0))
                {
                    if (ni.icon == "noicon") ni.icon = config.osmHier[0].default_icon;
                    SaveToCategory(ni, 0);
                    return true;
                };

                if (config.saveNoCategory == 2)
                {
                    if (ni.icon == "noicon") ni.icon = config.osmHier[1].default_icon;
                    SaveToCategory(ni, 1);
                    return true;
                };

                if (config.saveNoCategory == 3)
                {
                    foreach (string no_in_cats in no_cats)
                    {
                        OSMCatalogHierarchy hel = new OSMCatalogHierarchy(int.MaxValue, no_in_cats, ni.icon);
                        int index = config.osmHier.IndexOf(no_in_cats);
                        if (index < 0) index = AddNewCategory(hel);
                        SaveToCategory(ni, index);
                        if (config.onlyOneButValues) break;
                    };
                    return true;
                };
            };

            // IS OK
            if (in_cats_by_one.Count > 0)
                foreach (OSMCatalogHierarchy hel in in_cats_by_one)
                {
                    int index = config.osmHier.IndexOf(hel.id);
                    if (index < 0) index = AddNewCategory(hel);
                    SaveToCategory(ni, index);
                    if (config.onlyOneCategory) break;
                };            

            return in_cats_by_one.Count > 0;
        }

        private bool ApplyFilters(OSMPBFReader.NodeInfo ni)
        {
            if (config.onlyHasTags && (!ni.HasTags)) return false;
            if (config.onlyHasName && (!ni.HasName)) return false;
            if (config.onlyWithTags.Count > 0)
            {
                if (ni.tags.Count == 0) return false;
                foreach (KeyValuePair<string, string> _kvp in config.onlyWithTags)
                {
                    if (!ni.tags.ContainsKey(_kvp.Key)) return false;
                    if ((_kvp.Value != null) && (_kvp.Value != "") && (ni.tags[_kvp.Key] != _kvp.Value)) return false;
                };
            };
            if (config.onlyOneOfTags.Count > 0)
            {
                if (ni.tags.Count == 0) return false;
                bool skip = true;
                foreach (KeyValuePair<string, string> _kvp in config.onlyOneOfTags)
                {
                    if (ni.tags.ContainsKey(_kvp.Key))
                    {
                        if ((_kvp.Value == null) || (_kvp.Value == ""))
                            skip = false;
                        else if ((ni.tags[_kvp.Key] == _kvp.Value))
                            skip = false;
                    };
                };
                if (skip)
                    return false;
            };
            if (config.onlyMdfAfter != DateTime.MinValue) if (ni.datetime < config.onlyMdfAfter) return false;
            if (config.onlyMdfBefore != DateTime.MaxValue) if (ni.datetime > config.onlyMdfBefore) return false;
            if (config.onlyOfUser.Count > 0)
            {
                bool skip = true;
                foreach (string user in config.onlyOfUser)
                    if (ni.user == user)
                        skip = false;
                if (skip)
                    return false;
            };
            if (config.onlyVersion.Count > 0)
            {
                bool skip = true;
                foreach (int ver in config.onlyVersion)
                    if (ni.version == ver)
                        skip = false;
                if (skip)
                    return false;
            };
            if ((config.onlyInBox != null) && (config.onlyInBox.Length == 4))
            {
                if (ni.lat < config.onlyInBox[0]) return false;
                if (ni.lon < config.onlyInBox[1]) return false;
                if (ni.lat > config.onlyInBox[2]) return false;
                if (ni.lon > config.onlyInBox[3]) return false;
            };
            if ((config._in_polygon != null) && (config._in_polygon.Length > 2))
            {
                if (!UTILS.PointInPolygon(new PointF((float)ni.lon, (float)ni.lat), config._in_polygon))
                    return false;
            };
            if ((config._in_route != null) && (config._in_route.Length > 1))
            {
                bool intr = UTILS.PointInRoute(new PointF((float)ni.lon, (float)ni.lat), config._in_route, config.onlybyRtDist, config.onlybyRLOR);
                if (!intr)
                    return false;
            };

            if (config.hasTextRegex != null)
            {
                if (ni.tags == null) return false;
                if (ni.tags.Count == 0) return false;
                bool isok = false;
                foreach (KeyValuePair<string, string> kvp in ni.tags)
                {
                    if (config.hasTextRegex.IsMatch(kvp.Key)) isok = true;
                    if (config.hasTextRegex.IsMatch(kvp.Value)) isok = true;
                };
                if (!isok)
                    return false;
            };

            return true;
        }

        private int AddNewCategory(OSMCatalogHierarchy hel)
        {
            if (hel.id == int.MaxValue) hel.id = - 2 - (int)(++config.noValueButKeyCatsCount);
            config.osmHier.Add(hel);
            return config.osmHier.Count - 1;
        }

        private void SaveToCategory(OSMPBFReader.NodeInfo ni, int cIndex)
        {
            OSMCatalogHierarchy hel = config.osmHier[cIndex];            

            if (config.iconList.IndexOf(ni.icon) < 0) config.iconList.Add(ni.icon);
            if (hel.iconList.IndexOf(ni.icon) < 0) hel.iconList.Add(ni.icon);

            string nam = String.IsNullOrEmpty(ni.Name) ? "NoName" : ni.Name;            
            string xyz = ni.lon.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + ni.lat.ToString(System.Globalization.CultureInfo.InvariantCulture) + ",0";
            string desc = "";            
            desc += String.Format("{0}={1}\r\n", "tag_id", ni.id);
            if (ni.timestamp > 0)
            {
                desc += String.Format("{0}={1:yyyy-MM-ddTHH:mm:ss}\r\n", "tag_modified", ni.datetime);
                desc += String.Format("{0}={1}\r\n", "tag_timestamp", ni.timestamp);
            };
            if(ni.changeset > 0)
                desc += String.Format("{0}={1}\r\n", "tag_changeset", ni.changeset);
            if(ni.version > 0)
                desc += String.Format("{0}={1}\r\n", "tag_version", ni.version);
            if(!String.IsNullOrEmpty(ni.user))
                desc += String.Format("{0}={1}\r\n", "tag_user", ni.user);
            desc += "\r\n";

            foreach (KeyValuePair<string, string> kvp in ni.tags)
                desc += String.Format("{0}={1}\r\n", kvp.Key, kvp.Value);

            FileStream fs = new FileStream(config.tmpDir + "folder" + cIndex.ToString() + ".kml", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            fs.Position = fs.Length;
            sw.WriteLine("\t\t\t<Placemark>");
            sw.WriteLine("\t\t\t\t<name><![CDATA[" + nam + "]]></name>");
            sw.WriteLine("\t\t\t\t<description><![CDATA[" + desc + "]]></description>");
            sw.WriteLine("\t\t\t\t<styleUrl>#" + ni.icon + "</styleUrl>");
            sw.WriteLine("\t\t\t\t<Point><coordinates>" + xyz + "</coordinates></Point>");
            sw.WriteLine("\t\t\t</Placemark>");
            sw.Flush();

            hel.nodes++;
            hel.file_size = fs.Position;
            
            sw.Close();
            fs.Close();
        }

        private void UpdatePercentage(double currNode, double goodNodes, double ttlNodes, double filePos, double fileLen)
        {
            UpdatePercentage(currNode, goodNodes, ttlNodes, filePos, fileLen, "Reading file");
        }

        private void UpdatePercentage(double currNode, double goodNodes, double ttlNodes, double filePos, double fileLen, string prefix)
        {
            statusStrip2.Visible = false;
            status3.Text = "";            

            string file_read_percentage = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:P}", filePos / fileLen);
            string node_read_percentage = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:P}", currNode / ttlNodes);
            string node_good_percentage = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:P}", goodNodes / ttlNodes);
            string node_skip_percentage = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:P}", (ttlNodes - goodNodes) / ttlNodes);
            status.Text = String.Format(System.Globalization.CultureInfo.InvariantCulture, prefix + " {0} nodes {1} - {2}/{3} good {4} - {5}/{3}, skip - {6} - {7}/{3}", file_read_percentage, node_read_percentage, currNode, ttlNodes, node_good_percentage, goodNodes, node_skip_percentage, ttlNodes - goodNodes);
            long ttl = 0, cats = 0;
            for (int i = 0; i < config.osmHier.Count; i++)
            {
                if (i == catView.Items.Count)
                {
                    ListViewItem lvi = new ListViewItem(i.ToString());
                    lvi.SubItems.Add(config.osmHier[i].id.ToString());
                    string name = config.fullCatName ? config.osmHier[i].dict_fullname : config.osmHier[i].dict_name;
                    string nodn = config.fullCatName ? config.osmHier[i].fullname : config.osmHier[i].name;
                    if (nodn != name) name += " (" + nodn + ")";
                    lvi.SubItems.Add(name);
                    lvi.SubItems.Add("0");
                    lvi.SubItems.Add("0");
                    lvi.Checked = true;
                    lvi.ToolTipText = config.osmHier[i].fullname;
                    catView.Items.Add(lvi);
                };
                catView.Items[i].SubItems[3].Text = config.osmHier[i].nodes.ToString();
                if ((config.osmHier[i].nodes == 0) && (catView.Items[i].BackColor == SystemColors.Window))
                    catView.Items[i].BackColor = Color.LightGray;
                if ((config.osmHier[i].nodes > 0) && (catView.Items[i].BackColor == Color.LightGray))
                    catView.Items[i].BackColor = SystemColors.Window;
                catView.Items[i].SubItems[4].Text = config.osmHier[i].FileSize;

                ttl += config.osmHier[i].nodes;
                if (config.osmHier[i].nodes > 0)
                    cats++;
            };
            ttlTxt.Text = String.Format("Total {0} original nodes of {1} in {2}/{3} categories", goodNodes, ttl, cats, config.osmHier.Count);
            Application.DoEvents();
        }

        private string last_status = "";
        private void UpdateStatus(double pos, double ttl, string prefix)
        {
            last_status = status3.Text = String.Format(System.Globalization.CultureInfo.InvariantCulture, prefix + " {0:P} ", pos / ttl);
            statusStrip2.Visible = true;
            Application.DoEvents();
        }

        private void UpdateStatusZip(object sender, ProgressEventArgs e)
        {
            if ((sender is int) && ((int)sender == 1))
                status3.Text = last_status + String.Format(System.Globalization.CultureInfo.InvariantCulture, "- adding placemarks to {0:P}", e.PercentComplete / 100.0, e.Name);
            if ((sender is int) && ((int)sender == 2))
                status3.Text = last_status + String.Format(System.Globalization.CultureInfo.InvariantCulture, "- adding icons ...");
            if(!statusStrip2.Visible)
                statusStrip2.Visible = true;
            Application.DoEvents();
        }

        private void CopyIcons()
        {
            if (!Directory.Exists(config.imgDir)) return;
            if (config.iconList == null) return;
            if (config.iconList.Count == 0) return;

            bool ifx = File.Exists(config.iconsFileName);
            
            int nci = 0;
            for (int i = 0; i < config.iconList.Count; i++)
            {
                string icon = config.iconList[i] + ".png";
                Stream str = null;
                if (ifx) str = OSMCatalogHierarchyList.GetImageFileFromZip(config.iconsFileName, icon);
                if (str != null)
                {
                    try
                    {
                        FileStream fs = new FileStream(config.imgDir + icon, FileMode.Create, FileAccess.Write);
                        byte[] buff = new byte[ushort.MaxValue];
                        int len = 0;
                        while ((len = str.Read(buff, 0, buff.Length)) > 0)
                            fs.Write(buff, 0, len);
                        fs.Close();
                    }
                    catch
                    {
                        MessageBox.Show("Unable to save icon " + icon, "Copy Icons", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        str.Close();
                        UpdateStatus(config.iconList.Count, config.iconList.Count, "Error prepare Icons ");
                        return;
                    };
                    str.Close();
                }
                else
                {
                    Brush[] brushes = new Brush[] { Brushes.Fuchsia, Brushes.Green, Brushes.SkyBlue, Brushes.Pink, Brushes.LightGreen, Brushes.LightBlue, Brushes.Yellow, Brushes.Beige, Brushes.Cyan, Brushes.Teal, Brushes.GreenYellow, Brushes.Lime, Brushes.Orange, Brushes.Red, Brushes.SeaGreen, Brushes.Silver, Brushes.Snow, Brushes.Tan, Brushes.WhiteSmoke };
                    try
                    {
                        string text = System.Text.Encoding.ASCII.GetString(new byte[] { (byte)(65 + (nci % 20)) }, 0, 1);
                        int sb = nci / 20;
                        if (nci >= brushes.Length) nci = 0;
                        
                        Bitmap bmp = new Bitmap(32, 32);
                        Graphics g = Graphics.FromImage(bmp);
                        g.FillEllipse(brushes[nci], 0, 0, 32, 32);
                        g.DrawEllipse(new Pen(Color.Black, 2), 0, 0, 31, 31);
                        SizeF ms = g.MeasureString(text, new Font("Arial", 11, FontStyle.Bold));
                        g.DrawString(text, new Font("Arial", 11, FontStyle.Bold), Brushes.Black, 16 - ms.Width / 2, 16 - ms.Height / 2);
                        g.Dispose();
                        bmp.Save(config.imgDir + icon, ImageFormat.Png);
                        bmp.Dispose();
                    }
                    catch
                    {
                        MessageBox.Show("Unable to save icon " + icon, "Copy Icons", MessageBoxButtons.OK, MessageBoxIcon.Error);                        
                        UpdateStatus(config.iconList.Count, config.iconList.Count, "Error prepare Icons ");
                        return;
                    };
                    nci++;
                };
                UpdateStatus(i, config.iconList.Count, "Preparing Icons ");
            };
            UpdateStatus(config.iconList.Count, config.iconList.Count, "Preparing Icons ");
        }

        private void MakeKMZWithCustomIcons(string tosave, string whatsave, List<string> iconlist)
        {
            ProgressHandler ph = new ProgressHandler(UpdateStatusZip);
            TimeSpan interval = new TimeSpan(0, 0, config.updateStatusEvery);

            FileStream fsOut = File.Create(tosave);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);
            string comment_add = "";
            zipStream.SetComment("Google KMZ file For OruxMaps\r\n\r\n" + "Created at " + DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy") + "\r\nby " + this.Text + "\r\n\r\nUse OruxMaps for Android or KMZViewer for Windows to Explore file POI" + comment_add);
            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

            //write doc.kml file
            {
                string entryName = "doc.kml"; // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = DateTime.Now; // Note the zip format stores 2 second granularity                

                newEntry.Size = (new FileInfo(whatsave)).Length;
                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(whatsave))
                    StreamUtils.Copy(streamReader, zipStream, buffer, ph, new TimeSpan(0, 0, config.updateStatusEvery), 1, Path.GetFileName(tosave));
                zipStream.CloseEntry();
            };

            foreach(string ic in iconlist)
            {
                string icon = ic + ".png";
                FileInfo fi = new FileInfo(config.imgDir + icon);
                if (fi.Exists)
                {
                    string entryName = @"images\" + icon;
                    entryName = ZipEntry.CleanName(entryName);
                    ZipEntry newEntry = new ZipEntry(entryName);
                    newEntry.DateTime = fi.LastWriteTime;
                    newEntry.Size = fi.Length;
                    zipStream.PutNextEntry(newEntry);

                    byte[] buffer = new byte[4096];
                    using (FileStream streamReader = File.OpenRead(fi.FullName))
                        StreamUtils.Copy(streamReader, zipStream, buffer, ph, interval, 2, entryName);
                    zipStream.CloseEntry();
                };
            };

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        private void MakeKMZWithAllIcons(string tosave, string whatsave)
        {
            ProgressHandler ph = new ProgressHandler(UpdateStatusZip);
            TimeSpan interval = new TimeSpan(0, 0, config.updateStatusEvery);

            FileStream fsOut = File.Create(tosave);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);
            string comment_add = "";
            zipStream.SetComment("Google KMZ file For OruxMaps\r\n\r\n" + "Created at " + DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy") + "\r\nby " + this.Text + "\r\n\r\nUse OruxMaps for Android or KMZViewer for Windows to Explore file POI" + comment_add);
            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

            //write file
            {
                string entryName = "doc.kml"; // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = DateTime.Now; // Note the zip format stores 2 second granularity                

                newEntry.Size = (new FileInfo(whatsave)).Length;
                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(whatsave))
                    StreamUtils.Copy(streamReader, zipStream, buffer, ph, new TimeSpan(0, 0, config.updateStatusEvery), 1, Path.GetFileName(tosave));
                zipStream.CloseEntry();
            };
            CompressFolder(config.imgDir, zipStream, config.tmpDir.Length, ph, interval, 2);

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        private void MakeZipFolder(string tosave, string folder)
        {
            FileStream fsOut = File.Create(tosave);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);
            string comment_add = "";
            zipStream.SetComment("Google KMZ file For OruxMaps\r\n\r\n" + "Created at " + DateTime.Now.ToString("HH:mm:ss dd.MM.yyyy") + "\r\nby " + this.Text + "\r\n\r\nUse OruxMaps for Android or KMZViewer for Windows to Explore file POI" + comment_add);
            zipStream.SetLevel(3); //0-9, 9 being the highest level of compression
            // zipStream.Password = password;  // optional. Null is the same as not setting. Required if using AES.
            CompressFolder(folder, zipStream, folder.Length);
            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }
        
        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset, ProgressHandler ph, TimeSpan interval, int deepness)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string filename in files)
            {

                FileInfo fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity
                newEntry.Size = fi.Length;
                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                    StreamUtils.Copy(streamReader, zipStream, buffer, ph, interval, deepness, entryName);
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
                CompressFolder(folder, zipStream, folderOffset, ph, interval, deepness + 1);
        }

        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string filename in files)
            {

                FileInfo fi = new FileInfo(filename);

                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity
                newEntry.Size = fi.Length;
                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                zipStream.CloseEntry();
            }
            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
                CompressFolder(folder, zipStream, folderOffset);
        }

        private void KMZPOIMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(config.currentStatus == 1) config.currentStatus = 4;
            e.Cancel = (config.currentStatus == 1) || (config.currentStatus == 4) || (config.currentStatus == 5);

            if (!e.Cancel)
            {
                try
                {
                    if (Directory.Exists(config.tmpDir))
                        Directory.Delete(config.tmpDir, true);
                }
                catch { };
            };
        }

        private void props_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SetProperty(0);
        }

        private void props_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                SetProperty(0);
            if (e.KeyChar == ' ')
                SetProperty(1);
        }

        // 0 - dialog, +1 - list forward, -1 - list backward, 2 - erase
        private void SetProperty(int mode) 
        {
            if (in_progress) return;
            if (props.SelectedItems.Count == 0) return;
            int index = props.SelectedIndices[0];
            if (index < 0) return;

            //////////////////////////////////
            if ((index == 0) && (mode == 2))
            {
                config.importFileName = "";
                props.Items[0].SubItems[1].Text = "";
            };
            if ((index == 0) && (mode == 0))
            {
                string file = config.importFileName;
                if (InputBox.QueryFileBox("Select file", "Select file to Import:", ref file, "OSM PBF Files (*.pbf;*.osm)|*.pbf;*.osm") == DialogResult.OK)
                    if (File.Exists(file))
                    {
                        string ext = Path.GetExtension(file).ToLower();
                        if (ext == ".pbf")
                        {
                            OSMPBFReader fr = new OSMPBFReader(file);
                            if (!fr.ValidHeader)
                            {
                                fr.Close();
                                MessageBox.Show("Invalid OSMHeader", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            else
                            {
                                fr.Close();
                                config.importFileName = file;
                                props.Items[0].SubItems[1].Text = Path.GetFileName(file);
                                config.ReloadHierarchy();
                            };
                        };
                        if (ext == ".osm")
                        {
                            OSMXMLReader fr = new OSMXMLReader(file);
                            if (!fr.ValidHeader)
                            {
                                fr.Close();
                                MessageBox.Show("Invalid OSMHeader", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            else
                            {
                                fr.Close();
                                config.importFileName = file;
                                props.Items[0].SubItems[1].Text = Path.GetFileName(file);
                                config.ReloadHierarchy();
                            };
                        };
                    };
            };
            //////////////////////////////////
            if ((index == 1) && (mode == 2))
            {
                config.dictionaryFileName = "";
                props.Items[1].SubItems[1].Text = "";
                config.ReloadHierarchy();
            };
            if ((index == 1) && (mode == 0))
            {
                int si = 0;
                bool ex = false;
                string dir = AppDomain.CurrentDomain.BaseDirectory.Trim('\\') + @"\OSM\";
                List<string> options = new List<string>(new string[]{"[No File]"});
                if (Directory.Exists(dir))
                {
                    string[] files = Directory.GetFiles(dir, "*.json");
                    if((files != null) && (files.Length > 0))
                        foreach(string file in files)
                        {
                            string fn = Path.GetFileName(file);
                            if ((si == 0) && (fn.StartsWith("dict"))) si = options.Count;
                            if(File.Exists(config.dictionaryFileName))
                                if (Path.GetFileName(config.dictionaryFileName) == fn) { si = options.Count; ex = true; };
                            options.Add(fn);
                        };
                };
                if (!ex)
                {
                    if (File.Exists(config.dictionaryFileName))
                    {
                        si = options.Count;
                        options.Add(Path.GetFileName(config.dictionaryFileName));                        
                    };
                };
                options.Add("[Select...]");
                if (InputBox.Show("Dictionary", "Select Dictionary file:", options.ToArray(), ref si) == DialogResult.OK)
                {
                    if (si == 0) 
                        config.dictionaryFileName = "";
                    else if (si == (options.Count - 1))
                    {
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "JSON Files (*.json)|*.json";
                        if (ofd.ShowDialog() == DialogResult.OK)
                            config.dictionaryFileName = ofd.FileName;
                        ofd.Dispose();
                    }
                    else if ((si == (options.Count - 2)) && (!ex) && (File.Exists(config.dictionaryFileName)))
                    {

                    }
                    else
                        config.dictionaryFileName = dir + options[si];
                    props.Items[1].SubItems[1].Text = Path.GetFileName(config.dictionaryFileName);
                    config.ReloadHierarchy();
                };
            };
            //////////////////////////////////
            if ((index == 2) && (mode == 2))
            {
                config.catalogFileName = "";
                props.Items[2].SubItems[1].Text = "";
                config.ReloadHierarchy();
            };
            if ((index == 2) && (mode == 0))
            {
                int si = 0;
                bool ex = false;
                string dir = AppDomain.CurrentDomain.BaseDirectory.Trim('\\') + @"\OSM\";
                List<string> options = new List<string>(new string[] { "[No File]" });
                if (Directory.Exists(dir))
                {
                    string[] files = Directory.GetFiles(dir, "*.json");
                    if ((files != null) && (files.Length > 0))
                        foreach (string file in files)
                        {
                            string fn = Path.GetFileName(file);
                            if ((si == 0) && (fn.StartsWith("cat"))) si = options.Count;
                            if(File.Exists(config.catalogFileName))
                                if (Path.GetFileName(config.catalogFileName) == fn) { si = options.Count; ex = true; };
                            options.Add(fn);
                        };
                };
                if (!ex)
                {
                    if (File.Exists(config.catalogFileName))
                    {
                        si = options.Count;
                        options.Add(Path.GetFileName(config.catalogFileName));
                    };
                };
                options.Add("[Select...]");
                if (InputBox.Show("Catalog", "Select Catalog file:", options.ToArray(), ref si) == DialogResult.OK)
                {
                    if (si == 0)
                        config.catalogFileName = "";
                    else if (si == (options.Count - 1))
                    {
                        OpenFileDialog ofd = new OpenFileDialog();
                        ofd.Filter = "JSON Files (*.json)|*.json";
                        if (ofd.ShowDialog() == DialogResult.OK)
                            config.catalogFileName = ofd.FileName;
                        ofd.Dispose();
                    }
                    else if ((si == (options.Count - 2)) && (!ex) && (File.Exists(config.catalogFileName)))
                    {

                    }
                    else
                        config.catalogFileName = dir + options[si];
                    props.Items[2].SubItems[1].Text = Path.GetFileName(config.catalogFileName);
                    config.ReloadHierarchy();
                };
            };
            //////////////////////////////////
            if ((index == 3) && (mode == 2))
            {
                config.iconsFileName = "";
                props.Items[3].SubItems[1].Text = "";
            };
            if ((index == 3) && (mode == 0))
            {
                string file = config.iconsFileName;
                if (InputBox.QueryFileBox("Select file", "Select file with Icons:", ref file, "Zip Archive Files (*.zip)|*.zip") == DialogResult.OK)
                    if (File.Exists(file))
                    {
                        config.iconsFileName = file;
                        props.Items[3].SubItems[1].Text = Path.GetFileName(file);
                    };
            };
            //////////////////////////////////
            if (index == 4)
            {
                if (mode == 2)
                {
                    config.hierarchyIndex = 0;
                    props.Items[4].SubItems[1].Text = "0";
                    config.ReloadHierarchy();
                }
                else if (mode == 0)
                {
                    int val = config.hierarchyIndex;
                    if (InputBox.Show("Hierarchy Level:", "Select Hierarchy Level:", ref val, -10, 0) == DialogResult.OK)
                    {
                        config.hierarchyIndex = val;
                        if (config.hierarchyIndex > 0) config.hierarchyIndex = 0;
                        if (config.hierarchyIndex < -10) config.hierarchyIndex = -10;
                        props.Items[4].SubItems[1].Text = config.hierarchyIndex.ToString();
                        config.ReloadHierarchy();
                    };
                }
                else
                {
                    config.hierarchyIndex += mode;
                    if (config.hierarchyIndex > 0) config.hierarchyIndex = 0;
                    if (config.hierarchyIndex < -10) config.hierarchyIndex = -10;
                    props.Items[4].SubItems[1].Text = config.hierarchyIndex.ToString();
                    config.ReloadHierarchy();
                };
            };
            //////////////////////////////////
            if (index == 5)
            {
                if (mode == 2)
                    config.saveNoCategory = 0;
                else if (mode == 0)
                {
                    int new_cat = config.saveNoCategory;
                    if (InputBox.Show("Keep NoCategory", "Select Keep No Category:", Config.saveNoCategoryList, ref new_cat) == DialogResult.OK)
                        config.saveNoCategory = (byte)new_cat;
                }
                else
                {
                    int new_cat = config.saveNoCategory + mode;
                    if (new_cat >= Config.saveNoCategoryList.Length) new_cat = 0;
                    if (new_cat < 0) new_cat = Config.saveNoCategoryList.Length - 1;
                    config.saveNoCategory = (byte)new_cat;
                }; 
                props.Items[5].SubItems[1].Text = Config.saveNoCategoryList[config.saveNoCategory];;
            };
            //////////////////////////////////
            if (index == 6)
            {
                if (mode == 2)
                {
                    config.updateStatusEvery = 500;
                    props.Items[6].SubItems[1].Text = "500";
                }
                else if (mode == 0)
                {
                    int val = config.updateStatusEvery;
                    if (InputBox.Show("Update Interval:", "Select Update Interval in seconds:", ref val, 1, 60) == DialogResult.OK)
                    {
                        config.updateStatusEvery = val;
                        if (config.updateStatusEvery > 5000) config.updateStatusEvery = 5000;
                        if (config.updateStatusEvery < 1) config.updateStatusEvery = 1;
                        props.Items[6].SubItems[1].Text = config.updateStatusEvery.ToString();
                    };
                }
                else
                {
                    config.updateStatusEvery += mode;
                    if (config.updateStatusEvery > 5000) config.updateStatusEvery = 5000;
                    if (config.updateStatusEvery < 1) config.updateStatusEvery = 1;
                    props.Items[6].SubItems[1].Text = config.updateStatusEvery.ToString();
                };
            };
            //////////////////////////////////
            if (index == 7)
            {
                string[] options = new string[] { "No", "Yes" };
                if (mode == 2)
                {
                    config.onlyHasTags = true;
                    props.Items[7].SubItems[1].Text = config.onlyHasTags ? "Yes" : "No";
                }
                else if (mode == 0)
                {
                    int val = config.onlyHasTags ? 1 : 0;
                    if (InputBox.Show("Has Tag:", "Add Node only with Tags:", options, ref val) == DialogResult.OK)
                    {
                        config.onlyHasTags = val == 1;
                        props.Items[7].SubItems[1].Text = config.onlyHasTags ? "Yes" : "No";
                    };
                }
                else
                {
                    config.onlyHasTags = !config.onlyHasTags;
                    props.Items[7].SubItems[1].Text = config.onlyHasTags ? "Yes" : "No";
                };
            };
            //////////////////////////////////
            if (index == 8)
            {
                string[] options = new string[] { "No", "Yes" };
                if (mode == 2)
                {
                    config.onlyHasName = true;
                    props.Items[8].SubItems[1].Text = config.onlyHasName ? "Yes" : "No";
                }
                else if (mode == 0)
                {
                    int val = config.onlyHasName ? 1 : 0;
                    if (InputBox.Show("Has Name:", "Add Node only with Name:", options, ref val) == DialogResult.OK)
                    {
                        config.onlyHasName = val == 1;
                        props.Items[8].SubItems[1].Text = config.onlyHasName ? "Yes" : "No";
                    };
                }
                else
                {
                    config.onlyHasName = !config.onlyHasName;
                    props.Items[8].SubItems[1].Text = config.onlyHasName ? "Yes" : "No";
                };
            };
            //////////////////////////////////
            if (index == 9)
            {
                string[] options = new string[] { "No", "Yes" };
                if (mode == 2)
                {
                    config.fullCatName = true;
                    props.Items[9].SubItems[1].Text = config.fullCatName ? "Yes" : "No";
                    config.ReloadHierarchy();
                }
                else if (mode == 0)
                {
                    int val = config.fullCatName ? 1 : 0;
                    if (InputBox.Show("Has Name:", "Add Node only with Name:", options, ref val) == DialogResult.OK)
                    {
                        config.fullCatName = val == 1;
                        props.Items[9].SubItems[1].Text = config.fullCatName ? "Yes" : "No";
                        config.ReloadHierarchy();
                    };
                }
                else
                {
                    config.fullCatName = !config.fullCatName;
                    props.Items[9].SubItems[1].Text = config.fullCatName ? "Yes" : "No";
                    config.ReloadHierarchy();
                };
            };
            //////////////////////////////////
            if (index == 10)
            {
                if (mode == 2)
                {
                    config.onlyWithTags.Clear();
                    props.Items[10].SubItems[1].Text = "";
                };
                if (mode == 0)
                {
                    string txt = props.Items[10].SubItems[1].Text;
                    List<string> options = new List<string>();
                    options.Add("{key}");
                    options.Add("{key}={value}");
                    options.Add("{key},{key}");
                    options.Add("{key},{key}={value}");
                    if(txt.Length > 0)
                        if (options.IndexOf(txt) < 0) 
                            options.Add(txt);
                    if (InputBox.Show("Contains All Of Tags", "Enter Tag or Tag=Value list:", options.ToArray(), ref txt, true) == DialogResult.OK)
                    {
                        if (txt.Trim().Length == 0)
                        {
                            config.onlyWithTags.Clear();
                            props.Items[10].SubItems[1].Text = "";
                        }
                        else
                        {
                            string[] list = txt.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            config.onlyWithTags.Clear();
                            foreach (string li in list)
                            {
                                try
                                {
                                    string l = li.Trim().Trim('=');
                                    if (l.IndexOf("=") < 0)
                                        config.onlyWithTags.Add(l, null);
                                    else
                                    {
                                        string[] kv = l.Split(new char[] { '=' }, 2);
                                        config.onlyWithTags.Add(kv[0].Trim(), kv[1].Trim());
                                    };
                                }
                                catch { };
                            };
                            string tl = "";
                            foreach (KeyValuePair<string, string> kvp in config.onlyWithTags)
                            {
                                if (tl.Length > 0) tl += ",";
                                if ((kvp.Value == null) || (kvp.Value == ""))
                                    tl += kvp.Key;
                                else
                                    tl += String.Format("{0}={1}", kvp.Key, kvp.Value);
                            };
                            props.Items[10].SubItems[1].Text = tl;
                        };
                    };
                };
            };
            //////////////////////////////////
            if (index == 11)
            {
                if (mode == 2)
                {
                    config.onlyOneOfTags.Clear();
                    props.Items[11].SubItems[1].Text = "";
                };
                if (mode == 0)
                {
                    string txt = props.Items[11].SubItems[1].Text;
                    List<string> options = new List<string>();
                    options.Add("{key}");
                    options.Add("{key}={value}");
                    options.Add("{key},{key}");
                    options.Add("{key},{key}={value}");
                    if (txt.Length > 0)
                        if (options.IndexOf(txt) < 0)
                            options.Add(txt);
                    if (InputBox.Show("Contains Only One Of Tags", "Enter Tag or Tag=Value list:", options.ToArray(), ref txt, true) == DialogResult.OK)
                    {
                        if (txt.Trim().Length == 0)
                        {
                            config.onlyOneOfTags.Clear();
                            props.Items[11].SubItems[1].Text = "";
                        }
                        else
                        {
                            string[] list = txt.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            config.onlyOneOfTags.Clear();
                            foreach (string li in list)
                            {
                                try
                                {
                                    string l = li.Trim().Trim('=');
                                    if (l.IndexOf("=") < 0)
                                        config.onlyOneOfTags.Add(l, null);
                                    else
                                    {
                                        string[] kv = l.Split(new char[] { '=' }, 2);
                                        config.onlyOneOfTags.Add(kv[0].Trim(), kv[1].Trim());
                                    };
                                }
                                catch { };
                            };
                            string tl = "";
                            foreach (KeyValuePair<string, string> kvp in config.onlyOneOfTags)
                            {
                                if (tl.Length > 0) tl += ",";
                                if ((kvp.Value == null) || (kvp.Value == ""))
                                    tl += kvp.Key;
                                else
                                    tl += String.Format("{0}={1}", kvp.Key, kvp.Value);
                            };
                            props.Items[11].SubItems[1].Text = tl;
                        };
                    };
                };
            };
            //////////////////////////////////
            if (index == 12)
            {
                if (mode == 2)
                {
                    config.onlyMdfAfter = DateTime.MinValue;
                    props.Items[12].SubItems[1].Text = "";
                };
                if (mode == 0)
                {
                    string txt = "";
                    List<string> options = new List<string>();                    
                    options.Add(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                    options.Add("yyyy-MM-ddTHH:mm:ss");
                    if (config.onlyMdfAfter != DateTime.MinValue)
                        options.Add(txt = config.onlyMdfAfter.ToString("yyyy-MM-ddTHH:mm:ss"));
                    if (InputBox.Show("Only Modified After", "Enter DateTime To Filter After:", options.ToArray(), ref txt, true) == DialogResult.OK)
                    {
                        if(DateTime.TryParse(txt,out config.onlyMdfAfter))
                            props.Items[12].SubItems[1].Text = config.onlyMdfAfter.ToString("yyyy-MM-ddTHH:mm:ss");
                        else
                        {
                            config.onlyMdfAfter = DateTime.MinValue;
                            props.Items[12].SubItems[1].Text = "";
                        };
                    };
                };
            };
            //////////////////////////////////
            if (index == 13)
            {
                if (mode == 2)
                {
                    config.onlyMdfBefore = DateTime.MaxValue;
                    props.Items[13].SubItems[1].Text = "";
                };
                if (mode == 0)
                {
                    string txt = "";
                    List<string> options = new List<string>();
                    options.Add(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"));
                    options.Add("yyyy-MM-ddTHH:mm:ss");
                    if (config.onlyMdfBefore != DateTime.MaxValue)
                        options.Add(txt = config.onlyMdfBefore.ToString("yyyy-MM-ddTHH:mm:ss"));
                    if (InputBox.Show("Only Modified Before", "Enter DateTime To Filter Before:", options.ToArray(), ref txt, true) == DialogResult.OK)
                    {
                        if (DateTime.TryParse(txt, out config.onlyMdfBefore))
                            props.Items[13].SubItems[1].Text = config.onlyMdfBefore.ToString("yyyy-MM-ddTHH:mm:ss");
                        else
                        {
                            config.onlyMdfBefore = DateTime.MaxValue;
                            props.Items[13].SubItems[1].Text = "";
                        };
                    };
                };
            };
            //////////////////////////////////
            if (index == 14)
            {
                if (mode == 2)
                {
                    config.onlyOfUser.Clear();
                    props.Items[14].SubItems[1].Text = "";
                };
                if (mode == 0)
                {
                    string txt = props.Items[14].SubItems[1].Text;
                    List<string> options = new List<string>();
                    options.Add("{user}");
                    options.Add("{user},{user}");
                    if (txt.Length > 0)
                        if (options.IndexOf(txt) < 0)
                            options.Add(txt);
                    if (InputBox.Show("Modified One of User", "Enter Users List:", options.ToArray(), ref txt, true) == DialogResult.OK)
                    {
                        if (txt.Trim().Length == 0)
                        {
                            config.onlyOfUser.Clear();
                            props.Items[14].SubItems[1].Text = "";
                        }
                        else
                        {
                            string[] list = txt.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            config.onlyOfUser.Clear();
                            foreach (string li in list)
                                config.onlyOfUser.Add(li.Trim());
                            string tl = "";
                            foreach (string user in config.onlyOfUser)
                            {
                                if (tl.Length > 0) tl += ",";
                                tl += user;
                            };
                            props.Items[14].SubItems[1].Text = tl;
                        };
                    };
                };
            };
            //////////////////////////////////
            if (index == 15)
            {
                if (mode == 2)
                {
                    config.onlyVersion.Clear();
                    props.Items[15].SubItems[1].Text = "";
                };
                if (mode == 0)
                {
                    string txt = props.Items[15].SubItems[1].Text;
                    List<string> options = new List<string>();
                    options.Add("{id}");
                    options.Add("{id},{id}");
                    if (txt.Length > 0)
                        if (options.IndexOf(txt) < 0)
                            options.Add(txt);
                    if (InputBox.Show("Modified Version", "Enter Versions List:", options.ToArray(), ref txt, true) == DialogResult.OK)
                    {
                        if (txt.Trim().Length == 0)
                        {
                            config.onlyVersion.Clear();
                            props.Items[15].SubItems[1].Text = "";
                        }
                        else
                        {
                            string[] list = txt.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            config.onlyVersion.Clear();
                            try
                            {
                                foreach (string li in list)
                                    config.onlyVersion.Add(int.Parse(li.Trim()));
                            }
                            catch { };
                            string tl = "";
                            foreach (int ver in config.onlyVersion)
                            {
                                if (tl.Length > 0) tl += ",";
                                tl += ver.ToString();
                            };
                            props.Items[15].SubItems[1].Text = tl;
                        };
                    };
                };
            };
            //////////////////////////////////
            if (index == 16)
            {
                if (mode == 2)
                {
                    config.onlyInBox = null;
                    props.Items[16].SubItems[1].Text = "";
                };
                if (mode == 0)
                {
                    string txt = props.Items[16].SubItems[1].Text;
                    List<string> options = new List<string>();
                    options.Add("{bottom},{left},{top},{right}");
                    options.Add("{min_lat},{min_lon},{max_lat},{max_lon}");
                    options.Add("[Load From File...]");
                    if (txt.Length > 0)
                        if (options.IndexOf(txt) < 0)
                            options.Insert(0, txt);
                    if (InputBox.Show("Only In Box", "Enter Box Coordinates:", options.ToArray(), ref txt, true) == DialogResult.OK)
                    {
                        if (txt.Trim().Length == 0)
                        {
                            config.onlyInBox = null;
                            props.Items[16].SubItems[1].Text = "";
                        }
                        else
                        {
                            if (txt == "[Load From File...]")
                            {
                                LoadBoxFromShp();
                            }
                            else
                            {
                                string[] list = txt.Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (list.Length == 4)
                                {
                                    config.onlyInBox = new double[4];
                                    config.onlyInBox[0] = LatLonParser.Parse(list[0].Trim(), true);
                                    config.onlyInBox[1] = LatLonParser.Parse(list[1].Trim(), false);
                                    config.onlyInBox[2] = LatLonParser.Parse(list[2].Trim(), true);
                                    config.onlyInBox[3] = LatLonParser.Parse(list[3].Trim(), false);
                                    props.Items[16].SubItems[1].Text = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1},{2},{3}", new object[] { config.onlyInBox[0], config.onlyInBox[1], config.onlyInBox[2], config.onlyInBox[3] }); ;
                                }
                                else
                                {
                                    config.onlyInBox = null;
                                    props.Items[16].SubItems[1].Text = "";
                                };
                            };
                        };
                    };
                };
            };
            //////////////////////////////////
            if ((index == 17) && (mode == 2))
            {
                config.onlyInPolygon = "";
                props.Items[17].SubItems[1].Text = "";
            };
            if ((index == 17) && (mode == 0))
            {
                string file = config.onlyInPolygon;
                if (InputBox.QueryFileBox("Select file", "Select file with Polygon:", ref file, "KML & ESRI Shape files (*.kml;*.shp)|*.kml;*.shp") == DialogResult.OK)
                    if (File.Exists(file))
                    {
                        string ot;
                        try
                        {
                            PointF[] points = UTILS.loadpolyfull(file, out ot);
                            if ((points != null) && (points.Length > 2))
                            {
                                config.onlyInPolygon = file;
                                props.Items[17].SubItems[1].Text = Path.GetFileName(file);
                            }
                            else
                            {
                                MessageBox.Show("Could not Load Polygon File", "Inside Polygon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            };
                        }
                        catch
                        {
                            MessageBox.Show("Could not Load Polygon File", "Inside Polygon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        };                        
                    };
            };
            //////////////////////////////////
            if ((index == 18) && (mode == 2))
            {
                config.onlybyRoute = "";
                props.Items[18].SubItems[1].Text = "";
            };
            if ((index == 18) && (mode == 0))
            {
                string file = config.onlybyRoute;
                if (InputBox.QueryFileBox("Select file", "Select file with Route:", ref file, "KML, GPX & SHP files (*.kml;*.gpx;*.shp)|*.kml;*.gpx;*.shp") == DialogResult.OK)
                    if (File.Exists(file))
                    {
                        string ot;
                        try
                        {
                            PointF[] points = UTILS.loadroute(file, out ot);
                            if ((points != null) && (points.Length > 1))
                            {
                                config.onlybyRoute = file;
                                props.Items[18].SubItems[1].Text = Path.GetFileName(file);
                            }
                            else
                            {
                                MessageBox.Show("Could not Load Route File\r\nIncorrect Format", "Inside Polygon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            };
                        }
                        catch
                        {
                            MessageBox.Show("Could not Load Route File\r\nIncorrect Format", "Inside Polygon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        };
                    };
            };
            //////////////////////////////////
            if (index == 19)
            {
                if (mode == 2)
                {
                    config.onlybyRtDist = 1000;
                    props.Items[19].SubItems[1].Text = "1000";
                }
                else if (mode == 0)
                {
                    int val = config.onlybyRtDist;
                    if (InputBox.Show("Max Distance From Line", "Enter Max Distance in meters:", ref val, 15, 50000) == DialogResult.OK)
                    {
                        config.onlybyRtDist = val;
                        if (config.onlybyRtDist > 50000) config.onlybyRtDist = 50000;
                        if (config.onlybyRtDist < 15) config.onlybyRtDist = 15;
                        props.Items[19].SubItems[1].Text = config.onlybyRtDist.ToString();
                    };
                }
                else
                {
                    config.onlybyRtDist += mode;
                    if (config.onlybyRtDist > 5000) config.onlybyRtDist = 5000;
                    if (config.onlybyRtDist < 1) config.onlybyRtDist = 1;
                    props.Items[19].SubItems[1].Text = config.onlybyRtDist.ToString();
                };
            };
            //////////////////////////////////
            if (index == 20)
            {
                if (mode == 2)
                    config.onlybyRLOR = 0;
                else if (mode == 0)
                {
                    int new_cat = config.onlybyRLOR;
                    if (InputBox.Show("Keep NoCategory", "Select Keep No Category:", Config.onlybyRLORList, ref new_cat) == DialogResult.OK)
                        config.onlybyRLOR = (byte)new_cat;
                }
                else
                {
                    int new_cat = config.onlybyRLOR + mode;
                    if (new_cat >= Config.onlybyRLORList.Length) new_cat = 0;
                    if (new_cat < 0) new_cat = Config.onlybyRLORList.Length - 1;
                    config.onlybyRLOR = (byte)new_cat;
                };
                props.Items[20].SubItems[1].Text = Config.onlybyRLORList[config.onlybyRLOR]; ;
            };
            //////////////////////////////////
            if (index == 21)
            {
                string[] options = new string[] { "No", "Yes" };
                if (mode == 2)
                {
                    config.onlyOneCategory = true;
                    props.Items[21].SubItems[1].Text = config.onlyOneCategory ? "No" : "Yes";
                }
                else if (mode == 0)
                {
                    int val = config.onlyOneCategory ? 0 : 1;
                    if (InputBox.Show("Duplicate nodes", "Node can be in several categories:", options, ref val) == DialogResult.OK)
                    {
                        config.onlyOneCategory = val != 1;
                        props.Items[21].SubItems[1].Text = config.onlyOneCategory ? "No" : "Yes";
                    };
                }
                else
                {
                    config.onlyOneCategory = !config.onlyOneCategory;
                    props.Items[21].SubItems[1].Text = config.onlyOneCategory ? "No" : "Yes";
                };
            };
            //////////////////////////////////
            if (index == 22)
            {
                string[] options = new string[] { "No", "Yes" };
                if (mode == 2)
                {
                    config.onlyOneButValues = true;
                    props.Items[22].SubItems[1].Text = config.onlyOneButValues ? "No" : "Yes";
                }
                else if (mode == 0)
                {
                    int val = config.onlyOneButValues ? 0 : 1;
                    if (InputBox.Show("Duplicate nodes", "Node can be in several NoValuesButKeys:", options, ref val) == DialogResult.OK)
                    {
                        config.onlyOneButValues = val != 1;
                        props.Items[22].SubItems[1].Text = config.onlyOneButValues ? "No" : "Yes";
                    };
                }
                else
                {
                    config.onlyOneButValues = !config.onlyOneButValues;
                    props.Items[22].SubItems[1].Text = config.onlyOneButValues ? "No" : "Yes";
                };
            };
            //////////////////////////////////
            if (index == 23)
            {
                string[] options = new string[] { "No", "Yes" };
                if (mode == 2)
                {
                    config.onlyAllTags = true;
                    props.Items[23].SubItems[1].Text = config.onlyAllTags ? "Yes" : "No";
                }
                else if (mode == 0)
                {
                    int val = config.onlyAllTags ? 1 : 0;
                    if (InputBox.Show("All Catalog Tags", "Node Contains All Tags Of Catalog:", options, ref val) == DialogResult.OK)
                    {
                        config.onlyAllTags = val == 1;
                        props.Items[23].SubItems[1].Text = config.onlyAllTags ? "Yes" : "No";
                    };
                }
                else
                {
                    config.onlyAllTags = !config.onlyAllTags;
                    props.Items[23].SubItems[1].Text = config.onlyAllTags ? "Yes" : "No";
                };
            };
            //////////////////////////////////
            if (index == 24)
            {
                if (mode == 2)
                {
                    config.hasText = "";
                    props.Items[24].SubItems[1].Text = config.hasText;
                }
                else if (mode == 0)
                {
                    string txt = config.hasText;
                    List<string> options  = new List<string>();
                    options.Add("\\bsubway\\b");
                    options.Add("\\b(high|speed|road)*way\\b");
                    options.Add("\\bметро\\b");
                    options.Add("\\bметро(политен)*\\b");
                    if(!String.IsNullOrEmpty(txt)) options.Add(txt);
                    InputBox.defWidth = 500;
                    if (InputBox.QueryRegexBox("Node contains text", "Node contains tag or tag with text:", "You can test here:", options.ToArray(), ref txt, true) == DialogResult.OK)
                    {
                        config.hasText = txt == "" ? null : txt.ToLower();
                        props.Items[24].SubItems[1].Text = config.hasText;
                    };
                    InputBox.defWidth = 300;
                };
            };
            //////////////////////////////////
        }

        private void LoadBoxFromShp()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Shape Files (*.shp)|*.shp";
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                ofd.Dispose();
                return;
            };
            try
            {
                int tof = 0;
                PointF[] poly = UTILS.loadPoly(ofd.FileName, out tof);
                if ((poly != null) && (poly.Length > 0))
                {
                    config.onlyInBox = new double[] { double.MaxValue, double.MaxValue, double.MinValue, double.MinValue };
                    for (int i = 0; i < poly.Length; i++)
                    {
                        if (poly[i].Y < config.onlyInBox[0]) config.onlyInBox[0] = poly[i].Y;
                        if (poly[i].X < config.onlyInBox[1]) config.onlyInBox[1] = poly[i].X;
                        if (poly[i].Y > config.onlyInBox[2]) config.onlyInBox[2] = poly[i].Y;
                        if (poly[i].X > config.onlyInBox[3]) config.onlyInBox[3] = poly[i].X;
                    };
                    props.Items[16].SubItems[1].Text = String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.000000},{1:0.000000},{2:0.000000},{3:0.000000}", new object[] { config.onlyInBox[0], config.onlyInBox[1], config.onlyInBox[2], config.onlyInBox[3] }); ;
                };
            }
            catch { };
            ofd.Dispose();
        }

        private void props_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 46)
                SetProperty(2);
            if (e.KeyValue == 37)
                SetProperty(-1);
            if (e.KeyValue == 39)
                SetProperty(+1);
        }


        private void runBtn_Click(object sender, EventArgs e)
        {
            if ((config.currentStatus == 0) || (config.currentStatus == 2) || (config.currentStatus == 3) || (config.currentStatus == 6))
                ReadFileData();            
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            if (config.currentStatus == 1)
                config.currentStatus = 4;
        }

        private void checkNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (catView.Items.Count == 0) return;
            for (int i = 0; i < catView.Items.Count; i++)
                catView.Items[i].Checked = false;
        }

        private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (catView.Items.Count == 0) return;
            for (int i = 0; i < catView.Items.Count; i++)
                catView.Items[i].Checked = true;
        }

        private void inverseCheckingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (catView.Items.Count == 0) return;
            for (int i = 0; i < catView.Items.Count; i++)
                catView.Items[i].Checked = !catView.Items[i].Checked;
        }

        private void mnu2_Opening(object sender, CancelEventArgs e)
        {   
            ladoConfigToolStripMenuItem.Enabled = saveConfigToolStripMenuItem.Enabled =
                read2.Enabled =  runBtn.Enabled = 
                    (config.currentStatus == 0) || (config.currentStatus == 2) || (config.currentStatus == 3) || (config.currentStatus == 6);
            stop2.Enabled =  stopBtn.Enabled = config.currentStatus == 1;
            bSaveCH1.Enabled = ((config.currentStatus == 2) || (config.currentStatus == 3) || (config.currentStatus == 6)) && (config.total_nodes > 0) && (catView.CheckedItems.Count > 0);
            bSaveCHX.Enabled = ((config.currentStatus == 2) || (config.currentStatus == 3) || (config.currentStatus == 6)) && (config.total_nodes > 0) && (catView.CheckedItems.Count > 0);            
            bSaveALL1.Enabled = ((config.currentStatus == 2) || (config.currentStatus == 3) || (config.currentStatus == 6)) && (config.total_nodes > 0);
            bSaveALLX.Enabled = ((config.currentStatus == 2) || (config.currentStatus == 3) || (config.currentStatus == 6)) && (config.total_nodes > 0);

            runBtn.Font = read2.Font = new Font(read2.Font, File.Exists(config.importFileName) && read2.Enabled ? FontStyle.Bold : FontStyle.Regular);
            stopBtn.Font = stop2.Font = new Font(stop2.Font, stop2.Enabled ? FontStyle.Bold : FontStyle.Regular);

            openTemporaryFileToolStripMenuItem.Enabled = (catView.SelectedIndices.Count > 0) && (catView.SelectedItems[0].SubItems[3].Text != "0") && ((config.currentStatus == 0) || (config.currentStatus == 2) || (config.currentStatus == 3) || (config.currentStatus == 6));
        }

        public void SaveResultTo(bool only_checked, bool multiple)
        {
            if (config.currentStatus == 1) return;
            if (config.total_nodes == 0) return;
            if (only_checked && (catView.CheckedItems.Count == 0)) return;            

            List<int> indicies = new List<int>();
            if (only_checked)
            {
                for (int i = 0; i < catView.CheckedItems.Count; i++)
                    if (config.osmHier[catView.CheckedIndices[i]].nodes > 0)
                        indicies.Add(catView.CheckedIndices[i]);
            }
            else
            {
                for (int i = 0; i < catView.Items.Count; i++)
                    if (config.osmHier[i].nodes > 0)
                        indicies.Add(i);
            };

            if (indicies.Count == 0) return;

            string saveTo = "";
            if (!multiple)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ".kmz";
                sfd.Filter = "KMZ Files (*.kmz)|*.kmz";
                if (sfd.ShowDialog() != DialogResult.OK) return;
                saveTo = sfd.FileName;
                sfd.Dispose();
            }
            else
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() != DialogResult.OK) return;
                saveTo = fbd.SelectedPath.Trim('\0') + @"\";   
                fbd.Dispose();
            };

            CopyIcons();

            string tmpfile = config.tmpDir + "doc_tmp.kml";
            SetPropsEnables(false);
            if (!multiple)
            {
                long added = 0;
                UpdateStatus(0, indicies.Count, "Saving Result File");
                try
                {
                    FileStream fs = new FileStream(tmpfile, FileMode.Create, FileAccess.Write);
                    byte[] buff;
                    // WRITE HEADER
                    buff = config.osmHier.GetFileHeader("OSM KMZ POI");
                    fs.Write(buff, 0, buff.Length);
                    // WRITE FOLDERS
                    for (int i = 0; i < indicies.Count; i++)
                    {
                        UpdateStatus(i, indicies.Count, "Saving Result File");
                        buff = config.fullCatName ? config.osmHier[indicies[i]].FolderHeaderFull : config.osmHier[indicies[i]].FolderHeader;
                        fs.Write(buff, 0, buff.Length);
                        FileStream fc = new FileStream(config.tmpDir + "folder" + indicies[i].ToString() + ".kml", FileMode.Open, FileAccess.Read);
                        buff = new byte[ushort.MaxValue];
                        int len = 0;
                        while ((len = fc.Read(buff, 0, buff.Length)) > 0)
                            fs.Write(buff, 0, len);
                        fc.Close();
                        buff = config.osmHier[indicies[i]].FolderFooter;
                        fs.Write(buff, 0, buff.Length);
                        added += config.osmHier[indicies[i]].nodes;
                    };
                    UpdateStatus(indicies.Count, indicies.Count, "Saving Result File");
                    // WRITE STYLES
                    buff = config.osmHier.GetFileStyles(config.iconList.ToArray());
                    fs.Write(buff, 0, buff.Length);
                    // WRITE FOOTER
                    buff = config.osmHier.GetFileFooter();
                    fs.Write(buff, 0, buff.Length);
                    // END
                    fs.Close();
                    // MAKE ZIP
                    UpdateStatus(0, 1, "Making Zip File");
                    MakeKMZWithAllIcons(saveTo, tmpfile);
                    status3.Text = "Zip file " + Path.GetFileName(saveTo) + " with " + added.ToString() + " placemarks created sucessfull";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Creating KMZ File:\r\n" + ex.Message, "Saving Result File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
            }
            else
            {
                int files_created = 0;
                long added = 0;
                for (int i = 0; i < indicies.Count; i++)
                {
                    try
                    {
                        UpdateStatus(i, indicies.Count, "Saving Result Files");
                        FileStream fs = new FileStream(tmpfile, FileMode.Create, FileAccess.Write);
                        byte[] buff;
                        // WRITE HEADER
                        buff = config.fullCatName ? config.osmHier[indicies[i]].FileHeaderFull : config.osmHier[indicies[i]].FileHeader;
                        fs.Write(buff, 0, buff.Length);
                        // WRITE FOLDER
                        buff = config.fullCatName ? config.osmHier[indicies[i]].FolderHeaderFull : config.osmHier[indicies[i]].FolderHeader;
                        fs.Write(buff, 0, buff.Length);
                        // FRITE NODES
                        FileStream fc = new FileStream(config.tmpDir + "folder" + indicies[i].ToString() + ".kml", FileMode.Open, FileAccess.Read);
                        buff = new byte[ushort.MaxValue];
                        int len = 0;
                        while ((len = fc.Read(buff, 0, buff.Length)) > 0)
                            fs.Write(buff, 0, len);
                        fc.Close();
                        // FRITE FOOTER
                        buff = config.osmHier[indicies[i]].FolderFooter;
                        fs.Write(buff, 0, buff.Length);
                        // WRITE STYLE
                        buff = config.osmHier[indicies[i]].FileStyles;
                        fs.Write(buff, 0, buff.Length);
                        // WRITE FOOTER
                        buff = config.osmHier[indicies[i]].FileFooter;
                        fs.Write(buff, 0, buff.Length);
                        fs.Close();
                        // MAKE ZIP
                        UpdateStatus(i, indicies.Count, "Create Zip Files");
                        string fileName = saveTo + config.osmHier[indicies[i]].fullname.Replace(@"\", "-").Replace(":", "=") + ".kmz";
                        MakeKMZWithCustomIcons(fileName, tmpfile, config.osmHier[indicies[i]].iconList );
                        files_created++;
                        added += config.osmHier[indicies[i]].nodes;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error Creating KMZ File:\r\n" + ex.Message, "Saving Result File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    };
                };
                status3.Text = files_created.ToString() + " zip files with total " + added + " placemarks created sucessfull";
            };
            SetPropsEnables(true);
        }

        private void bSaveCH1_Click(object sender, EventArgs e)
        {
            SaveResultTo(true, false);
        }

        private void bSaveCHX_Click(object sender, EventArgs e)
        {
            SaveResultTo(true, true);
        }

        private void bSaveALL1_Click(object sender, EventArgs e)
        {
            SaveResultTo(false, false);
        }

        private void bSaveALLX_Click(object sender, EventArgs e)
        {
            SaveResultTo(false, true);
        }

        private void uncheckEmptiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (catView.Items.Count == 0) return;
            for (int i = 0; i < catView.Items.Count; i++)
                if(config.osmHier[i].nodes == 0)
                    catView.Items[i].Checked = false;
        }

        private void ladoConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Config files (*.cpf)|*.cpf";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                config = Config.Load(ofd.FileName, true);
                config.onReloadHierarchy += new EventHandler(config_onReloadHierarchy);
                config.onReloadProperties += new EventHandler(config_onReloadProperties);
                config.ReloadProperties();
                config.ReloadHierarchy();
            };
            ofd.Dispose();
        }

        private void saveConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfdd = new SaveFileDialog();
            sfdd.DefaultExt = ".cpf";
            sfdd.Filter = "Config files (*.cpf)|*.cpf";
            if (sfdd.ShowDialog() == DialogResult.OK)
                config.Save(sfdd.FileName);
            sfdd.Dispose();
        }

        private void openTemporaryFileToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            if ((catView.SelectedIndices.Count > 0) && (catView.SelectedItems[0].SubItems[3].Text != "0") && ((config.currentStatus == 0) || (config.currentStatus == 2) || (config.currentStatus == 3) || (config.currentStatus == 6)))
            {
                string file = config.tmpDir + "folder" + catView.SelectedIndices[0].ToString() + ".kml";                
                try
                {
                    Rectangle rect = catView.GetItemRect(catView.SelectedIndices[0]);
                    GongSolutions.Shell.ShellItem si = new GongSolutions.Shell.ShellItem(file);
                    GongSolutions.Shell.ShellContextMenu scm = new GongSolutions.Shell.ShellContextMenu(si);
                    scm.ShowContextMenu((Control)catView, new Point(rect.Left + 100, rect.Top + rect.Height - 2));
                }
                catch {  };
            };
        }    
    }

    public class UTILS
    {
        public static PointF[] loadPoly(string filename, out int tof)
        {
            List<PointF> result = new List<PointF>();

            tof = 0;

            System.IO.FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            System.IO.StreamReader sr = new StreamReader(fs);

            if (System.IO.Path.GetExtension(filename).ToLower() == ".shp")
            {
                fs.Position = 32;
                tof = fs.ReadByte();
                if ((tof == 3) || (tof == 5))
                {
                    fs.Position = 104;
                    byte[] ba = new byte[4];
                    fs.Read(ba, 0, ba.Length);
                    if (BitConverter.IsLittleEndian) Array.Reverse(ba);
                    int len = BitConverter.ToInt32(ba, 0) * 2;
                    fs.Read(ba, 0, ba.Length);
                    if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                    tof = BitConverter.ToInt32(ba, 0);
                    if ((tof == 3) || (tof == 5))
                    {
                        fs.Position += 32;
                        fs.Read(ba, 0, ba.Length);
                        if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                        if (BitConverter.ToInt32(ba, 0) == 1)
                        {
                            fs.Read(ba, 0, ba.Length);
                            if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                            int pCo = BitConverter.ToInt32(ba, 0);
                            fs.Read(ba, 0, ba.Length);
                            if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                            if (BitConverter.ToInt32(ba, 0) == 0)
                            {
                                ba = new byte[8];
                                for (int i = 0; i < pCo; i++)
                                {
                                    PointF ap = new PointF();
                                    fs.Read(ba, 0, ba.Length);
                                    if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                                    ap.X = (float)BitConverter.ToDouble(ba, 0);
                                    fs.Read(ba, 0, ba.Length);
                                    if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                                    ap.Y = (float)BitConverter.ToDouble(ba, 0);
                                    result.Add(ap);
                                };
                            };
                        };
                    };
                };
            };
            sr.Close();
            fs.Close();
            return result.ToArray();
        }

        public static PointF[] loadpolyfull(string filename, out string ftype)
        {
            ftype = "shp";

            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.InstalledUICulture;
            System.Globalization.NumberFormatInfo ni = (System.Globalization.NumberFormatInfo)ci.NumberFormat.Clone();
            ni.NumberDecimalSeparator = ".";

            List<PointF> points = new List<PointF>();

            System.IO.FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            if (Path.GetExtension(filename).ToLower() == ".kml")
            {
                ftype = "kml";
                System.IO.StreamReader sr = new StreamReader(fs);
                {
                    string file = sr.ReadToEnd();
                    int si = file.IndexOf("<coordinates>");
                    int ei = file.IndexOf("</coordinates>");
                    string co = file.Substring(si + 13, ei - si - 13).Trim().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
                    string[] arr = co.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    if ((arr != null) && (arr.Length > 0))
                        for (int i = 0; i < arr.Length; i++)
                        {
                            string[] xyz = arr[i].Split(new string[] { "," }, StringSplitOptions.None);
                            points.Add(new PointF(float.Parse(xyz[0], ni), float.Parse(xyz[1], ni)));
                        };
                };
                sr.Close();
            };
            if (Path.GetExtension(filename).ToLower() == ".shp")
            {
                ftype = "shp";
                fs.Position = 32;
                int tof = fs.ReadByte();
                if ((tof == 5))
                {
                    fs.Position = 104;
                    byte[] ba = new byte[4];
                    fs.Read(ba, 0, ba.Length);
                    if (BitConverter.IsLittleEndian) Array.Reverse(ba);
                    int len = BitConverter.ToInt32(ba, 0) * 2;
                    fs.Read(ba, 0, ba.Length);
                    if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                    tof = BitConverter.ToInt32(ba, 0);
                    if ((tof == 5))
                    {
                        fs.Position += 32;
                        fs.Read(ba, 0, ba.Length);
                        if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                        if (BitConverter.ToInt32(ba, 0) == 1)
                        {
                            fs.Read(ba, 0, ba.Length);
                            if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                            int pCo = BitConverter.ToInt32(ba, 0);
                            fs.Read(ba, 0, ba.Length);
                            if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                            if (BitConverter.ToInt32(ba, 0) == 0)
                            {
                                ba = new byte[8];
                                for (int i = 0; i < pCo; i++)
                                {
                                    PointF ap = new PointF();
                                    fs.Read(ba, 0, ba.Length);
                                    if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                                    ap.X = (float)BitConverter.ToDouble(ba, 0);
                                    fs.Read(ba, 0, ba.Length);
                                    if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                                    ap.Y = (float)BitConverter.ToDouble(ba, 0);
                                    points.Add(ap);
                                };
                            };
                        };
                    };
                };
            };
            fs.Close();

            return points.ToArray();
        }

        public static PointF[] loadroute(string filename, out string ftype)
        {
            ftype = "kml";

            List<PointF> LoadedRoute = new List<PointF>();

            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.InstalledUICulture;
            System.Globalization.NumberFormatInfo ni = (System.Globalization.NumberFormatInfo)ci.NumberFormat.Clone();
            ni.NumberDecimalSeparator = ".";
            
            System.IO.FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            System.IO.StreamReader sr = new StreamReader(fs);
            if (System.IO.Path.GetExtension(filename).ToLower() == ".kml")
            {
                ftype = "kml";
                string file = sr.ReadToEnd();
                int si = file.IndexOf("<coordinates>");
                int ei = file.IndexOf("</coordinates>");
                string co = file.Substring(si + 13, ei - si - 13).Trim().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
                string[] arr = co.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if ((arr != null) && (arr.Length > 0))
                    for (int i = 0; i < arr.Length; i++)
                    {
                        string[] xyz = arr[i].Split(new string[] { "," }, StringSplitOptions.None);
                        LoadedRoute.Add(new PointF(float.Parse(xyz[0], ni), float.Parse(xyz[1], ni)));
                    };
            };
            if (System.IO.Path.GetExtension(filename).ToLower() == ".gpx")
            {
                ftype = "gpx";
                string file = sr.ReadToEnd();
                int si = 0;
                int ei = 0;
                si = file.IndexOf("<rtept", ei);
                ei = file.IndexOf(">", si);
                while (si > 0)
                {
                    string rtept = file.Substring(si + 7, ei - si - 7).Replace("\"", "").Replace("/", "").Trim();
                    int ssi = rtept.IndexOf("lat=");
                    int sse = rtept.IndexOf(" ", ssi);
                    if (sse < 0) sse = rtept.Length;
                    string lat = rtept.Substring(ssi + 4, sse - ssi - 4);
                    ssi = rtept.IndexOf("lon=");
                    sse = rtept.IndexOf(" ", ssi);
                    if (sse < 0) sse = rtept.Length;
                    string lon = rtept.Substring(ssi + 4, sse - ssi - 4);
                    LoadedRoute.Add(new PointF(float.Parse(lon, ni), float.Parse(lat, ni)));

                    si = file.IndexOf("<rtept", ei);
                    if (si > 0)
                        ei = file.IndexOf(">", si);
                };
            };
            if (Path.GetExtension(filename).ToLower() == ".shp")
            {
                ftype = "shp";
                fs.Position = 32;
                int tof = fs.ReadByte();
                if ((tof == 3))
                {
                    fs.Position = 104;
                    byte[] ba = new byte[4];
                    fs.Read(ba, 0, ba.Length);
                    if (BitConverter.IsLittleEndian) Array.Reverse(ba);
                    int len = BitConverter.ToInt32(ba, 0) * 2;
                    fs.Read(ba, 0, ba.Length);
                    if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                    tof = BitConverter.ToInt32(ba, 0);
                    if ((tof == 3))
                    {
                        fs.Position += 32;
                        fs.Read(ba, 0, ba.Length);
                        if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                        if (BitConverter.ToInt32(ba, 0) == 1)
                        {
                            fs.Read(ba, 0, ba.Length);
                            if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                            int pCo = BitConverter.ToInt32(ba, 0);
                            fs.Read(ba, 0, ba.Length);
                            if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                            if (BitConverter.ToInt32(ba, 0) == 0)
                            {
                                ba = new byte[8];
                                for (int i = 0; i < pCo; i++)
                                {
                                    PointF ap = new PointF();
                                    fs.Read(ba, 0, ba.Length);
                                    if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                                    ap.X = (float)BitConverter.ToDouble(ba, 0);
                                    fs.Read(ba, 0, ba.Length);
                                    if (!BitConverter.IsLittleEndian) Array.Reverse(ba);
                                    ap.Y = (float)BitConverter.ToDouble(ba, 0);
                                    LoadedRoute.Add(ap);
                                };
                            };
                        };
                    };
                };
            };
            sr.Close();
            fs.Close();

            return LoadedRoute.ToArray();
        }


        public static bool PointInRoute(PointF point, PointF[] route, double MaxDistInMeter, int LeftOrRight)
        {
            bool skip = true;

            double x = point.X;
            double y = point.Y;
            
            double length2 = double.MaxValue;
            int side2 = 0;
            for (int i = 1; i < route.Length; i++)
            {
                PointF op;
                int side;
                double d = DistanceFromPointToLine(new PointF((float)x, (float)y), route[i - 1], route[i], out op, out side);
                if (d < length2)
                {
                    length2 = d;
                    side2 = side;
                };
            };

            if (length2 <= MaxDistInMeter)
            {
                if (LeftOrRight < 1)
                    skip = false;
                else
                    if ((LeftOrRight == 1) && (side2 <= 0))
                        skip = false;
                    else
                        if ((LeftOrRight == 2) && (side2 > 0)) skip = false;
            };

            return !skip;
        }

        public static bool PointInPolygon(PointF point, PointF[] polygon)
        {
            return PointInPolygon(point, polygon, 1E-09);
        }

        public static bool PointInPolygon(PointF point, PointF[] polygon, double EPS)
        {
            int count, up;
            count = 0;
            for (int i = 0; i < polygon.Length - 1; i++)
            {
                up = CRS(point, polygon[i], polygon[i + 1], EPS);
                if (up >= 0)
                    count += up;
                else
                    break;
            };
            up = CRS(point, polygon[polygon.Length - 1], polygon[0], EPS);
            if (up >= 0)
                return Convert.ToBoolean((count + up) & 1);
            else
                return false;
        }

        private static int CRS(PointF P, PointF A1, PointF A2, double EPS)
        {
            double x;
            int res = 0;
            if (Math.Abs(A1.Y - A2.Y) < EPS)
            {
                if ((Math.Abs(P.Y - A1.Y) < EPS) && ((P.X - A1.X) * (P.X - A2.X) < 0.0)) res = -1;
                return res;
            };
            if ((A1.Y - P.Y) * (A2.Y - P.Y) > 0.0) return res;
            x = A2.X - (A2.Y - P.Y) / (A2.Y - A1.Y) * (A2.X - A1.X);
            if (Math.Abs(x - P.X) < EPS)
            {
                res = -1;
            }
            else
            {
                if (x < P.X)
                {
                    res = 1;
                    if ((Math.Abs(A1.Y - P.Y) < EPS) && (A1.Y < A2.Y)) res = 0;
                    else
                        if ((Math.Abs(A2.Y - P.Y) < EPS) && (A2.Y < A1.Y)) res = 0;
                };
            };
            return res;
        }

        private static float DistanceFromPointToLine(PointF pt, PointF lineStart, PointF lineEnd, out PointF pointOnLine, out int side)
        {
            float dx = lineEnd.X - lineStart.X;
            float dy = lineEnd.Y - lineStart.Y;

            if ((dx == 0) && (dy == 0))
            {
                // line is a point
                // линия может быть с нулевой длиной после анализа TRA
                pointOnLine = lineStart;
                side = 0;
                //dx = pt.X - lineStart.X;
                //dy = pt.Y - lineStart.Y;                
                //return Math.Sqrt(dx * dx + dy * dy);
                return GetLengthMeters(pt.Y, pt.X, pointOnLine.Y, pointOnLine.X, false);
            };

            side = Math.Sign((lineEnd.X - lineStart.X) * (pt.Y - lineStart.Y) - (lineEnd.Y - lineStart.Y) * (pt.X - lineStart.X));

            // Calculate the t that minimizes the distance.
            float t = ((pt.X - lineStart.X) * dx + (pt.Y - lineStart.Y) * dy) / (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                pointOnLine = new PointF(lineStart.X, lineStart.Y);
                dx = pt.X - lineStart.X;
                dy = pt.Y - lineStart.Y;
            }
            else if (t > 1)
            {
                pointOnLine = new PointF(lineEnd.X, lineEnd.Y);
                dx = pt.X - lineEnd.X;
                dy = pt.Y - lineEnd.Y;
            }
            else
            {
                pointOnLine = new PointF(lineStart.X + t * dx, lineStart.Y + t * dy);
                dx = pt.X - pointOnLine.X;
                dy = pt.Y - pointOnLine.Y;
            };

            //return Math.Sqrt(dx * dx + dy * dy);
            return GetLengthMeters(pt.Y, pt.X, pointOnLine.Y, pointOnLine.X, false);
        }

        // Рассчет расстояния       
        #region LENGTH
        public static float GetLengthMeters(double StartLat, double StartLong, double EndLat, double EndLong, bool radians)
        {
            // use fastest
            float result = (float)GetLengthMetersD(StartLat, StartLong, EndLat, EndLong, radians);

            if (float.IsNaN(result))
            {
                result = (float)GetLengthMetersC(StartLat, StartLong, EndLat, EndLong, radians);
                if (float.IsNaN(result))
                {
                    result = (float)GetLengthMetersE(StartLat, StartLong, EndLat, EndLong, radians);
                    if (float.IsNaN(result))
                        result = 0;
                };
            };

            return result;
        }

        // Slower
        public static uint GetLengthMetersA(double StartLat, double StartLong, double EndLat, double EndLong, bool radians)
        {
            double D2R = Math.PI / 180;     // Преобразование градусов в радианы

            double a = 6378137.0000;     // WGS-84 Equatorial Radius (a)
            double f = 1 / 298.257223563;  // WGS-84 Flattening (f)
            double b = (1 - f) * a;      // WGS-84 Polar Radius
            double e2 = (2 - f) * f;      // WGS-84 Квадрат эксцентричности эллипсоида  // 1-(b/a)^2

            // Переменные, используемые для вычисления смещения и расстояния
            double fPhimean;                           // Средняя широта
            double fdLambda;                           // Разница между двумя значениями долготы
            double fdPhi;                           // Разница между двумя значениями широты
            double fAlpha;                           // Смещение
            double fRho;                           // Меридианский радиус кривизны
            double fNu;                           // Поперечный радиус кривизны
            double fR;                           // Радиус сферы Земли
            double fz;                           // Угловое расстояние от центра сфероида
            double fTemp;                           // Временная переменная, использующаяся в вычислениях

            // Вычисляем разницу между двумя долготами и широтами и получаем среднюю широту
            // предположительно что расстояние между точками << радиуса земли
            if (!radians)
            {
                fdLambda = (StartLong - EndLong) * D2R;
                fdPhi = (StartLat - EndLat) * D2R;
                fPhimean = ((StartLat + EndLat) / 2) * D2R;
            }
            else
            {
                fdLambda = StartLong - EndLong;
                fdPhi = StartLat - EndLat;
                fPhimean = (StartLat + EndLat) / 2;
            };

            // Вычисляем меридианные и поперечные радиусы кривизны средней широты
            fTemp = 1 - e2 * (sqr(Math.Sin(fPhimean)));
            fRho = (a * (1 - e2)) / Math.Pow(fTemp, 1.5);
            fNu = a / (Math.Sqrt(1 - e2 * (Math.Sin(fPhimean) * Math.Sin(fPhimean))));

            // Вычисляем угловое расстояние
            if (!radians)
            {
                fz = Math.Sqrt(sqr(Math.Sin(fdPhi / 2.0)) + Math.Cos(EndLat * D2R) * Math.Cos(StartLat * D2R) * sqr(Math.Sin(fdLambda / 2.0)));
            }
            else
            {
                fz = Math.Sqrt(sqr(Math.Sin(fdPhi / 2.0)) + Math.Cos(EndLat) * Math.Cos(StartLat) * sqr(Math.Sin(fdLambda / 2.0)));
            };
            fz = 2 * Math.Asin(fz);

            // Вычисляем смещение
            if (!radians)
            {
                fAlpha = Math.Cos(EndLat * D2R) * Math.Sin(fdLambda) * 1 / Math.Sin(fz);
            }
            else
            {
                fAlpha = Math.Cos(EndLat) * Math.Sin(fdLambda) * 1 / Math.Sin(fz);
            };
            fAlpha = Math.Asin(fAlpha);

            // Вычисляем радиус Земли
            fR = (fRho * fNu) / (fRho * sqr(Math.Sin(fAlpha)) + fNu * sqr(Math.Cos(fAlpha)));
            // Получаем расстояние
            return (uint)Math.Round(Math.Abs(fz * fR));
        }
        // Slowest
        public static uint GetLengthMetersB(double StartLat, double StartLong, double EndLat, double EndLong, bool radians)
        {
            double fPhimean, fdLambda, fdPhi, fAlpha, fRho, fNu, fR, fz, fTemp, Distance,
                D2R = Math.PI / 180,
                a = 6378137.0,
                e2 = 0.006739496742337;
            if (radians) D2R = 1;

            fdLambda = (StartLong - EndLong) * D2R;
            fdPhi = (StartLat - EndLat) * D2R;
            fPhimean = (StartLat + EndLat) / 2.0 * D2R;

            fTemp = 1 - e2 * Math.Pow(Math.Sin(fPhimean), 2);
            fRho = a * (1 - e2) / Math.Pow(fTemp, 1.5);
            fNu = a / Math.Sqrt(1 - e2 * Math.Sin(fPhimean) * Math.Sin(fPhimean));

            fz = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(fdPhi / 2.0), 2) +
              Math.Cos(EndLat * D2R) * Math.Cos(StartLat * D2R) * Math.Pow(Math.Sin(fdLambda / 2.0), 2)));
            fAlpha = Math.Asin(Math.Cos(EndLat * D2R) * Math.Sin(fdLambda) / Math.Sin(fz));
            fR = fRho * fNu / (fRho * Math.Pow(Math.Sin(fAlpha), 2) + fNu * Math.Pow(Math.Cos(fAlpha), 2));
            Distance = fz * fR;

            return (uint)Math.Round(Distance);
        }
        // Average
        public static uint GetLengthMetersC(double StartLat, double StartLong, double EndLat, double EndLong, bool radians)
        {
            double D2R = Math.PI / 180;
            if (radians) D2R = 1;
            double dDistance = Double.MinValue;
            double dLat1InRad = StartLat * D2R;
            double dLong1InRad = StartLong * D2R;
            double dLat2InRad = EndLat * D2R;
            double dLong2InRad = EndLong * D2R;

            double dLongitude = dLong2InRad - dLong1InRad;
            double dLatitude = dLat2InRad - dLat1InRad;

            // Intermediate result a.
            double a = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                       Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) *
                       Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

            // Intermediate result c (great circle distance in Radians).
            double c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));

            const double kEarthRadiusKms = 6378137.0000;
            dDistance = kEarthRadiusKms * c;

            return (uint)Math.Round(dDistance);
        }
        // Fastest
        public static double GetLengthMetersD(double sLat, double sLon, double eLat, double eLon, bool radians)
        {
            double EarthRadius = 6378137.0;

            double lon1 = radians ? sLon : DegToRad(sLon);
            double lon2 = radians ? eLon : DegToRad(eLon);
            double lat1 = radians ? sLat : DegToRad(sLat);
            double lat2 = radians ? eLat : DegToRad(eLat);

            return EarthRadius * (Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2)));
        }
        // Fastest
        public static double GetLengthMetersE(double sLat, double sLon, double eLat, double eLon, bool radians)
        {
            double EarthRadius = 6378137.0;

            double lon1 = radians ? sLon : DegToRad(sLon);
            double lon2 = radians ? eLon : DegToRad(eLon);
            double lat1 = radians ? sLat : DegToRad(sLat);
            double lat2 = radians ? eLat : DegToRad(eLat);

            /* This algorithm is called Sinnott's Formula */
            double dlon = (lon2) - (lon1);
            double dlat = (lat2) - (lat1);
            double a = Math.Pow(Math.Sin(dlat / 2), 2.0) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dlon / 2), 2.0);
            double c = 2 * Math.Asin(Math.Sqrt(a));
            return EarthRadius * c;
        }
        private static double sqr(double val)
        {
            return val * val;
        }
        public static double DegToRad(double deg)
        {
            return (deg / 180.0 * Math.PI);
        }
        #endregion LENGTH
    }
}