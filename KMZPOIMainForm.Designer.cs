namespace KMZPOIfromOSM
{
    partial class KMZPOIMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem(new string[] {
            "Import File",
            "",
            "File to Read (*.pbf) or (*.osm)"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem(new string[] {
            "Dictionary JSON File",
            "",
            "File with JSON dictionary (*.json)"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem(new string[] {
            "Catalog JSON File",
            "",
            "File with JSON catalog (*.json)"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem(new string[] {
            "Icons Zip File",
            "",
            "Zip file with icons (*.zip)"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem(new string[] {
            "Hierarchy Level",
            "",
            "Split Points by Categories with Hierarchy Level"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192))))), null);
            System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem(new string[] {
            "Keep NoCategory",
            "",
            "Add nodes not in Catalog"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192))))), null);
            System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem(new string[] {
            "Update Interval",
            "",
            "Update Status Every Seconds"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192))))), null);
            System.Windows.Forms.ListViewItem listViewItem8 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: HasTags",
            "",
            "Add nodes only with tags"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem9 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: HasName",
            "",
            "Add nodes only with tag `name`"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem10 = new System.Windows.Forms.ListViewItem(new string[] {
            "Full Category Path",
            "",
            "Select Full Hierarchy Category Path"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192))))), null);
            System.Windows.Forms.ListViewItem listViewItem11 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Contains All of Tags",
            "",
            "Contains All of Tags or Tag=Value"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem12 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Only One of Tags",
            "",
            "Contains Only One of Tag or Tag=Value"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem13 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Only Modified After",
            "",
            "Only Nodes Modified After"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem14 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Only Modified Before",
            "",
            "Only Nodes Modified Before"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem15 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Modified One of User",
            "",
            "Nodes Modified ony One of User"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem16 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Only Version One of",
            "",
            "Nodes Only with Version One of"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Only In Box",
            "",
            "Nodes Only inside Box Area"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192))))), null);
            System.Windows.Forms.ListViewItem listViewItem18 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Only In Polygon",
            "",
            "Nodes Only inside Polygon"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128))))), null);
            System.Windows.Forms.ListViewItem listViewItem19 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Only Near The Line",
            "",
            "Nodes Only Near The Line"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem20 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Only Near The Line Meters",
            "",
            "Nodes Only Near The Line In Meters"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem21 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Only Which Side of Line",
            "",
            "Nodes Only Which Side of Line"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem22 = new System.Windows.Forms.ListViewItem(new string[] {
            "Copies in Categories",
            "",
            "Node can be in several categories or not"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192))))), null);
            System.Windows.Forms.ListViewItem listViewItem23 = new System.Windows.Forms.ListViewItem(new string[] {
            "Copies in NoValueButKeys",
            "",
            "Node can be in several NotValueButKeys or not"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192))))), null);
            System.Windows.Forms.ListViewItem listViewItem24 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Node Contains All Tags",
            "",
            "Node Contains All Tags of Catalog Record or Just One"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255))))), null);
            System.Windows.Forms.ListViewItem listViewItem25 = new System.Windows.Forms.ListViewItem(new string[] {
            "Filter: Contains Text",
            "",
            "Node contains tag or tag with text (regex)"}, -1, System.Drawing.Color.Empty, System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255))))), null);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KMZPOIMainForm));
            this.mnu2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.runBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.stopBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.openTemporaryFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.uncheckEmptiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inverseCheckingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.bSaveCH1 = new System.Windows.Forms.ToolStripMenuItem();
            this.bSaveCHX = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.bSaveALL1 = new System.Windows.Forms.ToolStripMenuItem();
            this.bSaveALLX = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.status2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.mnu1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ladoConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.extractbbbikeorgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.read2 = new System.Windows.Forms.ToolStripMenuItem();
            this.stop2 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.status3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.props = new System.Windows.Forms.ListView();
            this.columnHeader11 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader12 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader13 = new System.Windows.Forms.ColumnHeader();
            this.catView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ttlTxt = new System.Windows.Forms.Label();
            this.mnu2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.mnu1.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnu2
            // 
            this.mnu2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runBtn,
            this.stopBtn,
            this.toolStripMenuItem3,
            this.openTemporaryFileToolStripMenuItem,
            this.toolStripMenuItem5,
            this.uncheckEmptiesToolStripMenuItem,
            this.checkNoneToolStripMenuItem,
            this.checkAllToolStripMenuItem,
            this.inverseCheckingToolStripMenuItem,
            this.toolStripMenuItem1,
            this.bSaveCH1,
            this.bSaveCHX,
            this.toolStripMenuItem2,
            this.bSaveALL1,
            this.bSaveALLX});
            this.mnu2.Name = "mnu2";
            this.mnu2.Size = new System.Drawing.Size(368, 270);
            this.mnu2.Opening += new System.ComponentModel.CancelEventHandler(this.mnu2_Opening);
            // 
            // runBtn
            // 
            this.runBtn.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.runBtn.Name = "runBtn";
            this.runBtn.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.runBtn.Size = new System.Drawing.Size(367, 22);
            this.runBtn.Text = "Read OSM File";
            this.runBtn.Click += new System.EventHandler(this.runBtn_Click);
            // 
            // stopBtn
            // 
            this.stopBtn.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.stopBtn.Name = "stopBtn";
            this.stopBtn.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.stopBtn.Size = new System.Drawing.Size(367, 22);
            this.stopBtn.Text = "Stop";
            this.stopBtn.Click += new System.EventHandler(this.stopBtn_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(364, 6);
            // 
            // openTemporaryFileToolStripMenuItem
            // 
            this.openTemporaryFileToolStripMenuItem.Name = "openTemporaryFileToolStripMenuItem";
            this.openTemporaryFileToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.openTemporaryFileToolStripMenuItem.Text = "Open Temporary File Shell Menu ...";
            this.openTemporaryFileToolStripMenuItem.Click += new System.EventHandler(this.openTemporaryFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(364, 6);
            // 
            // uncheckEmptiesToolStripMenuItem
            // 
            this.uncheckEmptiesToolStripMenuItem.Name = "uncheckEmptiesToolStripMenuItem";
            this.uncheckEmptiesToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.uncheckEmptiesToolStripMenuItem.Text = "Uncheck Empties";
            this.uncheckEmptiesToolStripMenuItem.Click += new System.EventHandler(this.uncheckEmptiesToolStripMenuItem_Click);
            // 
            // checkNoneToolStripMenuItem
            // 
            this.checkNoneToolStripMenuItem.Name = "checkNoneToolStripMenuItem";
            this.checkNoneToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.checkNoneToolStripMenuItem.Text = "Check None";
            this.checkNoneToolStripMenuItem.Click += new System.EventHandler(this.checkNoneToolStripMenuItem_Click);
            // 
            // checkAllToolStripMenuItem
            // 
            this.checkAllToolStripMenuItem.Name = "checkAllToolStripMenuItem";
            this.checkAllToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.checkAllToolStripMenuItem.Text = "Check All";
            this.checkAllToolStripMenuItem.Click += new System.EventHandler(this.checkAllToolStripMenuItem_Click);
            // 
            // inverseCheckingToolStripMenuItem
            // 
            this.inverseCheckingToolStripMenuItem.Name = "inverseCheckingToolStripMenuItem";
            this.inverseCheckingToolStripMenuItem.Size = new System.Drawing.Size(367, 22);
            this.inverseCheckingToolStripMenuItem.Text = "Inverse Checking";
            this.inverseCheckingToolStripMenuItem.Click += new System.EventHandler(this.inverseCheckingToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(364, 6);
            // 
            // bSaveCH1
            // 
            this.bSaveCH1.Name = "bSaveCH1";
            this.bSaveCH1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.bSaveCH1.Size = new System.Drawing.Size(367, 22);
            this.bSaveCH1.Text = "Save Checked To Single KMZ File ...";
            this.bSaveCH1.Click += new System.EventHandler(this.bSaveCH1_Click);
            // 
            // bSaveCHX
            // 
            this.bSaveCHX.Name = "bSaveCHX";
            this.bSaveCHX.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.bSaveCHX.Size = new System.Drawing.Size(367, 22);
            this.bSaveCHX.Text = "Save Each Checked Category To KMZ File ...";
            this.bSaveCHX.Click += new System.EventHandler(this.bSaveCHX_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(364, 6);
            // 
            // bSaveALL1
            // 
            this.bSaveALL1.Name = "bSaveALL1";
            this.bSaveALL1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.bSaveALL1.Size = new System.Drawing.Size(367, 22);
            this.bSaveALL1.Text = "Save All To Single KMZ File ...";
            this.bSaveALL1.Click += new System.EventHandler(this.bSaveALL1_Click);
            // 
            // bSaveALLX
            // 
            this.bSaveALLX.Name = "bSaveALLX";
            this.bSaveALLX.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.bSaveALLX.Size = new System.Drawing.Size(367, 22);
            this.bSaveALLX.Text = "Save Each of All Category To KMZ File ...";
            this.bSaveALLX.Click += new System.EventHandler(this.bSaveALLX_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ContextMenuStrip = this.mnu2;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status,
            this.status2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 522);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(747, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // status
            // 
            this.status.ForeColor = System.Drawing.Color.Maroon;
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(25, 17);
            this.status.Text = "Idle";
            // 
            // status2
            // 
            this.status2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.status2.Name = "status2";
            this.status2.Size = new System.Drawing.Size(0, 17);
            // 
            // mnu1
            // 
            this.mnu1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ladoConfigToolStripMenuItem,
            this.saveConfigToolStripMenuItem,
            this.toolStripMenuItem4,
            this.extractbbbikeorgToolStripMenuItem,
            this.toolStripMenuItem6,
            this.read2,
            this.stop2});
            this.mnu1.Name = "mnu1";
            this.mnu1.Size = new System.Drawing.Size(253, 148);
            this.mnu1.Opening += new System.ComponentModel.CancelEventHandler(this.mnu2_Opening);
            // 
            // ladoConfigToolStripMenuItem
            // 
            this.ladoConfigToolStripMenuItem.Name = "ladoConfigToolStripMenuItem";
            this.ladoConfigToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.ladoConfigToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.ladoConfigToolStripMenuItem.Text = "Load Config ...";
            this.ladoConfigToolStripMenuItem.Click += new System.EventHandler(this.ladoConfigToolStripMenuItem_Click);
            // 
            // saveConfigToolStripMenuItem
            // 
            this.saveConfigToolStripMenuItem.Name = "saveConfigToolStripMenuItem";
            this.saveConfigToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveConfigToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.saveConfigToolStripMenuItem.Text = "Save Config ...";
            this.saveConfigToolStripMenuItem.Click += new System.EventHandler(this.saveConfigToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(249, 6);
            // 
            // extractbbbikeorgToolStripMenuItem
            // 
            this.extractbbbikeorgToolStripMenuItem.Enabled = false;
            this.extractbbbikeorgToolStripMenuItem.Name = "extractbbbikeorgToolStripMenuItem";
            this.extractbbbikeorgToolStripMenuItem.Size = new System.Drawing.Size(252, 22);
            this.extractbbbikeorgToolStripMenuItem.Text = "Import From extract.bbbike.org ...";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(249, 6);
            // 
            // read2
            // 
            this.read2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.read2.Name = "read2";
            this.read2.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.read2.Size = new System.Drawing.Size(252, 22);
            this.read2.Text = "Read OSM File";
            this.read2.Click += new System.EventHandler(this.runBtn_Click);
            // 
            // stop2
            // 
            this.stop2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.stop2.Name = "stop2";
            this.stop2.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.stop2.Size = new System.Drawing.Size(252, 22);
            this.stop2.Text = "Stop";
            this.stop2.Click += new System.EventHandler(this.stopBtn_Click);
            // 
            // statusStrip2
            // 
            this.statusStrip2.ContextMenuStrip = this.mnu2;
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status3,
            this.toolStripStatusLabel4});
            this.statusStrip2.Location = new System.Drawing.Point(0, 500);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(747, 22);
            this.statusStrip2.TabIndex = 5;
            this.statusStrip2.Text = "statusStrip2";
            this.statusStrip2.Visible = false;
            // 
            // status3
            // 
            this.status3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.status3.Name = "status3";
            this.status3.Size = new System.Drawing.Size(25, 17);
            this.status3.Text = "Idle";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(0, 17);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.Gray;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.props);
            this.splitContainer1.Panel1MinSize = 160;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.catView);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(747, 522);
            this.splitContainer1.SplitterDistance = 161;
            this.splitContainer1.TabIndex = 6;
            // 
            // props
            // 
            this.props.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.props.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13});
            this.props.ContextMenuStrip = this.mnu1;
            this.props.Dock = System.Windows.Forms.DockStyle.Fill;
            this.props.FullRowSelect = true;
            this.props.GridLines = true;
            this.props.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7,
            listViewItem8,
            listViewItem9,
            listViewItem10,
            listViewItem11,
            listViewItem12,
            listViewItem13,
            listViewItem14,
            listViewItem15,
            listViewItem16,
            listViewItem17,
            listViewItem18,
            listViewItem19,
            listViewItem20,
            listViewItem21,
            listViewItem22,
            listViewItem23,
            listViewItem24,
            listViewItem25});
            this.props.Location = new System.Drawing.Point(0, 0);
            this.props.MultiSelect = false;
            this.props.Name = "props";
            this.props.Size = new System.Drawing.Size(747, 161);
            this.props.TabIndex = 5;
            this.props.UseCompatibleStateImageBehavior = false;
            this.props.View = System.Windows.Forms.View.Details;
            this.props.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.props_MouseDoubleClick);
            this.props.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.props_KeyPress);
            this.props.KeyDown += new System.Windows.Forms.KeyEventHandler(this.props_KeyDown);
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "Property";
            this.columnHeader11.Width = 175;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "Value";
            this.columnHeader12.Width = 230;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "Description";
            this.columnHeader13.Width = 316;
            // 
            // catView
            // 
            this.catView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.catView.CheckBoxes = true;
            this.catView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.catView.ContextMenuStrip = this.mnu2;
            this.catView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.catView.FullRowSelect = true;
            this.catView.GridLines = true;
            this.catView.Location = new System.Drawing.Point(0, 23);
            this.catView.Name = "catView";
            this.catView.ShowItemToolTips = true;
            this.catView.Size = new System.Drawing.Size(747, 334);
            this.catView.TabIndex = 7;
            this.catView.UseCompatibleStateImageBehavior = false;
            this.catView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Index";
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "ID";
            this.columnHeader2.Width = 97;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "CategoryName";
            this.columnHeader3.Width = 391;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Count";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "FileSize";
            this.columnHeader5.Width = 95;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.ttlTxt);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(747, 23);
            this.panel1.TabIndex = 8;
            // 
            // ttlTxt
            // 
            this.ttlTxt.AutoSize = true;
            this.ttlTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ttlTxt.Location = new System.Drawing.Point(3, 4);
            this.ttlTxt.Name = "ttlTxt";
            this.ttlTxt.Size = new System.Drawing.Size(106, 13);
            this.ttlTxt.TabIndex = 0;
            this.ttlTxt.Text = "Total Elements: 0";
            // 
            // KMZPOIMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 544);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip2);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "KMZPOIMainForm";
            this.Text = "KMZ POI From OSM";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.KMZPOIMaonForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.KMZPOIMainForm_FormClosing);
            this.mnu2.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.mnu1.ResumeLayout(false);
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.Windows.Forms.ToolStripStatusLabel status2;
        private System.Windows.Forms.ContextMenuStrip mnu2;
        private System.Windows.Forms.ToolStripMenuItem checkNoneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checkAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inverseCheckingToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem bSaveCH1;
        private System.Windows.Forms.ToolStripMenuItem bSaveCHX;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem bSaveALL1;
        private System.Windows.Forms.ToolStripMenuItem bSaveALLX;
        private System.Windows.Forms.ToolStripMenuItem runBtn;
        private System.Windows.Forms.ToolStripMenuItem stopBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem uncheckEmptiesToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel status3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ContextMenuStrip mnu1;
        private System.Windows.Forms.ToolStripMenuItem read2;
        private System.Windows.Forms.ToolStripMenuItem stop2;
        private System.Windows.Forms.ToolStripMenuItem ladoConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem openTemporaryFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView props;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private System.Windows.Forms.ListView catView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label ttlTxt;
        private System.Windows.Forms.ToolStripMenuItem extractbbbikeorgToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
    }
}

