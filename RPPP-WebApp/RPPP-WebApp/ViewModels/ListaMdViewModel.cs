using RPPP_WebApp.Models;
using System.Collections.Generic;

namespace RPPP_WebApp.ViewModels
{
  public class ListaMdViewModel
  {
    public IEnumerable<MdViewModel> Md { get; set; }
    public PagingInfo PagingInfo { get; set; }
  }
}