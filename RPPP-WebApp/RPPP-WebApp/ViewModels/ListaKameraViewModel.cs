using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
  public class ListaKameraViewModel
  {
    public IEnumerable<KameraViewModel> Kamere { get; set; }
    public PagingInfo PagingInfo { get; set; }
  }
}