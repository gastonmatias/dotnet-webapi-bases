using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibroDTO>> Get(int id){
            // FirstOrDefaultAsync : 1er registro qe coincida con la condicion
            var libro =  await context.Libros
                            //   .Include(libroBd => libroBd.Comentarios) //para incluir comentarios a momento de GET libro
                              .FirstOrDefaultAsync(x => x.Id == id);
            return mapper.Map<LibroDTO>(libro);
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

            if(libro.AutoresLibros != null){
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }

            context.Add(libro);
            await context.SaveChangesAsync();

            return Ok();

        }
    }
}