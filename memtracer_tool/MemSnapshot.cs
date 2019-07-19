using System;
using System.Collections.Generic;
using System.Text;

namespace MemTracer
{
    public class MemSnapshot
    {
        public void AddBlock(MemBlock block)
        {
            ///System.Console.Out.Write("Adding " + block.m_address.ToString("X") + "\n");
            /*MemBlock block2 = null;
            if (m_blocks.TryGetValue(block.m_address, out block2))
            {
                StackTracer.Symbol[] sym2 = new StackTracer.Symbol[block2.m_callStack.Count];
                for (int i = 0; i < block2.m_callStack.Count; ++i)
                    sym2[i] = StackTracer.GetSymbolForAddress(block2.m_callStack[i]);
                int k = 1;
            }*/

            if (m_blocks.ContainsKey(block.m_address))
            {
                return;
            }
            else
            {
                m_blocks.Add(block.m_address, block);
                m_allocatedBytes += block.m_size;
                if (m_blocks.Count > m_topAllocatedBlocks)
                    m_topAllocatedBlocks = m_blocks.Count;
                if (m_allocatedBytes > m_topAllocatedBytes)
                    m_topAllocatedBytes = m_allocatedBytes;
                if (block.m_size > m_largestAllocation)
                    m_largestAllocation = block.m_size;
            }
        }
        public void RemoveBlock(ulong address)
        {
            //System.Console.Out.Write("Freeing " + address.ToString("X") + "\n");
            MemBlock block = null;
            if (m_blocks.TryGetValue(address, out block))
            {
                m_blocks.Remove(address);
                m_allocatedBytes -= block.m_size;
            }
            else
            {
                //int breakHere = 1;
                //System.Console.Out.Write("Cant free " + address.ToString("X") + "\n");
            }
        }
        public void TagBlock(ulong address, ulong tag)
        {
            MemBlock block = null;
            if (m_blocks.TryGetValue(address, out block))
            {
                block.m_tag = tag;
                block.m_tagCRC = true;
            }
        }
        public MemSnapshot Difference(MemSnapshot other)
        {
            MemSnapshot ret = new MemSnapshot();
            //uint order = 0;
            Dictionary<MemBlock, int> usedBlocks = new Dictionary<MemBlock, int>();
            foreach (MemBlock otherBlock in other.m_blocks.Values)
            {
                /*StackTracer.Symbol[] syms = new StackTracer.Symbol[otherBlock.m_callStack.Count];
                for (int i = 0; i < otherBlock.m_callStack.Count; ++i)
                    syms[i] = StackTracer.GetSymbolForAddress(otherBlock.m_callStack[i]);*/

                bool found = false;
                foreach (MemBlock block in m_blocks.Values)
                {
                    if (!usedBlocks.ContainsKey(block) &&
                        otherBlock.m_size == block.m_size &&
                        otherBlock.IsCallStackEqual(block))
                    {
                        usedBlocks.Add(block, 1);
                        found = true;
                        break;
                    }
                }
                //otherBlock.m_tag = order;
                if (!found)
                    ret.AddBlock(otherBlock);

                //++order;
            }
            return ret;
        }

        public MemSnapshot Overlap(MemSnapshot other)
        {
            MemSnapshot ret = new MemSnapshot();
            foreach (MemBlock otherBlock in other.m_blocks.Values)
            {
                bool found = false;
                foreach (MemBlock block in m_blocks.Values)
                {
                    if (otherBlock == block)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                    ret.AddBlock(otherBlock);
            }
            return ret;
        }


        public Dictionary<ulong, MemBlock> Blocks { get { return m_blocks; } }
        public int NumAllocatedBlocks { get { return m_blocks.Count; } }
        public uint NumAllocatedBytes { get { return m_allocatedBytes; } }
        public int TopAllocatedBlocks { get { return m_topAllocatedBlocks; } }
        public uint TopAllocatedBytes { get { return m_topAllocatedBytes; } }
        public uint LargestAllocation { get { return m_largestAllocation; } }

        Dictionary<ulong, MemBlock> m_blocks = new Dictionary<ulong, MemBlock>();
        uint m_allocatedBytes = 0;
        uint m_topAllocatedBytes = 0;
        uint m_largestAllocation = 0;
        int m_topAllocatedBlocks = 0;
    }
}
