using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace webAPIAuthors.Entidades
{
    public class Comentario
    {
        public int Id { get; set; }
        [Required]
        public string Contenido { get; set; }
        public int LibroId { get; set; }

        // propiedad de navegacion, permite realizar joins sencillamete
        public Libro Libro {get ; set ;} // permite cargar la data del libro si se desea


    }
}