using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RPPP_WebApp.Models;
using RPPP_WebApp.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using RPPP_WebApp.Extensions.Selectors;
using RPPP_WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RPPP_WebApp.Controllers
{
  public class KameraController : Controller
  {
    private readonly RPPP09Context ctx;
    private readonly ILogger<KameraController> logger;
    private readonly AppSettings appSettings;

    public KameraController(RPPP09Context ctx, ILogger<KameraController> logger, IOptions<AppSettings> appSettings)
    {
      this.ctx = ctx;
      this.logger = logger;
      this.appSettings = appSettings.Value;
    }

    private async Task PrepareDropDownLists()
    {
      var entity = await ctx.Autocesta                  
                        .Select(d => new { d.AutocestaIme, d.AutocestaId })
                        .FirstOrDefaultAsync();

      var autoceste = await ctx.Autocesta                      
                            .OrderBy(d => d.AutocestaIme)
                            .Select(d => new { d.AutocestaIme, d.AutocestaId })
                            .ToListAsync(); 
      ViewBag.Autoceste = new SelectList(autoceste, nameof(entity.AutocestaId), nameof(entity.AutocestaIme));
      var entity2 = await ctx.VrstaKameres                  
                        .Select(d => new { d.VrstaKamereNaziv, d.VrstaKamereId })
                        .FirstOrDefaultAsync();

      var vrste = await ctx.VrstaKameres                      
                            .OrderBy(d => d.VrstaKamereNaziv)
                            .Select(d => new { d.VrstaKamereNaziv, d.VrstaKamereId })
                            .ToListAsync(); 
      ViewBag.Vrste = new SelectList(vrste, nameof(entity2.VrstaKamereId), nameof(entity2.VrstaKamereNaziv));
    }

    public IActionResult IndexSimple()
    {
      var autoceste = ctx.Autocesta.ToList();
      return View(autoceste);
    }

    public async Task<IActionResult> Index(int page = 1, int sort = 1, bool ascending = true)
    {
      int pagesize = appSettings.PageSize;

      var query = ctx.Kameras.AsNoTracking();
      int count = await query.CountAsync();

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

      var mjesta = await query
                          .Select(m => new KameraViewModel
                          {
                            KameraId = m.KameraId,
                            KameraKoordinate = m.KameraKoordinate,
                            KameraSmjer = m.KameraSmjer,
                            KameraUrl = m.KameraUrl,
                            NazivAutoceste = m.Autocesta.AutocestaIme,
                            NazivVrste = m.VrstaKamere.VrstaKamereNaziv
                          })
                          .Skip((page - 1) * pagesize)
                          .Take(pagesize)
                          .ToListAsync();
      var model = new ListaKameraViewModel
      {
        Kamere = mjesta,
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
     public async Task<IActionResult> Create(Kamera kamera)
    {
      Console.WriteLine(kamera.KameraId);
      if (ModelState.IsValid)
      {
        try
        {
          ctx.Add(kamera);
          await ctx.SaveChangesAsync();

          TempData[Constants.Message] = $"Autocesta {kamera.KameraId} dodana. Id autoceste = ";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index));

        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          await PrepareDropDownLists();
          return View(kamera);
        }
      }
      else
      {
        await PrepareDropDownLists();
        return View(kamera);
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id, int page = 1, int sort = 1, bool ascending = true)
    {
      var autocesta = ctx.Kameras.Find(id);                       
      if (autocesta != null)
      {
        try
        {
          int naziv = autocesta.KameraId;
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
        logger.LogWarning("Ne postoji autocesta s idem: {0} ", id);
        TempData[Constants.Message] = "Ne postoji autocesta s idem: " + id;
        TempData[Constants.ErrorOccurred] = true;
      }
      return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
    }

[HttpGet]
    public async Task<IActionResult> EditAsync(int id, int page = 1, int sort = 1, bool ascending = true)
    {
      var autocesta = ctx.Kameras.AsNoTracking().Where(d => d.KameraId == id).SingleOrDefault();
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
        Kamera autocesta = await ctx.Kameras
                          .Where(d => d.KameraId == id)
                          .FirstOrDefaultAsync();
        if (autocesta == null)
        {
          return NotFound("Neispravna oznaka autoceste: " + id);
        }

        if (await TryUpdateModelAsync<Kamera>(autocesta, "",
            d => d.KameraKoordinate, d => d.KameraSmjer, d => d.KameraUrl, d => d.AutocestaId, d => d.VrstaKamereId))
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
