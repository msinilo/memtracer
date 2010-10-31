namespace MemTracer
{
    partial class SnapshotInfoControl
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
            this.labelCaption = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelMemUsed = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelBlocks = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labelMaxMemUsed = new System.Windows.Forms.Label();
            this.labelMaxBlocks = new System.Windows.Forms.Label();
            this.labelLargest = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelCaption
            // 
            this.labelCaption.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.labelCaption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelCaption.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelCaption.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelCaption.Location = new System.Drawing.Point(0, 0);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(151, 16);
            this.labelCaption.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Memory Used";
            // 
            // labelMemUsed
            // 
            this.labelMemUsed.AutoSize = true;
            this.labelMemUsed.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelMemUsed.Location = new System.Drawing.Point(106, 23);
            this.labelMemUsed.Name = "labelMemUsed";
            this.labelMemUsed.Size = new System.Drawing.Size(2, 15);
            this.labelMemUsed.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Blocks";
            // 
            // labelBlocks
            // 
            this.labelBlocks.AutoSize = true;
            this.labelBlocks.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelBlocks.Location = new System.Drawing.Point(106, 48);
            this.labelBlocks.Name = "labelBlocks";
            this.labelBlocks.Size = new System.Drawing.Size(2, 15);
            this.labelBlocks.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Max Mem Used";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Max Blocks";
            // 
            // labelMaxMemUsed
            // 
            this.labelMaxMemUsed.AutoSize = true;
            this.labelMaxMemUsed.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelMaxMemUsed.Location = new System.Drawing.Point(106, 73);
            this.labelMaxMemUsed.Name = "labelMaxMemUsed";
            this.labelMaxMemUsed.Size = new System.Drawing.Size(2, 15);
            this.labelMaxMemUsed.TabIndex = 7;
            // 
            // labelMaxBlocks
            // 
            this.labelMaxBlocks.AutoSize = true;
            this.labelMaxBlocks.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelMaxBlocks.Location = new System.Drawing.Point(106, 98);
            this.labelMaxBlocks.Name = "labelMaxBlocks";
            this.labelMaxBlocks.Size = new System.Drawing.Size(2, 15);
            this.labelMaxBlocks.TabIndex = 8;
            // 
            // labelLargest
            // 
            this.labelLargest.AutoSize = true;
            this.labelLargest.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelLargest.Location = new System.Drawing.Point(106, 125);
            this.labelLargest.Name = "labelLargest";
            this.labelLargest.Size = new System.Drawing.Size(2, 15);
            this.labelLargest.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 125);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Largest Block";
            // 
            // SnapshotInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.labelLargest);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelMaxBlocks);
            this.Controls.Add(this.labelMaxMemUsed);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelBlocks);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelMemUsed);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.labelCaption);
            this.Name = "SnapshotInfoControl";
            this.Size = new System.Drawing.Size(151, 145);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label labelCaption;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelMemUsed;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelBlocks;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelMaxMemUsed;
        private System.Windows.Forms.Label labelMaxBlocks;
        private System.Windows.Forms.Label labelLargest;
        private System.Windows.Forms.Label label6;

    }
}
