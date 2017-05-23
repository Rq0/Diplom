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
    public class DishCompositionsController : Controller
    {
        private MenuUnitEntities db = new MenuUnitEntities();

        // GET: DishCompositions
        public ActionResult Index()
        {
            var dishComposition = db.DishComposition.Include(d => d.Dish1).Include(d => d.Ingredient);
            return View(dishComposition.ToList());
        }

        // GET: DishCompositions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DishComposition dishComposition = db.DishComposition.Find(id);
            if (dishComposition == null)
            {
                return HttpNotFound();
            }
            return View(dishComposition);
        }

        // GET: DishCompositions/Create
        public ActionResult Create()
        {
            ViewBag.Dish = new SelectList(db.Dish, "Id", "Name");
            ViewBag.Ingridient = new SelectList(db.Ingredient, "Id", "Name");
            return View();
        }

        // POST: DishCompositions/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Dish,Ingridient")] DishComposition dishComposition)
        {
            if (ModelState.IsValid)
            {
                db.DishComposition.Add(dishComposition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Dish = new SelectList(db.Dish, "Id", "Name", dishComposition.Dish);
            ViewBag.Ingridient = new SelectList(db.Ingredient, "Id", "Name", dishComposition.Ingridient);
            return View(dishComposition);
        }

        // GET: DishCompositions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DishComposition dishComposition = db.DishComposition.Find(id);
            if (dishComposition == null)
            {
                return HttpNotFound();
            }
            ViewBag.Dish = new SelectList(db.Dish, "Id", "Name", dishComposition.Dish);
            ViewBag.Ingridient = new SelectList(db.Ingredient, "Id", "Name", dishComposition.Ingridient);
            return View(dishComposition);
        }

        // POST: DishCompositions/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Dish,Ingridient")] DishComposition dishComposition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dishComposition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Dish = new SelectList(db.Dish, "Id", "Name", dishComposition.Dish);
            ViewBag.Ingridient = new SelectList(db.Ingredient, "Id", "Name", dishComposition.Ingridient);
            return View(dishComposition);
        }

        // GET: DishCompositions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DishComposition dishComposition = db.DishComposition.Find(id);
            if (dishComposition == null)
            {
                return HttpNotFound();
            }
            return View(dishComposition);
        }

        // POST: DishCompositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DishComposition dishComposition = db.DishComposition.Find(id);
            db.DishComposition.Remove(dishComposition);
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
