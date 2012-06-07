using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Prueba5.Models
{
    public class IngresarInformacion
    {
        [Required(ErrorMessage="Se te olvidó ingresar el Paradero y Recorrido")]
        //[ LL| Ll| lL| ll| PR| Pr| pR| pr| PA| Pa| pA| pa| D| d]
        [RegularExpression("[A-Za-z]{1,2}[0-9]{2,4} [A-Za-z]{0,1}[0-9]{2,3}[ec]{0,1}( LL| Ll| lL| ll| PR| Pr| pR| pr| PA| Pa| pA| pa| D| d| v| V){0,1}", ErrorMessage = "Debes seguir el formato")]
        [Display(Name="Ingreso de Información de TranSapo",Description="Description")]
        public virtual string ParaderoRecorrido { get; set; }
    }
}