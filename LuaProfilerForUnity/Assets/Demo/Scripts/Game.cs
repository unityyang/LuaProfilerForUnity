using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    static Game __inst;
    public static Game Instance { get { return __inst; } }
    public LuaProfilerScript m_luaProfiler;
    ProfilerProxyScript m_proxy;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        __inst = this;

        Init();

        SceneManager.LoadScene("demoCircle", LoadSceneMode.Single);
    }

    void Init()
    {
        m_luaProfiler = new LuaProfilerScript();
        m_proxy = new ProfilerProxyScript();
        ProfilerLibrary.ProfilerProxy.m_IProfilerProxy = m_proxy;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
