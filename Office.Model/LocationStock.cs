using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Office.Model
{
    // Bu tablo hangi eşyadan, hangi lokasyonda, kaç adet olduğunu tutar.
    public class LocationStock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        public int LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location Location { get; set; }

        [Required]
        public int Quantity { get; set; } // O lokasyondaki anlık adet
    }
}