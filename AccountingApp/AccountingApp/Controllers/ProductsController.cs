using AccountingApp.Data;
using AccountingApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountingApp.Controllers
{
    // This attribute uses the class name to determine the URL.
    // Class "ProductsController" = URL "api/Products"
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AccountingContext _context;

        public ProductsController(AccountingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get()
        {
            return await _context.Products.ToListAsync();
        }
    }
}