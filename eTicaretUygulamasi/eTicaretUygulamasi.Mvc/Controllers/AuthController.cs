using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using eTicaretUygulamasi.Mvc.App.Data.Entities;
using eTicaretUygulamasi.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace eTicaretUygulamasi.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataRepository _repo;

        public AuthController( IConfiguration config, IDataRepository repo)
        {
            
            _config = config;
            _repo = repo;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Lütfen tüm alanları doldurun";
                return View();
            }
            if (registerViewModel.Password != registerViewModel.ConfirmPassword)
            {
                ViewBag.ErrorMessage = "Şifreler eşleşmiyor";
                return View();
            }

            var usersList = await _repo.GetWhere<UserEntity>(u => u.Email == registerViewModel.Email);
            if (usersList.Any()) 
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
            await _repo.Add(newUser);
            
            ViewBag.SuccessMessage = "Kayıt başarılı, giriş yapabilirsiniz";

            return View();
        }

        [HttpGet]
        [AllowAnonymous] // Herkes bu sayfayı görebilmeli
        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous] // Giriş işlemi herkese açık olmalı
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Lütfen tüm alanları doldurun";
                return View();
            }
            var users = await _repo.GetAll<UserEntity>();

            var user = users.FirstOrDefault(u =>
            u.Email == loginViewModel.Email &&
             u.Password == loginViewModel.Password);

            if (user is null)
            {
                ViewBag.ErrorMessage = "Kullanıcı adı veya şifre hatalı";
                return View();
            }

         
            var roles = await _repo.GetWhere<RoleEntity>(r => r.Id == user.RoleId);
            user.Role = roles.FirstOrDefault();

            var claims = new List<Claim>()
            {           
             new Claim(JwtRegisteredClaimNames.Email, user.Email),
             new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

             new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

             new Claim(ClaimTypes.Role, user.Role?.Name ?? "User"), // Artık Role dolu gelecek, ama yine de null kontrolü
             new Claim(ClaimTypes.Name, user.FirstName)
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

        [Authorize("AllRoles")]
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
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordViewModel model)
        {

            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    ViewBag.ErrorMessage = "Lütfen email adresinizi girin";
                    return View();
                }
                
                //var user = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
                var user  = await _repo.GetWhere<UserEntity>(u => u.Email == model.Email);

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

            //var updatedUser = _dbContext.Users.FirstOrDefault(u => u.Email == model.Email);
            var Users = await _repo.GetWhere<UserEntity>(u => u.Email == model.Email);
            var updatedUser = Users.FirstOrDefault();
            if (updatedUser is null)
            {
                ViewBag.ErrorMessage = "Bu email adresi ile kayıtlı kullanıcı bulunamadı";
                return View(model);
            }

            updatedUser.Password = model.NewPassword;
            await _repo.Update(updatedUser);

            ViewBag.SuccessMessage = "Şifreniz başarıyla güncellendi, giriş yapabilirsiniz";

            return View();
        }
    }
}
