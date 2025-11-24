using System.Diagnostics;

namespace __Workspaces.Hugoi.Scripts
{
    public static class Perf
    {
        public static Stopwatch Measure(string label)
        {
            UnityEngine.Debug.Log($"⏱️ Start: {label}");
            var sw = new Stopwatch();
            sw.Start();
            return sw;
        }

        public static void End(Stopwatch sw, string label)
        {
            sw.Stop();
            UnityEngine.Debug.Log($"⏱️ End: {label} — {sw.ElapsedMilliseconds} ms");
        }
    }
}