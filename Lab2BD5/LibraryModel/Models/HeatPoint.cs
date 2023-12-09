using System;
using System.Collections.Generic;

namespace Lab2BD5
{
    public partial class HeatPoint : ITable
    {
        public HeatPoint()
        {
            PipelineSectionEndNodeNumberNavigation = new HashSet<PipelineSection>();
            PipelineSectionStartNodeNumberNavigation = new HashSet<PipelineSection>();
        }

        public int PointId { get; set; }
        public string PointName { get; set; }
        public int? NetworkId { get; set; }
        public int? NodeNumber { get; set; }

        public virtual ICollection<PipelineSection> PipelineSectionEndNodeNumberNavigation { get; set; }
        public virtual ICollection<PipelineSection> PipelineSectionStartNodeNumberNavigation { get; set; }
        public virtual HeatNetwork Network { get; set; }

        public int ID => PointId;
    }
}
