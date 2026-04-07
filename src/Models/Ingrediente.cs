using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Ingrediente
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public Ingrediente()
    {
        Id = Guid.NewGuid();
    }
}
}