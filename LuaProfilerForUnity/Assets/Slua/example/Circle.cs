using UnityEngine;
using System.Collections;
using SLua;
using System;

public class Circle : MonoBehaviour {


	LuaSvr svr;
	LuaTable self;
	LuaFunction update;

    [CustomLuaClass]
    public delegate void UpdateDelegate(object self);

    UpdateDelegate ud;

	void Start () {
		//MikuLuaProfiler.HookLuaSetup.OnStartGame();
        Debug.Log("Hooked: " + MikuLuaProfiler.LuaDLL.m_hooked);
        svr = new LuaSvr();
        svr.init(null, () =>
        {
            self = (LuaTable)svr.start("circle/circle");
            update = (LuaFunction)self["update"];
            ud = update.cast<UpdateDelegate>();
        });
        Debug.Log("Hooked2: " + MikuLuaProfiler.LuaDLL.m_hooked);
	}
	
	void Update () {
        CsLuaProfiler.CsInstance().BeginSample((int)ECsLuaProfilerSample.Sample3);
        if (ud != null) ud(self);
        CsLuaProfiler.CsInstance().EndSample();

    }
}
