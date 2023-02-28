using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webAPIAuthors.DTOs;
using webAPIAuthors.Entidades;

namespace webAPIAuthors.Controllers
{   
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")] // la ruta es DEPENDIENTE de LIBRO
    public class ComentariosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ComentariosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId){

            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if(!existeLibro){
                return NotFound(); // 404
            }
            
            // cargar listado de comentarios correspondientes al libroId pasado por parametro
            var comentarios = await context.Comentarios.
                              Where(comentarioDb=> comentarioDb.LibroId == libroId).ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id:int}", Name = "obtenerComentario")]
        public async Task<ActionResult<ComentarioDTO>> GetById(int id){

            var comentario = await context.Comentarios.FirstOrDefaultAsync(comentarioDb => comentarioDb.Id == id);

            if (comentario == null) { return NotFound();}

            return mapper.Map<ComentarioDTO>(comentario);

        }

        //! POST comentario
        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO){
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

            if(!existeLibro){
                return NotFound(); // 404
            }

            // se agrega el contenido a la instancia comentario
            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            
            // se agrega el id del libro comentado a la instancia comentario
            comentario.LibroId = libroId;

            // agregar y guardar en bd
            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("obtenerComentario", new {id = comentario.Id, libroId = libroId }, comentarioDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(ComentarioCreacionDTO comentarioCreacionDTO, int id, int libroId){

            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if(!existeLibro){ return NotFound();}

            var existeComentario = await context.Comentarios.AnyAsync(x => x.Id == id);
            if(!existeComentario){ return NotFound();}

            //? actualizacion de datos
            //? map solo mapeara el prop contenido, 
            //? las otras props seran asignadas mediante los args pasados x params
            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);   
            comentario.Id = id; // asignacion id pasado x params
            comentario.LibroId = libroId; // asignacion libroId pasado x params

            context.Update(comentario);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}