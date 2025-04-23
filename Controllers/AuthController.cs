using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjetoDBZ.Data;
using ProjetoDBZ.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc.Razor;
using System.ComponentModel.DataAnnotations;

namespace ProjetoDBZ.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(AppDbContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

      if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
      {
        return Unauthorized("Usuário ou senha inválidos");
      }

      var token = GenerateJwtToken(user);
      return Ok(new { Token = token });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
      if (await _context.Users.AnyAsync(u => u.Username == request.Username))
      {
        return Conflict("Nome de usuário já está em uso");
      }

      if (await _context.Users.AnyAsync(u => u.Email == request.Email))
      {
        return Conflict("E-mail já está cadastrado");
      }

      var user = new User
      {
        Username = request.Username,
        Email = request.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        ConfirmationToken = Guid.NewGuid().ToString()
      };

      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/auth/confirm?token={user.ConfirmationToken}";
      await SendConfirmationEmail(user.Email, confirmationLink);

      return Ok(new { Message = "Registro realizado com sucesso. Verifique seu e-mail para confirmar." });
    }

    [HttpGet("confirm")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.ConfirmationToken == token);
      if (user == null)
      {
        return NotFound("Token de confirmação inválido");
      }

      user.EmailConfirmed = true;
      user.ConfirmationToken = null;
      await _context.SaveChangesAsync();

      return Ok("E-mail confirmado com sucesso");
    }

    private string GenerateJwtToken(User user)
    {
      var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
      var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

      var claims = new[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("id", user.Id.ToString())
      };

      var token = new JwtSecurityToken(
        issuer: _configuration["Jwt:Issuer"],
        audience: _configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"])),
        signingCredentials: credentials
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task SendConfirmationEmail(string email, string confirmationLink)
    {
      var smtpSettings = _configuration.GetSection("SmtpSettings");
      using (var client = new SmtpClient(smtpSettings["Host"], int.Parse(smtpSettings["Port"])))
      {
        client.Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]);
        client.EnableSsl = true;

        var mail = new MailMessage
        {
          From = new MailAddress(smtpSettings["FromAddress"], smtpSettings["FromName"]),
          Subject = "Confirmação de Cadastro - ProjetoDBZ",
          Body = $"Por favor, confirme seu e-mail clicando no link: {confirmationLink}",
          IsBodyHtml = true
        };
        mail.To.Add(email);

        await client.SendMailAsync(mail);
      }
    }
  }

  public class LoginRequest
  {
    public string Username { get; set; }
    public string Password { get; set; }
  }

  public class RegisterRequest
  {
    [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(6)]
    public string Password { get; set; }
  }
}
