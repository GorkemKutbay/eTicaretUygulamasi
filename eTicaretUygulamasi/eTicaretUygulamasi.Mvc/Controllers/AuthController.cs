using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromForm] RegisterViewModel registerViewModel)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Lütfen tüm alanları doldurun";
                return View();
            }
            if(registerViewModel.Password != registerViewModel.ConfirmPassword)
            {
                ViewBag.ErrorMessage = "Şifreler eşleşmiyor";
                return View();
            }
            var dbSet = _dbContext.Set<UserEntity>();
            var existingUser = dbSet.FirstOrDefault(u => u.Email == registerViewModel.Email);
            if(existingUser is not null)
            {
                ViewBag.ErrorMessage = "Bu email zaten kayıtlı";
                return View();
            }
            var newUser = new UserEntity
            {
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName,
                Email = registerViewModel.Email,
                Password = registerViewModel.Password,
                RoleId = registerViewModel.RoleId,
                Enabled = true,
                CreatedAt = DateTime.Now
            };
            dbSet.Add(newUser);
            await _dbContext.SaveChangesAsync();
            ViewBag.SuccessMessage = "Kayıt başarılı, giriş yapabilirsiniz";

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Login([FromForm] LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Lütfen tüm alanları doldurun";
                return View();
            }
            var dbSet = _dbContext.Set<UserEntity>();
            var user = dbSet.FirstOrDefault(u => u.Email == loginViewModel.Email && u.Password == loginViewModel.Password);
            if (user is null)
            {
                ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı";
                return View();
            }
            var claims = new List<Claim>()
            {
                new Claim (JwtRegisteredClaimNames.Email,user.Email),
                new Claim (JwtRegisteredClaimNames.Sub,user.Id.ToString())

            };
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Secret"]));
            var tokenOptions = new JwtSecurityToken(
                issuer: "eTicaretUygulamasi",
                audience: "eTicaretUygulamasi",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256)
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            Response.Cookies.Append("access_token", tokenString, new CookieOptions
            {
                HttpOnly = true, // js ile erişilemesi
                Secure = true, // https ile kullanılabilsin
                SameSite = SameSiteMode.Strict // sadece bu sitede(uygulamada) kullanılabilsin. 
            });

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            return View();
        }

        public IActionResult ForgotPasword()
        {
            return View();
        }
    }
}
