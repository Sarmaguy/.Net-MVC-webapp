using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class Autocestum
    {
        public Autocestum()
        {
            Dionicas = new HashSet<Dionica>();
            Kameras = new HashSet<Kamera>();
            NaplatnaPostajas = new HashSet<NaplatnaPostaja>();
            ObilazniPravacs = new HashSet<ObilazniPravac>();
            Odmoristes = new HashSet<Odmoriste>();
        }

        public int AutocestaId { get; set; }
        public string AutocestaIme { get; set; }
        public int AutocestaDuljina { get; set; }
        public string Oibvlasnika { get; set; }

        public virtual VlasnikAutoceste OibvlasnikaNavigation { get; set; }
        public virtual ICollection<Dionica> Dionicas { get; set; }
        public virtual ICollection<Kamera> Kameras { get; set; }
        public virtual ICollection<NaplatnaPostaja> NaplatnaPostajas { get; set; }
        public virtual ICollection<ObilazniPravac> ObilazniPravacs { get; set; }
        public virtual ICollection<Odmoriste> Odmoristes { get; set; }
    }
}
