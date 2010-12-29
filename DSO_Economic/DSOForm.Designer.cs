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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.resources = new System.Windows.Forms.ListBox();
            this.graph = new ZedGraph.ZedGraphControl();
            this.items = new System.Windows.Forms.ListBox();
            this.TimeLeft = new System.Windows.Forms.Timer(this.components);
            this.itemsOverview = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.tabCtrl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ItemRefresh
            // 
            this.ItemRefresh.Interval = 10000;
            this.ItemRefresh.Tick += new System.EventHandler(this.ItemRefresh_Tick);
            // 
            // tabCtrl
            // 
            this.tabCtrl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCtrl.Controls.Add(this.tabPage1);
            this.tabCtrl.Controls.Add(this.tabPage2);
            this.tabCtrl.Location = new System.Drawing.Point(1, 1);
            this.tabCtrl.Name = "tabCtrl";
            this.tabCtrl.SelectedIndex = 0;
            this.tabCtrl.Size = new System.Drawing.Size(760, 587);
            this.tabCtrl.TabIndex = 5;
            this.tabCtrl.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabCtrl_Selected);
            this.tabCtrl.TabIndexChanged += new System.EventHandler(this.tabCtrl_TabIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.resources);
            this.tabPage1.Controls.Add(this.graph);
            this.tabPage1.Controls.Add(this.items);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(752, 561);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Graph";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.itemsOverview);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(752, 561);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Zeitübersicht";
            this.tabPage2.UseVisualStyleBackColor = true;
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
            this.graph.Size = new System.Drawing.Size(476, 550);
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
            // TimeLeft
            // 
            this.TimeLeft.Interval = 5000;
            this.TimeLeft.Tick += new System.EventHandler(this.TimeLeft_Tick);
            // 
            // itemsOverview
            // 
            this.itemsOverview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.itemsOverview.GridLines = true;
            this.itemsOverview.Location = new System.Drawing.Point(0, 0);
            this.itemsOverview.Name = "itemsOverview";
            this.itemsOverview.Size = new System.Drawing.Size(749, 555);
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
            // DSOEForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(759, 584);
            this.Controls.Add(this.tabCtrl);
            this.Name = "DSOEForm";
            this.Load += new System.EventHandler(this.DSOEForm_Load);
            this.tabCtrl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer ItemRefresh;
        private System.Windows.Forms.TabControl tabCtrl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox resources;
        private ZedGraph.ZedGraphControl graph;
        private System.Windows.Forms.ListBox items;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Timer TimeLeft;
        private System.Windows.Forms.ListView itemsOverview;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}

