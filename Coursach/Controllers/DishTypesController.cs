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
    public class DishTypesController : Controller
    {
        private MenuUnitEntities db = new MenuUnitEntities();

        // GET: DishTypes
        public ActionResult Index()
        {
            return View(db.DishTypes.ToList());
        }

        // GET: DishTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DishTypes dishTypes = db.DishTypes.Find(id);
            if (dishTypes == null)
            {
                return HttpNotFound();
            }
            return View(dishTypes);
        }

        // GET: DishTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DishTypes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] DishTypes dishTypes)
        {
            if (ModelState.IsValid)
            {
                db.DishTypes.Add(dishTypes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dishTypes);
        }

        // GET: DishTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DishTypes dishTypes = db.DishTypes.Find(id);
            if (dishTypes == null)
            {
                return HttpNotFound();
            }
            return View(dishTypes);
        }

        // POST: DishTypes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] DishTypes dishTypes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dishTypes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dishTypes);
        }

        // GET: DishTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DishTypes dishTypes = db.DishTypes.Find(id);
            if (dishTypes == null)
            {
                return HttpNotFound();
            }
            return View(dishTypes);
        }

        // POST: DishTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DishTypes dishTypes = db.DishTypes.Find(id);
            db.DishTypes.Remove(dishTypes);
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
