using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class TgmProject
    {
        public int ProjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}
