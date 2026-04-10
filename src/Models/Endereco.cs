using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Endereco
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Local { get; set; } = string.Empty;

        [Required]
        public string Bairro { get; set; } = string.Empty;

        [Required]
        public string Cep { get; set; } = string.Empty;

        public string Complemento { get; set; } = string.Empty;

        [Required]
        public Guid UsuarioId { get; set; }

        public bool Principal { get; set; } = false;

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        public Endereco()
        {
            Id = Guid.NewGuid();
        }
    }
}
