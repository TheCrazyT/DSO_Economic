namespace DSO_Economic
{
    partial class DSOEForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ItemRefresh = new System.Windows.Forms.Timer(this.components);
            this.tabCtrl = new System.Windows.Forms.TabControl();
            this.tabPage_Items = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.resources = new System.Windows.Forms.ListBox();
            this.graph = new ZedGraph.ZedGraphControl();
            this.items = new System.Windows.Forms.ListBox();
            this.tabPage_Time = new System.Windows.Forms.TabPage();
            this.itemsOverview = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.tabPage_Buildings = new System.Windows.Forms.TabPage();
            this.btn_export_clipboard = new System.Windows.Forms.Button();
            this.btn_export = new System.Windows.Forms.Button();
            this.lst_production = new System.Windows.Forms.ListView();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.lst_buildings = new System.Windows.Forms.ListBox();
            this.TimeLeft = new System.Windows.Forms.Timer(this.components);
            this.BuildingRefresh = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.btn_reconnect = new System.Windows.Forms.Button();
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.tsmi_help = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_forum = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_wiki = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmi_source = new System.Windows.Forms.ToolStripMenuItem();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.tabCtrl.SuspendLayout();
            this.tabPage_Items.SuspendLayout();
            this.tabPage_Time.SuspendLayout();
            this.tabPage_Buildings.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.mainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // ItemRefresh
            // 
            this.ItemRefresh.Interval = 10000;
            this.ItemRefresh.Tick += new System.EventHandler(this.ItemRefresh_Tick);
            // 
            // tabCtrl
            // 
            this.tabCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCtrl.Controls.Add(this.tabPage_Items);
            this.tabCtrl.Controls.Add(this.tabPage_Time);
            this.tabCtrl.Controls.Add(this.tabPage_Buildings);
            this.tabCtrl.Location = new System.Drawing.Point(1, 27);
            this.tabCtrl.Name = "tabCtrl";
            this.tabCtrl.SelectedIndex = 0;
            this.tabCtrl.Size = new System.Drawing.Size(758, 594);
            this.tabCtrl.TabIndex = 5;
            this.tabCtrl.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabCtrl_Selected);
            // 
            // tabPage_Items
            // 
            this.tabPage_Items.Controls.Add(this.label2);
            this.tabPage_Items.Controls.Add(this.label1);
            this.tabPage_Items.Controls.Add(this.resources);
            this.tabPage_Items.Controls.Add(this.graph);
            this.tabPage_Items.Controls.Add(this.items);
            this.tabPage_Items.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Items.Name = "tabPage_Items";
            this.tabPage_Items.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Items.Size = new System.Drawing.Size(750, 568);
            this.tabPage_Items.TabIndex = 0;
            this.tabPage_Items.Text = "Graph";
            this.tabPage_Items.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(134, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Rohstoffe:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Gegenstände auf Lager:";
            // 
            // resources
            // 
            this.resources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.resources.FormattingEnabled = true;
            this.resources.Location = new System.Drawing.Point(137, 31);
            this.resources.Name = "resources";
            this.resources.Size = new System.Drawing.Size(124, 524);
            this.resources.TabIndex = 7;
            // 
            // graph
            // 
            this.graph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.graph.IsEnableVPan = false;
            this.graph.IsEnableVZoom = false;
            this.graph.Location = new System.Drawing.Point(267, 5);
            this.graph.Name = "graph";
            this.graph.ScrollGrace = 0;
            this.graph.ScrollMaxX = 0;
            this.graph.ScrollMaxY = 0;
            this.graph.ScrollMaxY2 = 0;
            this.graph.ScrollMinX = 0;
            this.graph.ScrollMinY = 0;
            this.graph.ScrollMinY2 = 0;
            this.graph.Size = new System.Drawing.Size(474, 557);
            this.graph.TabIndex = 6;
            // 
            // items
            // 
            this.items.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.items.FormattingEnabled = true;
            this.items.Location = new System.Drawing.Point(14, 31);
            this.items.Name = "items";
            this.items.Size = new System.Drawing.Size(117, 524);
            this.items.TabIndex = 5;
            this.items.SelectedValueChanged += new System.EventHandler(this.items_SelectedValueChanged);
            // 
            // tabPage_Time
            // 
            this.tabPage_Time.Controls.Add(this.itemsOverview);
            this.tabPage_Time.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Time.Name = "tabPage_Time";
            this.tabPage_Time.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_Time.Size = new System.Drawing.Size(750, 568);
            this.tabPage_Time.TabIndex = 1;
            this.tabPage_Time.Text = "Zeitübersicht";
            this.tabPage_Time.UseVisualStyleBackColor = true;
            // 
            // itemsOverview
            // 
            this.itemsOverview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.itemsOverview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.itemsOverview.GridLines = true;
            this.itemsOverview.Location = new System.Drawing.Point(0, 0);
            this.itemsOverview.Name = "itemsOverview";
            this.itemsOverview.Size = new System.Drawing.Size(749, 566);
            this.itemsOverview.TabIndex = 0;
            this.itemsOverview.UseCompatibleStateImageBehavior = false;
            this.itemsOverview.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 170;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Zeit bis Ressource erschöpft";
            this.columnHeader2.Width = 160;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Zeit bis Lager voll";
            this.columnHeader3.Width = 160;
            // 
            // tabPage_Buildings
            // 
            this.tabPage_Buildings.Controls.Add(this.btn_export_clipboard);
            this.tabPage_Buildings.Controls.Add(this.btn_export);
            this.tabPage_Buildings.Controls.Add(this.lst_production);
            this.tabPage_Buildings.Controls.Add(this.lst_buildings);
            this.tabPage_Buildings.Location = new System.Drawing.Point(4, 22);
            this.tabPage_Buildings.Name = "tabPage_Buildings";
            this.tabPage_Buildings.Size = new System.Drawing.Size(750, 568);
            this.tabPage_Buildings.TabIndex = 2;
            this.tabPage_Buildings.Text = "Gebäude";
            this.tabPage_Buildings.UseVisualStyleBackColor = true;
            // 
            // btn_export_clipboard
            // 
            this.btn_export_clipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_export_clipboard.Location = new System.Drawing.Point(7, 523);
            this.btn_export_clipboard.Name = "btn_export_clipboard";
            this.btn_export_clipboard.Size = new System.Drawing.Size(186, 36);
            this.btn_export_clipboard.TabIndex = 7;
            this.btn_export_clipboard.Text = "Als CSV in die Zwischenablage exportieren";
            this.btn_export_clipboard.UseVisualStyleBackColor = true;
            this.btn_export_clipboard.Click += new System.EventHandler(this.btn_export_clipboard_Click);
            // 
            // btn_export
            // 
            this.btn_export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_export.Location = new System.Drawing.Point(7, 494);
            this.btn_export.Name = "btn_export";
            this.btn_export.Size = new System.Drawing.Size(186, 23);
            this.btn_export.TabIndex = 6;
            this.btn_export.Text = "Als CSV in eine Datei exportieren";
            this.btn_export.UseVisualStyleBackColor = true;
            this.btn_export.Click += new System.EventHandler(this.btn_export_Click);
            // 
            // lst_production
            // 
            this.lst_production.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lst_production.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.lst_production.Location = new System.Drawing.Point(202, 3);
            this.lst_production.Name = "lst_production";
            this.lst_production.Size = new System.Drawing.Size(540, 562);
            this.lst_production.TabIndex = 5;
            this.lst_production.UseCompatibleStateImageBehavior = false;
            this.lst_production.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Name";
            this.columnHeader4.Width = 130;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Produktionszeit";
            this.columnHeader5.Width = 130;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Level";
            this.columnHeader6.Width = 40;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Aktiv";
            // 
            // lst_buildings
            // 
            this.lst_buildings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lst_buildings.FormattingEnabled = true;
            this.lst_buildings.Location = new System.Drawing.Point(7, 3);
            this.lst_buildings.Name = "lst_buildings";
            this.lst_buildings.Size = new System.Drawing.Size(186, 485);
            this.lst_buildings.TabIndex = 0;
            this.lst_buildings.SelectedIndexChanged += new System.EventHandler(this.lst_buildings_SelectedIndexChanged);
            // 
            // TimeLeft
            // 
            this.TimeLeft.Interval = 5000;
            this.TimeLeft.Tick += new System.EventHandler(this.TimeLeft_Tick);
            // 
            // BuildingRefresh
            // 
            this.BuildingRefresh.Interval = 60000;
            this.BuildingRefresh.Tick += new System.EventHandler(this.BuildingRefresh_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 624);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(759, 22);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // status
            // 
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(133, 17);
            this.status.Text = "Verbindung unterbrochen!";
            // 
            // btn_reconnect
            // 
            this.btn_reconnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btn_reconnect.Location = new System.Drawing.Point(142, 627);
            this.btn_reconnect.Name = "btn_reconnect";
            this.btn_reconnect.Size = new System.Drawing.Size(87, 20);
            this.btn_reconnect.TabIndex = 12;
            this.btn_reconnect.Text = "neu verbinden";
            this.btn_reconnect.UseVisualStyleBackColor = true;
            this.btn_reconnect.Click += new System.EventHandler(this.btn_reconnect_Click);
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_help});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(759, 24);
            this.mainMenu.TabIndex = 8;
            this.mainMenu.Text = "menuStrip1";
            // 
            // tsmi_help
            // 
            this.tsmi_help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_forum,
            this.tsmi_wiki,
            this.tsmi_source});
            this.tsmi_help.Name = "tsmi_help";
            this.tsmi_help.Size = new System.Drawing.Size(40, 20);
            this.tsmi_help.Text = "&Hilfe";
            // 
            // tsmi_forum
            // 
            this.tsmi_forum.Name = "tsmi_forum";
            this.tsmi_forum.Size = new System.Drawing.Size(171, 22);
            this.tsmi_forum.Text = "&zum Forumeintrag";
            this.tsmi_forum.Click += new System.EventHandler(this.tsmi_forum_Click);
            // 
            // tsmi_wiki
            // 
            this.tsmi_wiki.Name = "tsmi_wiki";
            this.tsmi_wiki.Size = new System.Drawing.Size(171, 22);
            this.tsmi_wiki.Text = "zum &Wiki-Eintrag";
            this.tsmi_wiki.Click += new System.EventHandler(this.tsmi_wiki_Click);
            // 
            // tsmi_source
            // 
            this.tsmi_source.Name = "tsmi_source";
            this.tsmi_source.Size = new System.Drawing.Size(171, 22);
            this.tsmi_source.Text = "&Quellcode";
            this.tsmi_source.Click += new System.EventHandler(this.tsmi_source_Click);
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Buffs";
            // 
            // DSOEForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 646);
            this.Controls.Add(this.btn_reconnect);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabCtrl);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "DSOEForm";
            this.Load += new System.EventHandler(this.DSOEForm_Load);
            this.tabCtrl.ResumeLayout(false);
            this.tabPage_Items.ResumeLayout(false);
            this.tabPage_Items.PerformLayout();
            this.tabPage_Time.ResumeLayout(false);
            this.tabPage_Buildings.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer ItemRefresh;
        private System.Windows.Forms.TabControl tabCtrl;
        private System.Windows.Forms.TabPage tabPage_Items;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox resources;
        private ZedGraph.ZedGraphControl graph;
        private System.Windows.Forms.ListBox items;
        private System.Windows.Forms.TabPage tabPage_Time;
        private System.Windows.Forms.Timer TimeLeft;
        private System.Windows.Forms.ListView itemsOverview;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TabPage tabPage_Buildings;
        private System.Windows.Forms.ListBox lst_buildings;
        private System.Windows.Forms.ListView lst_production;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Button btn_export;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Timer BuildingRefresh;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.Windows.Forms.Button btn_reconnect;
        private System.Windows.Forms.Button btn_export_clipboard;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmi_help;
        private System.Windows.Forms.ToolStripMenuItem tsmi_forum;
        private System.Windows.Forms.ToolStripMenuItem tsmi_wiki;
        private System.Windows.Forms.ToolStripMenuItem tsmi_source;
        private System.Windows.Forms.ColumnHeader columnHeader8;
    }
}

