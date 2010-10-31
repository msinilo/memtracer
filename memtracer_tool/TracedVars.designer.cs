namespace MemTracer
{
    partial class TracedVars
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
            this.lbVars = new System.Windows.Forms.ListBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lbVars
            // 
            this.lbVars.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbVars.Enabled = false;
            this.lbVars.FormattingEnabled = true;
            this.lbVars.Location = new System.Drawing.Point(0, 0);
            this.lbVars.Name = "lbVars";
            this.lbVars.Size = new System.Drawing.Size(292, 264);
            this.lbVars.TabIndex = 0;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // TracedVars
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 271);
            this.Controls.Add(this.lbVars);
            this.Name = "TracedVars";
            this.Text = "TracedVars";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbVars;
        private System.Windows.Forms.Timer timer1;
    }
}