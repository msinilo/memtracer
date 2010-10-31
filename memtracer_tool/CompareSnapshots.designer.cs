namespace MemTracer
{
    partial class CompareSnapshots
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.snapshotInfoControl2 = new MemTracer.SnapshotInfoControl();
            this.snapshotInfoControl1 = new MemTracer.SnapshotInfoControl();
            this.label1 = new System.Windows.Forms.Label();
            this.labelSnap2 = new System.Windows.Forms.Label();
            this.labelSnap1 = new System.Windows.Forms.Label();
            this.memAllocTreeDiff = new MemTracer.MemAllocTree();
            this.memAllocTree2 = new MemTracer.MemAllocTree();
            this.memAllocTree1 = new MemTracer.MemAllocTree();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.labFile = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.snapshotInfoControl2);
            this.panel1.Controls.Add(this.snapshotInfoControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(767, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(265, 382);
            this.panel1.TabIndex = 2;
            // 
            // snapshotInfoControl2
            // 
            this.snapshotInfoControl2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.snapshotInfoControl2.Location = new System.Drawing.Point(5, 179);
            this.snapshotInfoControl2.Name = "snapshotInfoControl2";
            this.snapshotInfoControl2.Size = new System.Drawing.Size(260, 152);
            this.snapshotInfoControl2.TabIndex = 1;
            // 
            // snapshotInfoControl1
            // 
            this.snapshotInfoControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.snapshotInfoControl1.Location = new System.Drawing.Point(5, 0);
            this.snapshotInfoControl1.Name = "snapshotInfoControl1";
            this.snapshotInfoControl1.Size = new System.Drawing.Size(260, 151);
            this.snapshotInfoControl1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 165);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Difference";
            // 
            // labelSnap2
            // 
            this.labelSnap2.AutoSize = true;
            this.labelSnap2.Location = new System.Drawing.Point(382, 3);
            this.labelSnap2.Name = "labelSnap2";
            this.labelSnap2.Size = new System.Drawing.Size(35, 13);
            this.labelSnap2.TabIndex = 7;
            this.labelSnap2.Text = "label2";
            // 
            // labelSnap1
            // 
            this.labelSnap1.AutoSize = true;
            this.labelSnap1.Location = new System.Drawing.Point(9, 3);
            this.labelSnap1.Name = "labelSnap1";
            this.labelSnap1.Size = new System.Drawing.Size(35, 13);
            this.labelSnap1.TabIndex = 8;
            this.labelSnap1.Text = "label3";
            // 
            // memAllocTreeDiff
            // 
            this.memAllocTreeDiff.Location = new System.Drawing.Point(12, 181);
            this.memAllocTreeDiff.Name = "memAllocTreeDiff";
            this.memAllocTreeDiff.Size = new System.Drawing.Size(733, 189);
            this.memAllocTreeDiff.TabIndex = 5;
            // 
            // memAllocTree2
            // 
            this.memAllocTree2.Location = new System.Drawing.Point(385, 22);
            this.memAllocTree2.Name = "memAllocTree2";
            this.memAllocTree2.Size = new System.Drawing.Size(360, 136);
            this.memAllocTree2.TabIndex = 4;
            // 
            // memAllocTree1
            // 
            this.memAllocTree1.Location = new System.Drawing.Point(12, 22);
            this.memAllocTree1.Name = "memAllocTree1";
            this.memAllocTree1.Size = new System.Drawing.Size(360, 136);
            this.memAllocTree1.TabIndex = 3;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.richTextBox1.Location = new System.Drawing.Point(0, 382);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(1032, 273);
            this.richTextBox1.TabIndex = 9;
            this.richTextBox1.Text = "";
            // 
            // labFile
            // 
            this.labFile.AutoSize = true;
            this.labFile.Location = new System.Drawing.Point(8, 628);
            this.labFile.Name = "labFile";
            this.labFile.Size = new System.Drawing.Size(0, 13);
            this.labFile.TabIndex = 10;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelSnap1);
            this.panel2.Controls.Add(this.labelSnap2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.memAllocTreeDiff);
            this.panel2.Controls.Add(this.memAllocTree2);
            this.panel2.Controls.Add(this.memAllocTree1);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1032, 382);
            this.panel2.TabIndex = 11;
            // 
            // CompareSnapshots
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1032, 655);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.labFile);
            this.Controls.Add(this.richTextBox1);
            this.Name = "CompareSnapshots";
            this.Text = "CompareSnapshots";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private MemAllocTree memAllocTree1;
        private MemAllocTree memAllocTree2;
        private MemAllocTree memAllocTreeDiff;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelSnap2;
        private System.Windows.Forms.Label labelSnap1;
        private SnapshotInfoControl snapshotInfoControl2;
        private SnapshotInfoControl snapshotInfoControl1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label labFile;
        private System.Windows.Forms.Panel panel2;
    }
}