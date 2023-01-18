using RPPP_WebApp.Models;

namespace RPPP_WebApp.ViewModels
{
  public class MdViewModel
  {
    public int AutocestaId { get; set; }
    public string AutocestaIme { get; set; }
    public int AutocestaDuljina { get; set; }
    public string ImeVlasnika { get; set; }
    public IEnumerable<Kamera> Kameras { get; set; }
    public string KamerasString { get; set; }
    
    
    public override string ToString()
    {
      string s="";
      foreach(var k in Kameras){
        s+=k.KameraId+", ";
      }
      if (s!="") s=s.Substring(0,s.Length-2);
      return s;
    }
  }
}
