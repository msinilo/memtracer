using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MemTracer
{
    public partial class SnapshotInfoControl : UserControl
    {
        public SnapshotInfoControl()
        {
            InitializeComponent();
        }

        public void InitSnapshot()
        {
            /*labelCaption.Text = desc.name;
            FrameInfo info = FrameInfo.BuildSnapshotForDesc(desc);
            uint usedBytes = info.m_memAllocated;
            float usedMb = (float)usedBytes / (1024 * 1024);
            labelMemUsed.Text = usedMb.ToString("N") + " MB";
            float maxUsedMb = (float)info.m_topMemAllocated / (1024 * 1024);
            labelMaxMemUsed.Text = maxUsedMb.ToString("N") + " MB";
            labelMaxBlocks.Text = info.m_topBlockCount.ToString();
            labelBlocks.Text = info.m_blockCount.ToString();*/
        }

        public void Init(string name)
        {
            labelCaption.Text = name;
        }
        public void Update(MemSnapshot snapshot)
        {
            m_snapshot = snapshot;
            uint usedBytes = snapshot.NumAllocatedBytes;
            float usedMb = (float)usedBytes / (1024 * 1024);
            labelMemUsed.Text = usedMb.ToString("N") + " MB";
            float maxUsedMb = (float)snapshot.TopAllocatedBytes / (1024 * 1024);
            labelMaxMemUsed.Text = maxUsedMb.ToString("N") + " MB";
            labelMaxBlocks.Text = snapshot.TopAllocatedBlocks.ToString();
            labelBlocks.Text = snapshot.NumAllocatedBlocks.ToString();
            float largestMb = (float)snapshot.LargestAllocation / (1024 * 1024);
            labelLargest.Text = largestMb.ToString("N") + " MB";
        }
        public string SnapshotName { get { return labelCaption.Text; } }


        public MemSnapshot m_snapshot;
    }
}
