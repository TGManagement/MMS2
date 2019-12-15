using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSystem.Models.ViewModels
{
    public class MerchAndAllReplicasVM
    {
        public Merch Merch { get; set; }
        public IQueryable<ReplicaIndexListVm> ListOfReplicas { get; set; }
    }
}
