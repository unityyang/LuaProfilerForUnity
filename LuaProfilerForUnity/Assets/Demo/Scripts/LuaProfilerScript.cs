using ProfilerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LuaProfilerScript : ILuaProfiler
{
    static LuaProfilerScript __inst = new LuaProfilerScript();
    public static LuaProfilerScript Instance
    {
        get
        {
            if (!__inited)
            {
                __inst = new LuaProfilerScript();
                __inst.Init();
                __inited = true;
            }
            return __inst;
        }
    }
    static bool __inited = false;
    public void Init()
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