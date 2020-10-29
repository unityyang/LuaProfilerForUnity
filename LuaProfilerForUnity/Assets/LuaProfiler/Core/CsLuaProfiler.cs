using System;
using MikuLuaProfiler;
using SLua;
using UnityEngine;

[CustomLuaClass]
public interface ICsLuaProfiler
{
    void BeginSample(int sampleId);
    void EndSample();
}

[CustomLuaClass]
public class CsLuaProfiler : ICsLuaProfiler
{
    static CsLuaProfiler __inst = new CsLuaProfiler();

    public static ICsLuaProfiler CsInstance() { return (ICsLuaProfiler)__inst; }

    public delegate void BeginSampleDelegate(int smapleId);
    public static BeginSampleDelegate m_BeginSampleDelegate;
    public delegate void EndSampleDelegate();
    public static EndSampleDelegate m_EndSampleDelegate;


    [SLua.MonoPInvokeCallbackAttribute(typeof(SLua.LuaCSFunction))]
    [StaticExport]
    public static int LuaInstance(IntPtr l)
    {
        SLua.LuaObject.pushValue(l, true);
        SLua.LuaObject.pushObject(l, CsLuaProfiler.CsInstance());
        return 2;
    }


    [SLua.MonoPInvokeCallbackAttribute(typeof(SLua.LuaCSFunction))]
    public void BeginSample(int sampleId)
    {
        if(m_BeginSampleDelegate != null)
        {
            m_BeginSampleDelegate(sampleId);
        }
    }

    [SLua.MonoPInvokeCallbackAttribute(typeof(SLua.LuaCSFunction))]
    public void EndSample()
    {
        if(m_EndSampleDelegate != null)
        {
            m_EndSampleDelegate();
        }
    }

    public static string GetSampleName(int sampleId)
    {
        return ((ECsLuaProfilerSample)sampleId).ToString();
    }
}

public enum ECsLuaProfilerSample
{
    None = 0,
    Sample1,
    Sample2,
    Sample3,
    Sample4,
}

