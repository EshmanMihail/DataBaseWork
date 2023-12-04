using System;
using System.Collections.Generic;

namespace Lab2BD5
{
    public partial class Enterprise
    {
        public Enterprise()
        {
            HeatNetwork = new HashSet<HeatNetwork>();
        }

        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; }
        public string ManagementOrganization { get; set; }

        public virtual ICollection<HeatNetwork> HeatNetwork { get; set; }
    }
}
