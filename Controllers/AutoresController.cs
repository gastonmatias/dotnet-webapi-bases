using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIAuthors.DTOs;
using webAPIAuthors.Entidades;

namespace webAPIAuthors.Controllers
{
    [ApiController] // para validaciones automaticas
    [Route("api/autores")]
    // [Authorize]
    
    // 2da forma: en tiempo de ejecucion "[controller]" se transformara a "autores" (prefijo del nombre del controlador)
    // [Route("api/[controller]")] 
    public class AutoresController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        //# region[blue] //! INYECCION
        public AutoresController(ApplicationDbContext context,IMapper mapper, IConfiguration configuration){
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }
        //#endregion
        
        //! GET LISTADO AUTORES
        [HttpGet]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        // [ResponseCache(Duration =10)] /* las prox peticiones http qe lleguen en los prox 10 seg se serviran del cache */
        // public async Task<ActionResult<List<AutorDTO>>> Get(){
        public async Task<List<AutorDTO>> Get(){
            
            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);// .Map<destino>(fuente)
        }

        // ! obtener autor x id
        [HttpGet("{id:int}", Name = "obtenerAutor")]
        public async Task<ActionResult<AutorDTOConLibros>> Get(int id){
            var autor =  await context.Autores
                        .Include(autorDb => autorDb.AutoresLibros)
                        .ThenInclude(autorLibroDb => autorLibroDb.Libro)
                        .FirstOrDefaultAsync(x => x.Id == id); // 1er registro de la tabla o null

            if (autor == null)
            {
                return NotFound(); // not found hereda de ActionResult
            }

            return mapper.Map<AutorDTOConLibros>(autor); // .Map<destino>(fuente

        }

        // ! obtener autor/es x nombre
        [HttpGet("{nombre}")] /* nota: NO existe restriccion x string */
        public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute]string nombre){
            var autores =  await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<AutorDTO>>(autores);// .Map<destino>(fuente)
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DTOs.AutorCreacionDTO autorCreacionDTO){

            // validacion desde controller
            var existeAutorConMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre); // retorna bool

            if (existeAutorConMismoNombre)
            {   
                return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
            }

            // dbcontext no reconoceria autorCreacionDTO como entidad valida para bd,
            // por ello se necesitar mapear a "autor"
            
            // se le pasa al mapeador de automaper, la instancia de autorCreacionDTO que quiero mapear
            // hacia el tipo autor
            var autor = mapper.Map<Autor>(autorCreacionDTO);

            // autor mapeado pasado al context de bd
            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            // se pasa por params: ruta + objeto anonimo (contiene arg para la ruta) + objeto creado en BD
            return CreatedAtRoute("obtenerAutor", new { id = autor.Id}, autorDTO ); 
        }

        [HttpPut("{id:int}")] // api/autores/id
        public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id){

            // valida qe exista autor con id pasado por params
            var existe = await context.Autores.AnyAsync(x=> x.Id == id);
            if (!existe) { return NotFound();}

            // mapeo del DTO modificado a entidad
            var autor = mapper.Map<Autor>(autorCreacionDTO);

            // asignar la id
            autor.Id = id; //id qe viene por params

            context.Update(autor);
            await context.SaveChangesAsync();

            return NoContent(); // 204
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