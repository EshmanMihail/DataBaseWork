using System;
using System.Collections.Generic;

namespace Lab2BD5
{
    public partial class SteelPipe
    {
        public int PipeId { get; set; }
        public decimal? OuterDiameter { get; set; }
        public decimal? Thickness { get; set; }
        public decimal? LinearInternalVolume { get; set; }
        public decimal? LinearWeight { get; set; }
    }
}
