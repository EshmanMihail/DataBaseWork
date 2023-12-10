using System;
using System.Collections.Generic;

namespace ModelsLibrary.Views;

public partial class ViewPipelineSection
{
    public int SectionId { get; set; }

    public int? SectionNumber { get; set; }

    public string? StartNode { get; set; }

    public string? EndNode { get; set; }

    public decimal? PipelineLength { get; set; }

    public decimal? Diameter { get; set; }

    public decimal? Thickness { get; set; }

    public DateOnly? LastRepairDate { get; set; }
}
