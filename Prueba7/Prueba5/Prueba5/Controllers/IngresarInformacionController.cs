using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prueba5.Models;

namespace Prueba5.Controllers
{
    public class IngresarInformacionController : Controller
    {
        public ActionResult CrearInfo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CrearInfo(IngresarInformacion model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                int largo = model.ParaderoRecorrido.Split(' ').Length;
                if (largo < 2)
                {
                    ModelState.AddModelError("", "Hubieron errores en el formato");
                    return View(model);
                }
                string Paradero = model.ParaderoRecorrido.Split(' ')[0].ToUpper();
                string Recorrido = model.ParaderoRecorrido.Split(' ')[1].ToUpper();
                string Estado=null;
                if(largo==3)
                Estado= model.ParaderoRecorrido.Split(' ')[2].ToUpper();
                bool RecorridoParaderoExiste = false;
                bool RecorridoExiste = false;
                bool ParaderoExiste = false;
                int id_p=-1, id_r=-1;
                TranSapoContext db = new TranSapoContext();

                //Existe el paradero
                var p_existe = from Paradero p in db.Paradero
                               where p.codigo.ToUpper() == Paradero
                               select p;
                foreach (Paradero p in p_existe)
                {
                    ParaderoExiste = true;
                    id_p = p.ID;
                }

                //Existe el recorrido
                var r_existe = from Recorrido r in db.recorrido
                               where r.numero.ToUpper() == Recorrido
                               select r;
                foreach (Recorrido r in r_existe)
                {
                    RecorridoExiste = true;
                    id_r = r.ID;
                }

                //Existe la relación Paradero-Recorrido
                var rp_existe = from RecorridosParadero rp in db.recorridosParadero
                                where rp.Paradero.codigo.ToUpper() == Paradero &&
                                rp.Recorrido.numero.ToUpper() == Recorrido
                                select rp;
                foreach (RecorridosParadero rp in rp_existe)
                    RecorridoParaderoExiste = true;

                if (RecorridoParaderoExiste)
                {
                    int estado = 0;
                    switch (Estado)
                    {
                        case "PA":
                            estado = 1;
                            break;
                        case "V":
                            estado = 2;
                            break;
                        case "LL":
                            estado = 3;
                            break;
                        case "D":
                            estado = 4;
                            break;
                        case "PR":
                            estado = 5;
                            break;
                        default:
                            estado = 2;
                            break;
                    }

                    Informacion tsInfo= new Informacion();
                    tsInfo.ParaderoID = id_p;
                    tsInfo.RecorridoID = id_r;
                    tsInfo.EstadoID = estado;
                    tsInfo.fecha = DateTime.Now;

                    db.Informaciones.Add(tsInfo);
                    db.SaveChanges();

                    //Cambiar esta vista por algo mejor como "Gracias por tu aporte"
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //Algo no funciona bien, lanzar mensaje de error
                    if (!ParaderoExiste)
                    {
                        ModelState.AddModelError("", "El Paradero no Existe");
                    }
                    if (!RecorridoExiste)
                    {
                        ModelState.AddModelError("", "El Recorrido no Existe");
                    }
                    if (ParaderoExiste && RecorridoExiste)
                    {
                        ModelState.AddModelError("", "El Paradero especificado no está relacionado con el recorrido");
                    }
                    
                }
            }
            return View(model);
        }

        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Ingresar()
        {
            return View();
        }
    }
}
