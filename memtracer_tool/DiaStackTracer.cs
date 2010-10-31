using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using Dia2Lib;

namespace MemTracer
{
    class DiaStackTracer : IStackTracer
    {
        [Serializable]
        class ModuleInfo
        {
            public bool Init(string name, ulong modBase, ulong modSize)
            {
                diaSource = new DiaSourceClass();
                try
                {
                    if (name.Contains("pdb"))
                        diaSource.loadDataFromPdb(name);
                    else
                        diaSource.loadDataForExe(name, null, null);
                }
                catch (COMException)
                {
                    System.Windows.Forms.MessageBox.Show("Couldn't load symbols from " + name);
                    return false;
                }
                diaSource.openSession(out diaSession);
                diaSession.loadAddress = modBase;
                size = modSize;
                symbolFileName = name;
                baseAddress = modBase;
                return true;
            }
            public bool ContainsAddress(uint addr)
            {
                return addr >= baseAddress && addr < baseAddress + size;
            }
            public void GetSymbolForAddress(uint addr, ref Symbol ret)
            {
                IDiaSymbol symbol = null;
                SymTagEnum tagEnum = SymTagEnum.SymTagFunction;
                diaSession.findSymbolByVA((ulong)addr, tagEnum, out symbol);

                if (symbol != null)
                {
                    ret.functionName = (tagEnum == SymTagEnum.SymTagNull ? symbol.undecoratedName : symbol.name);
                    ret.address = addr;
                    ret.functionAddress = symbol.virtualAddress;

                    IDiaEnumLineNumbers lineNumbers;
                    diaSession.findLinesByVA(addr, 1, out lineNumbers);
                    uint celt = 0;
                    IDiaLineNumber line;
                    while (true)
                    {
                        lineNumbers.Next(1, out line, out celt);
                        if (celt == 1)
                        {
                            ret.fileName = line.sourceFile.fileName;
                            ret.line = line.lineNumber;
                            break;
                        }
                        if (celt != 1)
                            break;
                    }
                }
            }

            public string symbolFileName;
            public ulong baseAddress;
            public ulong size;
            [NonSerialized]
            IDiaDataSource diaSource;
            [NonSerialized]
            IDiaSession diaSession;
        };

        public override void AddModuleInfo(String pdbName, ulong moduleBase, ulong moduleSize)
        {
            ModuleInfo info = new ModuleInfo();
            if (info.Init(pdbName, moduleBase, moduleSize))
                m_modules.Add(info);
        }

        public override Symbol GetSymbolForAddress(uint addr)
        {
            if (addr == 0)
                return m_emptySymbol;

            Symbol ret;
            if (m_symbols.TryGetValue(addr, out ret))
            {
                return ret;
            }

            ret = new Symbol();
            foreach (ModuleInfo info in m_modules)
            {
                info.GetSymbolForAddress(addr, ref ret);
                if (ret.functionName != null)
                    break;
            }
            if (ret.functionName == null)
            {
                ret.functionName = addr.ToString("X");
                ret.address = addr;
            }
            m_symbols.Add(addr, ret);
            return ret;
        }

        public override void Serialize(Stream s, BinaryFormatter formatter)
        {
            formatter.Serialize(s, m_modules);
        }
        public override void Deserialize(Stream s, BinaryFormatter formatter)
        {
            List<ModuleInfo> infos = formatter.Deserialize(s) as List<ModuleInfo>;
            m_modules.Clear();
            foreach (ModuleInfo info in infos)
                AddModuleInfo(info.symbolFileName, info.baseAddress, info.size);
        }

        List<ModuleInfo> m_modules = new List<ModuleInfo>();
        Dictionary<uint, Symbol> m_symbols = new Dictionary<uint, Symbol>();
        Symbol m_emptySymbol = new Symbol();
    }
}