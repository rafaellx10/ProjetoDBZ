using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjetoDBZ.Models
{
  public class User
  {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
    [StringLength(50, ErrorMessage = "O nome de usuário deve ter no máximo 50 caracteres.")]
    [MinLength(3, ErrorMessage = "O nome de usuário deve ter no mínimo 3 caracteres.")]
    public string Username { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [NotMapped] // Não será armazenado no banco - apenas o hash
    public string Password { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    public bool EmailConfirmed { get; set; } = false;

    public string ConfirmationToken { get; set; }
  }
}
