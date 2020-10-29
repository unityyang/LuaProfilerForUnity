using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfilerLibrary
{
    public enum EProfilerSample
    {
        NONE,
        Sample1,
        Sample2,
        Sample3,
        Sample4,
    }
    public interface ILuaProfiler
    {
        void BeginSample(int sampleId);
        void EndSample();
    }
    public class Profilers
    {
        public static void BeginSample(int sampleId)
        {
            if(ProfilerProxy.m_IProfilerProxy != null)
            {
                ProfilerProxy.m_IProfilerProxy.GetLuaProfiler().BeginSample(sampleId);
            }
        }
        public static void EndSample()
        {
            if (ProfilerProxy.m_IProfilerProxy != null)
            {
                ProfilerProxy.m_IProfilerProxy.GetLuaProfiler().EndSample();
            }
        }
    }
}
