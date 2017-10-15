namespace CRMSecurityProvider.Utils
{
  using System;
  using System.Collections.Generic;
  using System.Threading;

  using Configuration;

  using Sitecore.Diagnostics;

  /// <summary>
  /// Conditional logging class.
  /// </summary>
  public static class ConditionalLog
  {
    static private readonly Dictionary<string, CrmStopwatch> watches;
    private static Timer timer;

    private static object sync;

    static ConditionalLog()
    {
      sync = new object();
      watches = new Dictionary<string, CrmStopwatch>();
      timer = new Timer(
        delegate
        {
          try
          {

            var tempWatches = new Dictionary<string, CrmStopwatch>(watches);
            foreach (KeyValuePair<string, CrmStopwatch> watchPair in tempWatches)
            {
              CrmStopwatch watch = watchPair.Value;
              if ((watch.ElapsedMilliseconds > TimeSpan.FromMinutes(10).TotalMilliseconds) && (Monitor.TryEnter(sync)))
              {
                try
                {
                  watches.Remove(watchPair.Key);
                }
                finally
                {
                  Monitor.Exit(sync);
                }
              }
            }
          }
          catch (Exception ex)
          {
            if (Settings.LoggingLevel >= LoggingLevel.Warning)
            {
              Log.Warn("ConditionalLog watches removing error", ex, null);
            }
          }
        },
        null,
        TimeSpan.FromMinutes(5),
        TimeSpan.FromMinutes(5));
    }

    public static void Error(string message, object owner)
    {
      if (Settings.LoggingLevel >= LoggingLevel.Error)
      {
        Log.Error(message, owner);
      }
    }

    public static void Error(string message, Exception sourceException, object owner)
    {
      if (Settings.LoggingLevel >= LoggingLevel.Error)
      {
        Log.Error(message, sourceException, owner);
      }
    }

    public static void Warn(string message, object owner)
    {
      if (Settings.LoggingLevel >= LoggingLevel.Warning)
      {
        Log.Warn(message, owner);
      }
    }

    public static void Warn(string message, Exception sourceException, object owner)
    {
      if (Settings.LoggingLevel >= LoggingLevel.Warning)
      {
        Log.Warn(message, sourceException, owner);
      }
    }

    public static void Info(string message, object owner)
    {
      if (Settings.LoggingLevel >= LoggingLevel.Details)
      {
        Info(message, owner, TimerAction.None, string.Empty);
      }
      else
      {
        if (Settings.LoggingLevel >= LoggingLevel.Info)
        {
          Log.Info(message, owner);
        }
      }
    }

    public static void Info(string message, object owner, TimerAction timerAction, string timerKey)
    {
      if (Settings.LoggingLevel >= LoggingLevel.Details)
      {
        int handle = Thread.CurrentThread.ManagedThreadId;
        string timerHandle = timerKey + handle;
        switch (timerAction)
        {
          case TimerAction.None:
            Log.Info(message, owner);
            break;
          case TimerAction.Start:
            CrmStopwatch startWatch = new CrmStopwatch();
            if (watches.ContainsKey(timerHandle))
            {
              Info(message, owner, TimerAction.Tick, timerKey);
            }
            else
            {
              lock (sync)
              {
                watches.Add(timerHandle, startWatch);
              }
              Log.Info(string.Format("{0} Timer started.", message), owner);
              startWatch.Start();
            }
            break;
          case TimerAction.Tick:

            CrmStopwatch tickWatch;
            lock (sync)
            {
              tickWatch = watches[timerHandle];
            }
            if (tickWatch != null)
            {
              TimeSpan delta = tickWatch.Delta;
              Log.Info(string.Format("{0} Timing: +{1}={2}", message, delta, tickWatch.Elapsed), owner);
            }
            break;
          case TimerAction.Stop:
            CrmStopwatch stopwatch; 
            lock(sync)
            {
              stopwatch = watches[timerHandle];
            }
            if (stopwatch != null)
            {
              TimeSpan timeTaken = stopwatch.Elapsed;
              stopwatch.Stop();
              Log.Info(string.Format("{0} Time elapsed: {1}", message, timeTaken), owner);
              lock(sync)
              {
                watches.Remove(timerHandle);
              }
            }
            break;
        }
      }
      else
      {
        if (Settings.LoggingLevel >= LoggingLevel.Info)
        {
          Log.Info(message, owner);
        }
      }
    }
  }
}
