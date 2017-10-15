namespace CRMSecurityProvider
{
  using System;
  
  public class CrmProfilerCounter
  {
    private long callCount;
    private long totalTime;
    private long maximumTime;
    private long minimumTime;

    private readonly object locker = new object();

    public CrmProfilerCounter(string operationName)
    {
      this.OperationName = operationName;
    }

    public string OperationName { get; private set; }

    public long CallCount
    {
      get
      {
        lock (this.locker)
        {
          return this.callCount;
        }
      }
      private set
      {
        lock (this.locker)
        {
          this.callCount = value;
        }
      }
    }

    public long TotalTime
    {
      get
      {
        lock (this.locker)
        {
          return this.totalTime;
        }
      }
      private set
      {
        lock(this.locker)
        {
          this.totalTime = value;
        }
      }
    }

    public long MaximumTime
    {
      get
      {
        lock (this.locker)
        {
          return this.maximumTime;
        }
      }
      private set
      {
        lock (this.locker)
        {
          this.maximumTime = value;
        }
      }
    }

    public long MinimumTime
    {
      get
      {
        lock (this.locker)
        {
          return this.minimumTime;
        }
      }
      private set
      {
        lock (this.locker)
        {
          this.minimumTime = value;
        }
      }
    }

    public long AverageTime
    {
      get
      {
        lock (this.locker)
        {
          return (this.CallCount != 0) ? (this.TotalTime / this.CallCount) : 0;
        }
      }
    }

    public void AddTiming(TimeSpan elapsedTime)
    {
      lock (this.locker)
      {
        var time = (long)elapsedTime.TotalMilliseconds;

        this.CallCount++;
        this.TotalTime += time;

        if (time > this.MaximumTime)
        {
          this.MaximumTime = time;
        }

        if (time < this.MinimumTime || this.MinimumTime == 0)
        {
          this.MinimumTime = time;
        }
      }
    }
  }
}
