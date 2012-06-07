using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Prueba5.Models
{

    public class Estado
    {
        public virtual int ID { get; set; }
        public virtual string NombreEstado { get; set; }
    }

        public class TranSapoContext : DbContext
        {
            public DbSet<Estado> Estados { get; set; }
            public DbSet<Recorrido> recorrido { get; set; }
            public DbSet<Paradero> Paradero { get; set; }
            public DbSet<RecorridosParadero> recorridosParadero { get; set; }

            public DbSet<Informacion> Informaciones { get; set; }
            //public DbSet<Comentario> comentario { get; set; }
            //public DbSet<ComentarioInformacion> comentarioInformacion { get; set; }
            //public DbSet<CallesParadero> callesParadero { get; set; }
            public DbSet<Cuenta> Cuentas { get; set; }
            
        }
    
}