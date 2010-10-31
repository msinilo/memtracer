using System;
using System.Collections.Generic;
using System.Text;

namespace MemTracer
{
    public class MemBlockTree
    {
        public class Node
        {
            class CompareBySize : IComparer<Node>
            {
                public int Compare(Node lhs, Node rhs)
                {
                    float lPerc = lhs.GetPercentageOfParentSize();
                    float rPerc = rhs.GetPercentageOfParentSize();
                    return (lPerc < rPerc ? +1 : (lPerc > rPerc ? -1 : 0));
                }
            }

            public Node FindChildByCallAddress(ulong callAddress, bool recursive)
            {
                foreach (Node node in m_children)
                {
                    if (node.m_callAddress == callAddress)
                        return node;

                    if (recursive)
                    {
                        Node childNode = node.FindChildByCallAddress(callAddress, true);
                        if (childNode != null)
                            return childNode;
                    }
                }
                return null;
            }
            public void CollectChildrenByCallAddress(ulong callAddress, bool recursive, List<Node> nodes)
            {
                foreach (Node node in m_children)
                {
                    if (node.m_callAddress == callAddress)
                        nodes.Add(node);

                    if (recursive)
                    {
                        node.CollectChildrenByCallAddress(callAddress, true, nodes);
                    }
                }
            }


            public Node AddChild(IStackTracer.Symbol symbol)
            {
                Node child = new Node();
                child.m_callAddress = symbol.address;
                child.m_symbol = symbol;
                child.m_parent = this;
                m_children.Add(child);
                return child;
            }
            public Node AddChildDetailed(MemBlock block)
            {
                DetailedNode child = new DetailedNode();
                child.m_parent = this;
                child.m_block = block;
                m_children.Add(child);
                return child;
            }
            public Node AddTaggedChild(ulong tag, bool tagCRC)
            {
                Node child = new Node();
                child.m_tag = tag;
                child.m_tagCRC = tagCRC;
                child.m_parent = this;
                m_children.Add(child);
                return child;
            }

            public virtual ulong GetAllocatedSize()
            {
                ParseChildren();
                return m_allocatedSize;
            }
            public int GetNumAllocatedBlocks()
            {
                ParseChildren();
                return m_allocatedBlocks;
            }
            public float GetPercentageOfParentSize()
            {
                return (float)GetAllocatedSize() * 100.0f / m_parent.GetAllocatedSize();
            }
            public virtual String GetText()
            {
                if (m_tag == 0)
                {
                    ulong kbs = GetAllocatedSize() >> 10;
                    String text = m_symbol.functionName + " (" + kbs + "kb, " + GetNumAllocatedBlocks() +
                        " block(s) , " + GetPercentageOfParentSize().ToString("N") + "%)";
                    return text;
                }
                else
                {
                    String stag;
                    if (m_tagCRC)
                    {
                        stag = TagDict.GetTag(m_tag);
                    }
                    else
                    {
                        char[] text = new char[4];
                        text[3] = (char)(m_tag & 0xFF);
                        text[2] = (char)((m_tag >> 8) & 0xFF);
                        text[1] = (char)((m_tag >> 16) & 0xFF);
                        text[0] = (char)((m_tag >> 24) & 0xFF);
                        stag = new string(text);
                    }
                    ulong kbs = GetAllocatedSize() >> 10;
                    string ret = stag + " (" + kbs + "kb, " + GetNumAllocatedBlocks() +
                        " block(s) , " + GetPercentageOfParentSize().ToString("N") + "%)";
                    return ret;
                }
            }

            public void Sort()
            {
                m_children.Sort(new CompareBySize());
                foreach (Node child in m_children)
                    child.Sort();
            }


            void ParseChildren()
            {
                if (m_allocatedSize == 0 || m_allocatedBlocks == 0)
                {
                    m_allocatedSize = 0;
                    m_allocatedBlocks = 0;
                    foreach (Node child in m_children)
                    {
                        m_allocatedSize += child.GetAllocatedSize();
                        m_allocatedBlocks += child.GetNumAllocatedBlocks();
                    }
                }
            }

            public List<Node> Children { get { return m_children; } }
            public IStackTracer.Symbol Symbol { get { return m_symbol; } }

            public ulong m_callAddress = 0;
            public ulong m_allocatedSize = 0;
            public int m_allocatedBlocks = 0;
            public ulong m_tag = 0;
            bool m_tagCRC = false;
            IStackTracer.Symbol m_symbol;
            Node m_parent;
            List<Node> m_children = new List<Node>();
        }
        public class DetailedNode : Node
        {
            public override ulong GetAllocatedSize()
            {
                return m_block.m_size;
            }
            public override String GetText()
            {
                string stag = m_block.GetTagString();
                if (m_memOpNr < 0)
                    m_memOpNr = MemTracerForm.ms_MainForm.MemOperationNrForBlock(m_block);

                return m_block.m_address.ToString("X") + ": " + (m_block.m_size >> 10) + "kb, tag: " + stag + ", #" +
                    m_memOpNr.ToString();
            }

            public MemBlock m_block = null;
            public int m_memOpNr = -1;
        }

        Node FindRootForTag(ulong tag)
        {
            foreach (Node node in m_root.Children)
            {
                if (node.m_tag == tag)
                    return node;
            }
            return null;
        }

        public void AddMemBlock(MemBlock block)
        {
            if (BottomUp)
            {
                AddMemBlockBottomUp(block);
                return;
            }

            if (m_subtreeRoot != 0 && !block.DoesCallStackContainAddress(m_subtreeRoot))
                return;

            int rootIndex = block.FindFirstValidSymbolIndex();
            if (rootIndex < 0)
                return;

            Node root = m_root;
            if (Tagged)
            {
                root = FindRootForTag(block.m_tag);
                if (root == null)
                {
                    root = m_root.AddTaggedChild(block.m_tag, block.m_tagCRC);
                }
            }

            uint[] callStack = CallstackTab.GetCallStack(block.m_callStackCRC);
            Node rootNode = root.FindChildByCallAddress(callStack[rootIndex], true);
            IStackTracer stackTracer = MemTracerForm.ms_MainForm.StackTracer;
            if (rootNode == null)
            {
                IStackTracer.Symbol symbol = stackTracer.GetSymbolForAddress(callStack[rootIndex]);
                rootNode = root.AddChild(symbol);
            }
            Node parentNode = rootNode;
            for (int i = rootIndex + 1; i < callStack.Length; ++i)
            {
                uint callAddress = callStack[i];
                Node thisNode = parentNode.FindChildByCallAddress(callAddress, false);

                if (thisNode == null)
                {
                    thisNode = parentNode.AddChild(stackTracer.GetSymbolForAddress(callAddress));
                }
                if (i == callStack.Length - 1)
                {
                    thisNode.m_allocatedSize += block.m_size;
                    ++thisNode.m_allocatedBlocks;
                }
                parentNode = thisNode;
            }
        }

        public void AddMemBlockSubTree(MemBlock block)
        {
            if (m_subtreeRoot != 0 && !block.DoesCallStackContainAddress(m_subtreeRoot))
                return;

            int rootIndex = block.FindFirstValidSymbolIndex(m_subtreeRoot);
            if (rootIndex < 0)
                return;

            Node root = m_root;
            if (Tagged)
            {
                root = FindRootForTag(block.m_tag);
                if (root == null)
                {
                    root = m_root.AddTaggedChild(block.m_tag, block.m_tagCRC);
                }
            }

            uint[] callStack = CallstackTab.GetCallStack(block.m_callStackCRC);
            Node rootNode = root.FindChildByCallAddress(callStack[rootIndex], true);
            IStackTracer stackTracer = MemTracerForm.ms_MainForm.StackTracer;
            if (rootNode == null)
            {
                IStackTracer.Symbol symbol = stackTracer.GetSymbolForAddress(callStack[rootIndex]);
                rootNode = root.AddChild(symbol);
            }
            Node parentNode = rootNode;
            for (int i = rootIndex + 1; i < callStack.Length; ++i)
            {
                uint callAddress = callStack[i];
                Node thisNode = parentNode.FindChildByCallAddress(callAddress, false);

                if (thisNode == null)
                {
                    thisNode = parentNode.AddChild(stackTracer.GetSymbolForAddress(callAddress));
                }
                if (i == callStack.Length - 1)
                {
                    thisNode.m_allocatedSize += block.m_size;
                    ++thisNode.m_allocatedBlocks;
                    thisNode.AddChildDetailed(block);
                }

                parentNode = thisNode;
            }
        }

        void AddMemBlockBottomUp(MemBlock block)
        {
            int endIndex = block.FindFirstValidSymbolIndex();
            if (endIndex < 0)
                return;

            Node root = m_root;
            if (Tagged)
            {
                root = FindRootForTag(block.m_tag);
                if (root == null)
                {
                    root = m_root.AddTaggedChild(block.m_tag, block.m_tagCRC);
                }
            }

            uint[] callStack = CallstackTab.GetCallStack(block.m_callStackCRC);
            int rootIndex = callStack.Length - 1;
            Node rootNode = root.FindChildByCallAddress(callStack[rootIndex], true);
            IStackTracer stackTracer = MemTracerForm.ms_MainForm.StackTracer;
            if (rootNode == null)
            {
                IStackTracer.Symbol symbol = stackTracer.GetSymbolForAddress(callStack[rootIndex]);
                rootNode = root.AddChild(symbol);
            }
            Node parentNode = rootNode;
            for (int i = rootIndex - 1; i >= endIndex; --i)
            {
                uint callAddress = callStack[i];
                Node thisNode = parentNode.FindChildByCallAddress(callAddress, false);

                if (thisNode == null)
                {
                    thisNode = parentNode.AddChild(stackTracer.GetSymbolForAddress(callAddress));
                }
                if (i == endIndex)
                {
                    thisNode.m_allocatedSize += block.m_size;
                    ++thisNode.m_allocatedBlocks;
                }
                parentNode = thisNode;
            }
        }

        public void Clear()
        {
            m_root = new Node();
        }
        public void Sort()
        {
            m_root.Sort();
        }

        public Node Root { get { return m_root; } }
        public bool BottomUp
        {
            get { return m_bottomUp; }
            set { m_bottomUp = value; }
        }
        public bool Tagged
        {
            get { return m_tagged; }
            set { m_tagged = value; }
        }
        public uint SubtreeRootAddress { set { m_subtreeRoot = value; } }

        Node m_root = new Node();
        bool m_bottomUp = true;
        bool m_tagged = false;
        uint m_subtreeRoot = 0;
    }
}
