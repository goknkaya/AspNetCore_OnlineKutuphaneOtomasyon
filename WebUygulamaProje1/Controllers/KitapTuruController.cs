using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebUygulamaProje1.Models;
using WebUygulamaProje1.Utility;

namespace WebUygulamaProje1.Controllers
{
    [Authorize(Roles = UserRoles.Role_Admin)] // Admin disinda kimse giremez.
    public class KitapTuruController : Controller
    {
        //private readonly UygulamaDbContext _uygulamaDbContext; // Dependency Injection araciligiyla nesne olusturma, buradaki _UygulamaDbContext' i kaldirip yerine olusturulan kitapTuruRepository kullanilir

        private readonly IKitapTuruRepository _kitapTuruRepository;

        // Design Pattern: Singleton -> new' lenmeden olusturulan nesne

        public KitapTuruController(IKitapTuruRepository context)
        {
            _kitapTuruRepository = context;
        }

        public IActionResult Index()
        {
            List<KitapTuru> objKitapTuruList = _kitapTuruRepository.GetAll().ToList(); // Controller yardimiyla KitapTuru tablosunu cekme
            return View(objKitapTuruList); // Contoller -> View
        }

        //Get
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(KitapTuru kitapTuru)
        {
            if (ModelState.IsValid)
            {
                _kitapTuruRepository.Ekle(kitapTuru);
                _kitapTuruRepository.Kaydet(); // SaveChanges yapmazsak bilgiler veritabanina eklenmez!
                TempData["basarili"] = "Kitap türü başarıyla eklendi.";
                return RedirectToAction("Index", "KitapTuru"); // Controller , Action
            }
            return View();
        }

        public IActionResult Guncelle(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            KitapTuru? kitapTuruVt = _kitapTuruRepository.Get(u=>u.Id==id); // Expression<Func<T, bool>> filter
            if (kitapTuruVt == null)
            {
                return NotFound();
            }
            return View(kitapTuruVt);
        }

        [HttpPost]
        public IActionResult Guncelle(KitapTuru kitapTuru)
        {
            if (ModelState.IsValid)
            {
                _kitapTuruRepository.Guncelle(kitapTuru);
                _kitapTuruRepository.Kaydet(); // SaveChanges yapmazsak bilgiler veritabanina eklenmez!
                TempData["basarili"] = "Kitap türü başarıyla güncellendi.";
                return RedirectToAction("Index", "kitapTuru"); // Controller , Action
            }
            return View();
        }

        // GET ACTION
        public IActionResult Sil(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            KitapTuru? kitapTuruVt = _kitapTuruRepository.Get(u => u.Id == id); // Expression<Func<T, bool>> filter
            if (kitapTuruVt == null)
            {
                return NotFound();
            }
            return View(kitapTuruVt);
        }

        [HttpPost, ActionName("Sil")]
        public IActionResult SilPOST(int? id)
        {
            KitapTuru? kitapTuru = _kitapTuruRepository.Get(u => u.Id == id); // Expression<Func<T, bool>> filter
            if (kitapTuru == null)
            {
                return NotFound();
            }

            _kitapTuruRepository.Sil(kitapTuru);
            _kitapTuruRepository.Kaydet();
            TempData["basarili"] = "Kitap türü silme işlemi başarılı."; 
            return RedirectToAction("Index", "kitapTuru");
        }
    }
}
