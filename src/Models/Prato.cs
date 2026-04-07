using System.ComponentModel.DataAnnotations;

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

    public Prato()
    {
        Id = Guid.NewGuid();
    }

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;
}
}