using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList (Dropdown) için gerekli
using Microsoft.EntityFrameworkCore; // Include (İlişkili tablo çekmek) için gerekli
using Office.Data;
using Office.Model;
using System.Linq;

namespace OfficeUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. LİSTELEME EKRANI
        public IActionResult Index()
        {
            var products = _context.Products.ToList();

            // YENİ: LocationStock tablosundaki verileri, mekan adlarıyla (Location) birlikte çekip arayüze (ViewBag) gönderiyoruz.
            ViewBag.StockDetails = _context.LocationStocks
                                           .Include(ls => ls.Location) // Hangi depoda olduğunu anlamak için SQL'de JOIN işlemi yapar
                                           .ToList();

            return View(products);
        }

        // 2. EKLEME EKRANINI AÇMA
        [HttpGet]
        public IActionResult Create()
        {
            // YENİ: Veri tabanındaki depoları/daireleri çekip, formdaki açılır listeye (Select) yolluyoruz.
            ViewBag.Locations = new SelectList(_context.Locations, "Id", "Name");
            return View();
        }

        // 3. EKLENEN EŞYAYI VE STOĞUNU KAYDETME
        [HttpPost]
        public IActionResult Create(Product product, int initialLocationId)
        {
            if (ModelState.IsValid)
            {
                // Önce eşyayı (Product) veri tabanına kaydet (Bu sayede sistem eşyaya bir ID verir)
                _context.Products.Add(product);
                _context.SaveChanges();

                // YENİ: Eğer stok girilmişse ve bir depo seçilmişse, bu eşyaları o depoya yerleştir!
                if (product.TotalStock > 0 && initialLocationId > 0)
                {
                    var firstStock = new LocationStock
                    {
                        ProductId = product.Id,
                        LocationId = initialLocationId,
                        Quantity = product.TotalStock
                    };
                    _context.LocationStocks.Add(firstStock);
                    _context.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            // Hata varsa açılır listeyi tekrar doldur ve formu geri göster
            ViewBag.Locations = new SelectList(_context.Locations, "Id", "Name");
            return View(product);
        }

        // --- YENİ: MEVCUT ÜRÜNE STOK GİRİŞİ (MAL KABUL) ---

        // 1. Stok Girme Ekranını Açma
        [HttpGet]
        public IActionResult AddStock(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            // İŞ KURALI: Tırdan inen yeni mallar sadece Depolara gidebilir!
            // Bu yüzden daireleri filtreleyip, sadece LocationType'ı "Depo" olanları çekiyoruz.
            var depots = _context.Locations.Where(l => l.LocationType == "Depo").ToList();
            ViewBag.Depots = new SelectList(depots, "Id", "Name");

            return View(product);
        }

        // 2. Girilen Stoğu Veri Tabanına İşleme
        [HttpPost]
        public IActionResult AddStock(int productId, int locationId, int addedQuantity)
        {
            if (addedQuantity > 0 && locationId > 0)
            {
                var product = _context.Products.Find(productId);

                // 1. Şirketin o eşyaya ait Toplam Stoğunu artırıyoruz
                product.TotalStock += addedQuantity;

                // 2. Seçilen depoda bu eşyadan daha önceden var mı diye bakıyoruz
                var existingStock = _context.LocationStocks
                                            .FirstOrDefault(ls => ls.ProductId == productId && ls.LocationId == locationId);

                if (existingStock != null)
                {
                    // Eğer depoda zaten varsa, mevcut sayının üstüne ekle (Örn: 10 + 5)
                    existingStock.Quantity += addedQuantity;
                }
                else
                {
                    // O depoya bu eşya ilk kez giriyorsa, sıfırdan kayıt aç
                    var newStock = new LocationStock
                    {
                        ProductId = productId,
                        LocationId = locationId,
                        Quantity = addedQuantity
                    };
                    _context.LocationStocks.Add(newStock);
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index"); // İşlem bitince listeye dön
        }
        // --- YENİ: ALSP (MAL KABUL / TEDARİK FİŞİ) ---

        // 1. ALSP Ekranını Açma
        [HttpGet]
        public IActionResult ALSP()
        {
            // Ürünleri SKU, İsim ve Varyant(Renk) bilgisiyle birleştirip dropdown için hazırlıyoruz
            var products = _context.Products
                .Select(p => new {
                    Id = p.Id,
                    // Örn: "DOK01 - Deri Ofis Koltuğu (Antrasit)"
                    DisplayName = $"{p.SkuCode} - {p.Name} ({(p.Variant != null ? p.Variant : "Varyantsız")})"
                }).ToList();

            ViewBag.MasterProducts = new SelectList(products, "Id", "DisplayName");

            // Sadece depoları çekiyoruz (1000Depo, 1000Depo Teşhir vb.)
            var depots = _context.Locations.Where(l => l.LocationType == "Depo").ToList();
            ViewBag.Depots = new SelectList(depots, "Id", "Name");

            return View();
        }

        // 2. ALSP Formunu Onaylama ve Stokları Dağıtma
        [HttpPost]
        public IActionResult ALSP(int productId, int locationId, int quantity)
        {
            if (quantity > 0 && productId > 0 && locationId > 0)
            {
                var product = _context.Products.Find(productId);

                // Şirket envanterini artır
                product.TotalStock += quantity;

                // İlgili deponun stoğunu bul veya yeni yarat
                var existingStock = _context.LocationStocks
                                            .FirstOrDefault(ls => ls.ProductId == productId && ls.LocationId == locationId);

                if (existingStock != null)
                {
                    existingStock.Quantity += quantity;
                }
                else
                {
                    _context.LocationStocks.Add(new LocationStock
                    {
                        ProductId = productId,
                        LocationId = locationId,
                        Quantity = quantity
                    });
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return RedirectToAction("ALSP"); // Veri eksikse formu yenile
        }
    }
}