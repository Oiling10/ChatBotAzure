using chatbot.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class PreguntaRespuesta
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PreguntaID { get; set; }

    [Required]
    [MaxLength(500)]
    public string Pregunta { get; set; }

    [Required]
    public string Respuesta { get; set; }
    public virtual ICollection<Interaccion> Interacciones { get; set; }
}
