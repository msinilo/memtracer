using System;
using System.Collections.Generic;
using System.Text;

namespace MemTracer
{
    class CallstackTab
    {
        public static ulong CalcCRC(ulong[] callStack)
        {
            ulong crc = 0;
            CRC32.Init(ref crc);
            for (int i = 0; i < callStack.Length; ++i)
                CRC32.Update(ref crc, callStack[i]);
            CRC32.Update(ref crc, (ulong)callStack.Length);
            CRC32.Finish(ref crc);
            return crc;
        }

        public static bool HasCallStack(ulong crc)
        {
            return m_callStackMap.ContainsKey(crc);
        }
        public static void AddCallStack(ulong crc, ulong[] callStack)
        {
            if (!HasCallStack(crc))
                m_callStackMap.Add(crc, callStack);
            else
            {
                ulong[] otherCs = GetCallStack(crc);
                System.Diagnostics.Debug.Assert(otherCs.Length == callStack.Length);
                for (int i = 0; i < callStack.Length; ++i)
                    System.Diagnostics.Debug.Assert(callStack[i] == otherCs[i]);
            }
        }
        public static ulong[] GetCallStack(ulong crc)
        {
            return m_callStackMap[crc];
        }

        public static Dictionary<ulong, ulong[]> m_callStackMap = new Dictionary<ulong, ulong[]>();
    }
}
