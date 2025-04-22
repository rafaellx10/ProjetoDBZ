using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoDBZ.Data;
using ProjetoDBZ.Models;

namespace ProjetoDBZ.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class PersonagensControllers : ControllerBase
  {
    private readonly AppDbContext _appDbContext;
    public PersonagensControllers(AppDbContext appDbContext)
    {
      _appDbContext = appDbContext;
    }

    [HttpPost]
    public async Task<IActionResult> AddPersonagem([FromBody] Personagem personagem)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      _appDbContext.DBZ.Add(personagem);
      await _appDbContext.SaveChangesAsync();

      return Created("Personagem criado com sucesso!", personagem);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Personagem>>> GetPersonagens()
    {
      var personagens = await _appDbContext.DBZ.ToListAsync();
      return Ok(personagens);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPersonagem(int id)
    {
      var personagem = await _appDbContext.DBZ.FindAsync(id);
      if (personagem == null)
      {
        return NotFound("Personagem não encontrado");
      }
      return Ok(personagem);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePersonagem(int id, [FromBody] Personagem updatedPersonagem)
    {
      var personagemExists = await _appDbContext.DBZ.FindAsync(id);
      if (personagemExists == null)
      {
        return NotFound("Personagem não encontrado");
      }
      _appDbContext.Entry(personagemExists).CurrentValues.SetValues(updatedPersonagem);
      await _appDbContext.SaveChangesAsync();
      return StatusCode(201, personagemExists);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePersonagem(int id)
    {
      var personagem = await _appDbContext.DBZ.FindAsync(id);
      if (personagem == null)
      {
        return NotFound("Personagem não encontrado");
      }
      _appDbContext.DBZ.Remove(personagem);
      await _appDbContext.SaveChangesAsync();
      return Ok("Personagem deletado com sucesso");
    }

  }
}