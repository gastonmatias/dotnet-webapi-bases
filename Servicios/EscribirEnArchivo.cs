using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webAPIAuthors.Servicios
{
    //! este servicio escribirá en un archivo cada 5 seg
    public class EscribirEnArchivo : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "Archivo 1.txt";

        private Timer timer;

        public EscribirEnArchivo(IWebHostEnvironment env)
        {
            this.env = env;
        }

        // cuando se carga la webapi x 1ra vez
        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork,null,TimeSpan.Zero, TimeSpan.FromSeconds(5));
            Escribir("Proceso Iniciado");
            return Task.CompletedTask;
        }

        // cuando se apaga la webapi
        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            Escribir("Proceso Finalizado");
            return Task.CompletedTask;
        }

        private void DoWork(object state){
            Escribir("Proceso en ejecución: "+DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
        }

        // metodo auxiliar para escribir en el archivo
        private void Escribir(string mensaje){
            // var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}"; 
            var ruta = $@"{env.ContentRootPath}/wwwroot/{nombreArchivo}"; // correcta escritura de ruta en linux

            // append: true, indica qe NO se sustituirá el archivo anterior, 
            // simplemente se abrirá el archivo de txt qe ya existe e iremos escribiendo linea x linea en este
            using(StreamWriter writer = new StreamWriter(ruta, append: true)){
                writer.WriteLine(mensaje);
            }
        }
    }
}

//https://learn.microsoft.com/es-es/dotnet/api/system.io.streamwriter?view=net-7.0