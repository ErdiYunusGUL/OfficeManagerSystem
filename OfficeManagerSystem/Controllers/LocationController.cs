using Microsoft.AspNetCore.Mvc;
using Office.Data;
using Office.Model;
using System.Linq;

namespace OfficeUI.Controllers
{
    public class LocationController : Controller
    {
        // Veri tabanı köprümüzü tutacağımız değişken
        private readonly ApplicationDbContext _context;

        // Dependency Injection: ASP.NET Core, Controller ayağa kalkarken veri tabanı bağlantımızı buraya otomatik enjekte eder.
        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. LİSTELEME EKRANI (Tüm Depo ve Daireleri getirir)
        public IActionResult Index()
        {
            var locations = _context.Locations.ToList();
            return View(locations);
        }

        // 2. EKLEME EKRANINI AÇMA (Sadece boş formu ekrana getirir)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. EKLENEN VERİYİ KAYDETME (Formdan gelen veriyi veri tabanına yazar)
        [HttpPost]
        public IActionResult Create(Location location)
        {
            // Eğer formdaki veriler kurallarımıza uyuyorsa (Boş bırakılmamışsa vs.)
            if (ModelState.IsValid)
            {
                _context.Locations.Add(location);
                _context.SaveChanges(); // SQL'e "INSERT" komutunu yollar

                return RedirectToAction("Index"); // Kaydettikten sonra listeye geri dön
            }

            // Kurallara uymuyorsa hatalarla birlikte formu tekrar göster
            return View(location);
        }
    }
}