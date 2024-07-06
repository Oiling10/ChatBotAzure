using System.ComponentModel.DataAnnotations;

namespace chatbot.Models.DTOs
{
    public class UsuarioDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
    }

}
