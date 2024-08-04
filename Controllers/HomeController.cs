using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SessionWorks;
using SessionWorks.Models;
using System.Diagnostics;

namespace SessionWorks.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Login(string? redirectUrl)
        {
            ViewBag.AuthError = TempData["AuthError"] as string;
            ViewBag.RedirectUrl = redirectUrl;
            return View(new User());
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Ana Sayfa";
            if (!CheckLogin())
            {
                ViewBag.mesaj = "Bu sayfayı görme yetkiniz yok. Lütfen giriş yapın.";
                return View();
            }
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View();
        }

        public bool CheckLogin()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("username")))
            {
                return false;
            }
            return true;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new User());
        }

        [HttpPost]
        [Route("/kayit")]
        public IActionResult Kayit(User model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Eksik alan bulunuyor!";
                return View("register",model);
            }

            if (!string.Equals(model.Password, model.PasswordRepeat))
            {
                ViewBag.ErrorMessage = "Şifreler uyuşmuyor!";
                return View("register",model);
            }

            using var connection = new SqlConnection(connectionString);

            var sql = "INSERT INTO users(Name, Email, Password, RolId, CreatedDate, UpdatedDate) VALUES (@Name, @Email, @Password, @RolId, @CreatedDate, @UpdatedDate)";
            var data = new
            {
                model.Name,
                model.Email,
                Password = Helper.Hash(model.Password),
                RolId = 1, 
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
            };

            var rowsAffected = connection.Execute(sql, data);
            if (rowsAffected > 0)
            {
                return RedirectToAction("Login");
            }
            else
            {
                ViewBag.ErrorMessage = "Kayıt işlemi başarısız oldu!";
                return View(model);
            }
        }

        [HttpPost]
        [Route("/giris")]
        public IActionResult GirisYap(User model)
        {
           
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Password))
            {
                TempData["AuthError"] = "Kullanıcı adı veya şifre boş olamaz";
                return RedirectToAction("Login");
            }

            using var connection = new SqlConnection(connectionString);
            var sql = "SELECT * FROM users WHERE Name = @Name AND Password = @Password";
            var user = connection.QuerySingleOrDefault<User>(sql, new 
            { model.Name,
              Password = Helper.Hash(model.Password),
         
            });

            if (user != null)
            {
                HttpContext.Session.SetString("username", user.Name);
                if (!string.IsNullOrEmpty(model.RedirectUrl))
                {
                    return Redirect(model.RedirectUrl);
                }
                return RedirectToAction("Index");
            }

            TempData["AuthError"] = "Kullanıcı adı veya şifre yanlış";
            return RedirectToAction("Login");
        }

        public IActionResult Cikis()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = "Başka sayfa";
            if (!CheckLogin())
            {
                ViewBag.mesaj = "Bu sayfayı görme yetkiniz yok. Lütfen giriş yapın.";
                return View();
            }
            ViewData["username"] = HttpContext.Session.GetString("username");
            return View();
        }
    }
}
