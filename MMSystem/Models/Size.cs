using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class Size
    {
        public Size()
        {
            Merch = new HashSet<Merch>();
        }

        public int SizeId { get; set; }
        public string Name { get; set; }
        public int MerchTypeId { get; set; }
        public string Value { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual ICollection<Merch> Merch { get; set; }
    }
}
