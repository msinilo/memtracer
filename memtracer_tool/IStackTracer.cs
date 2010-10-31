using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MemTracer
{
    public abstract class IStackTracer
    {
        public struct Symbol
        {
            public string functionName;
            public string fileName;
            public uint line;
            public uint address;
            public ulong functionAddress;
        };

        public abstract void AddModuleInfo(string pdbName, ulong moduleBase, ulong moduleSize);
        public abstract Symbol GetSymbolForAddress(uint addr);

        public abstract void Serialize(Stream s, BinaryFormatter formatter);
        public abstract void Deserialize(Stream s, BinaryFormatter formatter);
    }
}
