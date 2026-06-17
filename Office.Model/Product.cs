using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text;

namespace Office.Model
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Lütfen bir ürün adı giriniz.")]
        [StringLength(100)]
        [DisplayName("Eşya/Ürün Adı")]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        [DisplayName("Stok Kodu")]
        public string SkuCode { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Stok adedi eksi bir değer olamaz.")]
        [DisplayName("Sistemdeki Toplam Stok")]
        public int TotalStock { get; set; }

        [StringLength(50)]
        [DisplayName("Varyant (Renk/Özellik)")]
        // Örn: "Antrasit", "Krem", "Ceviz Rengi"
        public string Variant { get; set; }
    }
}
