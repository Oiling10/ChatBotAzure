using chatbot.Models;

namespace chatbot.Models
{
    public class Evaluacion
    {
        public int EvaluacionID { get; set; }
        public int InteraccionID { get; set; }
        public int Puntuacion { get; set; }
        public string Comentarios { get; set; }
        public Interaccion Interaccion { get; set; }
    }
}
