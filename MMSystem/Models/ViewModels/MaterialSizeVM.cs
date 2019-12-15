using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMSystem.Models.ViewModels
{
    public class MaterialSizeVM
    {
        public List<SelectListItem> MaterialIdList { get; set; }
        public List<SelectListItem>  SizeIdList { get; set; }
        public int MaterialId { get; set; }
        public string SelectedMaterialId { get; set; }
        public string SelectedSizeId { get; set; }
        public int MerchId { get; set; }
        public String SizeName { get; set; }
        public Merch Merch { get; set; }
    }
}
