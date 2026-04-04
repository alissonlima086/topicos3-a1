using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Usuario
    {
        [Key]
        public Guid Id { get; set; }  // setter público (EF Core precisa)

        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;

        [Required]
        [MaxLength(11)]
        public string Cpf { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

        [Required]
        public Funcao Funcao { get; set; }

        public Usuario()
        {
            Id = Guid.NewGuid();
        }

        public Usuario(string nome, string email, string senha, string cpf, DateTime dataNascimento, Funcao funcao)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Email = email;
            Senha = senha;
            Cpf = cpf;
            DataNascimento = dataNascimento;
            Funcao = funcao;
        }

        public void AlterarFuncao(Funcao funcao)
        {
            Funcao = funcao;
        }
    }
}