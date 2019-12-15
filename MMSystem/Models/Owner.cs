using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class Owner
    {
        public Owner()
        {
            Merch = new HashSet<Merch>();
            OwnerId = 1;
        }

        public int OwnerId { get; set; } = 1;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string Url { get; set; }
        public string WebsiteTitle { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Instagram { get; set; }
        public string Pintrest { get; set; }
        public string ProfilePicture { get; set; }

        public virtual ICollection<Merch> Merch { get; set; }
    }
}
