using System;
using System.Collections.Generic;

namespace ModelsLibrary;

public partial class ViewHeatPoint
{
    public int PointId { get; set; }

    public string? PointName { get; set; }

    public string? NetworkName { get; set; }

    public int? NodeNumber { get; set; }
}
