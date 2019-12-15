using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSystem.Models.ViewModels
{
    public class MerchReplicasVM
    {
        public Merch Merch { get; set; }
        public String SizeName { get; set; }
        public Replica Replica { get; set; }
        public List<Replica> ReplicaList { get; set; }
        

    }
}
