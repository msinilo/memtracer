using System;
using System.Collections.Generic;
using System.Text;

namespace MemTracer
{
    class TagDict
    {
        public static ulong AddTag(String tag)
        {
            ulong crc = CalcCRC(tag);

            if (!m_tags.ContainsKey(crc))
                m_tags.Add(crc, tag);

            return crc;
        }
        public static string GetTag(ulong tagCRC)
        {
            return m_tags[tagCRC];
        }
        // 0 if not found.
        public static ulong GetCRC(String tag)
        {
            foreach (ulong crc in m_tags.Keys)
            {
                if (m_tags[crc] == tag)
                    return crc;
            }
            return 0;
        }

        static ulong CalcCRC(String str)
        {
            ulong crc = 0;
            CRC32.Init(ref crc);
            for (int i = 0; i < str.Length; ++i)
                CRC32.UpdateB(ref crc, (byte)str[i]);
            CRC32.UpdateB(ref crc, (byte)str.Length);
            return crc;
        }

        public static Dictionary<ulong, String> m_tags = new Dictionary<ulong, string>();
    }
}
