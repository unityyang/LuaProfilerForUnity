using ProfilerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ProfilerProxyScript : IScriptProxy
{
    public ICsLuaProfiler GetCsLuaProfiler()
    {
        return CsLuaProfiler.Instance as ICsLuaProfiler;
    }
}
