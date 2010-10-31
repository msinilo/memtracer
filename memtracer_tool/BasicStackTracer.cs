using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MemTracer
{
    // Doesn't perform any kind of symbol lookup, only text representation of addresses.
    class BasicStackTracer : IStackTracer
    {
        public override void AddModuleInfo(string pdbName, ulong moduleBase, ulong moduleSize)
        {
        }
        public override Symbol GetSymbolForAddress(uint addr)
        {
            Symbol retSymbol;
            if (m_symbols.TryGetValue(addr, out retSymbol))
            {
                return retSymbol;
            }

            retSymbol = new Symbol();
            retSymbol.address = addr;
            retSymbol.functionName = addr.ToString("X");
            m_symbols.Add(addr, retSymbol);
            return retSymbol;
        }

        public override void Serialize(Stream s, BinaryFormatter formatter)
        {
        }
        public override void Deserialize(Stream s, BinaryFormatter formatter)
        {
        }

        static Dictionary<uint, Symbol> m_symbols = new Dictionary<uint, Symbol>();
    }
}
