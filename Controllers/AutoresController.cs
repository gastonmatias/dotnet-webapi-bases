using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIAuthors.Entidades;
using webAPIAuthors.Filtros;

namespace webAPIAuthors.Controllers
{
    [ApiController] // para validaciones automaticas
    [Route("api/autores")]
    // [Authorize]
    
    // 2da forma: en tiempo de ejecucion "[controller]" se transformara a "autores" (prefijo del nombre del controlador)
    // [Route("api/[controller]")] 
    public class AutoresController: ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly ApplicationDbContext context;
        private readonly ILogger<AutoresController> logger;

        // inyeccion de servicio ApplicationDbContext (configurado en program.cs) 
        // que controla conex a bd
        public AutoresController(ApplicationDbContext context, ILogger<AutoresController> logger){
            this.context = context;
            this.logger = logger;
        }

        [HttpGet]
        [HttpGet("/listado")]
        [ResponseCache(Duration =10)] /* las prox peticiones http qe lleguen en los prox 10 seg se serviran del cache */
        public async Task<ActionResult<List<Autor>>> Get(){
            
            // throw new NotImplementedException();// para testear filtro global FiltroDeExcepcion
            logger.LogInformation("Estamos obteniendo los autores");
            logger.LogError("MSJE DE PRUEBA");
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        [HttpGet("primero")] // api/autores/primero?nombre=peter
        // [Authorize]
        [ServiceFilter(typeof(MiFiltroDeAccion))]
        public async Task<ActionResult<Autor>> PrimerAutor([FromHeader] int miValor,[FromQuery] string nombre){
            return await context.Autores.FirstOrDefaultAsync();// 1er registro de la tabla o null
        }

        // ! obtener autor x id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Autor>> Get(int id){
            var autor =  await context.Autores.FirstOrDefaultAsync(x => x.Id == id); // 1er registro de la tabla o null

            if (autor == null)
            {
                return NotFound(); // not found hereda de ActionResult
            }

            return autor;

        }

        // ! obtener autor x id
        [HttpGet("{nombre}")] /* nota: NO existe restriccion x string */
        public async Task<ActionResult<Autor>> Get([FromRoute]string nombre){
            var autor =  await context.Autores.FirstOrDefaultAsync(x => x.Nombre.Contains(nombre)); // 1er registro de la tabla o null

            if (autor == null)
            {
                return NotFound();
            }

            return autor;

        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor){

            // validacion desde controller
            var existeAutorConMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre); // retorna bool

            if (existeAutorConMismoNombre)
            {   
                return BadRequest($"Ya existe un autor con el nombre {autor.Nombre}");
            }

            context.Add(autor);
            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id:int}")] // api/autores/id
        public async Task<ActionResult> Put(Autor autor, int id){

            if (autor.Id != id)
            {
                return BadRequest("El id del autor NO coincide con el id de la URL"); //error 4xx
            }

            var existe = await context.Autores.AnyAsync(x=> x.Id == id);

            if (!existe){
                return NotFound(); // error 404
            }

            context.Update(autor);
            await context.SaveChangesAsync();

            return Ok();
        }
        
        [HttpDelete("{id:int}")] //api/autores/id
        public async Task<ActionResult> Delete(int id){

            var existe = await context.Autores.AnyAsync(x=> x.Id == id);

            if (!existe){
                return NotFound(); // error 404
            }

            // NO se esta creando un nuevo autor en bd, sino qe "se crea un objeto tipo autor"
            // pq entity framework necesita una instancia para poder accionar
            context.Remove(new Autor(){Id = id});

            await context.SaveChangesAsync();

            return Ok();


        }

    }
}