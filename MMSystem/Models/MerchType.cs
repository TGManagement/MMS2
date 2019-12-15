using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class MerchType
    {
        public int MerchTypeId { get; set; }
        public string Name { get; set; }

        public virtual Merch MerchTypeNavigation { get; set; }
    }
}
