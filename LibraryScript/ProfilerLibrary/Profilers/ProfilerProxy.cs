using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfilerLibrary
{
    public interface IProfilerProxy
    {
        ILuaProfiler GetLuaProfiler();
    }
    public class ProfilerProxy
    {
        public static IProfilerProxy m_IProfilerProxy;
    }
}
