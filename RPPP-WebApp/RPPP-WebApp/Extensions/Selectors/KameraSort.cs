using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
  public static class KameraSort
  {
    public static IQueryable<Kamera> ApplySort(this IQueryable<Kamera> query, int sort, bool ascending)
    {
      Expression<Func<Kamera, object>> orderSelector = sort switch
      {
        1 => d => d.KameraId,
        2 => d => d.KameraKoordinate,
        3 => d => d.KameraSmjer,
        4 => d => d.KameraId,
        5 => d => d.Autocesta.AutocestaIme,
        6 => d => d.VrstaKamere.VrstaKamereNaziv,
        _ => null
      };
      
      if (orderSelector != null)
      {
        query = ascending ?
               query.OrderBy(orderSelector) :
               query.OrderByDescending(orderSelector);
      }

      return query;
    }
  }
}
