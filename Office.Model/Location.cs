using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Office.Model
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Lokasyon adı zorunludur.")]
        [StringLength(100)]
        [DisplayName("Lokasyon Adı")]

        // Örn: "Sıfır Ürünler Deposu", "Teşhir Ürünleri Deposu", "Şişli Daire 1", "Kağıthane Daire 2"
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        [DisplayName("Lokasyon Tipi")]
        public string LocationType { get; set; }

        [StringLength(250)]
        [DisplayName("Açık Adres")]
        public string Address { get; set; }

    }
}
