namespace MemTracer
{
    partial class MemAllocTree
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.subtreeDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cbTagged = new System.Windows.Forms.CheckBox();
            this.cbBottomUp = new System.Windows.Forms.CheckBox();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.cbTagged);
            this.splitContainer1.Panel2.Controls.Add(this.cbBottomUp);
            this.splitContainer1.Size = new System.Drawing.Size(150, 213);
            this.splitContainer1.SplitterDistance = 181;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView1
            // 
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(150, 181);
            this.treeView1.TabIndex = 1;
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.subtreeDetailsToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(159, 26);
            // 
            // subtreeDetailsToolStripMenuItem
            // 
            this.subtreeDetailsToolStripMenuItem.Name = "subtreeDetailsToolStripMenuItem";
            this.subtreeDetailsToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.subtreeDetailsToolStripMenuItem.Text = "Subtree Details";
            this.subtreeDetailsToolStripMenuItem.Click += new System.EventHandler(this.subtreeDetailsToolStripMenuItem_Click);
            // 
            // cbTagged
            // 
            this.cbTagged.AutoSize = true;
            this.cbTagged.Location = new System.Drawing.Point(83, 5);
            this.cbTagged.Name = "cbTagged";
            this.cbTagged.Size = new System.Drawing.Size(63, 17);
            this.cbTagged.TabIndex = 1;
            this.cbTagged.Text = "Tagged";
            this.cbTagged.UseVisualStyleBackColor = true;
            this.cbTagged.CheckedChanged += new System.EventHandler(this.cbTagged_CheckedChanged);
            // 
            // cbBottomUp
            // 
            this.cbBottomUp.AutoSize = true;
            this.cbBottomUp.Checked = true;
            this.cbBottomUp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBottomUp.Location = new System.Drawing.Point(0, 5);
            this.cbBottomUp.Name = "cbBottomUp";
            this.cbBottomUp.Size = new System.Drawing.Size(76, 17);
            this.cbBottomUp.TabIndex = 0;
            this.cbBottomUp.Text = "Bottom-Up";
            this.cbBottomUp.UseVisualStyleBackColor = true;
            this.cbBottomUp.CheckedChanged += new System.EventHandler(this.cbBottomUp_CheckedChanged);
            // 
            // MemAllocTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "MemAllocTree";
            this.Size = new System.Drawing.Size(150, 213);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.CheckBox cbBottomUp;
        private System.Windows.Forms.CheckBox cbTagged;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem subtreeDetailsToolStripMenuItem;
    }
}
