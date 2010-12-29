namespace DSO_Economic
{
    partial class FixForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.ItemRefresh = new System.Windows.Forms.Timer(this.components);
            this.items = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(485, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Es wurden mehr Werte erhalten als erwartet, \r\nbitte wähle den momentanten Wert fü" +
                "r Holzplanken aus, um die Rohstoffanzeige richtig zu kalibrieren.";
            // 
            // ItemRefresh
            // 
            this.ItemRefresh.Enabled = true;
            this.ItemRefresh.Interval = 1000;
            // 
            // items
            // 
            this.items.FormattingEnabled = true;
            this.items.Location = new System.Drawing.Point(222, 67);
            this.items.Name = "items";
            this.items.Size = new System.Drawing.Size(120, 433);
            this.items.TabIndex = 1;
            this.items.SelectedIndexChanged += new System.EventHandler(this.items_SelectedIndexChanged);
            // 
            // FixForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(507, 512);
            this.Controls.Add(this.items);
            this.Controls.Add(this.label1);
            this.Name = "FixForm";
            this.Text = "FixForm";
            this.Load += new System.EventHandler(this.FixForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer ItemRefresh;
        private System.Windows.Forms.ListBox items;
    }
}