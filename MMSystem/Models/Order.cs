using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? CartId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? OwnerId { get; set; }

        public virtual Cart Cart { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
