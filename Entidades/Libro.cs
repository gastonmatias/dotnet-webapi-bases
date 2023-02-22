using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webAPIAuthors.Validaciones;
// using webAPIAutores.Entidades;

namespace webAPIAuthors.Entidades
{
    public class Libro
    {
        public int Id { get; set; }
        
        [PrimeraLetraMayuscula] /* validacion personalizada */
        public string Titulo { get; set; }
        public int AutorId { get; set; }
        public Autor Autor { get; set; }
    }
}