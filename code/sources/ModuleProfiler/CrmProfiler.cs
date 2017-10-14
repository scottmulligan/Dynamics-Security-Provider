namespace CRMSecurityProvider
{
  using System;
  using System.Collections.Generic;

  using Sitecore.Diagnostics;
  
  public class CrmProfiler : IDisposable
  {
    private static readonly Dictionary<string, CrmProfilerCounter> Counters = new Dictionary<string, CrmProfilerCounter>();

    private readonly string methodName;
    private readonly HighResTimer timer;

    public static IDisposable Measure(string methodName)
    {
      return new CrmProfiler(methodName);
    }

    public static CrmProfilerCounter GetCounter(string methodName)
    {
      lock (Counters)
      {
        if (Counters.ContainsKey(methodName))
        {
          return Counters[methodName];
        }

        return null;
      }
    }

    public static IEnumerable<CrmProfilerCounter> GetCounters()
    {
      lock (Counters)
      {
        return Counters.Values;
      }
    }

    public static void Reset()
    {
      lock (Counters)
      {
        Counters.Clear();
      }
    }

    public CrmProfiler(string methodName)
    {
      lock (Counters)
      {
        if (!Counters.ContainsKey(methodName))
        {
          Counters.Add(methodName, new CrmProfilerCounter(methodName));
        }
      }

      this.methodName = methodName;
      this.timer = new HighResTimer(true);
    }

    public void Dispose()
    {
      var counter = GetCounter(this.methodName);

      if (counter != null)
      {
        counter.AddTiming(this.timer.ElapsedTimeSpan);
      }
    }
  }
}
