using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class Material
    {
        public int MaterialId { get; set; }
        public int? MerchTypeId { get; set; }
        public string Name { get; set; }
    }
}
