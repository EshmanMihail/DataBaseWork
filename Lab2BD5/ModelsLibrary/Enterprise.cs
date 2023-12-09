using System;
using System.Collections.Generic;

namespace ModelsLibrary;

public partial class Enterprise
{
    public int EnterpriseId { get; set; }

    public string? EnterpriseName { get; set; }

    public string? ManagementOrganization { get; set; }

    public virtual ICollection<HeatNetwork> HeatNetworks { get; set; } = new List<HeatNetwork>();
}
