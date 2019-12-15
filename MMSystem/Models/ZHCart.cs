using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class ZHCart
    {
        public int Hid { get; set; }
        public int? CartId { get; set; }
        public int? OwnerId { get; set; }
        public int? StatusId { get; set; }
        public decimal? Total { get; set; }
        public string UpdatedOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public Guid? CartGuid { get; set; }
        public string Activity { get; set; }
    }
}
