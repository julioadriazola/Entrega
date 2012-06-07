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
    public class RecorridoController : Controller
    {
        private TranSapoContext db = new TranSapoContext();

        //
        // GET: /Recorrido/

        public ViewResult Index()
        {
            return View(db.recorrido.ToList());
        }

        //
        // GET: /Recorrido/Details/5

        public ViewResult Details(int id)
        {
            Recorrido recorrido = db.recorrido.Find(id);
            return View(recorrido);
        }

        //
        // GET: /Recorrido/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Recorrido/Create

        [HttpPost]
        public ActionResult Create(Recorrido recorrido)
        {
            if (ModelState.IsValid)
            {
                db.recorrido.Add(recorrido);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(recorrido);
        }
        
        //
        // GET: /Recorrido/Edit/5
 
        public ActionResult Edit(int id)
        {
            Recorrido recorrido = db.recorrido.Find(id);
            return View(recorrido);
        }

        //
        // POST: /Recorrido/Edit/5

        [HttpPost]
        public ActionResult Edit(Recorrido recorrido)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recorrido).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(recorrido);
        }

        //
        // GET: /Recorrido/Delete/5
 
        public ActionResult Delete(int id)
        {
            Recorrido recorrido = db.recorrido.Find(id);
            return View(recorrido);
        }

        //
        // POST: /Recorrido/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Recorrido recorrido = db.recorrido.Find(id);
            db.recorrido.Remove(recorrido);
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