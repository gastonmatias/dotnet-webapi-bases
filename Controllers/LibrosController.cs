using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webAPIAuthors.DTOs;
using webAPIAuthors.Entidades;

namespace webAPIAuthors.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        //#region[blue]//! inyeccion
        public LibrosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        //endregion

        //! GET LIBRO X ID
        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDTOConAutores>> Get(int id){
            
            var libro =  await context.Libros
                              .Include(libroBd => libroBd.AutoresLibros) //para incluir autores a momento de GET libro
                              .ThenInclude(autorLibroBd => autorLibroBd.Autor ) // thenInclude es como un 2do join, desde dentro de AutoresLibros
                              .FirstOrDefaultAsync(x => x.Id == id);
            
            if(libro == null){ return NotFound();}

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x=> x.Orden).ToList(); // para ordenar por campo nombre ascendente
            
            return mapper.Map<LibroDTOConAutores>(libro);
        }

        //! POST NUEVO LIBRO
        [HttpPost]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO){
            
            // valida que se otorgue un autorid al momento de crear un libro
            if(libroCreacionDTO.AutoresIds == null){
                return BadRequest("No se puede crear un libro sin autores");
            }

            // valida que exista autor/es con ids pasados x cliente
            var autoresIds = await context.Autores
                                .Where(autorBd => libroCreacionDTO.AutoresIds.Contains(autorBd.Id)) // rescata id de autor/es pasados x params x cliente
                                .Select(x => x.Id).ToListAsync(); // lista los ids de los autores encontrados

            // verifica que exista = cantidad de ids entre "pasados por params y validados" y "en bd"
            if(libroCreacionDTO.AutoresIds.Count != autoresIds.Count){
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroCreacionDTO); // .map<destino>(fuente)

            AsignarOrdenAutores(libro);

            context.Add(libro);
            await context.SaveChangesAsync();
            
            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("obtenerLibro", new {id = libro.Id}, libroDTO );

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(LibroCreacionDTO libroCreacionDTO, int id){

            // seleccionar libro a actualizar ( se queda en memoria de netcore)
            var libroDB = await context.Libros
                            .Include(x => x.AutoresLibros)
                            .FirstOrDefaultAsync( x=> x.Id == id);
            
            if (libroDB == null){ return NotFound(); }

            // libroDB, se mantiene la misma instancia de libroDB en memoria, y se sobreescribe
            libroDB = mapper.Map(libroCreacionDTO,libroDB);
            AsignarOrdenAutores(libroDB);
            
            //se actualiza bd con el contenido de libroDB sobreescrito
            await context.SaveChangesAsync();

            return NoContent();
        }

        // fx helper para ordenar jerarquicamente autores (util para post y put)
        private void AsignarOrdenAutores(Libro libro){
                        // estable la prop orden segun el orden del array de autores otorgado x cliente
            if(libro.AutoresLibros != null){
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult>Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument){

            if (patchDocument == null){return BadRequest();}
            
            var libroBd = await context.Libros.FirstOrDefaultAsync(x=> x.Id == id);

            if(libroBd == null){ return NotFound();}

            // se llena libroPatchDTO con la info de libroBd, y se guarda en variable libroDTO
            var libroDTO = mapper.Map<LibroPatchDTO>(libroBd);

            // se aplica a libroDTO los cambios qe vienen en el patchdocument
            // ModelState:  en caso de algun error, se guarda alli
            patchDocument.ApplyTo(libroDTO, ModelState); 

            var esValido = TryValidateModel(libroDTO);
            if(!esValido){ return BadRequest(ModelState);}

            mapper.Map(libroDTO,libroBd);
            // mapper.Map<libroBd>(libroDTO);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")] 
        public async Task<ActionResult> Delete(int id){

            var existe = await context.Libros.AnyAsync(x=> x.Id == id);

            if (!existe){ return NotFound(); }

            // NO se esta creando un nuevo libro en bd, sino qe "se crea un objeto tipo libro"
            // pq entity framework necesita una instancia para poder accionar
            context.Remove(new Libro(){Id = id});

            await context.SaveChangesAsync();

            // nota: al borrar un libro se borraran sus dependecias:
            // comentarios + rows en AutorLibro

            return Ok();


        }
    }
}