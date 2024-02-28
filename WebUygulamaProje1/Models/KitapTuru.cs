using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebUygulamaProje1.Models
{
    public class KitapTuru
    {
        [Key] // PK
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Kitap Tür Adı boş bırakılamaz!")] // not null
        [MaxLength(25)]
        [DisplayName ("Kitap Türü Adı")] //Ad' in gorunmesi istenen degeri
        public string Ad { get; set; }

    }
}
