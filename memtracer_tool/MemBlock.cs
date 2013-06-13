using System;
using System.Collections.Generic;
using System.Text;

namespace MemTracer
{
    [Serializable]
    public class MemBlock
    {
        private static bool IsSymbolValid(int i, ulong[] callStack)
        {
            if (callStack[i] == 0)
                return false;

            IStackTracer.Symbol symbol = MemTracerForm.ms_MainForm.StackTracer.GetSymbolForAddress(callStack[i]);
            return (symbol.functionName != null);
        }
        public int FindFirstValidSymbolIndex()
        {
            ulong[] callStack = CallstackTab.GetCallStack(m_callStackCRC);
            for (int i = 0; i < callStack.Length; ++i)
            {
                if (IsSymbolValid(i, callStack))
                    return i;
            }
            return -1;
        }
        public int FindFirstValidSymbolIndex(ulong addr)
        {
            ulong[] callStack = CallstackTab.GetCallStack(m_callStackCRC);
            for (int i = 0; i < callStack.Length; ++i)
            {
                if (callStack[i] == addr && IsSymbolValid(i, callStack))
                    return i;
            }
            return -1;
        }

        public bool IsCallStackEqual(MemBlock other)
        {
            return m_callStackCRC == other.m_callStackCRC;
        }
        public bool DoesCallStackContainAddress(ulong addr)
        {
            ulong[] callStack = CallstackTab.GetCallStack(m_callStackCRC);
            for (int i = 0; i < callStack.Length; ++i)
            {
                if (callStack[i] == addr)
                    return true;
            }
            return false;
        }

        public string GetTagString()
        {
            string tagString;
            if (m_tagCRC)
            {
                tagString = TagDict.GetTag(m_tag);
            }
            else
            {
                char[] text = new char[4];
                text[3] = (char)(m_tag & 0xFF);
                text[2] = (char)((m_tag >> 8) & 0xFF);
                text[1] = (char)((m_tag >> 16) & 0xFF);
                text[0] = (char)((m_tag >> 24) & 0xFF);
                tagString = new string(text);
            }
            return tagString;
        }

        public ulong m_address = 0;
        public uint m_size = 0;
        public ulong m_callStackCRC = 0;
        public ulong m_tag = 0;
        public bool m_tagCRC = false;   // true if tag is CRC of string, not 4CC
    }
}
