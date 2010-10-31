using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MemTracer
{
    public partial class CompareSnapshots : Form
    {
        public CompareSnapshots(MemSnapshot snapA, MemSnapshot snapB, string nameA, string nameB)
        {
            InitializeComponent();

            snapshotInfoControl1.Init(nameA);
            snapshotInfoControl2.Init(nameB);
            snapshotInfoControl1.Update(snapA);
            snapshotInfoControl2.Update(snapB);

            memAllocTree1.BuildTree(snapA);

            labelSnap1.Text = nameA;
            labelSnap2.Text = nameB;

            MemSnapshot diff = snapA.Difference(snapB);
            memAllocTree1.BuildTree(snapA);
            memAllocTree2.BuildTree(snapB);
            memAllocTreeDiff.BuildTree(diff);
        }
    }
}
