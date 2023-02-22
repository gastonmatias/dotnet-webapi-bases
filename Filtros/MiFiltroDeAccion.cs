using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace webAPIAuthors.Filtros
{
    //  Recordar:
    // filtros de accion se ejecutan justo antes/despues de una accion
    public class MiFiltroDeAccion : IActionFilter
    {
        private readonly ILogger<MiFiltroDeAccion> logger;

        public MiFiltroDeAccion(ILogger<MiFiltroDeAccion> logger)
        {
            this.logger = logger;
        }

        // se ejecuta antes de ejecutar la accion
        public void OnActionExecuted(ActionExecutedContext context)
        {
            logger.LogInformation("-----ANTES de ejecutar la accion");
        }

        // se ejecuta cuando la accion ya se ha ejecutado
        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("-----DESPUES de ejecutar la accion");
        }
    }
}