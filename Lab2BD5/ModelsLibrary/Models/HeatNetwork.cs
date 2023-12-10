using System;
using System.Collections.Generic;

namespace ModelsLibrary.Models;

public partial class HeatNetwork : ITableSql
{
    public int NetworkId { get; set; }

    public string? NetworkName { get; set; }

    public int? NetworkNumber { get; set; }

    public int? EnterpriseId { get; set; }

    public string? NetworkType { get; set; }

    public virtual Enterprise? Enterprise { get; set; }

    public virtual ICollection<HeatConsumer> HeatConsumers { get; set; } = new List<HeatConsumer>();

    public virtual ICollection<HeatPoint> HeatPoints { get; set; } = new List<HeatPoint>();

    public virtual ICollection<HeatWell> HeatWells { get; set; } = new List<HeatWell>();

    public int ID => NetworkId;
}
