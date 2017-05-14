using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using Coursach;

namespace Coursach.Controllers
{
    public class MenuCompositionsController : Controller
    {
        private MenuUnitEntities db = new MenuUnitEntities();

        
        public ActionResult Generate([Bind(Include = "Id,Menu,DishComposition")] MenuComposition menuC)
        {
            foreach (DishTypes dishType in db.DishTypes)
            {
                Dish dish;

                MenuComposition menuComposition = new MenuComposition
                {
                    Id = 20,
                    Menu = menuC.Id,
                    DishComposition = db.Dish.OrderBy(m=>m.Frequency).First(n=>n.Type==dishType.Id).DishComposition.First().Id
                };
                if (ModelState.IsValid)
                {
                    db.MenuComposition.Add(menuComposition);
                    dish = db.Dish.Find(menuComposition.DishComposition1.Dish1.Id);
                    dish.Frequency++;
                    db.Entry(dish).State = EntityState.Modified;
                }
            }
            db.SaveChanges();

            return RedirectToAction("ShowWhere", "MenuCompositions", routeValues: new { id =menuC.Id});
        }

        // GET: MenuCompositions
        public ActionResult Index()
        {
            var menuComposition = db.MenuComposition.Include(m => m.DishComposition1);
            return View(menuComposition.ToList());
        }

        public ActionResult ShowWhere(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var menuComposition = db.MenuComposition.Include(m => m.DishComposition1).Where(m => m.Menu == id);
            return View(menuComposition.ToList());
        }

        // GET: MenuCompositions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuComposition menuComposition = db.MenuComposition.Find(id);
            if (menuComposition == null)
            {
                return HttpNotFound();
            }
            return View(menuComposition);
        }

        // GET: MenuCompositions/Create
        public ActionResult Create()
        {
            ViewBag.DishComposition = new SelectList(db.DishComposition, "Id", "Id");
            return View();
        }

        // POST: MenuCompositions/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Menu,DishComposition")] MenuComposition menuComposition)
        {
            if (ModelState.IsValid)
            {
                db.MenuComposition.Add(menuComposition);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DishComposition = new SelectList(db.DishComposition, "Id", "Id", menuComposition.DishComposition);
            return View(menuComposition);
        }

        // GET: MenuCompositions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuComposition menuComposition = db.MenuComposition.Find(id);
            if (menuComposition == null)
            {
                return HttpNotFound();
            }
            ViewBag.DishComposition = new SelectList(db.DishComposition, "Id", "Id", menuComposition.DishComposition);
            return View(menuComposition);
        }

        // POST: MenuCompositions/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Menu,DishComposition")] MenuComposition menuComposition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(menuComposition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DishComposition = new SelectList(db.DishComposition, "Id", "Id", menuComposition.DishComposition);
            return View(menuComposition);
        }

        // GET: MenuCompositions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MenuComposition menuComposition = db.MenuComposition.Find(id);
            if (menuComposition == null)
            {
                return HttpNotFound();
            }
            return View(menuComposition);
        }

        // POST: MenuCompositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MenuComposition menuComposition = db.MenuComposition.Find(id);
            db.MenuComposition.Remove(menuComposition);
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