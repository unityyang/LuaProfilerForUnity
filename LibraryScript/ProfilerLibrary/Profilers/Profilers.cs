using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProfilerLibrary
{
    public enum EProfilerSampleEnum
    {
        NONE,
        Sample1,
        Sample2,
        Sample3,
        Sample4,
        Sample5,
        Sample6,
        Sample7,
        Sample8,
        Sample9,
        Max
    }
    public interface ICsLuaProfiler
    {
        void BeginSample(int sampleId);
        void EndSample();
        void BeginSampleTag(string tag);
        void EndSampleTag();
        bool EnableProfile();
    }
    /// <summary>
    /// Sub profiler for interactive with lua profiler
    /// </summary>
    public class CsLuaProfilerLib
    {
        static ICsLuaProfiler GetProfiler()
        {
            return ProfilerProxy.m_ScriptProxy != null ? ProfilerProxy.m_ScriptProxy.GetCsLuaProfiler() : null;
        }
        static bool m_enableProfiler = false;
        public static bool EnableProfiler
        {
            get { return m_enableProfiler; }
            set { m_enableProfiler = value; }
        }
        public static void BeginSample(int sampleId)
        {
            var profiler = GetProfiler();
            if (profiler != null)
            {
                profiler.BeginSample(sampleId);
            }
        }
        public static void EndSample()
        {
            var profiler = GetProfiler();
            if (profiler != null)
            {
                profiler.EndSample();
            }
        }
        public static void BeginSampleTag(string tag)
        {
            var profiler = GetProfiler();
            if (profiler != null)
            {
                profiler.BeginSampleTag(tag);
            }
        }

        public static void EndSampleTag()
        {
            var profiler = GetProfiler();
            if (profiler != null)
            {
                profiler.EndSampleTag();
            }
        }
    }
    /// <summary>
    /// Main profiler for project
    /// </summary>
    public static class ScriptTimeProfiler
    {   
        static bool m_enableSampleTag;
        public static bool EnableSampleTag 
        { 
            get { return m_enableSampleTag; }
            set { m_enableSampleTag = value; }
        }

        static Dictionary<int, string> m_enumNames = new Dictionary<int, string>();
        const string defaultSampleEnumName = "[UNKNOWN_SAMPLE]";

        public static void BeginSample(EProfilerSampleEnum e)
        {
            if (CsLuaProfilerLib.EnableProfiler)
            {
                CsLuaProfilerLib.BeginSample((int)e);
            }
        }

        public static void EndSample()
        {
            if (CsLuaProfilerLib.EnableProfiler)
            {
                CsLuaProfilerLib.EndSample();
            }
        }

        public static void BeginSampleTag(string tag)
        {
            if (!EnableSampleTag) { return; }
            if (CsLuaProfilerLib.EnableProfiler)
            {
                CsLuaProfilerLib.BeginSampleTag(tag);
            }
        }

        public static void EndSampleTag()
        {
            if (!EnableSampleTag) { return; }
            if (CsLuaProfilerLib.EnableProfiler)
            {
                CsLuaProfilerLib.EndSampleTag();
            }
        }

        public static void SetEnumName(int enumValue, string enumName)
        {
            m_enumNames[enumValue] = enumName;
        }
        public static void ResetEnumName(int enumValue)
        {
            m_enumNames.Remove(enumValue);
        }
        public static string GetSampleEnumName(int enumValue)
        {
            string name = null;
            if(m_enumNames.TryGetValue(enumValue, out name))
            {
                return name;
            }
            else if(enumValue > (int)EProfilerSampleEnum.NONE && enumValue < (int)EProfilerSampleEnum.Max)
            {
                return ((EProfilerSampleEnum)enumValue).ToString();
            }
            return defaultSampleEnumName;
        }
    }
}
