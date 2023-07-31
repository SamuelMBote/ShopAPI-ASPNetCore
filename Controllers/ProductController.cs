using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
  [Route("v1/products")]
  public class ProductController : ControllerBase
  {
    [HttpGet]
    [Route(template: "")]
    [AllowAnonymous]
    public async Task<ActionResult<List<Product>>> GetAll([FromServices] DataContext context)
    {

      var products = await context.Products.Include(navigationPropertyPath: x => x.Category).AsNoTracking().ToListAsync();
      return Ok(value: products);
    }


    [HttpGet]
    [Route("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Product>> GetById(int id, [FromServices] DataContext context)
    {
      var product = await context.Products.Include(x => x.Category).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
      return Ok(product);

    }
    [HttpGet]
    [Route("categories/{id:int}")]
    public async Task<ActionResult<List<Product>>> GetByCategory(int id, [FromServices] DataContext context)
    {

      var products = await context.Products.Include(x => x.Category).AsNoTracking().Where(x => x.CategoryId == id).ToListAsync();
      return Ok(products);

    }

    [HttpPost]
    [Route("")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Product>> Post([FromBody] Product model, [FromServices] DataContext context)
    {
      context.Products.Add(model);

      await context.SaveChangesAsync();
      return Ok(model);
    }

    [HttpPut]
    [Route(template: "{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Product>> Put(int id, [FromBody] Product model, [FromServices] DataContext context)
    {
      if (id != model.Id)
        return NotFound(value: new { message = "Produto Não Encontrado" });

      if (!ModelState.IsValid)
        return BadRequest(modelState: ModelState);
      try
      {
        context.Entry<Product>(model).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return Ok(value: model);
      }
      catch (DbUpdateConcurrencyException)
      {
        return BadRequest(new { message = "Este registro ja foi atuaalizado" });
      }
      catch (System.Exception)
      {
        return BadRequest(new { message = "Não foi possível atualizar produto" });

      }


    }

    [HttpDelete]
    [Route(template: "{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Product>> Delete(int id, [FromServices] DataContext context)
    {
      var product = await context.Products.FirstOrDefaultAsync(predicate: x => x.Id == id);
      if (product == null)
        return NotFound(new { message = "Categoria não encontrada" });

      try
      {
        context.Products.Remove(product);
        await context.SaveChangesAsync();
        return Ok(new { message = "Produto removido com sucesso" });
      }
      catch (System.Exception ex)
      {
        return BadRequest(new { message = "Não foi possível remover o produto" });
      }

    }

  }
}