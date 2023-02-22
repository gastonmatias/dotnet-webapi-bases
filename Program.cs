using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
// using webAPIAutores;
using webAPIAuthors;
using webAPIAuthors.Filtros;
using webAPIAuthors.Middlewares;
using webAPIAuthors.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"))
);

builder.Services.AddControllers(opciones =>
    opciones.Filters.Add(typeof(FiltroDeExcepcion)) /* FILTRO GLOBAL */ 
).AddJsonOptions( x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
);

// "cuando una clase requiera un IServicio, otorgar un ServicioA"
// builder.Services.AddTransient<IServicio, ServicioA>();

// #region[green1]
// #endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// para ejecutar un codigo recurrente
builder.Services.AddHostedService<EscribirEnArchivo>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// filtro para uso de cache
builder.Services.AddResponseCaching();

// filtro para uso de autenticacion
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

// para uso de filtro personalizado
builder.Services.AddTransient<MiFiltroDeAccion>();

var app = builder.Build();

//! middlewares: se ejecutan segun orden de llegada
// forma valida pero sin utilizar la clase estatica LoguearRespuestaHTTPMiddlewareExtensions
// app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();

// la diferencia es qe aqui NO estamos exponiendo la clase qe estamos utilizando
// app.UseLoguearRespuestaHTTP();

//map crea una bifucarcion en la tuberia de procesos
app.Map("/ruta1", app => {
    // run corta la tuberia de procesos, en este caso de /ruta1
    app.Run(async contexto => {
        await contexto.Response.WriteAsync("estoy interceptando la tuberia");
    });
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();

app.Run();
