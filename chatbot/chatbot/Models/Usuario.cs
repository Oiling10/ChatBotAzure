using System.ComponentModel.DataAnnotations;

namespace chatbot.Models
{
    public class Usuario
    {
        public int UsuarioID { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        public ICollection<Interaccion> Interacciones { get; set; }
    }
}
