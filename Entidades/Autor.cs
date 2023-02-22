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
    public class Autor: IValidatableObject
    {
        public int Id { get; set; }

        // para qe al momento de crear un autor, sea exigido el campo nombre en la peticion POST
        [Required(ErrorMessage ="El campo {0} es requerido")]  /* {0} es luego traducido como el nombre de la propiedad referenciada */
        // [PrimeraLetraMayuscula] /* validacion personalizada */
        [StringLength(maximumLength:10,ErrorMessage="El campo {0} NO debe tener más de {1} caracteres")]
        public string Nombre { get; set; }
        
        // [Range(18,120)]
        // [NotMapped]
        // public int Edad { get; set; }

        // [CreditCard]
        // [NotMapped]
        // public int TarjetaCredito { get; set; }

        // [Url]
        // [NotMapped]
        // public string URL { get; set; }
        public List<Libro> Libros { get; set; }

        // [NotMapped]
        // public int MenorTest { get; set; }
        // [NotMapped]
        // public int MayorTest { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(!string.IsNullOrEmpty(Nombre)){
                var primeraLetra = Nombre[0].ToString();
                
                if(primeraLetra != primeraLetra.ToUpper()){
                    /* yield para ir llenado la coleccion de errores */
                    yield return new ValidationResult("La primera letra debe ser mayúscula", 
                        new string[]{nameof(Nombre)});/* esto para retornar el elemento en conflicto */
                }
            }

            // if (MenorTest > MayorTest)
            // {
            //     yield return new ValidationResult("Este valor NO puede ser más grande que el campo mayor",
            //          new string[]{ nameof(MenorTest) });
            // }
        }
    }
}

/* La instrucción se usa yield en un iterador para proporcionar el siguiente valor de una secuencia 
   al iterar la secuencia. La yield instrucción tiene las dos formas siguientes:
    - yield return
    - yield break
*/