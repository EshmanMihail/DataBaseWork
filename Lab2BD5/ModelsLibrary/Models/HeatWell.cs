using System;
using System.Collections.Generic;

namespace ModelsLibrary.Models;

public partial class HeatWell : ITableSql
{
    public int WellId { get; set; }

    public string? WellName { get; set; }

    public int? NetworkId { get; set; }

    public int? NodeNumber { get; set; }

    public virtual HeatNetwork? Network { get; set; }

    public int ID => WellId;
}
