using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
  [Route(template: "v1/user")]
  public class UserController : ControllerBase
  {
    [HttpPost]
    [Route(template: "signup")]
    [AllowAnonymous]
    public async Task<ActionResult<User>> SignUp([FromBody] User model, [FromServices] DataContext context)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);
      try
      {
        model.Role = "employee";

        context.Users.Add(model);
        await context.SaveChangesAsync();

        model.Password = "***";
        return Ok(model);
      }
      catch (System.Exception)
      {
        return BadRequest(new { message = "Não foi possível criar o usuário" });
        throw;
      }

    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<User>> Login([FromBody] User model, [FromServices] DataContext context)
    {
      var user = await context.Users.AsNoTracking().Where(x => x.Username == model.Username && x.Password == model.Password).FirstOrDefaultAsync();

      if (user == null)
        return NotFound(new { message = "Usuário ou senha inválida" });
      var token = TokenService.GenerateToken(user);
      user.Password = "***";
      return Ok(new { user = user, token = token });
    }

    [HttpGet]
    [Route("")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<List<User>>> GetAll([FromServices] DataContext context)
    {
      var users = await context.Users.AsNoTracking().ToListAsync();
      foreach (var user in users)
      {
        user.Password = "***";
      }
      return Ok(users);
    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<User>> Put(int id, [FromBody] User model, [FromServices] DataContext context)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (id != model.Id)
        return NotFound(new { message = "Usuário não encontrado" });

      try
      {
        context.Entry(model).State = EntityState.Modified;
        await context.SaveChangesAsync();
        model.Password = "***";
        return Ok(model);
      }
      catch (System.Exception)
      {
        return BadRequest(new { message = "Não foi possível alterar o usuário" });
        throw;
      }
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "manager")]
    public async Task<ActionResult<User>> Delete(int id, [FromServices] DataContext context)
    {
      var user = await context.Users.FirstOrDefaultAsync(predicate: x => x.Id == id);
      if (user == null)
        return NotFound(new { message = "Usuário não encontrado" });

      try
      {
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return Ok(new { message = "Usuário removido com sucesso" });
      }
      catch (System.Exception ex)
      {
        return BadRequest(new { message = "Não foi possível remover o usuário" });
      }

    }
  }
}