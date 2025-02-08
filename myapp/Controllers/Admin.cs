using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myapp.Data;
using myapp.Models;

namespace myapp.Controllers
{
    public class AdminController : Controller
    {
        private readonly myappContext _context;

        public AdminController(myappContext context)
        {
            _context = context;
        }




        public IActionResult ManageOrders(string status = null)
        {
            var isAdmin = HttpContext.Session.GetString("isAdmin");
            if (isAdmin == null)
                return NotFound();

            // Start building the query
            IQueryable<Order> ordersQuery = _context.Orders
                .Where(o => o.OrderStatus != "Pending")
                .Include(o => o.Customer) // Include customer details
                .Include(o => o.OrderItems) // Include order items
                .ThenInclude(oi => oi.Product); // Include product details for order items

            // Apply filter for OrderStatus if 'status' is provided
            if (!string.IsNullOrEmpty(status))
            {
                ordersQuery = ordersQuery.Where(o => o.OrderStatus == status);
            }
            else
            {
                // Default filter for specific statuses
                ordersQuery = ordersQuery.Where(o =>
                    o.OrderStatus == "Sent" ||
                    o.OrderStatus == "Processing" ||
                    o.OrderStatus == "Delivered");
            }

            // Execute the query
            var orders = ordersQuery.ToList();

            // Pass the current filter status to the view
            ViewBag.Status = status;

            return View(orders);
        }


        /*
		// عرض الطلبات وإدارتها
		[HttpGet]
		public IActionResult ManageOrders()
		{
			var isAdmin = HttpContext.Session.GetString("isAdmin");
			if (isAdmin == null)
				return NotFound();

			// جلب الطلبات بحالة "Sent"
			var orders = _context.Orders
				.Where(o => o.OrderStatus != "Pending")
				.Include(o => o.Customer) // تضمين معلومات العميل
				.Include(o => o.OrderItems) // تضمين عناصر الطلب
				.ThenInclude(oi => oi.Product) // تضمين معلومات المنتجات
				.ToList();

			return View(orders);
		}

        last one 
        */
        // تعديل حالة الطلب
        // Update order status
        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                TempData["ErrorMessage"] = "Invalid status.";
                return RedirectToAction(nameof(ManageOrders));
            }

            var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction(nameof(ManageOrders));
            }

            order.OrderStatus = status;

            if (status == "Delivered")
            {
                order.OrderFulfilled = DateTime.Now;
            }
            else
            {
                order.OrderFulfilled = null; // Reset the fulfilled date if not "Delivered"
            }

            try
            {
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Order status updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating order: {ex.Message}";
            }

            return RedirectToAction(nameof(ManageOrders));
        }

        // حذف الطلب
        [HttpPost]
		public IActionResult DeleteOrder(int orderId)
		{
			var order = _context.Orders
				.Include(o => o.OrderItems)
				.FirstOrDefault(o => o.Id == orderId);

			if (order == null)
			{
				TempData["ErrorMessage"] = "Order not found.";
				return RedirectToAction(nameof(ManageOrders));
			}

			// حذف عناصر الطلب
			_context.OrderItems.RemoveRange(order.OrderItems);

			// حذف الطلب
			_context.Orders.Remove(order);
			_context.SaveChanges();

			TempData["SuccessMessage"] = "Order deleted successfully.";
			return RedirectToAction(nameof(ManageOrders));
		}

		// GET: Admin/Login
		[HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        //   [ValidateAntiForgeryToken]
        public IActionResult AdminLogin(string email, string password)
        {
            // Check if user exists  
            if (email=="Admin@gmail.com" && password=="mo000000")
            {
                 HttpContext.Session.SetString("isAdmin", "True");

                // Example of session or temp data usage
                TempData["Message"] = $"Welcome, {email}!";
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Invalid email or password.";
            return View();
        }


        // GET: Admin
        public IActionResult Index()
        {

            var viewModel = new AdminViewModel
            {
                Categories = _context.Categorys.ToList(),
                Customers = _context.Customers.ToList(),
                Products = _context.Products.Include(p => p.Category).ToList()
            };
            var isAdmin = HttpContext.Session.GetString("isAdmin");
            if (isAdmin == null)
                return NotFound();
            return View(viewModel);
        }

        // POST: Add Category
        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categorys.Add(category);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Add Customer
        [HttpPost]
        public IActionResult AddCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Add Product
        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Update Category
        [HttpPost]
        public IActionResult UpdateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categorys.Update(category);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Update Customer
        [HttpPost]
        public IActionResult UpdateCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Update(customer);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Update Product
        [HttpPost]
        public IActionResult UpdateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Update(product);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete Category
        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categorys.Find(id);
            if (category != null)
            {
                _context.Categorys.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete Customer
        [HttpPost]
        public IActionResult DeleteCustomer(int id)
        {
            var customer = _context.Customers.Find(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Delete Product
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }

    public class AdminViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Customer> Customers { get; set; }
        public List<Product> Products { get; set; }
    }
}
