using UnityEngine;
using System.Collections;
using SLua;
using System;
using ProfilerLibrary;

public class DemoCircle : MonoBehaviour
{


    LuaSvr svr;
    LuaTable self;
    LuaFunction update;

    [CustomLuaClass]
    public delegate void UpdateDelegate(object self);

    UpdateDelegate ud;

    void Start()
    {
        svr = new LuaSvr();
        svr.init(null, () =>
        {
            self = (LuaTable)svr.start("DemoCircle");
            update = (LuaFunction)self["update"];
            ud = update.cast<UpdateDelegate>();
        });
    }

    void Update()
    {
        ScriptTimeProfiler.BeginSample(EProfilerSampleEnum.Sample3);
        if (ud != null) ud(self);
        ScriptTimeProfiler.EndSample();
    }
}
