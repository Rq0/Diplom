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
    public class MenuRequirementsController : Controller
    {
        private MenuUnitEntities db = new MenuUnitEntities();

        // GET: MenuRequirements
        public ActionResult Index()
        {
            var menuRequirement = db.MenuRequirement.Include(m => m.DishTypes).Include(m => m.Menu1);
            return View(menuRequirement.ToList());
        }

        // GET: MenuRequirements/Create
        public ActionResult Create()
        {
            ViewBag.DishType = new SelectList(db.DishTypes, "Id", "Name");
            ViewBag.Menu = new SelectList(db.Menu, "Id", "Id");
            return View();
        }

        // POST: MenuRequirements/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Menu,DishType,Count")] MenuRequirement menuRequirement)
        {
            menuRequirement.Menu = menuRequirement.Id;
            if (ModelState.IsValid)
            {
                db.MenuRequirement.Add(menuRequirement);
                db.SaveChanges();
                return RedirectToAction("Create");
            }

            ViewBag.DishType = new SelectList(db.DishTypes, "Id", "Name", menuRequirement.DishType);
            ViewBag.Menu = new SelectList(db.Menu, "Id", "Id", menuRequirement.Menu);
            return View(menuRequirement);
        }

        // GET: MenuRequirements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuRequirement menuRequirement = db.MenuRequirement.Find(id);
            if (menuRequirement == null)
            {
                return HttpNotFound();
            }
            ViewBag.DishType = new SelectList(db.DishTypes, "Id", "Name", menuRequirement.DishType);
            ViewBag.Menu = new SelectList(db.Menu, "Id", "Id", menuRequirement.Menu);
            return View(menuRequirement);
        }

        // POST: MenuRequirements/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Menu,DishType,Count")] MenuRequirement menuRequirement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menuRequirement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DishType = new SelectList(db.DishTypes, "Id", "Name", menuRequirement.DishType);
            ViewBag.Menu = new SelectList(db.Menu, "Id", "Id", menuRequirement.Menu);
            return View(menuRequirement);
        }

        // GET: MenuRequirements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuRequirement menuRequirement = db.MenuRequirement.Find(id);
            if (menuRequirement == null)
            {
                return HttpNotFound();
            }
            return View(menuRequirement);
        }

        // POST: MenuRequirements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MenuRequirement menuRequirement = db.MenuRequirement.Find(id);
            db.MenuRequirement.Remove(menuRequirement);
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
