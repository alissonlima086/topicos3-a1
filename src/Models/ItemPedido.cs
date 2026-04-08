using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{

    public class ItemPedido
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public int Quantidade { get; set; }

    [Required]
    public float PrecoUnitario { get; set; }

    [Required]
    public string NomePrato { get; set; } = string.Empty;

    public bool FoiSugestao { get; set; }

    public string Observacao { get; set; } = string.Empty;

    [Required]
    public Guid PedidoId { get; set; }

    public Pedido? Pedido { get; set; }

    public ItemPedido()
    {
        Id = Guid.NewGuid();
    }
}
}