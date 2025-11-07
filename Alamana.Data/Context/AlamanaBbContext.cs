using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alamana.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Alamana.Data.Context
{
    public class AlamanaBbContext : IdentityDbContext<ApplicationUser>
    {
        public AlamanaBbContext(DbContextOptions<AlamanaBbContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // تأكد من استدعاء الأساس لمنع الأخطاء

            //// إذا كنت تحتاج لتعريف المفاتيح المركبة يدويًا:
            //modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });
            //modelBuilder.Entity<IdentityUserRole<string>>().HasKey(x => new { x.UserId, x.RoleId });
            //modelBuilder.Entity<IdentityUserToken<string>>().HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
            modelBuilder.Entity<FavouriteProducts>()
                .HasIndex(f => new { f.ProductId, f.UserId })
                .IsUnique();

            // هيراركي العناوين (لو حابّة تسيبيها Cascade)
            modelBuilder.Entity<Governorate>()
                .HasOne(g => g.Country)
                .WithMany(c => c.Governorate)
                .HasForeignKey(g => g.CountryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<city>()
                .HasOne(c => c.Governorate)
                .WithMany(g => g.Cities)                 // عدّلي الاسم حسب الموديل عندك
                .HasForeignKey(c => c.GovernorateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<District>()
                .HasOne(d => d.City)
                .WithMany(c => c.Districts)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            // علاقات Order → بدون Cascade لمنع multiple cascade paths
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Country)
                .WithMany()
                .HasForeignKey(o => o.CountryId)
                .OnDelete(DeleteBehavior.Restrict);      // أو .NoAction()

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Governorate)
                .WithMany()
                .HasForeignKey(o => o.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.City)
                .WithMany()
                .HasForeignKey(o => o.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.District)
                .WithMany()
                .HasForeignKey(o => o.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.PaymentMethod)
                .WithMany()
                .HasForeignKey(o => o.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.user)
                .WithMany()                               // أو WithMany(u => u.Orders) لو عندك Navigation
                .HasForeignKey(o => o.userId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<ProductMedia>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Url).IsRequired().HasMaxLength(1024);
                b.HasOne(x => x.Product)
                 .WithMany(p => p.Media)
                 .HasForeignKey(x => x.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);

            });



        }

        public DbSet<Categories> Category { get; set; }
        public DbSet<Products> Product { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartItems> CartItems { get; set; }
        public DbSet<country> Country { get; set; }
        public DbSet<Governorate> Governorate { get; set; }
        public DbSet<city> City { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Advertisements> Advertisements { get; set; }
        public DbSet<ProductMedia> ProductMedia { get; set; }
        public DbSet<FavouriteProducts> FavouriteProducts { get; set; }
    }
}
