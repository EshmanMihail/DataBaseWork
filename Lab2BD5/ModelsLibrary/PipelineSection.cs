using System;
using System.Collections.Generic;

namespace ModelsLibrary;

public partial class PipelineSection
{
    public int SectionId { get; set; }

    public int? SectionNumber { get; set; }

    public int? StartNodeNumber { get; set; }

    public int? EndNodeNumber { get; set; }

    public decimal? PipelineLength { get; set; }

    public decimal? Diameter { get; set; }

    public decimal? Thickness { get; set; }

    public DateOnly? LastRepairDate { get; set; }

    public virtual HeatPoint? EndNodeNumberNavigation { get; set; }

    public virtual HeatPoint? StartNodeNumberNavigation { get; set; }
}
