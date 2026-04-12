using System.ComponentModel.DataAnnotations;
using WebApplication1.Models.Enums;

namespace WebApplication1.Models
{
    public class Prato
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    [Required]
    public float PrecoBase { get; set; }

    [Required]
    public bool Ativo { get; set; }

    [Required]
    public Turno Turno { get; set; }

    public ICollection<PratoIngrediente> PratoIngredientes { get; set; } = new List<PratoIngrediente>();

    public Prato()
    {
        Id = Guid.NewGuid();
    }

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;
}
}