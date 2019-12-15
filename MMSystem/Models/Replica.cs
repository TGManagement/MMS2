using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class Replica
    {
        public int ReplicaId { get; set; }
        public int MerchId { get; set; }
        public int SizeId { get; set; }
        public int MaterialId { get; set; }
        public int Price { get; set; }

        public virtual Merch Merch { get; set; }
    }
}
