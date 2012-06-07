using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prueba5.Models;

namespace Prueba5.Controllers
{
    public class HomeController : Controller
    {

        public static List<string> Mensajes = new List<string>();


        // GET: /Home/Index
        public ActionResult Index()
        {
            return View();
        }

        // POST: /Home/Index
        [HttpPost]
        public ActionResult Index(IngresarInformacion model, string button)
        {
            if (button == "Ingresar")
            {
                return CrearInfo(model);
            }
            if (button == "Buscar")
            {
                return Resultado(model);
            }
            return View(model);
        }

        // GET: /Home/Index1
        /// <summary>
        /// Retorna la vista post- Ingreso de información exitosa (Con Mensaje "Gracias...")
        /// </summary>
        public ActionResult Index1()
        {
            return View();
        }

        /// <summary>
        /// Entrega la vista cuando una consulta se hace de la forma /Busqueda/{query}
        /// </summary>
        /// <param name="query">La consulta requerida</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Resultado(string query)
        {
            IngresarInformacion model=new IngresarInformacion();
            model.ParaderoRecorrido=query;
            return Resultado(model);
        }

        #region Métodos privados



        /// <summary>
        /// Entrega la vista post- Consulta de información
        /// </summary>
        private ActionResult Resultado(IngresarInformacion model)
        {
            if (ModelState.IsValid)
            {
                TranSapoContext db = new TranSapoContext();
                List<string> ListaParametros = StringToUpperList(model.ParaderoRecorrido);
                IQueryable<Paradero> Paraderos = ObtenerParaderos(ListaParametros, db);
                IQueryable<Recorrido> Recorridos = ObtenerRecorridos(ListaParametros, db);
                IQueryable<RecorridosParadero> RecorridosParaderos = ObtenerRecorridoParadero(ListaParametros, db);
                bool ParaderoExiste = false; Paradero paradero = null;
                bool RecorridoExiste = false; Recorrido recorrido = null;
                bool RecorridoParaderoExiste = false; RecorridosParadero recorridoParadero = null;

                foreach (Paradero p in Paraderos)
                {
                    ParaderoExiste = true;
                    paradero = p; break;
                }
                foreach (Recorrido r in Recorridos)
                {
                    RecorridoExiste = true;
                    recorrido = r; break;
                }
                foreach (RecorridosParadero rp in RecorridosParaderos)
                {
                    RecorridoParaderoExiste = true;
                    recorridoParadero = rp; break;
                }
                model.Paraderos = ObtenerParaderos(recorrido, db);
                model.RecorridoIngresado = recorrido;
                model.Recorridos = ObtenerRecorridos(paradero, db);
                model.ParaderoIngresado = paradero;
                model.RPIngresado = recorridoParadero;
                model.ResultadoBusqueda = ResultadoBusqueda(model, db);
                if (!RecorridoParaderoExiste & (ParaderoExiste & RecorridoExiste))
                {
                    ModelState.AddModelError("", "El Paradero y Recorrido especificados No están Asociados. Seleccione alguna de nuestras sugerencias");
                }
                if (!RecorridoParaderoExiste & ParaderoExiste & !RecorridoExiste)
                {
                    ModelState.AddModelError("", "El Recorrido No existe o No se especificó. Seleccione uno de la lista");
                }
                if (!RecorridoParaderoExiste & !ParaderoExiste & RecorridoExiste)
                {
                    ModelState.AddModelError("", "El Paradero No existe o No se especificó. Seleccione uno de la lista");
                }
                if (!RecorridoParaderoExiste & !ParaderoExiste & !RecorridoExiste)
                {
                    ModelState.AddModelError("", "El Paradero y Recorrido especificados No existen. Inténtelo de Nuevo");
                }
            }

            return View("Resultado", model);
        }

        /// <summary>
        /// Entrega los Resultados de Búsqueda asociados al modelo
        /// </summary>
        private List<ResultadoBusqueda> ResultadoBusqueda(IngresarInformacion model, TranSapoContext db)
        {
            if (model.RPIngresado == null)
                return null;
            //Aqui definir delta (Cuántos se mostrarán para atrás y para adelantes)
            int delta = 2;
            int min = model.RPIngresado.NumeroParada - delta;
            int max = model.RPIngresado.NumeroParada + delta;
            int UltimaParada, PrimeraParada;

            var recorridosParaderos = from RecorridosParadero rp in db.recorridosParadero.OrderBy(rp => rp.NumeroParada)
                                      where rp.Recorrido.numero == model.RecorridoIngresado.numero
                                      select rp.NumeroParada;

            PrimeraParada = recorridosParaderos.Min();
            UltimaParada = recorridosParaderos.Max();


            List<ResultadoBusqueda> QueryResultado = new List<ResultadoBusqueda>();
            //Suponiendo que AL MENOS hay 3 paraderos (Sino no va a funcionar mal D:)
            if (max > UltimaParada)
            {
                for (int i = min; i <= min + 2 * delta; i++)
                {
                    QueryResultado = QueryResultado.Union(InformacionReciente(i % (UltimaParada + 1), 5, model, db, UltimaParada)).ToList();
                }
            }
            else if (min < 0)
            {
                int aux = UltimaParada + min + 1;
                for (int i = aux; i <= aux + 2 * delta; i++)
                {
                    QueryResultado = QueryResultado.Union(InformacionReciente(i % (UltimaParada + 1), 5, model, db, UltimaParada)).ToList();
                }
            }
            else
            {
                for (int i = min; i <= max; i++)
                {
                    QueryResultado = QueryResultado.Union(InformacionReciente(i % (UltimaParada + 1), 5, model, db, UltimaParada)).ToList();
                }
            }

            // List<ResultadoBusqueda> resultado = new List<ResultadoBusqueda>();
            //foreach (var q in QueryResultado)
            //  resultado.Add(new ResultadoBusqueda(q.Recorrido, q.Lejania, q.NombreEstado, q.Fecha));
            return QueryResultado;
        }

        /// <summary>
        /// Entrega la información ingresada (en los últimos 45 minutos) asociada a la posición @posicion asociada al paradero en el recorrido especificado en el modelo @model
        /// </summary>
        /// <param name="posicion">Posición de la parada en el Recorrido de interés</param>
        /// <param name="limite">Cuántas tuplas se devolverán como máximo</param>
        private List<ResultadoBusqueda> InformacionReciente(int posicion, int limite, IngresarInformacion model, TranSapoContext db, int max)
        {
            var informaciones = from Informacion i in db.Informaciones.OrderBy(i => i.fecha)
                                where i.Recorrido.numero == model.RecorridoIngresado.numero //&& ( i.fecha <= DateTime.Now.Add( new TimeSpan(0,45,0)) && i.fecha.Add(new TimeSpan(0,45,0)) >= DateTime.Now)                               
                                select i;
            var recorridoparadero = from RecorridosParadero rp in db.recorridosParadero
                                    where rp.Recorrido.numero == model.RecorridoIngresado.numero & rp.NumeroParada == posicion
                                    select rp;
            int _lejania = Lejania(posicion, model.RPIngresado.NumeroParada, max);
            var query = informaciones.AsQueryable().Join(recorridoparadero, i => i.Paradero, rc => rc.Paradero, (i, rc) => new { Recorrido = i.Recorrido.numero, Fecha = i.fecha, NombreEstado = i.Estado.NombreEstado, Lejania = _lejania });

            int count = 0;
            List<ResultadoBusqueda> ListaResultados = new List<ResultadoBusqueda>();
            foreach (var q in query)
            {
                if (count >= limite)
                    break;
                ListaResultados.Add(new ResultadoBusqueda(q.Recorrido, q.Lejania, q.NombreEstado, q.Fecha));
                count++;
            }
            return ListaResultados;
        }

        private int Lejania(int posicionParadaActual, int posicionParadaInteres, int valorMaximoParada)
        {
            int a = posicionParadaInteres - posicionParadaActual;
            int b = valorMaximoParada + 1 - Math.Abs(a);
            if (a < 0)
            {
                b = Math.Abs(b);
            }
            if (a > 0)
            {
                b = -Math.Abs(b);
            }

            /*if (a == 0)
            {
                return 0;
            }*/
            if (Math.Abs(a) < Math.Abs(b))
            {
                return a;
            }
            else if (Math.Abs(a) > Math.Abs(b))
            {
                return b;
            }
            else
                return 0;
        }


        /// <summary>
        /// Entrega una lista con los valores de input.Split(' ') en mayúsculas
        /// </summary>
        private List<string> StringToUpperList(string input)
        {
            List<string> Lista=new List<string>();
            if (input == null)
            {
                return Lista;
            }

            foreach(string txt in input.Split(' '))
            {
                Lista.Add(txt.ToUpper());
            }
            return Lista;
        }

        /// <summary>
        /// Entrega todos los paraderos que se encuentren en @ListaParametros
        /// </summary>
        private IQueryable<Paradero> ObtenerParaderos(List<string> ListaParametros,TranSapoContext db)
        {
            var p_existe = from Paradero p in db.Paradero
                           where ListaParametros.Contains(p.codigo.ToUpper())
                           select p;
            return p_existe;
        }

        /// <summary>
        /// Entrega todos los paraderos que están asociados al @recorrido, ordenados por el numero de la parada
        /// </summary>
        private IQueryable<Paradero> ObtenerParaderos(Recorrido recorrido,TranSapoContext db)
        {
            if (recorrido != null)
            {
                var query = from rp in db.recorridosParadero.OrderBy(r => r.NumeroParada)
                            where rp.Recorrido.numero == recorrido.numero
                            select rp.Paradero;
                return query;
            }
            return null;
        }

        /// <summary>
        /// Entrega todos los recorridos encontrados en @ListaParametros
        /// </summary>
        private IQueryable<Recorrido> ObtenerRecorridos(List<string> ListaParametros,TranSapoContext db)
        {
            var r_existe = from Recorrido r in db.recorrido
                               where ListaParametros.Contains(r.numero.ToUpper())
                               select r;
            return r_existe;
        }

        /// <summary>
        /// Entrega todos los recorridos que están asociados al @paradero
        /// </summary>
        private IQueryable<Recorrido> ObtenerRecorridos(Paradero paradero, TranSapoContext db)
        {
            if (paradero != null)
            {
                var query = from rp in db.recorridosParadero
                            where rp.Paradero.codigo == paradero.codigo
                            select rp.Recorrido;
                return query;
            }
            return null;
        }
        
        /// <summary>
        /// Entrega los Estados encontrados en @ListaParametros
        /// </summary>
        private int ObtenerIdEstado(List<string> ListaParametros)
        {
            int estado = 2;
            bool encontrado = false;
            foreach (string Estado in ListaParametros)
            {
                if (!encontrado)
                {
                    switch (Estado)
                    {
                        case "PA":
                            estado = 1;
                            encontrado = true;
                            break;
                        case "V":
                            estado = 2;
                            encontrado = true;
                            break;
                        case "LL":
                            estado = 3;
                            encontrado = true;
                            break;
                        case "D":
                            estado = 4;
                            encontrado = true;
                            break;
                        case "PR":
                            estado = 5;
                            encontrado = true;
                            break;
                        default:
                            estado = 2;
                            break;
                    }
                }
            }
            return estado;
        }

        /// <summary>
        /// Entrega todos los pares Recorrido - Paradero que están asociados, encontrados en la Lista
        /// </summary>
        private IQueryable<RecorridosParadero> ObtenerRecorridoParadero(List<string> ListaParametros, TranSapoContext db)
        {
            var rp_existe = from RecorridosParadero rp in db.recorridosParadero
                            where ListaParametros.Contains(rp.Paradero.codigo.ToUpper()) &&
                            ListaParametros.Contains(rp.Recorrido.numero.ToUpper())
                            select rp;
            return rp_existe;
        }

        /// <summary>
        /// Entrega la vista para post- Ingreso de información
        /// </summary>
        private ActionResult CrearInfo(IngresarInformacion model)
        {
            if (ModelState.IsValid)
            {
                List<string> ListaParametros = StringToUpperList(model.ParaderoRecorrido);

                bool RecorridoParaderoExiste = false;
                bool RecorridoExiste = false;
                bool ParaderoExiste = false;
                int id_p = -1, id_r = -1;
                TranSapoContext db = new TranSapoContext();

                var p_existe = ObtenerParaderos(ListaParametros, db);
                foreach (Paradero p in p_existe)
                {
                    ParaderoExiste = true;
                    id_p = p.ID; break;
                }

                var r_existe = ObtenerRecorridos(ListaParametros, db);
                foreach (Recorrido r in r_existe)
                {
                    RecorridoExiste = true;
                    id_r = r.ID; break;
                }

                var rp_existe = ObtenerRecorridoParadero(ListaParametros, db);
                foreach (RecorridosParadero rp in rp_existe)
                { RecorridoParaderoExiste = true; break; }

                if (RecorridoParaderoExiste)
                {
                    int estado = ObtenerIdEstado(ListaParametros);
                    Informacion tsInfo = new Informacion();
                    tsInfo.ParaderoID = id_p;
                    tsInfo.RecorridoID = id_r;
                    tsInfo.EstadoID = estado;
                    tsInfo.fecha = DateTime.Now;

                    db.Informaciones.Add(tsInfo);
                    db.SaveChanges();

                    HomeController.Mensajes.Add("¡Gracias por tu Aporte!");
                    return RedirectToAction("Index");
                }
                else
                {
                    //Algo no funciona bien, lanzar mensaje de error
                    if (!ParaderoExiste)
                    {
                        ModelState.AddModelError("", "No se encontró el paradero");
                    }
                    if (!RecorridoExiste)
                    {
                        ModelState.AddModelError("", "No se encontró el recorrido");
                    }
                    if (ParaderoExiste && RecorridoExiste)
                    {
                        ModelState.AddModelError("", "El Paradero especificado no está relacionado con el recorrido");
                    }

                }
            }
            return View(model);
        }



        #endregion
    }
}
