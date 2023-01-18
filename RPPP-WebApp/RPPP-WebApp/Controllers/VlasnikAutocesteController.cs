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
  public class VlasnikAutocesteController : Controller
  {
    private readonly RPPP09Context ctx;
    private readonly ILogger<VlasnikAutoceste> logger;
    private readonly AppSettings appSettings;

    public VlasnikAutocesteController(RPPP09Context ctx, ILogger<VlasnikAutoceste> logger, IOptions<AppSettings> appSettings)
    {
      this.ctx = ctx;
      this.logger = logger;
      this.appSettings = appSettings.Value;
    }



    public IActionResult IndexSimple()
    {
      var autoceste = ctx.VlasnikAutocestes.ToList();
      return View(autoceste);
    }

    public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
    {
      int pagesize = appSettings.PageSize;

      var query = ctx.VlasnikAutocestes.AsNoTracking();

      int count = query.Count();
      if (count == 0)
      {
        logger.LogInformation("Ne postoji nijedna VlasnikAutoceste.");
        TempData[Constants.Message] = "Ne postoji niti jedna VlasnikAutoceste.";
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
                  .Skip((page - 1) * pagesize)
                  .Take(pagesize)
                  .ToList();

      var model = new VlasnikAutocesteViewModel
      {
        VlasnikAutoceste = autoceste,
        PagingInfo = pagingInfo
      };

      return View(model);
    }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
    [ValidateAntiForgeryToken]
     public async Task<IActionResult> Create(VlasnikAutoceste VlasnikAutoceste)
    {
      if (ModelState.IsValid)
      {
        try
        {
          ctx.Add(VlasnikAutoceste);
          await ctx.SaveChangesAsync();

          TempData[Constants.Message] = $"VlasnikAutoceste {VlasnikAutoceste.VlasnikIme} dodan. Oib Vlasnika = {VlasnikAutoceste.Oib}";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index));

        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          return View(VlasnikAutoceste);
        }
      }
      else
      {
        return View(VlasnikAutoceste);
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(string oib, int page = 1, int sort = 1, bool ascending = true)
    {
      var VlasnikAutoceste = ctx.VlasnikAutocestes.Find(oib);                       
      if (VlasnikAutoceste != null)
      {
        try
        {
          string naziv = VlasnikAutoceste.VlasnikIme;
          ctx.Remove(VlasnikAutoceste);
          ctx.SaveChanges();
          logger.LogInformation($"VlasnikAutoceste {naziv} uspješno obrisana");
          TempData[Constants.Message] = $"VlasnikAutoceste {naziv} uspješno obrisana";
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
        logger.LogWarning("Ne postoji VlasnikAutoceste s oibom: {0} ", oib);
        TempData[Constants.Message] = "Ne postoji VlasnikAutoceste s oibom: " + oib;
        TempData[Constants.ErrorOccurred] = true;
      }
      return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
    }

[HttpGet]
    public IActionResult Edit(string oib, int page = 1, int sort = 1, bool ascending = true)
    {
      var VlasnikAutoceste = ctx.VlasnikAutocestes.AsNoTracking().Where(d => d.Oib.Equals(oib)).SingleOrDefault();
      Console.WriteLine("oib je "+oib);
      if (VlasnikAutoceste == null)
      {
        logger.LogWarning("Ne postoji VlasnikAutoceste s oibom: {0} ", oib);
        return NotFound("Ne postoji VlasnikAutoceste s oibom: " + oib);
      }
      else
      {
        ViewBag.Page = page;
        ViewBag.Sort = sort;
        ViewBag.Ascending = ascending;
        return View(VlasnikAutoceste);
      }
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string oib, int page = 1, int sort = 1, bool ascending = true)
    {
      //za različite mogućnosti ažuriranja pogledati
      //attach, update, samo id, ...
      //https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud#update-the-edit-page

      try
      {
        VlasnikAutoceste VlasnikAutoceste = await ctx.VlasnikAutocestes
                          .Where(d => d.Oib == oib)
                          .FirstOrDefaultAsync();
        if (VlasnikAutoceste == null)
        {
          return NotFound("Neispravna oznaka autoceste: " + oib);
        }

        if (await TryUpdateModelAsync<VlasnikAutoceste>(VlasnikAutoceste, "",
            d => d.VlasnikIme
        ))
        {
          ViewBag.Page = page;
          ViewBag.Sort = sort;
          ViewBag.Ascending = ascending;
          try
          {
            await ctx.SaveChangesAsync();
            TempData[Constants.Message] = "VlasnikAutoceste ažurirana.";
            TempData[Constants.ErrorOccurred] = false;
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
          }
          catch (Exception exc)
          {
            ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
            return View(VlasnikAutoceste);
          }
        }
        else
        {
          ModelState.AddModelError(string.Empty, "Podatke o autocesti nije moguće povezati s forme");
          return View(VlasnikAutoceste);
        }
      }
      catch (Exception exc)
      {
        TempData[Constants.Message] = exc.CompleteExceptionMessage();
        TempData[Constants.ErrorOccurred] = true;
        return RedirectToAction(nameof(Edit), oib);
      }
    }
  }
  
  
}
