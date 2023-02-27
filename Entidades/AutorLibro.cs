using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webAPIAuthors.Entidades
{
    // clase destinada a regular la relacion n:n entre autores y libros
    public class AutorLibro
    {
        /*  como el Id para esta tabla será una combinacion entre
            autorid + libroid, se debera ocupar la utilidad de fluent api
            de entity framework. se debe configurar en applicationDbContext*/
        public int LibroId { get; set; }
        public int AutorId { get; set; }
        public int Orden { get; set; } // para determinar orden los autores de un libro

        // props de navegacion
        public Libro Libro { get; set; }
        public Autor Autor { get; set; }
    }
}

