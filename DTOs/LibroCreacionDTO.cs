using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using webAPIAuthors.Validaciones;

namespace webAPIAuthors.DTOs
{
    public class LibroCreacionDTO
    {
        [PrimeraLetraMayuscula] /* validacion personalizada */
        [StringLength(maximumLength:250)]
        public string Titulo { get; set; }

        public DateTime FechaPublicacion { get; set; }
        // al crear un libro se debe enviar un array de ids de su/s autor/es
        public List<int> AutoresIds { get; set; }
        // PERO ojo, la entidad libro espera un List<AutoresLibros> para ser guardada
        // en bd, por ello a momento de POST libro, se necesitara mapear
    }
}