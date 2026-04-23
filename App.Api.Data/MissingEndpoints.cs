using App.Data.Entities;
using App.Data.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data;

public static class MissingEndpoints 
{
    public static void MapMissingEndpoints(this WebApplication app) 
    {
        app.MapGet("api/v1/categories/slider", async (IDataRepository repo) =>
        {
            var data = await repo.GetAll<CategoryEntity>().Select(c => new { c.Id, c.Name, c.Color, c.IconCssClass }).ToListAsync();
            return Results.Ok(data);
        });

        app.MapGet("api/v1/blogcategories/sidebar", async (IDataRepository repo) =>
        {
            var data = await repo.GetAll<BlogCategoryEntity>()
                .Select(c => new { c.Id, c.Name, ArticleCount = c.BlogRelations.Count })
                .ToListAsync();
            return Results.Ok(data);
        });

        app.MapGet("api/v1/products/featured", async (IDataRepository repo) =>
        {
            var data = await repo.GetAll<ProductEntity>()
                .Where(p => p.Enabled)
                .Select(p => new {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    DiscountPercentage = p.Discount == null ? (decimal?)null : p.Discount.DiscountRate,
                    ImageUrl = p.Images.Count != 0 ? p.Images.First().Url : null
                })
                .ToListAsync();
            return Results.Ok(data);
        });

        app.MapGet("api/v1/products", async (IDataRepository repo) =>
        {
            var data = await repo.GetAll<ProductEntity>()
                .Where(p => p.Enabled)
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    DiscountPercentage = p.Discount == null ? (byte?)null : p.Discount.DiscountRate,
                    ImageUrl = p.Images.Select(i => i.Url).FirstOrDefault()
                })
                .ToListAsync();
            return Results.Ok(data);
        });

        app.MapGet("api/v1/products/{id:int}/home", async (int id, IDataRepository repo) =>
        {
            var product = await repo.GetAll<ProductEntity>()
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Discount)
                .Include(p => p.Images)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Enabled && p.Id == id);

            if (product == null)
            {
                return Results.NotFound();
            }

            var data = new
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                DiscountRate = product.Discount == null ? (byte?)null : product.Discount.DiscountRate,
                Description = product.Description,
                StockAmount = product.StockAmount,
                SellerName = $"{product.Seller.FirstName} {product.Seller.LastName}",
                CategoryName = product.Category.Name,
                CategoryId = product.CategoryId,
                ImageUrls = product.Images.Select(i => i.Url).ToArray(),
                Reviews = product.Comments.Select(c => new
                {
                    Id = c.Id,
                    Text = c.Text,
                    StarCount = c.StarCount,
                    UserName = $"{c.User.FirstName} {c.User.LastName}"
                }).ToArray(),
                IsInCart = false
            };

            return Results.Ok(data);
        });

        app.MapGet("api/v1/blogs", async (int take, IDataRepository repo) =>
        {
            var data = await repo.GetAll<BlogEntity>()
                .OrderByDescending(b => b.CreatedAt)
                .Take(take)
                .Select(b => new {
                    Id = b.Id,
                    Title = b.Title,
                    Content = b.Content,
                    ImageUrl = b.ImageUrl,
                    CreatedAt = b.CreatedAt,
                    CommentCount = b.Comments.Count
                })
                .ToListAsync();
            return Results.Ok(data);
        });

        app.MapGet("api/v1/products/latest", async (int take, IDataRepository repo) =>
        {
            var items = await repo.GetAll<ProductEntity>()
                .Where(p => p.Enabled)
                .OrderByDescending(p => p.CreatedAt)
                .Take(take)
                .Select(p => new {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    DiscountPercentage = p.Discount == null ? (decimal?)null : p.Discount.DiscountRate,
                    ImageUrl = p.Images.Count != 0 ? p.Images.First().Url : null
                })
                .ToListAsync();
            return Results.Ok(new { Title = "Latest Products", Items = items });
        });

        app.MapGet("api/v1/products/review", async (int take, IDataRepository repo) =>
        {
            var items = await repo.GetAll<ProductEntity>()
                .Where(p => p.Enabled)
                .OrderByDescending(p => p.Comments.Any() ? p.Comments.Average(c => c.StarCount) : 0)
                .Take(take)
                .Select(p => new {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    DiscountPercentage = p.Discount == null ? (decimal?)null : p.Discount.DiscountRate,
                    ImageUrl = p.Images.Count != 0 ? p.Images.First().Url : null
                })
                .ToListAsync();
            return Results.Ok(new { Title = "Review Products", Items = items });
        });

        app.MapGet("api/v1/products/top-rated", async (int take, IDataRepository repo) =>
        {
            var items = await repo.GetAll<ProductEntity>()
                .Where(p => p.Enabled)
                .OrderByDescending(p => p.Comments.Any() ? p.Comments.Average(c => c.StarCount) : 0)
                .Take(take)
                .Select(p => new {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    DiscountPercentage = p.Discount == null ? (decimal?)null : p.Discount.DiscountRate,
                    ImageUrl = p.Images.Count != 0 ? p.Images.First().Url : null
                })
                .ToListAsync();
            return Results.Ok(new { Title = "Top Rated Products", Items = items });
        });
        
        app.MapGet("api/v1/blog", async (IDataRepository repo) =>
        {
            var data = await repo.GetAll<BlogEntity>()
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new {
                    Id = b.Id,
                    Title = b.Title,
                    SummaryContent = b.Content.Length > 100 ? b.Content.Substring(0, 100) : b.Content,
                    ImageUrl = b.ImageUrl,
                    CommentCount = b.Comments.Count,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();
            return Results.Ok(data);
        });

        app.MapGet("api/v1/blog/{id:int}", async (int id, IDataRepository repo) =>
        {
            var b = await repo.GetByIdAsync<BlogEntity>(id);
            return b == null ? Results.NotFound() : Results.Ok(b);
        });

        app.MapGet("product/{id:int}", async (int id, IDataRepository repo) => 
        {
            var p = await repo.GetByIdAsync<ProductEntity>(id);
            return p == null ? Results.NotFound() : Results.Ok(p);
        });
        
        app.MapPost("product", async ([FromBody] ProductEntity p, IDataRepository repo) => 
        {
            await repo.AddAsync(p);
            return Results.Ok(p);
        });
        
        app.MapPut("product/{id:int}", async (int id, [FromBody] ProductEntity p, IDataRepository repo) => 
        {
            await repo.UpdateAsync(p);
            return Results.Ok(p);
        });
        
        app.MapDelete("product/{id:int}", async (int id, IDataRepository repo) => 
        {
            await repo.DeleteAsync<ProductEntity>(id);
            return Results.Ok();
        });
        
        app.MapPost("products/{id:int}/comment", async (int id, [FromBody] ProductCommentEntity c, IDataRepository repo) => 
        {
            c.ProductId = id;
            await repo.AddAsync(c);
            return Results.Ok(c);
        });
    }
}
