namespace MemTracer
{
    partial class MemOpTree
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.labelFrameOpStats = new System.Windows.Forms.Label();
            this.cbShowFreed = new System.Windows.Forms.CheckBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.cbShowFreed);
            this.splitContainer1.Panel1.Controls.Add(this.labelFrameOpStats);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.treeView1);
            this.splitContainer1.Size = new System.Drawing.Size(150, 150);
            this.splitContainer1.SplitterDistance = 28;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(150, 118);
            this.treeView1.TabIndex = 1;
            // 
            // labelFrameOpStats
            // 
            this.labelFrameOpStats.AutoSize = true;
            this.labelFrameOpStats.Location = new System.Drawing.Point(0, 5);
            this.labelFrameOpStats.Name = "labelFrameOpStats";
            this.labelFrameOpStats.Size = new System.Drawing.Size(0, 13);
            this.labelFrameOpStats.TabIndex = 0;
            // 
            // cbShowFreed
            // 
            this.cbShowFreed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbShowFreed.AutoSize = true;
            this.cbShowFreed.Checked = true;
            this.cbShowFreed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowFreed.Location = new System.Drawing.Point(30, 8);
            this.cbShowFreed.Name = "cbShowFreed";
            this.cbShowFreed.Size = new System.Drawing.Size(114, 17);
            this.cbShowFreed.TabIndex = 1;
            this.cbShowFreed.Text = "Show freed blocks";
            this.cbShowFreed.UseVisualStyleBackColor = true;
            this.cbShowFreed.CheckedChanged += new System.EventHandler(this.cbShowFreed_CheckedChanged);
            // 
            // MemOpTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "MemOpTree";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label labelFrameOpStats;
        private System.Windows.Forms.CheckBox cbShowFreed;

    }
}
