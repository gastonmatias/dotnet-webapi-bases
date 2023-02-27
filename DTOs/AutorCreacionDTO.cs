using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using webAPIAuthors.Validaciones;

namespace webAPIAuthors.DTOs
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage ="El campo {0} es requerido")]  /* {0} es luego traducido como el nombre de la propiedad referenciada */
        [PrimeraLetraMayuscula] /* validacion personalizada */
        [StringLength(maximumLength:50,ErrorMessage="El campo {0} NO debe tener más de {1} caracteres")]
        public string Nombre { get; set; }
    }
}