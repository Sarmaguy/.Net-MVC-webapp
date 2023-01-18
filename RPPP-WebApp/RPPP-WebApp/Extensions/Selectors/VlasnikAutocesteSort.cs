using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
  public static class VlasnikAutocesteSort
  {
    public static IQueryable<VlasnikAutoceste> ApplySort(this IQueryable<VlasnikAutoceste> query, int sort, bool ascending)
    {
      Expression<Func<VlasnikAutoceste, object>> orderSelector = sort switch
      {
        1 => d => d.Oib,
        2 => d => d.VlasnikIme,
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
