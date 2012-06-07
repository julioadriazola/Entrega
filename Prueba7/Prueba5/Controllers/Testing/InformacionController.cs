using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Prueba5.Models;

namespace Prueba5.Controllers
{ 
    public class InformacionController : Controller
    {
        private TranSapoContext db = new TranSapoContext();

        //
        // GET: /Informacion/

        public ViewResult Index()
        {
            var informaciones = db.Informaciones.Include(i => i.Estado).Include(i => i.Paradero).Include(i => i.Recorrido);
            return View(informaciones.ToList());
        }

        //
        // GET: /Informacion/Details/5

        public ViewResult Details(int id)
        {
            Informacion informacion = db.Informaciones.Find(id);
            return View(informacion);
        }

        //
        // GET: /Informacion/Create

        public ActionResult Create()
        {
            ViewBag.EstadoID = new SelectList(db.Estados, "ID", "NombreEstado");
            ViewBag.ParaderoID = new SelectList(db.Paradero, "ID", "codigo");
            ViewBag.RecorridoID = new SelectList(db.recorrido, "ID", "numero");
            return View();
        } 

        //
        // POST: /Informacion/Create

        [HttpPost]
        public ActionResult Create(Informacion informacion)
        {
            if (ModelState.IsValid)
            {
                db.Informaciones.Add(informacion);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.EstadoID = new SelectList(db.Estados, "ID", "NombreEstado", informacion.EstadoID);
            ViewBag.ParaderoID = new SelectList(db.Paradero, "ID", "codigo", informacion.ParaderoID);
            ViewBag.RecorridoID = new SelectList(db.recorrido, "ID", "numero", informacion.RecorridoID);
            return View(informacion);
        }
        
        //
        // GET: /Informacion/Edit/5
 
        public ActionResult Edit(int id)
        {
            Informacion informacion = db.Informaciones.Find(id);
            ViewBag.EstadoID = new SelectList(db.Estados, "ID", "NombreEstado", informacion.EstadoID);
            ViewBag.ParaderoID = new SelectList(db.Paradero, "ID", "codigo", informacion.ParaderoID);
            ViewBag.RecorridoID = new SelectList(db.recorrido, "ID", "numero", informacion.RecorridoID);
            return View(informacion);
        }

        //
        // POST: /Informacion/Edit/5

        [HttpPost]
        public ActionResult Edit(Informacion informacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(informacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EstadoID = new SelectList(db.Estados, "ID", "NombreEstado", informacion.EstadoID);
            ViewBag.ParaderoID = new SelectList(db.Paradero, "ID", "codigo", informacion.ParaderoID);
            ViewBag.RecorridoID = new SelectList(db.recorrido, "ID", "numero", informacion.RecorridoID);
            return View(informacion);
        }

        //
        // GET: /Informacion/Delete/5
 
        public ActionResult Delete(int id)
        {
            Informacion informacion = db.Informaciones.Find(id);
            return View(informacion);
        }

        //
        // POST: /Informacion/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Informacion informacion = db.Informaciones.Find(id);
            db.Informaciones.Remove(informacion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}