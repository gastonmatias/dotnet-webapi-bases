using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using webAPIAuthors.Validaciones;

namespace webAPIAuthors.DTOs
{
    public class LibroPatchDTO
    {
        [PrimeraLetraMayuscula] /* validacion personalizada */
        [StringLength(maximumLength:250)]
        public string Titulo { get; set; }

        public DateTime FechaPublicacion { get; set; }
    }
}