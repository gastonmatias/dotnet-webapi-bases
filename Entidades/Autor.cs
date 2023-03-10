using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using webAPIAuthors.Entidades;
using webAPIAuthors.Validaciones;

// namespace webAPIAutores.Entidades
namespace webAPIAuthors.Entidades
{
    public class Autor
    {
        public int Id { get; set; }

        // para qe al momento de crear un autor, sea exigido el campo nombre en la peticion POST
        [Required(ErrorMessage ="El campo {0} es requerido")]  /* {0} es luego traducido como el nombre de la propiedad referenciada */
        [PrimeraLetraMayuscula] /* validacion personalizada */
        [StringLength(maximumLength:50,ErrorMessage="El campo {0} NO debe tener más de {1} caracteres")]
        public string Nombre { get; set; }

        // props de navegacion  
        public List<AutorLibro> AutoresLibros { get; set; }
     
    }
}