using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
  public class AutocestaViewModel
  {
    public IEnumerable<FancyAutocestaViewModel> Autoceste { get; set; }
    public PagingInfo PagingInfo { get; set; }
  }
}
