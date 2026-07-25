using Microsoft.AspNetCore.Mvc;

namespace MagicCardApp.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password) // Pinalitan ang 'email' ng 'username'
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                Models.GlobalData.IsLoggedIn = true;
                return RedirectToAction("Index", "Card");
            }

            return View();
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string username, string password) // Pinalitan din dito
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        public IActionResult ForgotPassword(string username)
        {
            if (!string.IsNullOrEmpty(username))
            {
                // Magpapadala tayo ng success message pabalik sa View gamit ang ViewBag
                ViewBag.Message = $"Password reset instructions have been sent to {username}.";
            }
            return View();
        }
    }
}