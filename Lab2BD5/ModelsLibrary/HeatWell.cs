using System;
using System.Collections.Generic;

namespace ModelsLibrary;

public partial class HeatWell
{
    public int WellId { get; set; }

    public string? WellName { get; set; }

    public int? NetworkId { get; set; }

    public int? NodeNumber { get; set; }

    public virtual HeatNetwork? Network { get; set; }
}
