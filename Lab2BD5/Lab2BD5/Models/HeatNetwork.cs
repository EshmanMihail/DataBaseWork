using System;
using System.Collections.Generic;

namespace Lab2BD5
{
    public partial class HeatNetwork
    {
        public HeatNetwork()
        {
            HeatConsumer = new HashSet<HeatConsumer>();
            HeatPoint = new HashSet<HeatPoint>();
            HeatWell = new HashSet<HeatWell>();
        }

        public int NetworkId { get; set; }
        public string NetworkName { get; set; }
        public int? NetworkNumber { get; set; }
        public int? EnterpriseId { get; set; }
        public string NetworkType { get; set; }

        public virtual ICollection<HeatConsumer> HeatConsumer { get; set; }
        public virtual ICollection<HeatPoint> HeatPoint { get; set; }
        public virtual ICollection<HeatWell> HeatWell { get; set; }
        public virtual Enterprise Enterprise { get; set; }
    }
}
