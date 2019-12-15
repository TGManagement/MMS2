using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class Merch
    {
        public Merch()
        {
            Replica = new HashSet<Replica>();
        }

        public int MerchId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MerchTypeId { get; set; }
        public int? MaterialId { get; set; }
        public int SizeId { get; set; }
        public int? Price { get; set; }
        public int StatusId { get; set; }
        public string PictureUrl { get; set; }
        public int OwnerId { get; set; }
        public DateTime? PurchasedOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual Owner Owner { get; set; }
        public virtual Size Size { get; set; }
        public virtual MerchType MerchType { get; set; }
        public virtual ICollection<Replica> Replica { get; set; }
    }
}
