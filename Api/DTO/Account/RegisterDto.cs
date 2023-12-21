using System.ComponentModel.DataAnnotations;

namespace Api.DTO.Account
{
    public class RegisterDto
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Primeiro nome deve ter pelo menos {2}, e no máximo {1} carácter")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Primeiro nome deve ter pelo menos {2}, e no máximo {1} carácter")]
        public string LastName { get; set; }
        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Email invalido")]
        public string Email { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Senha deve te pelo menos {2}, e no máximo {1} carácter")]
        public string Password { get; set; }
    }
}
