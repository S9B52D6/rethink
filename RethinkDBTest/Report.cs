using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RethinkDBTest
{
    public class Report
    {

        public double Period = 360;
        public double PropertyValue = 1;
        public double OutstandingBal = 1;
        public double SchedPrin = 0.0;
        public double Int = 0.0;
        public double Current = 1;
        public double Ppy = 0.0;
        public double BalanceForgiveness = 0.0;
        public double CapitalizedInterest = 0.0;
        public double CumulativeAccruedArrears = 0.0;
        public double Cure = 0.0;
        public double DeferredInt = 0.0;
        public double DirtyBalance = 0.0;
        public double DQ = 0.0;
        public double ExcessServicing = 0.0;
        public double GFee = 0.0;
        public double Liq = 0.0;
        public double Loss = 0.0;
        public double LostInt = 0.0;
        public double MasterServicing = 0.0;
        public double MIFee = 0.0;
        public double Mod = 0.0;
        public double RecoveredArrears = 0.0;
        public double RecoveryPrin = 0.0;
        public double SubServicing = 0.0;
        public double Total_DQ = 0.0;
        public string ScenarioID;

        private static Random r = new Random();

        public Report()
        {
            Current = r.Next(50000, 500000);
            OutstandingBal = r.Next(50000, 500000);
            PropertyValue = r.Next(50000, 500000);
            ScenarioID = Guid.NewGuid().ToString();
        }
    }
}
