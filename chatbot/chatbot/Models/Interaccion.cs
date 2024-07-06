namespace chatbot.Models
{
    public class Interaccion
    {
        public int InteraccionID { get; set; }
        public int UsuarioID { get; set; }
        public int PreguntaID { get; set; }
        public DateTime FechaHora { get; set; }
        public Usuario Usuario { get; set; }
        public PreguntaRespuesta PreguntaRespuesta { get; set; }
        public string Pregunta { get; set; }
        public Evaluacion Evaluacion { get; set; }
    }
}
