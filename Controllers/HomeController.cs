using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
  [Route("v1")]
  public class HomeController : ControllerBase
  {
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<dynamic>> Get([FromServices] DataContext context)
    {
      User employee = new User { Username = "robin", Password = "robin", Role = "employee" };
      User manager = new User { Username = "batman", Password = "batman", Role = "manager" };
      Category category = new Category { Id = 1, Title = "Informática" };
      Product product = new Product { Id = 1, Category = category, Title = "Mouse", Price = 10, Description = "Mouse gamer" };

      context.Users.Add(employee);
      context.Users.Add(manager);
      context.Categories.Add(category);
      context.Products.Add(product);

      await context.SaveChangesAsync();

      return Ok(new { message = "Dados configurados" });

    }

  }
}