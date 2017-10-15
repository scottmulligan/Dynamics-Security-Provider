namespace CRMSecurityProvider.Utils
{
  using System;
  using System.Diagnostics;

  class CrmStopwatch : Stopwatch
  {
    private TimeSpan lastElapsed;

    public new TimeSpan Elapsed
    {
      get
      {
        lastElapsed = base.Elapsed;
        return lastElapsed;
      }
    }

    public TimeSpan Delta
    {
      get
      {
        return base.Elapsed - lastElapsed;
      }
    }
  }
}
