using System;
using MikuLuaProfiler;
using SLua;
using UnityEngine;

[CustomLuaClass]
public class CsLuaProfiler
{
    static CsLuaProfiler __inst = new CsLuaProfiler();
    public static CsLuaProfiler Instance { get { return __inst; } }

    public delegate void BeginSampleDelegate(int smapleId);
    public static BeginSampleDelegate m_BeginSampleDelegate;
    public delegate void EndSampleDelegate();
    public static EndSampleDelegate m_EndSampleDelegate;
    public delegate string GetSampleNameDelegate(int sampleId);
    public static GetSampleNameDelegate m_GetSampleNameDelegate;


    [SLua.MonoPInvokeCallbackAttribute(typeof(SLua.LuaCSFunction))]
    [StaticExport]
    public static int LuaInstance(IntPtr l)
    {
        SLua.LuaObject.pushValue(l, true);
        SLua.LuaObject.pushObject(l, CsLuaProfiler.Instance);
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
        if(m_GetSampleNameDelegate != null)
        {
            return m_GetSampleNameDelegate(sampleId);
        }
        return "[defaultSample]";
    }
}
