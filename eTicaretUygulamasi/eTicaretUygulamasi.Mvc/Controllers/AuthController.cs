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
                RoleId = 3,
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
            Response.Cookies.Delete("access_token");
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword([FromForm] ForgotPasswordViewModel model)
        {
           
            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    ViewBag.ErrorMessage = "Lütfen email adresinizi girin";
                    return View();
                }

                var user = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);

                if (user is null)
                {
                    ViewBag.ErrorMessage = "Bu email adresi ile kayıtlı kullanıcı bulunamadı";
                    return View();
                }

                ViewBag.SuccessMessage = "Lütfen yeni şifrenizi girin";
                ViewBag.EmailVerified = true;
                return View(model);
            }

           
            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ViewBag.ErrorMessage = "Şifreler eşleşmiyor";
                ViewBag.EmailVerified = true;
                return View(model);
            }

            if (model.NewPassword.Length < 6)
            {
                ViewBag.ErrorMessage = "Şifre en az 6 karakter olmalıdır";
                ViewBag.EmailVerified = true;
                return View(model);
            }

            var updatedUser = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);

            if (updatedUser is null)
            {
                ViewBag.ErrorMessage = "Bu email adresi ile kayıtlı kullanıcı bulunamadı";
                return View(model);
            }

            updatedUser.Password = model.NewPassword;
            _dbContext.SaveChanges();

            ViewBag.SuccessMessage = "Şifreniz başarıyla güncellendi, giriş yapabilirsiniz";

            return View();
        }
    }
}
