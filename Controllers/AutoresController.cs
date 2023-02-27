using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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

        //# region[blue] //! INYECCION
        public AutoresController(ApplicationDbContext context,IMapper mapper){
            this.context = context;
            this.mapper = mapper;
        }
        //#endregion
        
        //! GET LISTADO AUTORES
        [HttpGet]
        // [ResponseCache(Duration =10)] /* las prox peticiones http qe lleguen en los prox 10 seg se serviran del cache */
        // public async Task<ActionResult<List<AutorDTO>>> Get(){
        public async Task<List<AutorDTO>> Get(){
            
            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTO>>(autores);// .Map<destino>(fuente)
        }

        // ! obtener autor x id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AutorDTO>> Get(int id){
            var autor =  await context.Autores.FirstOrDefaultAsync(x => x.Id == id); // 1er registro de la tabla o null

            if (autor == null)
            {
                return NotFound(); // not found hereda de ActionResult
            }

            return mapper.Map<AutorDTO>(autor); // .Map<destino>(fuente

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