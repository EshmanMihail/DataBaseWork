using System;
using System.Collections.Generic;

namespace Lab2BD5
{
    public partial class PipelineSection : ITable
    {
        public int SectionId { get; set; }
        public int? SectionNumber { get; set; }
        public int? StartNodeNumber { get; set; }
        public int? EndNodeNumber { get; set; }
        public decimal? PipelineLength { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Thickness { get; set; }
        public DateTime? LastRepairDate { get; set; }

        public virtual HeatPoint EndNodeNumberNavigation { get; set; }
        public virtual HeatPoint StartNodeNumberNavigation { get; set; }

        public int ID => SectionId;
    }
}
