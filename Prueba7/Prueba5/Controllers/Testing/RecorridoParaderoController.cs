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
    public class RecorridoParaderoController : Controller
    {
        private TranSapoContext db = new TranSapoContext();

        //
        // GET: /RecorridoParadero/

        public ViewResult Index()
        {
            var recorridosparadero = db.recorridosParadero.Include(r => r.Paradero).Include(r => r.Recorrido);
            return View(recorridosparadero.ToList());
        }

        //
        // GET: /RecorridoParadero/Details/5

        public ViewResult Details(int id)
        {
            RecorridosParadero recorridosparadero = db.recorridosParadero.Find(id);
            return View(recorridosparadero);
        }

        //
        // GET: /RecorridoParadero/Create

        public ActionResult Create()
        {
            ViewBag.ParaderoID = new SelectList(db.Paradero, "ID", "codigo");
            ViewBag.RecorridoID = new SelectList(db.recorrido, "ID", "numero");
            return View();
        } 

        //
        // POST: /RecorridoParadero/Create

        [HttpPost]
        public ActionResult Create(RecorridosParadero recorridosparadero,int NumeroParada)
        {
            if (ModelState.IsValid)
            {
                db.recorridosParadero.Add(recorridosparadero);
                db.SaveChanges();
                recorridosparadero.NumeroParada = NumeroParada;
                return RedirectToAction("Index");  
            }

            ViewBag.ParaderoID = new SelectList(db.Paradero, "ID", "codigo", recorridosparadero.ParaderoID);
            ViewBag.RecorridoID = new SelectList(db.recorrido, "ID", "numero", recorridosparadero.RecorridoID);
            return View(recorridosparadero);
        }
        
        //
        // GET: /RecorridoParadero/Edit/5
 
        public ActionResult Edit(int id)
        {
            RecorridosParadero recorridosparadero = db.recorridosParadero.Find(id);
            ViewBag.ParaderoID = new SelectList(db.Paradero, "ID", "codigo", recorridosparadero.ParaderoID);
            ViewBag.RecorridoID = new SelectList(db.recorrido, "ID", "numero", recorridosparadero.RecorridoID);
            return View(recorridosparadero);
        }

        //
        // POST: /RecorridoParadero/Edit/5

        [HttpPost]
        public ActionResult Edit(RecorridosParadero recorridosparadero,int NumeroParada)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recorridosparadero).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParaderoID = new SelectList(db.Paradero, "ID", "codigo", recorridosparadero.ParaderoID);
            ViewBag.RecorridoID = new SelectList(db.recorrido, "ID", "numero", recorridosparadero.RecorridoID);
            return View(recorridosparadero);
        }

        //
        // GET: /RecorridoParadero/Delete/5
 
        public ActionResult Delete(int id)
        {
            RecorridosParadero recorridosparadero = db.recorridosParadero.Find(id);
            return View(recorridosparadero);
        }

        //
        // POST: /RecorridoParadero/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            RecorridosParadero recorridosparadero = db.recorridosParadero.Find(id);
            db.recorridosParadero.Remove(recorridosparadero);
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