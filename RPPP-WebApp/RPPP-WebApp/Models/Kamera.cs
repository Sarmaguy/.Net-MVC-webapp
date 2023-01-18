using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class Kamera
    {
        public int KameraId { get; set; }
        public string KameraKoordinate { get; set; }
        public string KameraSmjer { get; set; }
        public string KameraUrl { get; set; }
        public int VrstaKamereId { get; set; }
        public int AutocestaId { get; set; }

        public virtual Autocestum Autocesta { get; set; }
        public virtual VrstaKamere VrstaKamere { get; set; }
    }
}
