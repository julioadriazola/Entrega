using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Prueba5.Models
{
    public class IngresarInformacion
    {
        //Formulario de ingreso
        [Required(ErrorMessage="Se te olvidó ingresar el Paradero y Recorrido")]
        //(Recorrido|Paradero|Estado){1}( Recorrido| Paradero| Estado| ){1,5}
        [RegularExpression("([A-Za-z]{0,1}[0-9]{2,3}[ec]{0,1}|[A-Za-z]{1,2}[0-9]{2,4}|LL|Ll|lL|ll|PR|Pr|pR|pr|PA|Pa|pA|pa|D|d|v|V){1}( [A-Za-z]{0,1}[0-9]{2,3}[ec]{0,1}| [A-Za-z]{1,2}[0-9]{2,4}| LL| Ll| lL| ll| PR| Pr| pR| pr| PA| Pa| pA| pa| D| d| v| V| ){0,10}", ErrorMessage = "Debes seguir el formato")]
        [Display(Name="Ingreso y Búsqueda de Información",Description="Description")]   
        [StringLength(20, ErrorMessage="No se puede sobrepasar el largo máximo de 20 caracteres")]
        public virtual string ParaderoRecorrido { get; set; }

       
        //Para el tratamiento de una búsqueda
        public IQueryable<Paradero> Paraderos { get; set; }
        public IQueryable<Recorrido> Recorridos { get; set; }
        public RecorridosParadero RPIngresado { get; set; }
        public Paradero ParaderoIngresado { get; set; }
        public Recorrido RecorridoIngresado { get; set; }
        public List<ResultadoBusqueda> ResultadoBusqueda { get; set; }
    }

    public class ResultadoBusqueda
    {
        public string Recorrido { get; set; }
        public int Lejania { get; set; }
        public string NombreEstado { get; set; }
        public DateTime Fecha { get; set; }
        public ResultadoBusqueda(string recorrido, int lejania, string nombreestado, DateTime fecha)
        {
            Recorrido = recorrido;
            Lejania = lejania;
            NombreEstado = nombreestado;
            Fecha = fecha;
        }
    }

}