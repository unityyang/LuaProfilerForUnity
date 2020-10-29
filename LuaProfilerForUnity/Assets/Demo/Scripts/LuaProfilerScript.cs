using ProfilerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LuaProfilerScript : ILuaProfiler
{
    public LuaProfilerScript()
    {
        CsLuaProfiler.m_GetSampleNameDelegate = GetSampleName;
    }

    public void BeginSample(int sampleId)
    {
        CsLuaProfiler.Instance.BeginSample(sampleId);
    }

    public void EndSample()
    {
        CsLuaProfiler.Instance.EndSample();
    }
    public string GetSampleName(int sampleId)
    {
        return ((EProfilerSample)sampleId).ToString();
    }
}