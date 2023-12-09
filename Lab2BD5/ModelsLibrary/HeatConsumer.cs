using System;
using System.Collections.Generic;

namespace ModelsLibrary;

public partial class HeatConsumer
{
    public int ConsumerId { get; set; }

    public string? ConsumerName { get; set; }

    public int? NetworkId { get; set; }

    public int? NodeNumber { get; set; }

    public decimal? CalculatedPower { get; set; }

    public virtual HeatNetwork? Network { get; set; }
}
