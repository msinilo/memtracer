namespace MemTracer
{
    partial class MemTracerForm
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
            this.toolBar1 = new System.Windows.Forms.ToolBar();
            this.butConnect = new System.Windows.Forms.ToolBarButton();
            this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
            this.butPrevFrame = new System.Windows.Forms.ToolBarButton();
            this.butNextFrame = new System.Windows.Forms.ToolBarButton();
            this.butPlay = new System.Windows.Forms.ToolBarButton();
            this.butRewind = new System.Windows.Forms.ToolBarButton();
            this.butFFwd = new System.Windows.Forms.ToolBarButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSnapshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mostAllocatedBlocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allocationPointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tracedVariablesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frameAnalysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.snapshotList1 = new MemTracer.SnapshotList();
            this.panel1 = new System.Windows.Forms.Panel();
            this.butAdd = new System.Windows.Forms.Button();
            this.tabGraph = new System.Windows.Forms.TabPage();
            this.usageGraph = new CustomUIControls.Graphing.C2DPushGraph();
            this.tabPageFrameSnapshot = new System.Windows.Forms.TabPage();
            this.memAllocTreeFrame = new MemTracer.MemAllocTree();
            this.tabPageFrameOps = new System.Windows.Forms.TabPage();
            this.memOpTree1 = new MemTracer.MemOpTree();
            this.snapshotInfo2 = new MemTracer.SnapshotInfoControl();
            this.snapshotInfo1 = new MemTracer.SnapshotInfoControl();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusFrame = new System.Windows.Forms.ToolStripStatusLabel();
            this.numericUpDownSpeed = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabGraph.SuspendLayout();
            this.tabPageFrameSnapshot.SuspendLayout();
            this.tabPageFrameOps.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // toolBar1
            // 
            this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.butConnect,
            this.toolBarButton1,
            this.butPrevFrame,
            this.butNextFrame,
            this.butPlay,
            this.butRewind,
            this.butFFwd});
            this.toolBar1.DropDownArrows = true;
            this.toolBar1.Location = new System.Drawing.Point(0, 24);
            this.toolBar1.Name = "toolBar1";
            this.toolBar1.ShowToolTips = true;
            this.toolBar1.Size = new System.Drawing.Size(992, 42);
            this.toolBar1.TabIndex = 5;
            this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
            // 
            // butConnect
            // 
            this.butConnect.Name = "butConnect";
            this.butConnect.Text = "Connect";
            this.butConnect.ToolTipText = "Connect";
            // 
            // toolBarButton1
            // 
            this.toolBarButton1.Name = "toolBarButton1";
            this.toolBarButton1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
            // 
            // butPrevFrame
            // 
            this.butPrevFrame.Enabled = false;
            this.butPrevFrame.Name = "butPrevFrame";
            this.butPrevFrame.Text = "<";
            this.butPrevFrame.ToolTipText = "Prev frame";
            // 
            // butNextFrame
            // 
            this.butNextFrame.Enabled = false;
            this.butNextFrame.Name = "butNextFrame";
            this.butNextFrame.Text = ">";
            this.butNextFrame.ToolTipText = "Next frame";
            // 
            // butPlay
            // 
            this.butPlay.Enabled = false;
            this.butPlay.Name = "butPlay";
            this.butPlay.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.butPlay.Text = "Play";
            // 
            // butRewind
            // 
            this.butRewind.Enabled = false;
            this.butRewind.Name = "butRewind";
            this.butRewind.Text = "<<";
            this.butRewind.ToolTipText = "Go to first frame";
            // 
            // butFFwd
            // 
            this.butFFwd.Enabled = false;
            this.butFFwd.Name = "butFFwd";
            this.butFFwd.Text = ">>";
            this.butFFwd.ToolTipText = "Go to last frame";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(992, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSnapshotToolStripMenuItem,
            this.mostAllocatedBlocksToolStripMenuItem,
            this.allocationPointsToolStripMenuItem,
            this.tracedVariablesToolStripMenuItem,
            this.frameAnalysisToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(45, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // addSnapshotToolStripMenuItem
            // 
            this.addSnapshotToolStripMenuItem.Name = "addSnapshotToolStripMenuItem";
            this.addSnapshotToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.addSnapshotToolStripMenuItem.Text = "Add Snapshot";
            this.addSnapshotToolStripMenuItem.Click += new System.EventHandler(this.addSnapshotToolStripMenuItem_Click);
            // 
            // mostAllocatedBlocksToolStripMenuItem
            // 
            this.mostAllocatedBlocksToolStripMenuItem.Name = "mostAllocatedBlocksToolStripMenuItem";
            this.mostAllocatedBlocksToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.mostAllocatedBlocksToolStripMenuItem.Text = "Allocation Overview";
            this.mostAllocatedBlocksToolStripMenuItem.Click += new System.EventHandler(this.mostAllocatedBlocksToolStripMenuItem_Click);
            // 
            // allocationPointsToolStripMenuItem
            // 
            this.allocationPointsToolStripMenuItem.Name = "allocationPointsToolStripMenuItem";
            this.allocationPointsToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.allocationPointsToolStripMenuItem.Text = "Allocation Points";
            this.allocationPointsToolStripMenuItem.Click += new System.EventHandler(this.allocationPointsToolStripMenuItem_Click);
            // 
            // tracedVariablesToolStripMenuItem
            // 
            this.tracedVariablesToolStripMenuItem.Name = "tracedVariablesToolStripMenuItem";
            this.tracedVariablesToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.tracedVariablesToolStripMenuItem.Text = "Traced Variables";
            this.tracedVariablesToolStripMenuItem.Click += new System.EventHandler(this.tracedVariablesToolStripMenuItem_Click);
            // 
            // frameAnalysisToolStripMenuItem
            // 
            this.frameAnalysisToolStripMenuItem.CheckOnClick = true;
            this.frameAnalysisToolStripMenuItem.Name = "frameAnalysisToolStripMenuItem";
            this.frameAnalysisToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.frameAnalysisToolStripMenuItem.Text = "Frame Analysis";
            this.frameAnalysisToolStripMenuItem.CheckedChanged += new System.EventHandler(this.frameAnalysisToolStripMenuItem_CheckedChanged);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "MemTrace";
            this.saveFileDialog1.Filter = "Mem Trace files|*.MemTrace";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "MemTrace";
            this.openFileDialog1.Filter = "Mem Trace files|*.MemTrace";
            this.openFileDialog1.Title = "Load MemTrace file";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.splitContainer1);
            this.panel2.Location = new System.Drawing.Point(0, 72);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(992, 341);
            this.panel2.TabIndex = 12;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.snapshotInfo2);
            this.splitContainer1.Panel2.Controls.Add(this.snapshotInfo1);
            this.splitContainer1.Size = new System.Drawing.Size(992, 341);
            this.splitContainer1.SplitterDistance = 800;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabGraph);
            this.tabControl1.Controls.Add(this.tabPageFrameSnapshot);
            this.tabControl1.Controls.Add(this.tabPageFrameOps);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 341);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.TabIndex = 12;
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.snapshotList1);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(792, 315);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Snapshots";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // snapshotList1
            // 
            this.snapshotList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.snapshotList1.Location = new System.Drawing.Point(3, 3);
            this.snapshotList1.Name = "snapshotList1";
            this.snapshotList1.OnCompareSnapshots = null;
            this.snapshotList1.OnOverlapSnapshots = null;
            this.snapshotList1.Size = new System.Drawing.Size(661, 309);
            this.snapshotList1.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.butAdd);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(664, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(125, 309);
            this.panel1.TabIndex = 22;
            // 
            // butAdd
            // 
            this.butAdd.Location = new System.Drawing.Point(8, 283);
            this.butAdd.Name = "butAdd";
            this.butAdd.Size = new System.Drawing.Size(110, 23);
            this.butAdd.TabIndex = 19;
            this.butAdd.Text = "Add Snapshot";
            this.butAdd.UseVisualStyleBackColor = true;
            this.butAdd.Click += new System.EventHandler(this.addSnapshotToolStripMenuItem_Click);
            // 
            // tabGraph
            // 
            this.tabGraph.Controls.Add(this.usageGraph);
            this.tabGraph.Location = new System.Drawing.Point(4, 22);
            this.tabGraph.Name = "tabGraph";
            this.tabGraph.Padding = new System.Windows.Forms.Padding(3);
            this.tabGraph.Size = new System.Drawing.Size(792, 315);
            this.tabGraph.TabIndex = 1;
            this.tabGraph.Text = "Graph";
            this.tabGraph.UseVisualStyleBackColor = true;
            // 
            // usageGraph
            // 
            this.usageGraph.AutoAdjustPeek = false;
            this.usageGraph.BackColor = System.Drawing.Color.Black;
            this.usageGraph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.usageGraph.GridColor = System.Drawing.Color.Green;
            this.usageGraph.GridSize = ((ushort)(15));
            this.usageGraph.HighQuality = true;
            this.usageGraph.LineInterval = ((ushort)(5));
            this.usageGraph.Location = new System.Drawing.Point(3, 3);
            this.usageGraph.MaxLabel = "Max";
            this.usageGraph.MaxPeekMagnitude = 100;
            this.usageGraph.MinLabel = "Minimum";
            this.usageGraph.MinPeekMagnitude = 0;
            this.usageGraph.Name = "usageGraph";
            this.usageGraph.ShowGrid = true;
            this.usageGraph.ShowLabels = true;
            this.usageGraph.Size = new System.Drawing.Size(786, 309);
            this.usageGraph.TabIndex = 0;
            this.usageGraph.Text = "Usage";
            this.usageGraph.TextColor = System.Drawing.Color.Yellow;
            // 
            // tabPageFrameSnapshot
            // 
            this.tabPageFrameSnapshot.Controls.Add(this.memAllocTreeFrame);
            this.tabPageFrameSnapshot.Location = new System.Drawing.Point(4, 22);
            this.tabPageFrameSnapshot.Name = "tabPageFrameSnapshot";
            this.tabPageFrameSnapshot.Size = new System.Drawing.Size(792, 315);
            this.tabPageFrameSnapshot.TabIndex = 2;
            this.tabPageFrameSnapshot.Text = "Frame snapshot";
            this.tabPageFrameSnapshot.UseVisualStyleBackColor = true;
            // 
            // memAllocTreeFrame
            // 
            this.memAllocTreeFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memAllocTreeFrame.Location = new System.Drawing.Point(0, 0);
            this.memAllocTreeFrame.Name = "memAllocTreeFrame";
            this.memAllocTreeFrame.Size = new System.Drawing.Size(792, 315);
            this.memAllocTreeFrame.TabIndex = 0;
            // 
            // tabPageFrameOps
            // 
            this.tabPageFrameOps.Controls.Add(this.memOpTree1);
            this.tabPageFrameOps.Location = new System.Drawing.Point(4, 22);
            this.tabPageFrameOps.Name = "tabPageFrameOps";
            this.tabPageFrameOps.Size = new System.Drawing.Size(792, 315);
            this.tabPageFrameOps.TabIndex = 3;
            this.tabPageFrameOps.Text = "Frame ops";
            this.tabPageFrameOps.UseVisualStyleBackColor = true;
            // 
            // memOpTree1
            // 
            this.memOpTree1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memOpTree1.Location = new System.Drawing.Point(0, 0);
            this.memOpTree1.Name = "memOpTree1";
            this.memOpTree1.Size = new System.Drawing.Size(792, 315);
            this.memOpTree1.TabIndex = 0;
            // 
            // snapshotInfo2
            // 
            this.snapshotInfo2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.snapshotInfo2.Location = new System.Drawing.Point(6, 180);
            this.snapshotInfo2.Name = "snapshotInfo2";
            this.snapshotInfo2.Size = new System.Drawing.Size(179, 154);
            this.snapshotInfo2.TabIndex = 22;
            this.snapshotInfo2.Visible = false;
            // 
            // snapshotInfo1
            // 
            this.snapshotInfo1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.snapshotInfo1.Location = new System.Drawing.Point(6, 7);
            this.snapshotInfo1.Name = "snapshotInfo1";
            this.snapshotInfo1.Size = new System.Drawing.Size(179, 154);
            this.snapshotInfo1.TabIndex = 21;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusFrame});
            this.statusStrip1.Location = new System.Drawing.Point(0, 403);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(992, 22);
            this.statusStrip1.TabIndex = 13;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusFrame
            // 
            this.toolStripStatusFrame.Name = "toolStripStatusFrame";
            this.toolStripStatusFrame.Size = new System.Drawing.Size(0, 17);
            // 
            // numericUpDownSpeed
            // 
            this.numericUpDownSpeed.Location = new System.Drawing.Point(221, 34);
            this.numericUpDownSpeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSpeed.Name = "numericUpDownSpeed";
            this.numericUpDownSpeed.Size = new System.Drawing.Size(106, 20);
            this.numericUpDownSpeed.TabIndex = 14;
            this.numericUpDownSpeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSpeed.ValueChanged += new System.EventHandler(this.numericUpDownSpeed_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(333, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Replay speed";
            // 
            // MemTracerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 425);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownSpeed);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.toolBar1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(700, 451);
            this.Name = "MemTracerForm";
            this.Text = "Mem Tracer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabGraph.ResumeLayout(false);
            this.tabPageFrameSnapshot.ResumeLayout(false);
            this.tabPageFrameOps.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolBar toolBar1;
        private System.Windows.Forms.ToolBarButton butConnect;
        private System.Windows.Forms.ToolBarButton toolBarButton1;
        private System.Windows.Forms.ToolBarButton butPrevFrame;
        private System.Windows.Forms.ToolBarButton butNextFrame;
        private System.Windows.Forms.ToolBarButton butPlay;
        private System.Windows.Forms.ToolBarButton butFFwd;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem addSnapshotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mostAllocatedBlocksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allocationPointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tracedVariablesToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private SnapshotList snapshotList1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button butAdd;
        private System.Windows.Forms.TabPage tabGraph;
        private CustomUIControls.Graphing.C2DPushGraph usageGraph;
        private SnapshotInfoControl snapshotInfo2;
        private SnapshotInfoControl snapshotInfo1;
        private System.Windows.Forms.TabPage tabPageFrameSnapshot;
        private MemAllocTree memAllocTreeFrame;
        private System.Windows.Forms.TabPage tabPageFrameOps;
        private MemOpTree memOpTree1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolBarButton butRewind;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusFrame;
        private System.Windows.Forms.NumericUpDown numericUpDownSpeed;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem frameAnalysisToolStripMenuItem;
    }
}

