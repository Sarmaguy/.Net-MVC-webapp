using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class VrstaKamere
    {
        public VrstaKamere()
        {
            Kameras = new HashSet<Kamera>();
        }

        public int VrstaKamereId { get; set; }
        public string VrstaKamereNaziv { get; set; }

        public virtual ICollection<Kamera> Kameras { get; set; }
    }
}
