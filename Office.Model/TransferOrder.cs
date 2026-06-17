using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Office.Model
{
    public class TransferOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Transfer Edilecek Eşya")]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        [DisplayName("Çıkış Lokasyonu")]
        public int FromLocationId { get; set; }

        // Çıkış yeri bir depo da olabilir, boşaltılan bir daire de olabilir.
        [ForeignKey("FromLocationId")]
        public Location FromLocation { get; set; }

        [Required]
        [DisplayName("Varış Lokasyonu")]
        public int ToLocationId { get; set; }

        // Varış yeri yeni döşenen bir daire de olabilir, eşya dönüyorsa depo da olabilir.
        [ForeignKey("ToLocationId")]
        public Location ToLocation { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Transfer adedi 1 ile 100 arasında olmalıdır.")]
        [DisplayName("Transfer Adedi")]
        public int Quantity { get; set; }

        [DisplayName("Transfer Tarihi")]
        public DateTime TransferDate { get; set; }

        [Required]
        [StringLength(30)]
        [DisplayName("Durum")]
        // Örn: "Yolda", "Daireye Yerleştirildi", "Depoya İade Edildi"
        public string Status { get; set; }
    }
}