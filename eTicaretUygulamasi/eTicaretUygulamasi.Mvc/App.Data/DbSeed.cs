using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eTicaretUygulamasi.Mvc.App.Data
{
    public class DbSeed
    {
        public static async Task SeedAsync(AppDbContext dbContext)
        {
            // Kategoriler tablosunda herhangi bir veri varsa metottan çık (Çiftlemeyi önler)
            if (await dbContext.Categories.AnyAsync()) return;

            // 1. Rolleri Ekle
            var buyer = new RoleEntity { Name = "Buyer", CreatedAt = DateTime.Now };
            var seller = new RoleEntity { Name = "Seller", CreatedAt = DateTime.Now };
            var admin = new RoleEntity { Name = "Admin", CreatedAt = DateTime.Now };
            await dbContext.Roles.AddRangeAsync(buyer, seller, admin);

            // 2. Admin Kullanıcısını Ekle
            var owner = new UserEntity
            {
                FirstName = "Mahmut",
                LastName = "Taşkaya",
                Email = "mahmuttaskaya@outlook.com",
                Password = "mahmut123",
                Role = admin,
                CreatedAt = DateTime.Now,
                Enabled = true
            };
            await dbContext.Users.AddAsync(owner);

            // 3. Kategorileri Ekle (Truncated hatası almamak için renkleri kısa tuttum)
            var categories = new List<CategoryEntity>
            {
                new CategoryEntity { Name="FreshMeat", Color="Red", IconCssClass="card", CreatedAt=DateTime.Now },
                new CategoryEntity { Name="Vegetable", Color="Green", IconCssClass="card", CreatedAt=DateTime.Now },
                new CategoryEntity { Name="Fish", Color="Blue", IconCssClass="card", CreatedAt=DateTime.Now },
                new CategoryEntity { Name="Beverage", Color="Red", IconCssClass="card", CreatedAt=DateTime.Now },
                new CategoryEntity { Name="Snack", Color="Black", IconCssClass="card", CreatedAt=DateTime.Now },
                new CategoryEntity { Name="FastFood", Color="Red", IconCssClass="card", CreatedAt=DateTime.Now },
                new CategoryEntity { Name="Alcohol", Color="Orng", IconCssClass="card", CreatedAt=DateTime.Now },
                new CategoryEntity { Name="IceCream", Color="White", IconCssClass="card", CreatedAt=DateTime.Now },
                new CategoryEntity { Name="DriedFruit", Color="Orng", IconCssClass="card", CreatedAt=DateTime.Now },
                new CategoryEntity { Name="Cosmetic", Color="Blue", IconCssClass="card", CreatedAt=DateTime.Now }
            };

            await dbContext.Categories.AddRangeAsync(categories);

            // Her şeyi tek seferde kaydet
            await dbContext.SaveChangesAsync();
        }
    }
}