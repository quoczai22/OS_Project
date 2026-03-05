using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Race_Condition.Models
{
    public class MoPhongKetQuaModel
    {
        public int GiaTriKyVong { get; set; }
        public int GiaTriThuc { get; set; }
        public double ThoiGianThucHien { get;set; }
        public bool IsRaceCondition { get; set; }
        public string Mode { get; set; }

    }
}
