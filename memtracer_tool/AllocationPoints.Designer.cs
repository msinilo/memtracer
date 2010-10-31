namespace MemTracer
{
    partial class AllocationPoints
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridAllocs = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBytes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAllocs = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPerc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridAllocs)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridAllocs);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer1.Size = new System.Drawing.Size(525, 403);
            this.splitContainer1.SplitterDistance = 256;
            this.splitContainer1.TabIndex = 0;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // dataGridAllocs
            // 
            this.dataGridAllocs.AllowUserToAddRows = false;
            this.dataGridAllocs.AllowUserToDeleteRows = false;
            this.dataGridAllocs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridAllocs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.colBytes,
            this.colAllocs,
            this.colPerc});
            this.dataGridAllocs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridAllocs.Location = new System.Drawing.Point(0, 0);
            this.dataGridAllocs.Name = "dataGridAllocs";
            this.dataGridAllocs.ReadOnly = true;
            this.dataGridAllocs.RowHeadersVisible = false;
            this.dataGridAllocs.Size = new System.Drawing.Size(525, 256);
            this.dataGridAllocs.TabIndex = 1;
            this.dataGridAllocs.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dataGridView1_SortCompare);
            this.dataGridAllocs.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridAllocs_CellDoubleClick);
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Column1.HeaderText = "Point";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 56;
            // 
            // colBytes
            // 
            this.colBytes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colBytes.HeaderText = "Memory";
            this.colBytes.Name = "colBytes";
            this.colBytes.ReadOnly = true;
            this.colBytes.Width = 69;
            // 
            // colAllocs
            // 
            this.colAllocs.HeaderText = "Blocks";
            this.colAllocs.Name = "colAllocs";
            this.colAllocs.ReadOnly = true;
            // 
            // colPerc
            // 
            this.colPerc.HeaderText = "%";
            this.colPerc.Name = "colPerc";
            this.colPerc.ReadOnly = true;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(525, 143);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // AllocationPoints
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 403);
            this.Controls.Add(this.splitContainer1);
            this.Name = "AllocationPoints";
            this.Text = "AllocationPoints";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridAllocs)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridAllocs;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBytes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAllocs;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPerc;


    }
}