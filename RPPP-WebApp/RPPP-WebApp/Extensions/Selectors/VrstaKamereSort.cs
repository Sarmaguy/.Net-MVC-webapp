using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
  public static class VrstaKamereSort
  {
    public static IQueryable<VrstaKamere> ApplySort(this IQueryable<VrstaKamere> query, int sort, bool ascending)
    {
      Expression<Func<VrstaKamere, object>> orderSelector = sort switch
      {
        1 => d => d.VrstaKamereId,
        2 => d => d.VrstaKamereNaziv,
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
