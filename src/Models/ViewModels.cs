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

    public class CadastroUsuarioViewModel
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

    public class ReservaCreateViewModel
    {
        [Required(ErrorMessage = "Informe a data.")]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "Informe o horário de início.")]
        public string HoraInicio { get; set; } = string.Empty;

        [Required(ErrorMessage = "Informe o horário de fim.")]
        public string HoraFim { get; set; } = string.Empty;

        [Required]
        public int NumeroPessoas { get; set; }

        [Required(ErrorMessage = "Selecione uma mesa.")]
        public Guid MesaId { get; set; }

        public Guid? UsuarioId { get; set; }
        public string? UsuarioCpf { get; set; }

        public List<MesaSelecaoViewModel> Mesas { get; set; } = new();
    }

    public class MesaSelecaoViewModel
    {
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public int Capacidade { get; set; }
    }
}
