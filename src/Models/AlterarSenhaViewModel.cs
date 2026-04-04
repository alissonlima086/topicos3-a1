using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class AlterarSenhaViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string SenhaAtual { get; set; } = string.Empty;

        [Required]
        public string NovaSenha { get; set; } = string.Empty;

        [Required]
        [Compare("NovaSenha", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmarNovaSenha { get; set; } = string.Empty;
    }
}