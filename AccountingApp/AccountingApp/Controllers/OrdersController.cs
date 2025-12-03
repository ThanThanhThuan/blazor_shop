using AccountingApp.Data;
using AccountingApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AccountingContext _context;

        public OrdersController(AccountingContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(List<OrderDto> cartItems)
        {
            if (cartItems == null || !cartItems.Any()) return BadRequest("Cart is empty");

            // Use a transaction to ensure we don't save half an order if one item fails
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order { OrderDate = DateTime.Now, Items = new List<OrderItem>() };
                decimal grandTotal = 0;

                foreach (var item in cartItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);

                    if (product == null)
                        return BadRequest($"Product ID {item.ProductId} not found.");

                    // 1. Check Stock
                    if (product.Stock < item.Quantity)
                    {
                        return BadRequest($"Not enough stock for '{product.Name}'. Available: {product.Stock}");
                    }

                    // 2. Decrement Stock
                    product.Stock -= item.Quantity;

                    // 3. Create Order Item
                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };

                    order.Items.Add(orderItem);
                    grandTotal += (product.Price * item.Quantity);
                }

                order.TotalAmount = grandTotal;
                _context.Orders.Add(order);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); // Commit changes only if everything succeeded

                return Ok(new { OrderId = order.Id, Total = grandTotal });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error processing order: " + ex.Message);
            }
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)             // Load OrderItems
                .ThenInclude(i => i.Product)       // Load Product details for each Item
                .OrderByDescending(o => o.OrderDate) // Newest first
                .ToListAsync();

            return orders;
        }
    }
}