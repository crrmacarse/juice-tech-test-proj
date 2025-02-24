using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;

namespace WebApplication1
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public static Product product = new();
        
        [HttpGet]
        public ActionResult<Product[]> GetProducts() {
            return Ok();
        }

        [HttpGet("user")]
        public ActionResult<Product[]> GetProductsByUser() {
            return Ok();
        }
    }
}