using UnityEngine;
using UnityEngine.Profiling;

public static class ProfilerSetup
{
    [RuntimeInitializeOnLoadMethod]
    private static void InitProfiler()
    {
        Profiler.maxUsedMemory = 256 * 1024 * 1024;
    }
}
