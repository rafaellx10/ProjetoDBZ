using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoDBZ.Models
{
  public class Personagem
  {
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome do personagem é obrigatório.")]
    [StringLength(50, ErrorMessage = "O nome do personagem deve ter no máximo 50 caracteres.")]
    [MinLength(3, ErrorMessage = "O nome do personagem deve ter no mínimo 3 caracteres.")]
    [DataType(DataType.Text)]
    [DisplayFormat(ConvertEmptyStringToNull = true)]
    public string Name { get; set; }

    [Required(ErrorMessage = "O tipo do personagem é obrigatória.")]
    [StringLength(50, ErrorMessage = "O tipo do personagem deve ter no máximo 50 caracteres.")]
    [MinLength(3, ErrorMessage = "O tipo do personagem deve ter no mínimo 3 caracteres.")]
    public string Type { get; set; }
  }
}