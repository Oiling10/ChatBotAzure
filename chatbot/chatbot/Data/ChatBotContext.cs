using Microsoft.EntityFrameworkCore;
using chatbot.Models;

namespace chatbot.Data
{
    public class ChatBotContext : DbContext
    {
        public ChatBotContext(DbContextOptions<ChatBotContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<PreguntaRespuesta> PreguntasRespuestas { get; set; }
        public DbSet<Interaccion> Interacciones { get; set; }
        public DbSet<Evaluacion> Evaluaciones { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<PreguntaRespuesta>()
                .HasKey(p => p.PreguntaID);
            modelBuilder.Entity<PreguntaRespuesta>()
                .Property(p => p.Pregunta)
                .IsRequired()
                .HasMaxLength(500);
            modelBuilder.Entity<PreguntaRespuesta>()
                .Property(p => p.Respuesta)
                .IsRequired();
            modelBuilder.Entity<Interaccion>()
                .HasKey(i => i.InteraccionID);
            modelBuilder.Entity<Interaccion>()
                .HasOne(i => i.Usuario)
                .WithMany(u => u.Interacciones)
                .HasForeignKey(i => i.UsuarioID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Interaccion>()
                .HasOne(i => i.PreguntaRespuesta)
                .WithMany(p => p.Interacciones)
                .HasForeignKey(i => i.PreguntaID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Evaluacion>()
                .HasKey(e => e.EvaluacionID);
            modelBuilder.Entity<Evaluacion>()
                .Property(e => e.Comentarios)
                .HasMaxLength(1000);
            modelBuilder.Entity<Evaluacion>()
                .HasOne(e => e.Interaccion)
                .WithOne(i => i.Evaluacion)
                .HasForeignKey<Evaluacion>(e => e.InteraccionID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
