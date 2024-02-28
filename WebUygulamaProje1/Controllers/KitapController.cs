using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebUygulamaProje1.Models;
using WebUygulamaProje1.Utility;

namespace WebUygulamaProje1.Controllers
{
    public class KitapController : Controller
    {
        //private readonly UygulamaDbContext _uygulamaDbContext; // Dependency Injection araciligiyla nesne olusturma, buradaki _UygulamaDbContext' i kaldirip yerine olusturulan kitapTuruRepository kullanilir

        private readonly IKitapRepository _kitapRepository;
        private readonly IKitapTuruRepository _kitapTuruRepository; // FK iliskisinden dolayi eklendi
        public readonly IWebHostEnvironment _webHostEnvironment; // Design Pattern: Singleton -> new' lenmeden olusturulan nesne

        public KitapController(IKitapRepository kitapRepository, IKitapTuruRepository kitapTuruRepository, IWebHostEnvironment webHostEnvironment)
        {
            _kitapRepository = kitapRepository;
            _kitapTuruRepository = kitapTuruRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [Authorize(Roles = "Admin,Ogrenci")] // Admin disinda kimse giremez.
        public IActionResult Index()
        {
            List<Kitap> objKitapList = _kitapRepository.GetAll(includeProps: "KitapTuru").ToList();
            return View(objKitapList); // Contoller -> View
        }

        //Get
        [Authorize(Roles = UserRoles.Role_Admin)] // Admin disinda kimse giremez.
        public IActionResult EkleGuncelle(int? id)
        {
            //Kitap Turunu Getirmek icin
            IEnumerable<SelectListItem> kitapTuruList = _kitapTuruRepository.GetAll().Select(k => new SelectListItem
            {
                Text = k.Ad,
                Value = k.Id.ToString()
            });
            ViewBag.kitapTuruList = kitapTuruList;

            if (id == null || id == 0)
            {
                // ekleme
                return View();
            }
            else
            {
                // guncelleme
                Kitap? kitapVt = _kitapRepository.Get(u => u.Id == id); // Expression<Func<T, bool>> filter
                if (kitapVt == null)
                {
                    return NotFound();
                }
                return View(kitapVt);
            }
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Role_Admin)] // Admin disinda kimse giremez.
        public IActionResult EkleGuncelle(Kitap kitap, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRothPath = _webHostEnvironment.WebRootPath; // wwwroot' un pathini verir.
                string kitapPath = Path.Combine(wwwRothPath, @"img");

                if (file != null)
                {
                    using (var fileStream = new FileStream(Path.Combine(kitapPath, file.FileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    kitap.ResimUrl = @"\img\" + file.FileName;
                }

                if (kitap.Id == 0)
                {
                    _kitapRepository.Ekle(kitap);
                    TempData["basarili"] = "Yeni Kitap başarıyla oluşturuldu!";
                }
                else
                {
                    _kitapRepository.Guncelle(kitap);
                    TempData["basarili"] = "Kitap güncelleme başarılı!";
                }

                _kitapRepository.Kaydet(); // SaveChanges() yapmazsanız bilgiler veri tabanına eklenmez!			
                return RedirectToAction("Index", "Kitap");
            }
            return View();
        }

        // GET ACTION
        [Authorize(Roles = UserRoles.Role_Admin)] // Admin disinda kimse giremez.
        public IActionResult Sil(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Kitap? kitapVt = _kitapRepository.Get(u => u.Id == id); // Expression<Func<T, bool>> filter
            if (kitapVt == null)
            {
                return NotFound();
            }
            return View(kitapVt);
        }

        [HttpPost, ActionName("Sil")]
        [Authorize(Roles = UserRoles.Role_Admin)] // Admin disinda kimse giremez.
        public IActionResult SilPOST(int? Id)
        {
            Kitap? kitap = _kitapRepository.Get(u => u.Id == Id); // Expression<Func<T, bool>> filter
            if (kitap == null)
            {
                return NotFound();
            }

            _kitapRepository.Sil(kitap);
            _kitapRepository.Kaydet();
            TempData["basarili"] = "Kitap silme işlemi başarılı."; 
            return RedirectToAction("Index", "kitap");
        }
    }
}
