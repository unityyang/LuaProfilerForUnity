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

    [SLua.MonoPInvokeCallbackAttribute(typeof(SLua.LuaCSFunction))]
    [StaticExport]
    public static int LuaInstance(IntPtr l)
    {
        LuaObject.pushValue(l, true);
        LuaObject.pushObject(l, CsInstance());
        return 2;
    }

    IntPtr ptr_luaState { get { return LuaProfiler.mainL; } } //Get your self lua state ptr

    bool IsProfiling()
    {
        return MikuLuaProfiler.LuaDLL.m_hooked && Application.isPlaying && ptr_luaState != IntPtr.Zero;
    }


    [SLua.MonoPInvokeCallbackAttribute(typeof(SLua.LuaCSFunction))]
    public void BeginSample(int sampleId)
    {
        if (IsProfiling())
        {
            LuaProfiler.BeginSample(ptr_luaState, sampleId);
        }
    }

    [SLua.MonoPInvokeCallbackAttribute(typeof(SLua.LuaCSFunction))]
    public void EndSample()
    {
        if (IsProfiling())
        {
            LuaProfiler.EndSample(LuaProfiler.mainL);
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

