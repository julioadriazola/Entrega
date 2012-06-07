using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Prueba5.Models
{
    public class Informacion
    {
            public virtual int ID { get; set; }
            public virtual Estado Estado { get; set; }
            public virtual int EstadoID { get; set; }
            public virtual DateTime fecha { get; set; }
            public virtual Paradero Paradero { get; set; }
            public virtual int ParaderoID { get; set; }
            public virtual Recorrido Recorrido { get; set; }
            public virtual int RecorridoID { get; set; }
            //public virtual Cuenta Cuenta { get; set; }
            //public virtual int CuentaID { get; set; }
        
    }
}