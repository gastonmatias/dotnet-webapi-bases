using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using webAPIAuthors.DTOs;

namespace webAPIAuthors.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;

        // region[blue]
        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }//endregion

        [HttpPost("registrar")] // api/cuentas/registrar
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario ){
            
            var usuario = new IdentityUser{
                UserName = credencialesUsuario.Email, 
                Email= credencialesUsuario.Email
            };

            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.password);

            if (resultado.Succeeded)
            {
                //Json web token
                return ConstruirToken(credencialesUsuario);    
            }
            else{
                return BadRequest(resultado.Errors);
            }

        }

        private RespuestaAutenticacion ConstruirToken(CredencialesUsuario credencialesUsuario){
            // claim: par llave-valor.
            // guarda info emitida por una fuente de confianza (info NO sensible, ojo)
            var claims = new List<Claim>(){
                new Claim("email", credencialesUsuario.Email)
            };

            // obtencion de llave secreta, alojada en archivo de configuraciones (escondida)
            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            
            // firmar credenciales con llave + algoritmo a utilizar
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            // tiempo de expiracion para el token
            var expiracion = DateTime.UtcNow.AddYears(1);

            // construccion del token
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion(){
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken), //retorna string qe representa token
                Expiracion = expiracion
            };
        }


    }
}