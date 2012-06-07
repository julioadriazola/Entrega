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
    public class ParaderoController : Controller
    {
        private TranSapoContext db = new TranSapoContext();

        //
        // GET: /Paradero/

        public ViewResult Index()
        {
            return View(db.Paradero.ToList());
        }

        //
        // GET: /Paradero/Details/5

        public ViewResult Details(int id)
        {
            Paradero paradero = db.Paradero.Find(id);
            return View(paradero);
        }

        //
        // GET: /Paradero/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Paradero/Create

        [HttpPost]
        public ActionResult Create(Paradero paradero)
        {
            if (ModelState.IsValid)
            {
                db.Paradero.Add(paradero);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(paradero);
        }
        
        //
        // GET: /Paradero/Edit/5
 
        public ActionResult Edit(int id)
        {
            Paradero paradero = db.Paradero.Find(id);
            return View(paradero);
        }

        //
        // POST: /Paradero/Edit/5

        [HttpPost]
        public ActionResult Edit(Paradero paradero)
        {
            if (ModelState.IsValid)
            {
                db.Entry(paradero).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(paradero);
        }

        //
        // GET: /Paradero/Delete/5
 
        public ActionResult Delete(int id)
        {
            Paradero paradero = db.Paradero.Find(id);
            return View(paradero);
        }

        //
        // POST: /Paradero/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Paradero paradero = db.Paradero.Find(id);
            db.Paradero.Remove(paradero);
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