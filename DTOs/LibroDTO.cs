using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webAPIAuthors.DTOs
{
    public class LibroDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaPublicacion { get; set; }
    }
}