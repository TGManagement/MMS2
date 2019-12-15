using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Order = new HashSet<Order>();
        }

        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int? Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int ZipCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public int StatusId { get; set; }
        public int? CartId { get; set; }

        public virtual ICollection<Order> Order { get; set; }
    }
}
