using System;
using System.Collections.Generic;
using System.Text;

namespace MemTracer
{
    [Serializable]
    public class MemOperation
    {
        public enum Type
        {
            Invalid = 0,
            Alloc,
            Free,
            FrameEnd
        };

        public MemOperation(Type type)
        {
            m_type = type;
            m_data = null;
        }

        public Type OpType { get { return m_type; } }
        public Object UserData
        {
            get { return m_data; }
            set { m_data = value; }
        }

        Type    m_type;
        Object  m_data;
    }
}
