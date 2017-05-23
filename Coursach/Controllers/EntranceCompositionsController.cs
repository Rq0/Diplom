using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Coursach;

namespace Coursach.Controllers
{
    public class EntranceCompositionsController : Controller
    {
        private MenuUnitEntities db = new MenuUnitEntities();

        // GET: EntranceCompositions
        public ActionResult Index()
        {
            var entranceComposition = db.EntranceComposition.Include(e => e.Entrance1).Include(e => e.Ingredient1);
            return View(entranceComposition.ToList());
        }

        // GET: EntranceCompositions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EntranceComposition entranceComposition = db.EntranceComposition.Find(id);
            if (entranceComposition == null)
            {
                return HttpNotFound();
            }
            return View(entranceComposition);
        }

        // GET: EntranceCompositions/Create
        public ActionResult Create()
        {
            ViewBag.Entrance = new SelectList(db.Entrance, "Id", "Id");
            ViewBag.Ingredient = new SelectList(db.Ingredient, "Id", "Name");
            return View();
        }

        // POST: EntranceCompositions/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Entrance,Ingredient,Count,Cost")] EntranceComposition entranceComposition)
        {
            if (ModelState.IsValid)
            {
                db.EntranceComposition.Add(entranceComposition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Entrance = new SelectList(db.Entrance, "Id", "Id", entranceComposition.Entrance);
            ViewBag.Ingredient = new SelectList(db.Ingredient, "Id", "Name", entranceComposition.Ingredient);
            return View(entranceComposition);
        }

        // GET: EntranceCompositions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EntranceComposition entranceComposition = db.EntranceComposition.Find(id);
            if (entranceComposition == null)
            {
                return HttpNotFound();
            }
            ViewBag.Entrance = new SelectList(db.Entrance, "Id", "Id", entranceComposition.Entrance);
            ViewBag.Ingredient = new SelectList(db.Ingredient, "Id", "Name", entranceComposition.Ingredient);
            return View(entranceComposition);
        }

        // POST: EntranceCompositions/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Entrance,Ingredient,Count,Cost")] EntranceComposition entranceComposition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entranceComposition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Entrance = new SelectList(db.Entrance, "Id", "Id", entranceComposition.Entrance);
            ViewBag.Ingredient = new SelectList(db.Ingredient, "Id", "Name", entranceComposition.Ingredient);
            return View(entranceComposition);
        }

        // GET: EntranceCompositions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EntranceComposition entranceComposition = db.EntranceComposition.Find(id);
            if (entranceComposition == null)
            {
                return HttpNotFound();
            }
            return View(entranceComposition);
        }

        // POST: EntranceCompositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EntranceComposition entranceComposition = db.EntranceComposition.Find(id);
            db.EntranceComposition.Remove(entranceComposition);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
