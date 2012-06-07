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
    public class EstadoController : Controller
    {
        private TranSapoContext db = new TranSapoContext();

        //
        // GET: /Estado/

        public ViewResult Index()
        {
            return View(db.Estados.ToList());
        }

        //
        // GET: /Estado/Details/5

        public ViewResult Details(int id)
        {
            Estado estado = db.Estados.Find(id);
            return View(estado);
        }

        //
        // GET: /Estado/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Estado/Create

        [HttpPost]
        public ActionResult Create(Estado estado)
        {
            if (ModelState.IsValid)
            {
                db.Estados.Add(estado);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(estado);
        }
        
        //
        // GET: /Estado/Edit/5
 
        public ActionResult Edit(int id)
        {
            Estado estado = db.Estados.Find(id);
            return View(estado);
        }

        //
        // POST: /Estado/Edit/5

        [HttpPost]
        public ActionResult Edit(Estado estado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estado).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estado);
        }

        //
        // GET: /Estado/Delete/5
 
        public ActionResult Delete(int id)
        {
            Estado estado = db.Estados.Find(id);
            return View(estado);
        }

        //
        // POST: /Estado/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Estado estado = db.Estados.Find(id);
            db.Estados.Remove(estado);
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