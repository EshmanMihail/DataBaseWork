using System;
using System.Collections.Generic;

namespace Lab2BD5
{
    public partial class HeatConsumer : ITable
    {
        public int ConsumerId { get; set; }
        public string ConsumerName { get; set; }
        public int? NetworkId { get; set; }
        public int? NodeNumber { get; set; }
        public decimal? CalculatedPower { get; set; }

        public virtual HeatNetwork Network { get; set; }

        public int ID => ConsumerId;
    }
}
