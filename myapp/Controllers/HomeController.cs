using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using myapp.Models;
using myapp.Data;

namespace myapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly myappContext _context;
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, myappContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Customer/Register
       
        public IActionResult Register()
        {
            
            return View();
        }

        // POST: Customer/Register
        [HttpPost]
        public IActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // التحقق من وجود البريد الإلكتروني أو رقم الهاتف في قاعدة البيانات
                if (_context.Customers.Any(c => c.Email == customer.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                }

                if (_context.Customers.Any(c => c.Phone == customer.Phone))
                {
                    ModelState.AddModelError("Phone", "Phone is already in use.");
                }

                // إذا لم تكن هناك أخطاء، قم بالتسجيل
                if (ModelState.ErrorCount == 0)
                {
                   
                    _context.Customers.Add(customer);
                    _context.SaveChanges();

                    // ضبط قيم الـ Session بعد التسجيل الناجح
                    HttpContext.Session.SetString("isUser", "True");
                    HttpContext.Session.SetInt32("UserId", customer.Id); // تخزين رقم المستخدم

                    TempData["Message"] = $"Welcome, {customer.FirstName}!";
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            // إعادة عرض الصفحة مع الأخطاء
            return View(customer);
        }

        // GET: Customer/Login
        [HttpGet]
      
        public IActionResult Login()
        {
            return View();
        }

        // POST: Customer/Login
        [HttpPost]
        public IActionResult Login(string email , string password)
        {
            // التحقق من وجود المستخدم في قاعدة البيانات
            var user = _context.Customers.FirstOrDefault(c => c.Email == email && c.Password == password);
           
            if (user != null )
            {
                // ضبط قيم الـ Session
                HttpContext.Session.SetString("isUser", "True");
                HttpContext.Session.SetInt32("UserId", user.Id); // حفظ معرف المستخدم في الجلسة

                // استخدام Session أو TempData لإظهار رسالة نجاح
                TempData["Message"] = $"Welcome, {user.FirstName}!";

                // إعادة التوجيه إلى Dashboard
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Invalid email/ or password.";
            return View();
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
