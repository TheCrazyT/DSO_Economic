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
            this.items = new System.Windows.Forms.ListBox();
            this.ItemRefresh = new System.Windows.Forms.Timer(this.components);
            this.graph = new ZedGraph.ZedGraphControl();
            this.resources = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // items
            // 
            this.items.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.items.FormattingEnabled = true;
            this.items.Location = new System.Drawing.Point(12, 12);
            this.items.Name = "items";
            this.items.Size = new System.Drawing.Size(105, 550);
            this.items.TabIndex = 0;
            this.items.SelectedValueChanged += new System.EventHandler(this.items_SelectedValueChanged);
            // 
            // ItemRefresh
            // 
            this.ItemRefresh.Interval = 10000;
            this.ItemRefresh.Tick += new System.EventHandler(this.ItemRefresh_Tick);
            // 
            // graph
            // 
            this.graph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.graph.IsEnableVPan = false;
            this.graph.IsEnableVZoom = false;
            this.graph.Location = new System.Drawing.Point(225, 12);
            this.graph.Name = "graph";
            this.graph.ScrollGrace = 0;
            this.graph.ScrollMaxX = 0;
            this.graph.ScrollMaxY = 0;
            this.graph.ScrollMaxY2 = 0;
            this.graph.ScrollMinX = 0;
            this.graph.ScrollMinY = 0;
            this.graph.ScrollMinY2 = 0;
            this.graph.Size = new System.Drawing.Size(536, 550);
            this.graph.TabIndex = 1;
            // 
            // resources
            // 
            this.resources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.resources.FormattingEnabled = true;
            this.resources.Location = new System.Drawing.Point(114, 12);
            this.resources.Name = "resources";
            this.resources.Size = new System.Drawing.Size(105, 550);
            this.resources.TabIndex = 2;
            // 
            // DSOEForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(773, 584);
            this.Controls.Add(this.resources);
            this.Controls.Add(this.graph);
            this.Controls.Add(this.items);
            this.Name = "DSOEForm";
            this.Load += new System.EventHandler(this.DSOEForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox items;
        private System.Windows.Forms.Timer ItemRefresh;
        private ZedGraph.ZedGraphControl graph;
        private System.Windows.Forms.ListBox resources;
    }
}

