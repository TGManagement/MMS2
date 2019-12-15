using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class Cart
    {
        public Cart()
        {
            Order = new HashSet<Order>();
        }

        public int CartId { get; set; }
        public int? OwnerId { get; set; }
        public int? StatusId { get; set; }
        public decimal? Total { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? CartGuid { get; set; }
        public string SessionId { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}
