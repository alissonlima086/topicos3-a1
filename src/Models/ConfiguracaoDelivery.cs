using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public enum TipoDelivery
    {
        App,
        Proprio
    }

    public class ConfiguracaoDelivery
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public TipoDelivery Tipo { get; set; }

        [MaxLength(100)]
        public string? NomeApp { get; set; }

        [Range(0, 100)]
        public int? ComissaoPorcentagem { get; set; }

        public float? TaxaAdicionalApp { get; set; }

        public float? TaxaFixaProprio { get; set; }

        public bool Ativo { get; set; } = true;

        public ConfiguracaoDelivery()
        {
            Id = Guid.NewGuid();
        }
    }
}
