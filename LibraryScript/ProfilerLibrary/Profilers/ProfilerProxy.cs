using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfilerLibrary
{
    public interface IScriptProxy
    {
        ICsLuaProfiler GetCsLuaProfiler();
    }
    public class ProfilerProxy
    {
        public static IScriptProxy m_ScriptProxy;
    }
}
