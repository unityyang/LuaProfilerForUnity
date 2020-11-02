using MikuLuaProfiler;
using ProfilerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProfilerProxyScript : IProfilerProxy
{
    public ILuaProfiler GetLuaProfiler()
    {
        return LuaProfilerScript.Instance;
    }
}
