using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Endereco
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Local {  get; set; } = string.Empty;

        [Required]
        public string Bairro { get; set; } = string.Empty;

        [Required]
        public string Cep {  get; set; } = string.Empty;

        public String Complemento {  get; set; } = string.Empty;

        [Required]
        public Guid UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }


        public Endereco()
        {
            Id = Guid.NewGuid();
        }

        public Endereco(string local, string bairro,  string cep, String complemento, Guid usuarioId)
        {
            Id = Guid.NewGuid();
            Bairro = bairro;
            Cep = cep;
            Complemento = complemento;
            UsuarioId = usuarioId;
        }

    }
}
