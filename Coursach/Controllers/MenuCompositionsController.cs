using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using Coursach;

namespace Coursach.Controllers
{
    public class MenuCompositionsController : Controller
    {
        private readonly MenuUnitEntities db = new MenuUnitEntities();


        public ActionResult Generate(int? id)
        {
            var thisDishes = db.Dish.OrderBy(m => m.Frequency).ToList();
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Генерирование меню {0}", id);
            foreach (var dishType in db.DishTypes)
            {
                bool haveRequirements = false;
                int? endOfFor;
                try
                {
                    endOfFor = db.MenuRequirement.First(m => (m.Menu == id && m.DishType == dishType.Id)).Count;
                    haveRequirements = true;
                }
                catch (Exception e)
                {
                    haveRequirements = false;
                    logger.Warn("Нет требований на тип {0} в меню {1}", dishType.Id, id);
                    endOfFor = 0;
                }
                if (endOfFor > (thisDishes.Count(m => m.Type==dishType.Id)) && haveRequirements)
                {
                    logger.Error("Недостаточно блюд типа {0} для генерации меню {1}",dishType.Name, id);
                    return RedirectToAction("Index", "Menus", routeValues: new { id = id });
                }

                for (var i = 0;
                    i < endOfFor;
                    i++)
                {
                    MenuComposition menuComposition = new MenuComposition { };
                    menuComposition.Id = 0;
                    menuComposition.Menu = id;
                    Dish dish = thisDishes.Find(m=>m.Type ==dishType.Id);

                    menuComposition.DishComposition = db.DishComposition.First(m=>m.Dish== dish.Id).Id;

                    db.MenuComposition.Add(menuComposition);
                    
                    if (dish.Frequency == null)
                    {
                        dish.Frequency = 0;
                    }
                    dish.Frequency++;
                    db.Entry(dish).State = EntityState.Modified;
                    logger.Info("Блюдо {0} добавлено в меню {1}", dish.Name, menuComposition.Menu);
                    thisDishes.Remove(dish);
                }
            }
            db.SaveChanges();

            return RedirectToAction("ShowWhere", "MenuCompositions", routeValues: new {id = id});
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