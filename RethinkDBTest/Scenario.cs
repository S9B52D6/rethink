using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RethinkDBTest
{
    public class Scenario
    {
        public int ReportType;
        public int PortfolioId;
        public int ViewId;
        public String UserId;
        public DateTime YieldMatrixDate;
        public DateTime SettlementDate;
        public String[] ScenarioKeys;
        public String[] Errors = { };
        public object AsOfDates;
        public object Reports;
        public string compositeIndex;

        private static Random r = new Random();

        public Scenario()
        {
            ReportType = 0;
            PortfolioId = r.Next(0, 10000);
            ViewId = 0;
            UserId = Guid.NewGuid().ToString();
            YieldMatrixDate = DateTime.Now.AddYears(r.Next(5, 30));
            SettlementDate = DateTime.Now.AddYears(r.Next(5, 30));
            ScenarioKeys = new String[] { "FBA Base", "FBA Base 1", "FBA Base 2" };
            Errors = new String[] { };
            compositeIndex = PortfolioId.ToString() + '_' + UserId + '_' + SettlementDate.ToFileTimeUtc().ToString();

            AsOfDates = new
            {
                ScenarioViewModel = new
                {
                    CurrentCoupon = DateTime.Parse("2017-12-01T00:00:00"),
                    ForwardCurve = DateTime.Parse("2017-12-01T00:00:00"),
                    HpForecast = DateTime.Parse("2017-12-01T00:00:00"),
                    UnempForecast = DateTime.Parse("2017-12-01T00:00:00")
                }
            };

            Reports = new
            {
                FBA_Base = GenerateReportList(360),
                FBA_Base_1 = GenerateReportList(360),
                FBA_Base_2 = GenerateReportList(360)
            };
        }

        private static List<Report> GenerateReportList(int amount)
        {
            List<Report> reports = new List<Report>(amount);

            for(int i = 0; i < amount; i++)
            {
                reports.Add(new Report());
            }

            return reports;
        }
    }
}
