using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI_Log.Custom;
using WebAPI_Log.Models;
using WebAPI_Log.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI_Log.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccesoController : ControllerBase
    {
        private readonly DbpruebaContext _dbpruebaContext;
        private readonly Utilidades _utilidades;
        public AccesoController(DbpruebaContext dbpruebaContext, Utilidades utilidades)
        {
            _dbpruebaContext = dbpruebaContext;
            _utilidades = utilidades;
        }

        [HttpPost]
        [Route("Registrarse")]

        public async Task<IActionResult> Registrarse(UsuarioDTO objeto)
        {
            var modeloUsuario = new Usuario
            {
                Nombre = objeto.Nombre,
                Correo = objeto.Correo,
                Clave = _utilidades.EncriptarSHA256(objeto.Clave),
            };

            await _dbpruebaContext.Usuarios.AddAsync(modeloUsuario);    
            await _dbpruebaContext.SaveChangesAsync();

            if (modeloUsuario.IdUsuario != 0)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }

        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login(LoginDTO objeto)
        {
            var usuarioEncontrado = await _dbpruebaContext.Usuarios
                                    .Where(u =>
                                        u.Correo == objeto.Correo &&
                                        u.Clave == _utilidades.EncriptarSHA256(objeto.Clave)
                                        ).FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
                    else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _utilidades.generarJWT(usuarioEncontrado) });
        }

        //Validacion del Token 
        [HttpGet]
        [Route("ValidarToken")]

        public IActionResult ValidarToken([FromQuery]string token)
        {
                bool respuesta = _utilidades.validarToken(token);
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = respuesta });
 
        }




    }
}
