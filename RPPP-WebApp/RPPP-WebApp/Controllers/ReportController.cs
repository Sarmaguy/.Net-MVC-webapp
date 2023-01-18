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
using PdfRpt.ColumnsItemsTemplates;
using PdfRpt.Core.Contracts;
using PdfRpt.Core.Helper;
using PdfRpt.FluentInterface;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace RPPP_WebApp.Controllers
{
  public class ReportController : Controller
  {
    private readonly RPPP09Context ctx;
    private readonly IWebHostEnvironment environment;
    private const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public ReportController(RPPP09Context ctx, IWebHostEnvironment environment)
    {
      this.ctx = ctx;
      this.environment = environment;
    }

    public IActionResult Index()
    {
      return View();
    }
public async Task<IActionResult> AutocesteMDExcel()
    {
        var query = ctx.Autocesta.AsNoTracking();

      var autoceste = query
                  .Select(m => new MdViewModel{
                    AutocestaId = m.AutocestaId,
                    AutocestaIme = m.AutocestaIme,
                    AutocestaDuljina = m.AutocestaDuljina,
                    ImeVlasnika = m.OibvlasnikaNavigation.VlasnikIme,
                    Kameras = m.Kameras
                  }
                  
                  )
                  .ToList();

      foreach(var a in autoceste){
        a.KamerasString = a.ToString();
      }

      byte[] content;
      using (ExcelPackage excel = new ExcelPackage())
      {
        excel.Workbook.Properties.Title = "Popis autocesta i njihoivh vlasnika";
        excel.Workbook.Properties.Author = "Jura";
        var worksheet = excel.Workbook.Worksheets.Add("Autoceste");

        //First add the headers
        worksheet.Cells[1, 1].Value = "Id autoceste";
        worksheet.Cells[1, 2].Value = "Ime autoceste";
        worksheet.Cells[1, 3].Value = "Duljin autoceste";
        worksheet.Cells[1, 4].Value = "Vlasnik autoceste";
        worksheet.Cells[1, 5].Value = "Idevi kamera postavljenih na autocesti";

        for (int i = 0; i < autoceste.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = autoceste[i].AutocestaId;
          worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          worksheet.Cells[i + 2, 2].Value = autoceste[i].AutocestaIme;
          worksheet.Cells[i + 2, 3].Value = autoceste[i].AutocestaDuljina;
          worksheet.Cells[i + 2, 4].Value = autoceste[i].ImeVlasnika;
          worksheet.Cells[i + 2, 5].Value = autoceste[i].KamerasString;
        }

        worksheet.Cells[1, 1, autoceste.Count + 1, 4].AutoFitColumns();

        content = excel.GetAsByteArray();
      }
      return File(content, ExcelContentType, "Master-Detail autoceste.xlsx");
    }
    public async Task<IActionResult> AutocesteExcel()
    {

        var autoceste = await ctx.Autocesta.AsNoTracking().ToListAsync();
        var vlasnici = await ctx.VlasnikAutocestes.AsNoTracking().ToListAsync();
        List<FancyAutocestaViewModel> autocestaVlasnik = new List<FancyAutocestaViewModel>();

        foreach (var autocesta in autoceste)
        {
          var vlasnik = vlasnici.Where(v => v.Oib == autocesta.Oibvlasnika).FirstOrDefault();
          autocestaVlasnik.Add(new FancyAutocestaViewModel
          {
            AutocestaId = autocesta.AutocestaId,
            AutocestaIme = autocesta.AutocestaIme,
            AutocestaDuljina = autocesta.AutocestaDuljina,
            ImeVlasnika = vlasnik.VlasnikIme
          });
        }

      byte[] content;
      using (ExcelPackage excel = new ExcelPackage())
      {
        excel.Workbook.Properties.Title = "Popis autocesta i njihoivh vlasnika";
        excel.Workbook.Properties.Author = "Jura";
        var worksheet = excel.Workbook.Worksheets.Add("Autoceste");

        //First add the headers
        worksheet.Cells[1, 1].Value = "Id autoceste";
        worksheet.Cells[1, 2].Value = "Ime autoceste";
        worksheet.Cells[1, 3].Value = "Duljin autoceste";
        worksheet.Cells[1, 4].Value = "Vlasnik autoceste";

        for (int i = 0; i < autocestaVlasnik.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = autocestaVlasnik[i].AutocestaId;
          worksheet.Cells[i + 2, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          worksheet.Cells[i + 2, 2].Value = autocestaVlasnik[i].AutocestaIme;
          worksheet.Cells[i + 2, 3].Value = autocestaVlasnik[i].AutocestaDuljina;
          worksheet.Cells[i + 2, 4].Value = autocestaVlasnik[i].ImeVlasnika;
        }

        worksheet.Cells[1, 1, autocestaVlasnik.Count + 1, 4].AutoFitColumns();

        content = excel.GetAsByteArray();
      }
      return File(content, ExcelContentType, "Autoceste i vlasnici.xlsx");
    }

     public async Task<IActionResult> Autoceste()
    {
      string naslov = "Popis autocesta i amera";

      var query = ctx.Autocesta.AsNoTracking();

      var autoceste = query
                  .Select(m => new MdViewModel{
                    AutocestaId = m.AutocestaId,
                    AutocestaIme = m.AutocestaIme,
                    AutocestaDuljina = m.AutocestaDuljina,
                    ImeVlasnika = m.OibvlasnikaNavigation.VlasnikIme,
                    Kameras = m.Kameras
                  }
                  
                  )
                  .ToList();

      foreach(var a in autoceste){
        a.KamerasString = a.ToString();
      }

      PdfReport report = CreateReport(naslov);
      #region Podnožje i zaglavlje
            report.PagesFooter(footer =>
      {
        footer.DefaultFooter(DateTime.Now.ToString("dd.MM.yyyy."));
      })
      .PagesHeader(header =>
      {
        header.CacheHeader(cache: true); // It's a default setting to improve the performance.
              header.DefaultHeader(defaultHeader =>
        {
          defaultHeader.RunDirection(PdfRunDirection.LeftToRight);
          defaultHeader.Message(naslov);
        });
      });

      #endregion
      #region Postavljanje izvora podataka i stupaca
      report.MainTableDataSource(dataSource => dataSource.StronglyTypedList(autoceste));
      
      report.MainTableColumns(columns =>
      {
        columns.AddColumn(column =>
        {
          column.IsRowNumber(true);
          column.CellsHorizontalAlignment(HorizontalAlignment.Right);
          column.IsVisible(true);
          column.Order(0);
          column.Width(1);
          column.HeaderCell("#", horizontalAlignment: HorizontalAlignment.Right);
        });

        columns.AddColumn(column =>
        {
          column.PropertyName(nameof(MdViewModel.AutocestaId));
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(1);
          column.Width(2);
          column.HeaderCell("Id autoceste");
        });

        columns.AddColumn(column =>
        {
          column.PropertyName<MdViewModel>(x => x.AutocestaIme);
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(2);
          column.Width(3);
          column.HeaderCell("Naziv autoceste", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
          column.PropertyName<MdViewModel>(x => x.AutocestaDuljina);
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(3);
          column.Width(1);
          column.HeaderCell("Duljina autoceste", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
          column.PropertyName<MdViewModel>(x => x.ImeVlasnika);
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(4);
          column.Width(1);
          column.HeaderCell("Vlasnik autoceste", horizontalAlignment: HorizontalAlignment.Center);
        });

        columns.AddColumn(column =>
        {
          column.PropertyName<MdViewModel>(x => x.KamerasString);
          column.CellsHorizontalAlignment(HorizontalAlignment.Center);
          column.IsVisible(true);
          column.Order(4);
          column.Width(1);
          column.HeaderCell("Idevi kamera postavljeni na autocesti", horizontalAlignment: HorizontalAlignment.Center);
        });
      });

      #endregion
      byte[] pdf = report.GenerateAsByteArray();

      if (pdf != null)
      {
        Response.Headers.Add("content-disposition", "inline; filename=Kamere na autocestama.pdf");
        return File(pdf, "application/pdf");
        //return File(pdf, "application/pdf", "drzave.pdf"); //Otvara save as dialog
      }
      else
      {
        return NotFound();
      }
    }

    private PdfReport CreateReport(string naslov)
    {
      var pdf = new PdfReport();

      pdf.DocumentPreferences(doc =>
      {
        doc.Orientation(PageOrientation.Portrait);
        doc.PageSize(PdfPageSize.A4);
        doc.DocumentMetadata(new DocumentMetadata
        {
          Author = "Jura",
          Application = "Firma.MVC Core",
          Title = naslov
        });
        doc.Compression(new CompressionSettings
        {
          EnableCompression = true,
          EnableFullCompression = true
        });
      })

      //
      .MainTableTemplate(template =>
      {
        template.BasicTemplate(BasicTemplate.ProfessionalTemplate);
      })
      .MainTablePreferences(table =>
      {
        table.ColumnsWidthsType(TableColumnWidthType.Relative);
              //table.NumberOfDataRowsPerPage(20);
              table.GroupsPreferences(new GroupsPreferences
        {
          GroupType = GroupType.HideGroupingColumns,
          RepeatHeaderRowPerGroup = true,
          ShowOneGroupPerPage = true,
          SpacingBeforeAllGroupsSummary = 5f,
          NewGroupAvailableSpacingThreshold = 150,
          SpacingAfterAllGroupsSummary = 5f
        });
        table.SpacingAfter(4f);
      });

      return pdf;
    }

  }
}