using System;
using System.Text;
using System.Collections.Generic;
using SLua;
using UnityEngine;
using UnityEngine.Networking.PlayerConnection;
using ProfilerLibrary;

public class CsLuaProfiler : ICsLuaProfiler
{
    static CsLuaProfiler __inst = new CsLuaProfiler();
    public static CsLuaProfiler Instance { get { return __inst; } }

    public static bool LuaInited { get; set; }

    public delegate void BeginSampleCallback(int sampleId);
    public static BeginSampleCallback m_beginSampleCallback;
    public delegate void BeginSampleTagCallback(string tag);
    public static BeginSampleTagCallback m_beginSampleTagCallback;
    public delegate void EndSampleCallback();
    public static EndSampleCallback m_endSampleCallback;

    public delegate void BeginSampleImplCallback(int sampleId, string name, long time, int lua_memory, int mono_memory, int frame_count, float fps, uint pss, float power, bool needShow);
    public static BeginSampleImplCallback m_beginSampleImplCallback;
    public delegate void EndSampleImplCallback(long time, int lua_memory, int mono_memory);
    public static EndSampleImplCallback m_endSampleImplCallback;

    public static void BindSampleRuntimeCallbacks()
    {
        m_beginSampleCallback = BeginSampleRuntime;
        m_beginSampleTagCallback = BeginSampleTagRuntime;
        m_endSampleCallback = EndSampleRuntime;
    }

    public static void UnbindSampleRuntimeCallbacks()
    {
        m_beginSampleCallback = null;
        m_beginSampleTagCallback = null;
        m_endSampleCallback = null;
    }

    /// <summary>
    /// Get your self lua state ptr
    /// </summary>
    public static IntPtr ptr_luaState
    {
        get
        {
            if (LuaInited)
            {
                if (LuaState.main != null)
                {
                    return LuaState.main.L;
                }
            }
            return IntPtr.Zero;
        }
    }
    public void BeginSample(int sampleId)
    {
        if (m_beginSampleCallback != null)
        {
            m_beginSampleCallback(sampleId);
        }
        //LuaProfiler.BeginSample(ptr_luaState, sampleId);
    }
    public static void BeginSampleRuntime(int sampleId)
    {
        long time = GetSampleTime();
        int lua_memory = GetLuaMemory();
        int mono_memory = GetMonoMemory();
        Send_BeginSample(sampleId, null, time, lua_memory, mono_memory, SampleData.frameCount, SampleData.fps, SampleData.pss, SampleData.power);
    }

    public void EndSample()
    {
        if (m_endSampleCallback != null)
        {
            m_endSampleCallback();
        }
        //LuaProfiler.EndSample(ptr_luaState);
    }
    public static void EndSampleRuntime()
    {
        long time = GetSampleTime();
        int lua_memory = GetLuaMemory();
        int mono_memory = GetMonoMemory();
        Send_EndSample(time, lua_memory, mono_memory);
    }

    public void BeginSampleTag(string tag)
    {
        if (m_beginSampleTagCallback != null)
        {
            m_beginSampleTagCallback(tag);
        }
        //LuaProfiler.BeginSample(ptr_luaState, tag);
    }

    public static void BeginSampleTagRuntime(string tag)
    {
        long time = GetSampleTime();
        int lua_memory = GetLuaMemory();
        int mono_memory = GetMonoMemory();
        Send_BeginSample(-1, tag, time, lua_memory, mono_memory, SampleData.frameCount, SampleData.fps, SampleData.pss, SampleData.power);
    }

    public void EndSampleTag()
    {
        if (m_endSampleCallback != null)
        {
            m_endSampleCallback();
        }
        //LuaProfiler.EndSample(ptr_luaState);
    }

    public static void SetProfilerEnable(bool enable)
    {
        CsLuaProfilerLib.EnableProfiler = enable;
    }

    public bool EnableProfile()
    {
        return true;
    }

    int count = 0;
    float currentTime = 0;
    float deltaTime = 0f;

    public void Reset()
    {
        count = 0;
        currentTime = 0;
        deltaTime = 0;
    }
    public void Tick()
    {
        CsLuaProfiler.FlushSendMessagePackage();
        if (CsLuaProfilerLib.EnableProfiler)
        {
            SampleData.frameCount = Time.frameCount;
            count++;
            deltaTime += Time.unscaledDeltaTime;
            if (deltaTime >= 1f)
            {
                SampleData.fps = count / deltaTime;
                count = 0;
                deltaTime = 0f;
            }
            if (Time.unscaledTime - currentTime > 0.1f)
            {
                SampleData.pss = (uint)UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
                SampleData.power = 100;
                currentTime = Time.unscaledTime;
            }
        }
        else
        {
            Reset();
        }
    }

    static long GetSampleTime()
    {
        return System.Diagnostics.Stopwatch.GetTimestamp();
    }
    static int GetLuaMemory()
    {
        return ptr_luaState != IntPtr.Zero ? SLua.LuaDLL.lua_gc(ptr_luaState, SLua.LuaGCOptions.LUA_GCCOUNT, 0) : 0;
    }
    static int GetMonoMemory()
    {
        return (int)GC.GetTotalMemory(false);
    }

    public static string GetSampleName(int sampleId)
    {
        return ScriptTimeProfiler.GetSampleEnumName(sampleId);
    }
    #region SendPlayerConnectionMessage
    public static readonly System.Guid m_playerConnectionMsgTick = new System.Guid("2911dcb5-5f64-45bf-b6c6-b0261753acd4");
    public static readonly System.Guid m_playerConnectionMsgEditorCmd = new System.Guid("04905ac4-cd66-42fd-8155-c8faa2681230");
    public static List<byte> packageByteList = new List<byte>();
    static List<byte> singleByteList = new List<byte>();
    public static void Send_BeginSample(int sampleId, string name, long time, int lua_memory, int mono_memory, int frame_count, float fps, uint pss, float power)
    {
        if (!CsLuaProfilerLib.EnableProfiler)
        {
            return;
        }
        singleByteList.Clear();
        singleByteList.AddRange(BitConverter.GetBytes(0));
        singleByteList.AddRange(BitConverter.GetBytes(sampleId));
        if (string.IsNullOrEmpty(name))
        {
            singleByteList.AddRange(BitConverter.GetBytes((int)0));
        }
        else
        {
            byte[] name_bytes = Encoding.UTF8.GetBytes(name);
            int name_byte_size = name_bytes.Length + sizeof(int);
            singleByteList.AddRange(BitConverter.GetBytes(name_byte_size));
            singleByteList.AddRange(name_bytes);
        }
        singleByteList.AddRange(BitConverter.GetBytes(time));
        singleByteList.AddRange(BitConverter.GetBytes(lua_memory));
        singleByteList.AddRange(BitConverter.GetBytes(mono_memory));
        singleByteList.AddRange(BitConverter.GetBytes(frame_count));
        singleByteList.AddRange(BitConverter.GetBytes(fps));
        singleByteList.AddRange(BitConverter.GetBytes(pss));
        singleByteList.AddRange(BitConverter.GetBytes(power));
        int length = singleByteList.Count + sizeof(int);
        singleByteList.InsertRange(0, BitConverter.GetBytes(length));
        packageByteList.AddRange(singleByteList);
    }
    public static void Send_EndSample(long time, int lua_memory, int mono_memory)
    {
        if (!CsLuaProfilerLib.EnableProfiler)
        {
            return;
        }
        singleByteList.Clear();
        singleByteList.AddRange(BitConverter.GetBytes(1));
        singleByteList.AddRange(BitConverter.GetBytes(time));
        singleByteList.AddRange(BitConverter.GetBytes(lua_memory));
        singleByteList.AddRange(BitConverter.GetBytes(mono_memory));
        int length = singleByteList.Count + sizeof(int);
        singleByteList.InsertRange(0, BitConverter.GetBytes(length));
        packageByteList.AddRange(singleByteList);
    }
    public static void FlushSendMessagePackage()
    {
        if (!CsLuaProfilerLib.EnableProfiler)
        {
            return;
        }
        if (CsLuaProfiler.packageByteList.Count > 0)
        {
            PlayerConnection.instance.Send(m_playerConnectionMsgTick, CsLuaProfiler.packageByteList.ToArray());
            CsLuaProfiler.packageByteList.Clear();
        }
    }
    #endregion

    #region HandlePlayerConnectionMessage
    public static void Handle_MsgPackage(byte[] bytes)
    {
        int index = 0;
        while (index + sizeof(int) < bytes.Length)
        {
            int currIndex = index;
            int length = BitConverter.ToInt32(bytes, currIndex); currIndex += sizeof(int);
            index += length;
            if (currIndex + sizeof(int) < bytes.Length)
            {
                int type = BitConverter.ToInt32(bytes, currIndex); currIndex += sizeof(int);
                if (type == 0)
                {
                    ParseMsgBeginSample(bytes, currIndex);
                }
                else if (type == 1)
                {
                    ParseMsgEndSample(bytes, currIndex);
                }
            }
        }
    }
    static void ParseMsgBeginSample(byte[] bytes, int startIndex)
    {
        int sampleId = BitConverter.ToInt32(bytes, startIndex); startIndex += sizeof(int);
        string name = null;
        int str_length = BitConverter.ToInt32(bytes, startIndex); startIndex += sizeof(int);
        if (str_length > 0)
        {
            name = System.Text.Encoding.UTF8.GetString(bytes, startIndex, str_length - sizeof(int));
            startIndex += str_length - sizeof(int);
        }
        long time = BitConverter.ToInt64(bytes, startIndex); startIndex += sizeof(long);
        int lua_memory = BitConverter.ToInt32(bytes, startIndex); startIndex += sizeof(int);
        int mono_memory = BitConverter.ToInt32(bytes, startIndex); startIndex += sizeof(int);
        int frame_count = BitConverter.ToInt32(bytes, startIndex); startIndex += sizeof(int);
        float fps = BitConverter.ToSingle(bytes, startIndex); startIndex += sizeof(float);
        uint pss = BitConverter.ToUInt32(bytes, startIndex); startIndex += sizeof(uint);
        float power = BitConverter.ToSingle(bytes, startIndex); startIndex += sizeof(float);
        if (m_beginSampleImplCallback != null)
        {
            m_beginSampleImplCallback(sampleId, name, time, lua_memory, mono_memory, frame_count, fps, pss, power, false);
        }
    }
    static void ParseMsgEndSample(byte[] bytes, int startIndex)
    {
        long time = BitConverter.ToInt64(bytes, startIndex); startIndex += sizeof(long);
        int lua_memory = BitConverter.ToInt32(bytes, startIndex); startIndex += sizeof(int);
        int mono_memory = BitConverter.ToInt32(bytes, startIndex); startIndex += sizeof(int);
        if (m_endSampleImplCallback != null)
        {
            m_endSampleImplCallback(time, lua_memory, mono_memory);
        }
    }
    #endregion

    public class SampleData
    {
        public static int frameCount;
        public static float fps;
        public static uint pss;
        public static float power;
    }
}