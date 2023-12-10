using System;
using System.Collections.Generic;

namespace ModelsLibrary.Models;

public partial class HeatPoint : ITableSql
{
    public int PointId { get; set; }

    public string? PointName { get; set; }

    public int? NetworkId { get; set; }

    public int? NodeNumber { get; set; }

    public virtual HeatNetwork? Network { get; set; }

    public virtual ICollection<PipelineSection> PipelineSectionEndNodeNumberNavigations { get; set; } = new List<PipelineSection>();

    public virtual ICollection<PipelineSection> PipelineSectionStartNodeNumberNavigations { get; set; } = new List<PipelineSection>();

    public int ID => PointId;
}
