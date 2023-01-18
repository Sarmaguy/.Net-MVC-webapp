using System;
using System.Collections.Generic;

namespace RPPP_WebApp.Models
{
    public partial class PrateciSadrzaj
    {
        public PrateciSadrzaj()
        {
            MultimedijaPrateciSadrzajs = new HashSet<MultimedijaPrateciSadrzaj>();
        }

        public int PrateciSadrzajId { get; set; }
        public string PrateciSadrzajNaziv { get; set; }
        public string PrateciSadrzajKoordinate { get; set; }
        public string PrateciSadrzajRadnoVrijeme { get; set; }
        public int PrateciSadrzajKapacitet { get; set; }
        public int VrstaSadrzajaId { get; set; }
        public int OdmoristeId { get; set; }

        public virtual Odmoriste Odmoriste { get; set; }
        public virtual VrstaSadrzaja VrstaSadrzaja { get; set; }
        public virtual ICollection<MultimedijaPrateciSadrzaj> MultimedijaPrateciSadrzajs { get; set; }
    }
}
