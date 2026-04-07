using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

        public bool LembrarMe { get; set; }
    }

    public class CadastroViewModel
    {
        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(11)]
        public string Cpf { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }

    public class AlterarSenhaViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string SenhaAtual { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        public string NovaSenha { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("NovaSenha", ErrorMessage = "As senhas não coincidem.")]
        public string ConfirmarNovaSenha { get; set; } = string.Empty;
    }
}