using RPPP_WebApp.Models;
using System.Linq.Expressions;

namespace RPPP_WebApp.Extensions.Selectors
{
  public static class AutocestaSort
  {
    public static IQueryable<Autocestum> ApplySort(this IQueryable<Autocestum> query, int sort, bool ascending)
    {
      Expression<Func<Autocestum, object>> orderSelector = sort switch
      {
        1 => d => d.AutocestaId,
        2 => d => d.AutocestaIme,
        3 => d => d.AutocestaDuljina,
        4 => d => d.OibvlasnikaNavigation.VlasnikIme,
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
