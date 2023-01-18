using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
  public class VrstaKamereViewModel
  {
    public IEnumerable<VrstaKamere> VrstaKamere { get; set; }
    public PagingInfo PagingInfo { get; set; }
  }
}
