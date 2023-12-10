using System;
using System.Collections.Generic;

namespace ModelsLibrary;

public partial class SteelPipe : ITableSql
{
    public int PipeId { get; set; }

    public decimal? OuterDiameter { get; set; }

    public decimal? Thickness { get; set; }

    public decimal? LinearInternalVolume { get; set; }

    public decimal? LinearWeight { get; set; }

    public int ID => PipeId;
}
