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
    public class EntrancesController : Controller
    {
        private MenuUnitEntities db = new MenuUnitEntities();
        public ActionResult Calculate(int? id)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Пересчет стоимости ингредиентов из поставки {0}", id);
            Entrance entrance = db.Entrance.FirstOrDefault(m => m.Id == id);
            if (!entrance.Recalculation)
            {
                logger.Info("Запуск пересчета");
                List<EntranceComposition> entranceCompositions =
                    db.EntranceComposition.Where(m => m.Entrance == entrance.Id).ToList();
                entrance.Recalculation = true;
                foreach (var e in entranceCompositions)
                {
//Ingredient1 увеличивает время работы, можно заменить на Ingredient, наверно
                    var ingredient = db.Ingredient.FirstOrDefault(m => m.Id == e.Ingredient1.Id);

                    var sumIngredientCost = Convert.ToDouble(ingredient.Cost) * Convert.ToDouble(ingredient.Count);
                    var sumIngredientInEntranceCost = Convert.ToDouble(e.Cost) * Convert.ToDouble(e.Count);
                    var newCount = (ingredient.Count + e.Count);

                    ingredient.Cost = Convert.ToDecimal((sumIngredientCost + sumIngredientInEntranceCost) / newCount);
                    ingredient.Count = newCount;

                    if (ModelState.IsValid)
                    {
                        db.Entry(ingredient).State = EntityState.Modified;
                        db.Entry(entrance).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            }
            else logger.Info("Поставка {0} уже была пересчитана",id);
            ViewBag.Entrance = new SelectList(db.Entrance, "Id", "Id");
            ViewBag.Ingredient = new SelectList(db.Ingredient, "Id", "Name");
            return RedirectToAction("Index");
        }
        // GET: Entrances
        public ActionResult Index()
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Отображение списка поставок");
            return View(db.Entrance.ToList());
        }

        // GET: Entrances/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entrance entrance = db.Entrance.Find(id);
            if (entrance == null)
            {
                return HttpNotFound();
            }
            return View(entrance);
        }

        // GET: Entrances/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Entrances/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date")] Entrance entrance)
        {
            if (ModelState.IsValid)
            {
                db.Entrance.Add(entrance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(entrance);
        }

        // GET: Entrances/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entrance entrance = db.Entrance.Find(id);
            if (entrance == null)
            {
                return HttpNotFound();
            }
            return View(entrance);
        }

        // POST: Entrances/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date")] Entrance entrance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(entrance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(entrance);
        }

        // GET: Entrances/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Entrance entrance = db.Entrance.Find(id);
            if (entrance == null)
            {
                return HttpNotFound();
            }
            return View(entrance);
        }

        // POST: Entrances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Entrance entrance = db.Entrance.Find(id);
            db.Entrance.Remove(entrance);
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
