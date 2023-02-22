using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webAPIAuthors.Middlewares
{

    // metodos de restriccion solamente se pueden colocar en classes estaticas

    // clase qe servira para NO exponer la clase qe estamos utilizando al declarar este middleware
    // en program.cs con app.use...
    public static class LoguearRespuestaHTTPMiddlewareExtensions{
        public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app){
            return app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
        }
    }

    public class LoguearRespuestaHTTPMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoguearRespuestaHTTPMiddleware> logger;

        public LoguearRespuestaHTTPMiddleware(
            RequestDelegate siguiente, 
            ILogger<LoguearRespuestaHTTPMiddleware> logger
        )
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        // regla para poder utilizar class como middleware
        // debe tener metodo publico invoke or invokeAsync
        public async Task InvokeAsync(HttpContext contexto){

            // memoryStream se necesita para guardar en memoria la respuesta de la peticion http,
            // pq esta se encuentra en un buffer
            // hay qe 1) copiarla 2) escribirla en un stream 3) volver a colocarla en el buffer (para qe el cliente pueda leerla)
            using (var ms = new MemoryStream()){
                var cuerpoOriginalRespuesta = contexto.Response.Body; 
                contexto.Response.Body = ms;

                // con el siguiente, se le permite a la tuberia de procesos continuar
                await siguiente(contexto); 

                ms.Seek(0,SeekOrigin.Begin);

                // esto guardara lo qe sea qe vayamos a responder al cliente en este string
                string respuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0,SeekOrigin.Begin);

                // ahora se necesita volver a colocar el string en la posicion inicial, asi se le puede enviar
                // la respuesta correctamente al usuario
                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                contexto.Response.Body = cuerpoOriginalRespuesta;

                // esta manipulacion nos permite leer el string y volver a colocarlo como estaba,
                // para qe el cliente final pueda utilizarlo
                logger.LogInformation(respuesta);
            }
        }

    }
}