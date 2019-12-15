using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSystem.Models.ViewModels
{
    public class ReplicaIndexListVm
    {
        public Merch Merch {get;set;}
        public Replica Replica { get; set; }
        public string SizeName { get; set; }
        public string MaterialName { get; set; }
        public string PictureUrl { get; set; }


    }
}
