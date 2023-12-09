using System;
using System.Collections.Generic;

namespace ModelsLibrary;

public partial class ViewHeatConsumer
{
    public int ConsumerId { get; set; }

    public string? ConsumerName { get; set; }

    public string? NetworkName { get; set; }

    public int? NodeNumber { get; set; }

    public decimal? CalculatedPower { get; set; }
}
