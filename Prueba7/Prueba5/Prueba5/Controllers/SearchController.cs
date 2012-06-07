using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prueba5.Logica;

namespace Prueba5.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Results(string query)
        {
            Busqueda b = new Busqueda(query);
            ViewBag.query = query; // sirve para mostrar la query en la vista. Aunque ahora no lo hago
            return View(b);
        }
    }
}
