using System;
using System.Collections.Generic;

namespace Lab2BD5
{
    public partial class HeatWell : ITable
    {
        public int WellId { get; set; }
        public string WellName { get; set; }
        public int? NetworkId { get; set; }
        public int? NodeNumber { get; set; }

        public virtual HeatNetwork Network { get; set; }

        public int ID => WellId;
    }
}
