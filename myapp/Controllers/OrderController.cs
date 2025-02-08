using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myapp.Data;
using myapp.Models;

namespace myapp.Controllers
{
    public class OrderController : Controller
    {
        private readonly myappContext _context;

        public OrderController(myappContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Fetch the user's active order
            var order = _context.Orders
                .Where(o => o.CustomerId == userId && o.OrderStatus == "Pending")
                .FirstOrDefault();

            if (order == null)
            {
                ViewBag.Message = "You don't have any active orders.";
                return View();
            }

            var orderItems = _context.OrderItems
                .Where(oi => oi.OrderId == order.Id).Include(oi => oi.Product).ToList();

            ViewBag.Order = order;
            return View(orderItems);
        }



      
        public IActionResult ConfirmedOrders(string status = null)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            // Start building the query
            IQueryable<Order> ordersQuery = _context.Orders
                .Where(o => o.CustomerId == userId)
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

       


















        [HttpPost]
        public IActionResult ConfirmOrder()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var order = _context.Orders
                .FirstOrDefault(o => o.CustomerId == userId && o.OrderStatus == "Pending");

            if (order == null)
            {
                TempData["Message"] = "No active order found.";
                return RedirectToAction("Index");
            }

            // Update order status to 'Sent'
            order.OrderStatus = "Sent";
            order.OrderPlaced = DateTime.Now;

            _context.SaveChanges();

            TempData["Message"] = "Your order has been confirmed!";
            return RedirectToAction("Index", "Dashboard");
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
                return RedirectToAction(nameof(ConfirmedOrders));
            }

            // حذف عناصر الطلب
            _context.OrderItems.RemoveRange(order.OrderItems);

            // حذف الطلب
            _context.Orders.Remove(order);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Order deleted successfully.";
            return RedirectToAction("Index", "Dashboard");
        }


        [HttpPost]
        public IActionResult UpdateOrderItem(int id, int quantity)
        {
            var orderItem = _context.OrderItems.Include(oi=>oi.Order).FirstOrDefault(oi => oi.Id == id);

            if (orderItem == null)
            {
                TempData["Message"] = "Order item not found.";
                return RedirectToAction("Index");
            }

            // Update quantity
            if (orderItem != null && orderItem.Order != null)
            {
                if(orderItem.Quantity < quantity)
                {
                    orderItem.Order.TotalAmount= orderItem.Order.TotalAmount + (orderItem.UnitPrice * (quantity - orderItem.Quantity)) ;
                }
                else if (orderItem.Quantity > quantity)
                {
                    orderItem.Order.TotalAmount = orderItem.Order.TotalAmount - (orderItem.UnitPrice * ( orderItem.Quantity -  quantity  ));
                }

            }
            else
            {
                // التعامل مع حالة عدم العثور على الطلب
                throw new Exception("Order or OrderItem not found.");
            }

            orderItem.Quantity = quantity;
           

            _context.SaveChanges();

            TempData["Message"] = "Order item updated successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteOrderItem(int id )
        {

            var orderItem = _context.OrderItems.Include(oi=>oi.Order).FirstOrDefault(oi => oi.Id == id);

            if (orderItem == null)
            {
                TempData["Message"] = "Order item not found.";
                return RedirectToAction("Index");
            }
            if (orderItem != null && orderItem.Order != null)
            {
                orderItem.Order.TotalAmount -= orderItem.UnitPrice*orderItem.Quantity;
            }
            else
            {
                // التعامل مع حالة عدم العثور على الطلب
                throw new Exception("Order or OrderItem not found.");
            }
            _context.OrderItems.Remove(orderItem);
           
            _context.SaveChanges();

            TempData["Message"] = "Order item removed successfully!";
            return RedirectToAction("Index");
        }

    }
}
