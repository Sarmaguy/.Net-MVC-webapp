using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
  public class VlasnikAutocesteViewModel
  {
    public IEnumerable<VlasnikAutoceste> VlasnikAutoceste { get; set; }
    public PagingInfo PagingInfo { get; set; }
  }
}
