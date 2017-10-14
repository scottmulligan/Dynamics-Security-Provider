namespace CRMSecurityProvider
{
  using System;
  using System.Linq;
  using System.Web;
  using System.Web.UI;
  using System.Web.UI.HtmlControls;

  using Sitecore.Web;

  public partial class CRMProviderProfiler : Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      if (this.IsPostBack)
      {
        return;
      }
      this.CheckSecurity();

      this.ShowStatistics();
    }

    protected void Update_Click(object sender, EventArgs e)
    {
      this.ShowStatistics();
    }

    protected void Reset_Click(object sender, EventArgs e)
    {
      CrmProfiler.Reset();
      CrmProfilerExtension.Reset();

      this.ShowStatistics();
    }

    private void CheckSecurity()
    {
      if (Sitecore.Context.User.IsAdministrator)
      {
        return;
      }

      var site = Sitecore.Context.Site;
      var loginPage = (site != null) ? site.LoginPage : String.Empty;

      if (!String.IsNullOrEmpty(loginPage))
      {
        this.Response.Redirect(String.Format("{0}?returnUrl={1}", loginPage, HttpUtility.UrlEncode(this.Request.Url.PathAndQuery)));
      }
    }

    private void ShowStatistics()
    {
      var table1 = new HtmlTable();
      table1.Border = 1;

      HtmlUtil.AddRow(table1, "Operation", "Count", "Avg. time (ms)", "Min. time (ms)", "Max. time (ms)", "Total time (ms)");

      var counters = CrmProfiler.GetCounters().OrderBy(counter => counter.OperationName);
      foreach (var counter in counters)
      {
        HtmlUtil.AddRow(table1, counter.OperationName, counter.CallCount, counter.AverageTime, counter.MinimumTime, counter.MaximumTime, counter.TotalTime);
      }

      this.statistics.Controls.Add(table1);

      this.statistics.Controls.Add(new LiteralControl("<br/>"));

      var table2 = new HtmlTable();
      table2.Border = 1;

      HtmlUtil.AddRow(table2, "CRM calls count", "Requests size (kB)", "Responses size (kB)");
      HtmlUtil.AddRow(table2, CrmProfilerExtension.CallCount, Math.Round(CrmProfilerExtension.TotalRequestLength, 3), Math.Round(CrmProfilerExtension.TotalResponseLength, 3));

      this.statistics.Controls.Add(table2);
    }
  }
}
