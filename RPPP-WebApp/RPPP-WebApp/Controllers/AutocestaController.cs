using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RPPP_WebApp.ViewModels;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RPPP_WebApp.Controllers
{
  public class AutocestaController : Controller
  {
    private readonly RPPP09Context ctx;
    private readonly ILogger<AutocestaController> logger;
    private readonly AppSettings appSettings;

    public AutocestaController(RPPP09Context ctx, ILogger<AutocestaController> logger, IOptions<AppSettings> appSettings)
    {
      this.ctx = ctx;
      this.logger = logger;
      this.appSettings = appSettings.Value;
    }

    private async Task PrepareDropDownLists()
    {
      var entity = await ctx.VlasnikAutocestes                  
                        .Select(d => new { d.VlasnikIme, d.Oib })
                        .FirstOrDefaultAsync();

      var vlasnici = await ctx.VlasnikAutocestes                      
                            .OrderBy(d => d.VlasnikIme)
                            .Select(d => new { d.VlasnikIme, d.Oib })
                            .ToListAsync(); 
      ViewBag.Vlasnici = new SelectList(vlasnici, nameof(entity.Oib), nameof(entity.VlasnikIme));
    }

    public IActionResult IndexSimple()
    {
      var autoceste = ctx.Autocesta.ToList();
      return View(autoceste);
    }

    public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
    {
      int pagesize = appSettings.PageSize;

      var query = ctx.Autocesta.AsNoTracking();

      int count = query.Count();
      if (count == 0)
      {
        logger.LogInformation("Ne postoji nijedna autocesta.");
        TempData[Constants.Message] = "Ne postoji niti jedna autocesta.";
        TempData[Constants.ErrorOccurred] = false;
        return RedirectToAction(nameof(Create));
      }

      var pagingInfo = new PagingInfo
      {
        CurrentPage = page,
        Sort = sort,
        Ascending = ascending,
        ItemsPerPage = pagesize,
        TotalItems = count
      };
      if (page < 1 || page > pagingInfo.TotalPages)
      {
        return RedirectToAction(nameof(Index), new { page = 1, sort, ascending });
      }

      query = query.ApplySort(sort, ascending);
      


      var autoceste = query
                  .Select(m => new FancyAutocestaViewModel{
                    AutocestaId = m.AutocestaId,
                    AutocestaIme = m.AutocestaIme,
                    AutocestaDuljina = m.AutocestaDuljina,
                    ImeVlasnika = m.OibvlasnikaNavigation.VlasnikIme
                  }
                  
                  )
                  .Skip((page - 1) * pagesize)
                  .Take(pagesize)
                  .ToList();

      var model = new AutocestaViewModel
      {
        Autoceste = autoceste,
        PagingInfo = pagingInfo
      };

      return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      await PrepareDropDownLists();
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
     public async Task<IActionResult> Create(Autocestum autocesta)
    {
      if (ModelState.IsValid)
      {
        try
        {
          ctx.Add(autocesta);
          await ctx.SaveChangesAsync();

          TempData[Constants.Message] = $"Autocesta {autocesta.AutocestaIme} dodana. Id autoceste = {autocesta.AutocestaId}";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index));

        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          await PrepareDropDownLists();
          return View(autocesta);
        }
      }
      else
      {
        await PrepareDropDownLists();
        return View(autocesta);
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int AutocestaId, int page = 1, int sort = 1, bool ascending = true)
    {
      var autocesta = ctx.Autocesta.Find(AutocestaId);                       
      if (autocesta != null)
      {
        try
        {
          string naziv = autocesta.AutocestaIme;
          ctx.Remove(autocesta);
          ctx.SaveChanges();
          logger.LogInformation($"Autocesta {naziv} uspješno obrisana");
          TempData[Constants.Message] = $"Autocesta {naziv} uspješno obrisana";
          TempData[Constants.ErrorOccurred] = false;
        }
        catch (Exception exc)
        {
          TempData[Constants.Message] = "Pogreška prilikom brisanja autoceste: " + exc.CompleteExceptionMessage();
          TempData[Constants.ErrorOccurred] = true;
          logger.LogError("Pogreška prilikom brisanja autoceste: " + exc.CompleteExceptionMessage());
        }
      }
      else
      {
        logger.LogWarning("Ne postoji autocesta s idem: {0} ", AutocestaId);
        TempData[Constants.Message] = "Ne postoji autocesta s idem: " + AutocestaId;
        TempData[Constants.ErrorOccurred] = true;
      }
      return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
    }

[HttpGet]
    public async Task<IActionResult> EditAsync(int id, int page = 1, int sort = 1, bool ascending = true)
    {
      var autocesta = ctx.Autocesta.AsNoTracking().Where(d => d.AutocestaId == id).SingleOrDefault();
      if (autocesta == null)
      {
        logger.LogWarning("Ne postoji autocesta s oznakom: {0} ", id);
        return NotFound("Ne postoji autocesta s oznakom: " + id);
      }
      else
      {
        ViewBag.Page = page;
        ViewBag.Sort = sort;
        ViewBag.Ascending = ascending;
        await PrepareDropDownLists();
        return View(autocesta);
      }
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, int page = 1, int sort = 1, bool ascending = true)
    {
      //za različite mogućnosti ažuriranja pogledati
      //attach, update, samo id, ...
      //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud#update-the-edit-page

      try
      {
        Autocestum autocesta = await ctx.Autocesta
                          .Where(d => d.AutocestaId == id)
                          .FirstOrDefaultAsync();
        if (autocesta == null)
        {
          return NotFound("Neispravna oznaka autoceste: " + id);
        }

        if (await TryUpdateModelAsync<Autocestum>(autocesta, "",
            d => d.AutocestaIme, d => d.AutocestaDuljina, d => d.Oibvlasnika
        ))
        {
          ViewBag.Page = page;
          ViewBag.Sort = sort;
          ViewBag.Ascending = ascending;
          try
          {
            await ctx.SaveChangesAsync();
            TempData[Constants.Message] = "Autocesta ažurirana.";
            TempData[Constants.ErrorOccurred] = false;
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
          }
          catch (Exception exc)
          {
            ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
            return View(autocesta);
          }
        }
        else
        {
          ModelState.AddModelError(string.Empty, "Podatke o autocesti nije moguće povezati s forme");
          return View(autocesta);
        }
      }
      catch (Exception exc)
      {
        TempData[Constants.Message] = exc.CompleteExceptionMessage();
        TempData[Constants.ErrorOccurred] = true;
        return RedirectToAction(nameof(EditAsync), id);
      }
    }
  }
  
  
}
