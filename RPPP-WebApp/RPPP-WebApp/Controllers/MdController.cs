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
  public class MdController : Controller
  {
    private readonly RPPP09Context ctx;
    private readonly ILogger<MdController> logger;
    private readonly AppSettings appSettings;

    public MdController(RPPP09Context ctx, ILogger<MdController> logger, IOptions<AppSettings> appSettings)
    {
      this.ctx = ctx;
      this.logger = logger;
      this.appSettings = appSettings.Value;
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
                  .Select(m => new MdViewModel{
                    AutocestaId = m.AutocestaId,
                    AutocestaIme = m.AutocestaIme,
                    AutocestaDuljina = m.AutocestaDuljina,
                    ImeVlasnika = m.OibvlasnikaNavigation.VlasnikIme,
                    Kameras = m.Kameras
                  }
                  
                  )
                  .Skip((page - 1) * pagesize)
                  .Take(pagesize)
                  .ToList();

      foreach(var a in autoceste){
        a.KamerasString = a.ToString();
      }

      var model = new ListaMdViewModel
      {
        Md = autoceste,
        PagingInfo = pagingInfo
      };

      return View(model);
    }

    [HttpGet]
    public   IActionResult Detail(int id, string ime,int duljina, string vlasnik, int page = 1, int sort = 1, bool ascending = true)
    {
      var query = ctx.Kameras.AsNoTracking();
      query = query.ApplySort(sort, ascending);
      var kamere = query.Where(d => d.AutocestaId == id).AsNoTracking()
      .Select(m => new KameraViewModel{
                    KameraId = m.KameraId,
                    KameraKoordinate = m.KameraKoordinate,
                    KameraSmjer = m.KameraSmjer,
                    KameraUrl = m.KameraUrl,
                    NazivVrste = m.VrstaKamere.VrstaKamereNaziv,
                    NazivAutoceste = m.Autocesta.AutocestaIme,
                    VrstaKamereId = m.VrstaKamere.VrstaKamereId

      }).
      ToList();

      var vrstakamera = ctx.VrstaKameres.AsNoTracking().ToList();

      var vlasnici = ctx.VlasnikAutocestes.AsNoTracking().ToList();
      

      foreach(var k in kamere){
        Console.WriteLine("eheh"+k.KameraId);
      }

      //var autocesta = ctx.Autocesta.AsNoTracking().Where(d => d.AutocestaId == id).SingleOrDefault();
      if (kamere == null)
      {
        logger.LogWarning("Ne postoji autocesta s oznakom: {0} ", id);
        return NotFound("Ne postoji autocesta s oznakom: " + id);
      }
      else
      {
        PagingInfo pagingInfo = new PagingInfo
        {
          CurrentPage = page,
          Sort = sort,
          Ascending = ascending,
          ItemsPerPage = appSettings.PageSize,
          TotalItems = kamere.Count()
        };

        return View( new DetailMdViewModel
        {
          Autocesta = new FancyAutocestaViewModel
          {
            AutocestaId = id,
            AutocestaIme = ime,
            AutocestaDuljina = duljina,
            ImeVlasnika = vlasnik
          },
          Kameras = kamere,
          PagingInfo = pagingInfo,
          VrstaKameras = vrstakamera,
          Vlasnici = vlasnici

        });
        };
      }
    [HttpPost]
    public IActionResult Edit(DetailMdViewModel model, int id){
      var autocesta = ctx.Autocesta.Find(id);
      var vlasnik = ctx.VlasnikAutocestes.Find(model.Autocesta.Oibvlasnika);
      if(autocesta != null){
        autocesta.AutocestaIme = model.Autocesta.AutocestaIme;
        autocesta.AutocestaDuljina = model.Autocesta.AutocestaDuljina;
        autocesta.Oibvlasnika = model.Autocesta.Oibvlasnika;
        ctx.SaveChanges();
      }
      return RedirectToAction(nameof(Detail), new { id = id, ime = model.Autocesta.AutocestaIme, duljina = model.Autocesta.AutocestaDuljina, vlasnik = vlasnik.VlasnikIme });
    }

    [HttpPost]
    public IActionResult CreateKamera(DetailMdViewModel model, int autocestaId, string ime, int duljina, string vlasnik){
        var kamera = new Kamera{
          KameraKoordinate = model.Kamera.KameraKoordinate,
          KameraSmjer = model.Kamera.KameraSmjer,
          KameraUrl = model.Kamera.KameraUrl,
          VrstaKamereId = model.Kamera.VrstaKamereId,
          AutocestaId = autocestaId
        };
        ctx.Add(kamera);
        ctx.SaveChanges();
        return RedirectToAction(nameof(Detail), new { id = autocestaId, ime = ime, duljina = duljina, vlasnik = vlasnik });
    }

    [HttpPost]
    public IActionResult DeleteKamera(int KameraId, int autocestaId, string ime, int duljina, string vlasnik){
      var kamera = ctx.Kameras.Find(KameraId);
      if(kamera != null){
        ctx.Remove(kamera);
        ctx.SaveChanges();
      }
      return RedirectToAction(nameof(Detail), new { id = autocestaId, ime = ime, duljina = duljina, vlasnik = vlasnik });

    }

    [HttpPost]
    public IActionResult EditKamera(){
      var n = +1;
      return RedirectToAction(nameof(Detail));
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
          var vlasnik = ctx.VlasnikAutocestes.Find(autocesta.Oibvlasnika);
          await ctx.SaveChangesAsync();

          TempData[Constants.Message] = $"Autocesta {autocesta.AutocestaIme} dodana. Id autoceste = {autocesta.AutocestaId}";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Detail), new { id = autocesta.AutocestaId, ime = autocesta.AutocestaIme, duljina = autocesta.AutocestaDuljina, vlasnik = vlasnik.VlasnikIme });

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
    public IActionResult Delete(int AutocestaId, string kamere, int page = 1, int sort = 1, bool ascending = true)
    {
      var autocesta = ctx.Autocesta.Find(AutocestaId);

      IEnumerable<String> kamereList = kamere.Split(',').ToList();

      foreach (var kamera in kamereList)
      {
        var kameraId = int.Parse(kamera);
        var kameraToDelete = ctx.Kameras.Find(kameraId);
        if (kameraToDelete != null)
        {
          ctx.Remove(kameraToDelete);
        }
      }


      if (autocesta != null)
      {
        ctx.Remove(autocesta);
        ctx.SaveChanges();
      }
      return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
    }



  }
}
  
  
