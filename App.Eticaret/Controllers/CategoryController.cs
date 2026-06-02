using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace App.Eticaret.Controllers
{
    [Route("/category")]
    public class CategoryController(IHttpClientFactory clientFactory) : BaseController
    {
        private HttpClient Client => clientFactory.CreateClient("Api.Data");

        [HttpGet("filter")]
        public async Task<IActionResult> Filter([FromQuery] int categoryId)
        {
            // get category to read its name
            var catResp = await Client.GetAsync($"category/{categoryId}");

            if (!catResp.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var category = await catResp.Content.ReadFromJsonAsync<CategoryListViewModel>();

            if (category is null)
            {
                return NotFound();
            }

            // get all products and filter by category name (API product list contains CategoryName)
            var prodResp = await Client.GetAsync("products");

            if (!prodResp.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var products = await prodResp.Content.ReadFromJsonAsync<List<ProductListingViewModel>>();

            var filtered = products?.Where(p => p.CategoryName == category.Name).ToList() ?? new List<ProductListingViewModel>();

            return View("Listing", filtered);
        }
    }
}
