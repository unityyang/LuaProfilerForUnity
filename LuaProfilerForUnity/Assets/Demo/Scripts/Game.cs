using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    ProfilerProxyScript m_proxy;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        Init();
        SceneManager.LoadScene("demoCircle", LoadSceneMode.Single);
    }

    void Init()
    {
        m_proxy = new ProfilerProxyScript();
        ProfilerLibrary.ProfilerProxy.m_ScriptProxy = m_proxy;
        InitCsLuaProfiler();
    }

    void Update()
    {
        
    }

    void LateUpdate()
    {
        CsLuaProfiler.Instance.Tick();
    }

    #region lua_profiler
    bool m_playerConnectionConnected = false;
    bool m_initedCsLuaProfiler = false;
    void InitCsLuaProfiler()
    {
#if UNITY_EDITOR
        return; //Init Only Device
#endif
        if (m_initedCsLuaProfiler)
        {
            return;
        }
        m_initedCsLuaProfiler = true;
        Debug.Log("InitCsLuaProfiler");
        CsLuaProfiler.SetProfilerEnable(false);
        CsLuaProfiler.UnbindSampleRuntimeCallbacks();
        CsLuaProfiler.BindSampleRuntimeCallbacks();

        PlayerConnection.instance.RegisterConnection(OnConnect);
        PlayerConnection.instance.RegisterDisconnection(OnDisconnect);
        PlayerConnection.instance.Register(CsLuaProfiler.m_playerConnectionMsgEditorCmd, Handle_PlayerConnectionMsgEditorCmd);
    }
    void OnConnect(int arg)
    {
        m_playerConnectionConnected = true;
        Debug.Log("OnConnectEditor 0_0");
    }
    void OnDisconnect(int arg)
    {
        m_playerConnectionConnected = false;
        Debug.Log("OnDisconnectEditor -_-");
    }
    void Handle_PlayerConnectionMsgEditorCmd(MessageEventArgs evt)
    {
        byte[] bytes = evt.data;
        int index = 0;
        int cmd = BitConverter.ToInt32(bytes, index); index += sizeof(int);
        if (cmd == 1)
        {
            bool enable = BitConverter.ToBoolean(bytes, index); index += sizeof(bool);
            CsLuaProfiler.SetProfilerEnable(enable);
        }
    }
    #endregion
}
