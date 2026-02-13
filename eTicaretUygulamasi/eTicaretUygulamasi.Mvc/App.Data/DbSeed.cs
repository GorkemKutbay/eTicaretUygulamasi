using eTicaretUygulamasi.Mvc.App.Data.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Drawing;

namespace eTicaretUygulamasi.Mvc.App.Data
{
    public class DbSeed
    {
        public static async Task SeedAsync(AppDbContext dbContext)
        {
            var buyer = new RoleEntity()
            {
                Name = "Buyer",
                CreatedAt = DateTime.Now,
            };
            dbContext.Roles.Add(buyer);
            var seller = new RoleEntity()
            {
                Name = "Seller",
                CreatedAt = DateTime.Now,
            };
            dbContext.Roles.Add(seller);
            var Admin = new RoleEntity()
            {
                Name = "Admin",
                CreatedAt = DateTime.Now,
            };
            dbContext.Roles.Add(Admin);

            var Owner = new UserEntity()
            {

                FirstName = "Mahmut",
                LastName = "Taşkaya",
                Email = "mahmuttaskaya@outlook.com",
                Password = "mahmut123",
                Role = Admin,
                CreatedAt = DateTime.Now,
                Enabled = true

            };
            dbContext.Users.Add(Owner);


            //            Id
            //            Name
            //            Color
            //            IconCssClass   
            //            CreatedAt
            var FreshMeat = new CategoryEntity()
            {
                Name="FreshMeat",
                Color="Kırmızı",
                IconCssClass="card",
                CreatedAt= DateTime.Now,
            };
            dbContext.Categories.Add(FreshMeat);

            var Vegetable = new CategoryEntity()
            {
                Name="Vegetable",
                Color="Yeşil",
                IconCssClass="card",
                CreatedAt= DateTime.Now,
            };
            dbContext.Categories.Add(Vegetable);

            var Fish = new CategoryEntity()
            {
                Name="Fish",
                Color="Mavi",
                IconCssClass="card",
                CreatedAt= DateTime.Now,
            };
            dbContext.Categories.Add(Fish);
            var Beverage= new CategoryEntity()
            {
                Name= "Beverage",
                Color="Kırmızı",
                IconCssClass="card",
                CreatedAt= DateTime.Now,
            };
            dbContext.Categories.Add(Beverage);
            var Snack = new CategoryEntity()
            {
                Name= "Snack",
                Color="Siyah",
                IconCssClass="card",
                CreatedAt= DateTime.Now,
            };
            dbContext.Categories.Add(Snack);
            var FastFood = new CategoryEntity()
            {
                Name= "FastFood",
                Color="Kırmızı",
                IconCssClass="card",
                CreatedAt= DateTime.Now,
            };
            dbContext.Categories.Add(FastFood);
            var Alcohol = new CategoryEntity()
            {
                Name= "Alcohol",
                Color="Turuncu", //viski :))
                IconCssClass="card",
                CreatedAt= DateTime.Now,
            };
            dbContext.Categories.Add(Alcohol);
            var IceCream = new CategoryEntity()
            {
                Name= "IceCream",
                Color="Beyaz",
                IconCssClass="card",
                CreatedAt= DateTime.Now,
            };
            dbContext.Categories.Add(IceCream);
            var DriedFruit = new CategoryEntity()
            {
                Name= "DriedFruit",
                Color="Turuncu",
                IconCssClass="card",
                CreatedAt= DateTime.Now,
            };
            dbContext.Categories.Add(DriedFruit);
            var Cosmetic = new CategoryEntity()
            {
                Name= "Cosmetic",
                Color="Mavi",
                IconCssClass="card",
                CreatedAt= DateTime.Now,
            };
            dbContext.Categories.Add(Cosmetic);




            await dbContext.SaveChangesAsync();

        }
    }
}
