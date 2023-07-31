using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
namespace Shop.Controllers
{
  [Route("v1/categories")]
  public class CategoryController : ControllerBase
  {

    [HttpGet]
    [Route("")]
    [AllowAnonymous]
    [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
    public async Task<ActionResult<List<Category>>> GetAll([FromServices] DataContext context)
    {
      try
      {
        var categories = await context.Categories.AsNoTracking().ToListAsync();
        if (categories == null)
          return NotFound(new { message = "Não foram encontradaos resultados em categorias" });
        return Ok(categories);
      }
      catch (System.Exception)
      {
        return BadRequest(new { message = "Erro ao buscar categorias" });
        throw;
      }


    }
    [HttpGet]
    [Route(template: "{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<Category>> GetById(int id, [FromServices] DataContext context)
    {
      try
      {
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (category == null)
          return NotFound(new { message = "Não foi encontrado categoria com esse ID" });
        return Ok(category);
      }
      catch (System.Exception)
      {

        return BadRequest(new { message = "Erro ao buscar categoria" });
      }


    }


    [HttpPost]
    [Route(template: "")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Post([FromBody] Category model, [FromServices] DataContext context)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);
      try
      {
        context.Categories.Add(entity: model);
        await context.SaveChangesAsync();

        return Ok(model);
      }
      catch (System.Exception)
      {
        return BadRequest(new { message = "Não foi possível criar categoria" });

      }


    }


    [HttpPut]
    [Route(template: "{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Put(int id, [FromBody] Category model, [FromServices] DataContext context)
    {
      if (id != model.Id)
        return NotFound(value: new { message = "Categoria Não Encontrada" });

      if (!ModelState.IsValid)
        return BadRequest(modelState: ModelState);
      try
      {
        context.Entry<Category>(model).State = EntityState.Modified;
        await context.SaveChangesAsync();

        return Ok(value: model);
      }
      catch (DbUpdateConcurrencyException)
      {
        return BadRequest(new { message = "Este registro ja foi atuaalizado" });
      }
      catch (System.Exception)
      {
        return BadRequest(new { message = "Não foi possível atualizar categoria" });

      }


    }
    [HttpDelete]
    [Route(template: "{id:int}")]
    [Authorize(Roles = "employee")]
    public async Task<ActionResult<Category>> Delete(int id, [FromServices] DataContext context)
    {
      var category = await context.Categories.FirstOrDefaultAsync(predicate: x => x.Id == id);
      if (category == null)
        return NotFound(new { message = "Categoria não encontrada" });

      try
      {
        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return Ok(new { message = "Categoria removida com sucesso" });
      }
      catch (System.Exception ex)
      {
        return BadRequest(new { message = "Não foi possível remover a categoria" });
      }

    }

  }
}