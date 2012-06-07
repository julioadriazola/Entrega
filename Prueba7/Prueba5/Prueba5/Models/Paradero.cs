using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Prueba5.Models
{
    public class Paradero
    {
        public virtual int ID { get; set; }
        public virtual string codigo { get; set; }
    }
}