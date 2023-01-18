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
  public class VrstaKamereController : Controller
  {
    private readonly RPPP09Context ctx;
    private readonly ILogger<VrstaKamere> logger;
    private readonly AppSettings appSettings;

    public VrstaKamereController(RPPP09Context ctx, ILogger<VrstaKamere> logger, IOptions<AppSettings> appSettings)
    {
      this.ctx = ctx;
      this.logger = logger;
      this.appSettings = appSettings.Value;
    }



    public IActionResult IndexSimple()
    {
      var autoceste = ctx.VrstaKameres.ToList();
      return View(autoceste);
    }

    public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
    {
      int pagesize = appSettings.PageSize;

      var query = ctx.VrstaKameres.AsNoTracking();

      int count = query.Count();
      if (count == 0)
      {
        logger.LogInformation("Ne postoji nijedna VrstaKamere.");
        TempData[Constants.Message] = "Ne postoji niti jedna VrstaKamere.";
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

      var model = new VrstaKamereViewModel
      {
        VrstaKamere = autoceste,
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
     public async Task<IActionResult> Create(VrstaKamere VrstaKamere)
    {
      if (ModelState.IsValid)
      {
        try
        {
          ctx.Add(VrstaKamere);
          await ctx.SaveChangesAsync();

          TempData[Constants.Message] = $"VrstaKamere {VrstaKamere.VrstaKamereNaziv} dodan. id Vlasnika = {VrstaKamere.VrstaKamereId}";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index));

        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          return View(VrstaKamere);
        }
      }
      else
      {
        return View(VrstaKamere);
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id, int page = 1, int sort = 1, bool ascending = true)
    {
      var VrstaKamere = ctx.VrstaKameres.Find(id);                       
      if (VrstaKamere != null)
      {
        try
        {
          string naziv = VrstaKamere.VrstaKamereNaziv;
          ctx.Remove(VrstaKamere);
          ctx.SaveChanges();
          logger.LogInformation($"VrstaKamere {naziv} uspješno obrisana");
          TempData[Constants.Message] = $"VrstaKamere {naziv} uspješno obrisana";
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
        logger.LogWarning("Ne postoji VrstaKamere s idom: {0} ", id);
        TempData[Constants.Message] = "Ne postoji VrstaKamere s idom: " + id;
        TempData[Constants.ErrorOccurred] = true;
      }
      return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
    }

[HttpGet]
    public IActionResult Edit(int id, int page = 1, int sort = 1, bool ascending = true)
    {
      var VrstaKamere = ctx.VrstaKameres.AsNoTracking().Where(d => d.VrstaKamereId==id).SingleOrDefault();
      Console.WriteLine("id je "+id);
      if (VrstaKamere == null)
      {
        logger.LogWarning("Ne postoji VrstaKamere s idom: {0} ", id);
        return NotFound("Ne postoji VrstaKamere s idom: " + id);
      }
      else
      {
        ViewBag.Page = page;
        ViewBag.Sort = sort;
        ViewBag.Ascending = ascending;
        return View(VrstaKamere);
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
        VrstaKamere VrstaKamere = await ctx.VrstaKameres
                          .Where(d => d.VrstaKamereId == id)
                          .FirstOrDefaultAsync();
        if (VrstaKamere == null)
        {
          return NotFound("Neispravna oznaka autoceste: " + id);
        }

        if (await TryUpdateModelAsync<VrstaKamere>(VrstaKamere, "",
            d => d.VrstaKamereNaziv
        ))
        {
          ViewBag.Page = page;
          ViewBag.Sort = sort;
          ViewBag.Ascending = ascending;
          try
          {
            await ctx.SaveChangesAsync();
            TempData[Constants.Message] = "VrstaKamere ažurirana.";
            TempData[Constants.ErrorOccurred] = false;
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
          }
          catch (Exception exc)
          {
            ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
            return View(VrstaKamere);
          }
        }
        else
        {
          ModelState.AddModelError(string.Empty, "Podatke o autocesti nije moguće povezati s forme");
          return View(VrstaKamere);
        }
      }
      catch (Exception exc)
      {
        TempData[Constants.Message] = exc.CompleteExceptionMessage();
        TempData[Constants.ErrorOccurred] = true;
        return RedirectToAction(nameof(Edit), id);
      }
    }
  }
  
  
}
