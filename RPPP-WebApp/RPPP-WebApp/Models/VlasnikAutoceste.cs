using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class VlasnikAutoceste
    {
        public VlasnikAutoceste()
        {
            Autocesta = new HashSet<Autocestum>();
        }

        public string Oib { get; set; }
        public string VlasnikIme { get; set; }

        public virtual ICollection<Autocestum> Autocesta { get; set; }
    }
}
