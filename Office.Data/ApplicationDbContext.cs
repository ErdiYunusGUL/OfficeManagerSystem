using Microsoft.EntityFrameworkCore;
using Office.Model; // Modellerimizi kullanabilmek için ekledik

namespace Office.Data
{
    // DbContext'ten miras alarak bu sınıfın bir EF Core köprüsü olduğunu belirtiyoruz
    public class ApplicationDbContext : DbContext
    {
        // Program.cs'den gelecek olan veri tabanı bağlantı ayarlarını (options) içeri alıyoruz
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // --- TABLOLARIMIZ (DbSet'ler) ---
        // SQL'de oluşacak tabloların adlarını çoğul (s takısı ile) veriyoruz

        public DbSet<Location> Locations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<TransferOrder> TransferOrders { get; set; }

        public DbSet<LocationStock> LocationStocks { get; set; }


        // (Opsiyonel ama Profesyonel) Çift yönlü ilişkilerde çakışmaları önlemek için:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TransferOrder tablosundaki FromLocation ve ToLocation alanları aynı Location tablosuna gittiği için
            // SQL'in kafası karışabilir. Bunu önlemek için "Cascade Delete" (Otomatik Silme) işlemini kapatıyoruz.

            modelBuilder.Entity<TransferOrder>()
                .HasOne(t => t.FromLocation)
                .WithMany()
                .HasForeignKey(t => t.FromLocationId)
                .OnDelete(DeleteBehavior.Restrict); // Silinmeyi kısıtla

            modelBuilder.Entity<TransferOrder>()
                .HasOne(t => t.ToLocation)
                .WithMany()
                .HasForeignKey(t => t.ToLocationId)
                .OnDelete(DeleteBehavior.Restrict); // Silinmeyi kısıtla

            base.OnModelCreating(modelBuilder);
        }
    }
}