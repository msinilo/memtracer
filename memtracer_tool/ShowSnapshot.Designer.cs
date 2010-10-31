namespace MemTracer
{
    partial class ShowSnapshot
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
            this.memAllocTree1 = new MemTracer.MemAllocTree();
            this.snapshotInfoControl1 = new MemTracer.SnapshotInfoControl();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // memAllocTree1
            // 
            this.memAllocTree1.Dock = System.Windows.Forms.DockStyle.Left;
            this.memAllocTree1.Location = new System.Drawing.Point(0, 0);
            this.memAllocTree1.Name = "memAllocTree1";
            this.memAllocTree1.Size = new System.Drawing.Size(622, 300);
            this.memAllocTree1.TabIndex = 4;
            // 
            // snapshotInfoControl1
            // 
            this.snapshotInfoControl1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.snapshotInfoControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.snapshotInfoControl1.Location = new System.Drawing.Point(624, 0);
            this.snapshotInfoControl1.Name = "snapshotInfoControl1";
            this.snapshotInfoControl1.Size = new System.Drawing.Size(260, 300);
            this.snapshotInfoControl1.TabIndex = 5;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.richTextBox1.Location = new System.Drawing.Point(0, 300);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(884, 196);
            this.richTextBox1.TabIndex = 6;
            this.richTextBox1.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(155, 265);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.snapshotInfoControl1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.memAllocTree1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(884, 300);
            this.panel1.TabIndex = 8;
            // 
            // ShowSnapshot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 496);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.richTextBox1);
            this.Name = "ShowSnapshot";
            this.Text = "ShowSnapshot";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private MemAllocTree memAllocTree1;
        private SnapshotInfoControl snapshotInfoControl1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
    }
}