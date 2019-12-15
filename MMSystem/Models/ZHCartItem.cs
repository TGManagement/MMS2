using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class ZHCartItem
    {
        public int Hid { get; set; }
        public int? CartItemId { get; set; }
        public int? CartId { get; set; }
        public int? MerchId { get; set; }
        public int? ReplicaId { get; set; }
        public int? IsReplica { get; set; }
        public string Activity { get; set; }
    }
}
