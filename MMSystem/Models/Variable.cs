using System;
using System.Collections.Generic;

namespace MMSystem.Models
{
    public partial class Variable
    {
        public int VariableId { get; set; }
        public int? VariableGroupId { get; set; }
        public string Name { get; set; }
        public int? Value { get; set; }
        public string Value2 { get; set; }
        public string Notes { get; set; }
    }
}
