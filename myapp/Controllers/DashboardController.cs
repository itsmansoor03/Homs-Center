using Microsoft.AspNetCore.Mvc;
using myapp.Data;
using myapp.Models;

namespace myapp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly myappContext _context;

        public DashboardController(myappContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var isUser = HttpContext.Session.GetString("isUser");
            if (isUser == null)
                return NotFound();

            var categories = _context.Categorys.ToList();

            ViewBag.Message = TempData["Message"];
            return View(categories);
        }

        public IActionResult Products(int categoryId)
        {
            var isUser = HttpContext.Session.GetString("isUser");
            if (isUser == null)
                return NotFound();

            var products = _context.Products.Where(p => p.CategoryId == categoryId).ToList();
            ViewBag.Message = TempData["Message"];

            return View(products);
        }

        [HttpPost]
        public IActionResult AddToOrder(int productId)
        {
            var isUser = HttpContext.Session.GetString("isUser");
            if (isUser == null)
                return NotFound();

            // Fetch the selected product
            var selectedProduct = _context.Products.FirstOrDefault(p => p.Id == productId);
           
            if (selectedProduct == null)
            {
                TempData["Message2"] = "Product not found.";
                return RedirectToAction("Products", new { categoryId = selectedProduct?.CategoryId });
            }
           

            // Simulate user identification 

            var userId = HttpContext.Session.GetInt32("UserId");

            int currentUserId = (int)userId; // Replace with actual user ID from session or authentication context
            var activeOrder = _context.Orders.FirstOrDefault(o => o.CustomerId == currentUserId && o.OrderStatus == "Pending");

            // Create a new order if no active order exists
            
            if (activeOrder == null)
            {
            
                activeOrder = new Order
                {
                    CustomerId = currentUserId,
                    OrderPlaced = DateTime.Now,
                    OrderStatus = "Pending",
                    TotalAmount = 0 // Start with zero total amount
                };

                _context.Orders.Add(activeOrder);
                _context.SaveChanges();
            }

            // Add the product as an OrderItem
            var existingOrderItem = _context.OrderItems.FirstOrDefault(oi => oi.OrderId == activeOrder.Id && oi.ProductId == selectedProduct.Id);
            if (existingOrderItem != null)
            {
                // If the product already exists in the order, increase its quantity
                existingOrderItem.Quantity += 1;
                existingOrderItem.UnitPrice = selectedProduct.Price;
            }
            else
            {
                // Otherwise, add it as a new OrderItem
                var orderItem = new OrderItem
                {
                    ProductId = selectedProduct.Id,
                    Quantity = 1, // Default quantity
                    UnitPrice = selectedProduct.Price,
                    OrderId = activeOrder.Id
                };

                _context.OrderItems.Add(orderItem);
            }

            // Update the order's total amount
            activeOrder.TotalAmount += selectedProduct.Price;
            _context.SaveChanges();

            // Confirmation message
            TempData["Message2"] = $"Product '{selectedProduct.Name}' has been added to your order!";
            return RedirectToAction("Products", new { categoryId = selectedProduct.CategoryId });
        }
    }
}
