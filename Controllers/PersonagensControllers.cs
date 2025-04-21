using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> AddPersonagem(Personagem personagem)
    {
      _appDbContext.DBZ.Add(personagem);
      await _appDbContext.SaveChangesAsync();

      return Ok(personagem);
    }
  }
}