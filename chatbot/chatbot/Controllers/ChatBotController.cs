using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using chatbot.Data;
using chatbot.Models;
using chatbot.Services;
using System;
using System.Threading.Tasks;
using chatbot.Models.DTOs;

namespace chatbot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        private readonly ChatBotContext _context;
        private readonly IAzCognitiveIntegrator _azureApiService;
        private readonly ILogger<ChatBotController> _logger;

        public ChatBotController(ChatBotContext context, IAzCognitiveIntegrator azureApiService, ILogger<ChatBotController> logger)
        {
            _context = context;
            _azureApiService = azureApiService;
            _logger = logger;
        }

        [HttpPost("registrarUsuario")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] UsuarioDTO usuarioDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo de usuario no válido: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var usuarioExistente = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == usuarioDTO.Email);
                if (usuarioExistente != null)
                {
                    _logger.LogInformation("El usuario con email {Email} ya existe.", usuarioDTO.Email);
                    return Conflict("El usuario ya existe.");
                }

                var nuevoUsuario = new Usuario { Nombre = usuarioDTO.Nombre, Email = usuarioDTO.Email };
                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuario registrado exitosamente: {UsuarioID}", nuevoUsuario.UsuarioID);
                return CreatedAtAction(nameof(RegistrarUsuario), new { id = nuevoUsuario.UsuarioID }, nuevoUsuario);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error al guardar en la base de datos.");
                return StatusCode(500, "Error al guardar en la base de datos.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar el usuario.");
                return StatusCode(500, "Ocurrió un error al registrar el usuario.");
            }
        }

        [HttpPost("iniciarSesion")]
        public async Task<IActionResult> IniciarSesion([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo de usuario no válido: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var existente = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == usuario.Email);
                if (existente == null)
                {
                    _context.Usuarios.Add(usuario);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Usuario registrado exitosamente: {Usuario}", usuario.Email);
                    return CreatedAtAction(nameof(IniciarSesion), usuario);
                }

                _logger.LogInformation("Usuario ya registrado: {Usuario}", existente.Email);
                return Ok(existente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar o verificar el usuario.");
                return StatusCode(500, "Ocurrió un error al procesar la solicitud.");
            }
        }

        [HttpPost("preguntar")]
        public async Task<IActionResult> Preguntar([FromBody] Interaccion interaccion)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Interacción no válida: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                var respuesta = await _azureApiService.ObtenerRespuestaAsync(interaccion.Pregunta);
                if (respuesta == null)
                {
                    _logger.LogError("Error al obtener la respuesta del API de Azure.");
                    return StatusCode(500, "Error al obtener la respuesta del API de Azure");
                }

                var preguntaRespuesta = new PreguntaRespuesta { Pregunta = interaccion.Pregunta, Respuesta = respuesta };
                _context.PreguntasRespuestas.Add(preguntaRespuesta);
                await _context.SaveChangesAsync();

                interaccion.PreguntaID = preguntaRespuesta.PreguntaID;
                interaccion.FechaHora = DateTime.UtcNow;

                var usuarioExistente = await _context.Usuarios.FindAsync(interaccion.UsuarioID);
                if (usuarioExistente == null)
                {
                    _logger.LogWarning("Usuario no encontrado: {UsuarioID}", interaccion.UsuarioID);
                    return NotFound($"Usuario con ID {interaccion.UsuarioID} no encontrado.");
                }

                await _context.Interacciones.AddAsync(interaccion);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Interacción registrada exitosamente para el usuario: {UsuarioID}", interaccion.UsuarioID);
                return Ok(preguntaRespuesta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el procesamiento de la pregunta.");
                return StatusCode(500, "Ocurrió un error en el procesamiento de la pregunta.");
            }
        }

        [HttpPost("evaluar")]
        public async Task<IActionResult> EvaluarRespuesta([FromBody] Evaluacion evaluacion)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Evaluación no válida: {ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                await _context.Evaluaciones.AddAsync(evaluacion);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Evaluación registrada exitosamente: {EvaluacionID}", evaluacion.EvaluacionID);
                return Ok(evaluacion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar la evaluación.");
                return StatusCode(500, "Ocurrió un error al registrar la evaluación.");
            }
        }
    }
}
