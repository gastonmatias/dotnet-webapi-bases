using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webAPIAuthors.Entidades;

namespace webAPIAuthors.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController : Controller
    {
        private readonly ApplicationDbContext context;

        //! inyeccion
        public LibrosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Libro>> Get(int id){
            // FirstOrDefaultAsync : 1er registro qe coincida con la condicion
            return await context.Libros.Include(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Libro libro){
            var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
            
            if (!existeAutor)
            {
                return BadRequest($"No existe autor con Id: {libro.AutorId}");
            } 
            
            context.Add(libro);
            await context.SaveChangesAsync();

            return Ok();

        }
    }
}