using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebUygulamaProje1.Models;
using WebUygulamaProje1.Utility;

namespace WebUygulamaProje1.Controllers
{
    [Authorize(Roles = UserRoles.Role_Admin)] // Admin disinda kimse giremez.
    public class KiralamaController : Controller
    {
        //private readonly UygulamaDbContext _uygulamaDbContext; // Dependency Injection araciligiyla nesne olusturma, buradaki _UygulamaDbContext' i kaldirip yerine olusturulan kitapTuruRepository kullanilir

        private readonly IKiralamaRepository _kiralamaRepository;
        private readonly IKitapRepository _kitapRepository; // FK iliskisi
        public readonly IWebHostEnvironment _webHostEnvironment; // Design Pattern: Singleton -> new' lenmeden olusturulan nesne

        public KiralamaController(IKiralamaRepository kiralamaRepository, IKitapRepository kitapRepository, IWebHostEnvironment webHostEnvironment)
        {
            _kiralamaRepository = kiralamaRepository;
            _kitapRepository = kitapRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Kiralama> objKiralamaList = _kiralamaRepository.GetAll(includeProps: "Kitap").ToList();
            return View(objKiralamaList); // Contoller -> View
        }

        //Get
        public IActionResult EkleGuncelle(int? id)
        {
            //Kitap Turunu Getirmek icin
            IEnumerable<SelectListItem> kitapList = _kitapRepository.GetAll().Select(k => new SelectListItem
            {
                Text = k.KitapAdi,
                Value = k.Id.ToString()
            });
            ViewBag.kitapList = kitapList;

            if (id == null || id == 0)
            {
                // ekleme
                return View();
            }
            else
            {
                // guncelleme
                Kiralama? kiralamaVt = _kiralamaRepository.Get(u => u.Id == id); // Expression<Func<T, bool>> filter
                if (kiralamaVt == null)
                {
                    return NotFound();
                }
                return View(kiralamaVt);
            }
        }

        [HttpPost]
        public IActionResult EkleGuncelle(Kiralama kiralama)
        {
            if (ModelState.IsValid)
            {
                if (kiralama.Id == 0)
                {
                    _kiralamaRepository.Ekle(kiralama);
                    TempData["basarili"] = "Yeni kiralama işlemi başarıyla oluşturuldu!";
                }
                else
                {
                    _kiralamaRepository.Guncelle(kiralama);
                    TempData["basarili"] = "Kiralama kayıt güncelleme başarılı!";
                }

                _kiralamaRepository.Kaydet(); // SaveChanges() yapmazsanız bilgiler veri tabanına eklenmez!			
                return RedirectToAction("Index", "Kiralama");
            }
            return View();
        }

        // GET ACTION
        public IActionResult Sil(int? id)
        {
            //Kitap Turunu Getirmek icin
            IEnumerable<SelectListItem> kitapList = _kitapRepository.GetAll().Select(k => new SelectListItem
            {
                Text = k.KitapAdi,
                Value = k.Id.ToString()
            });
            ViewBag.kitapList = kitapList;

            if (id == null || id == 0)
            {
                return NotFound();
            }
            Kiralama? kiralamaVt = _kiralamaRepository.Get(u => u.Id == id); // Expression<Func<T, bool>> filter
            if (kiralamaVt == null)
            {
                return NotFound();
            }
            return View(kiralamaVt);
        }

        [HttpPost, ActionName("Sil")]
        public IActionResult SilPOST(int? Id)
        {
            Kiralama? kiralama = _kiralamaRepository.Get(u => u.Id == Id); // Expression<Func<T, bool>> filter
            if (kiralama == null)
            {
                return NotFound();
            }

            _kiralamaRepository.Sil(kiralama);
            _kiralamaRepository.Kaydet();
            TempData["basarili"] = "Kayıt silme işlemi başarılı."; 
            return RedirectToAction("Index", "Kiralama");
        }
    }
}
