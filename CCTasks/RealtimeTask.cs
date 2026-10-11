using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTasks
{
    public class RealtimeTask
    {
        public string Name { get; set; } = "";
        public string Hardness { get; set; } = "";

        public double C { get; set; }
        public double D { get; set; }
        public double T { get; set; }

        public string deadlineType;
        public double L;
        public string signFeasibility;
    }
}
