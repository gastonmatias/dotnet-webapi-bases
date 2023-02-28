using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using webAPIAuthors.Validaciones;
// using webAPIAutores.Entidades;

namespace webAPIAuthors.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        
        [Required]
        [PrimeraLetraMayuscula] /* validacion personalizada */
        [StringLength(maximumLength:250)]
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }

        // props de navegacion, permiten joins de manera sencilla
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
    }
}