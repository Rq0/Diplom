using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Coursach;
using Microsoft.Owin.Logging;
using NLog;

namespace Coursach.Controllers
{
    public class DishesController : Controller
    {
        private MenuUnitEntities db = new MenuUnitEntities();
        public ActionResult Calculate(int? id)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Пересчет стоимости блюд");
            Dish dish = db.Dish.FirstOrDefault(m => m.Id == id);
            List<DishComposition> dishCompositions = db.DishComposition.Where(m => m.Dish == dish.Id).ToList();
// чет на херню похоже, надо приглядеться
            //dish = db.Dish.Find(dish.Id);
            dish.Cost = 0;
// заменить на foreach
            for (int i = 0; i< dishCompositions.Count;i++)
            {
                var ingridient = db.Ingredient.Find(dishCompositions[i].Ingridient);
                if (ingridient != null)
                    dish.Cost = dish.Cost + ingridient.Cost * Convert.ToDecimal(ingridient.Count);
            }
            if (ModelState.IsValid)
            {
                db.Entry(dish).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            logger.Warn("ModelState не валидна");
            ViewBag.Type = new SelectList(db.DishTypes, "Id", "Name", dish.Type);
            return RedirectToAction("Index");
        }

        // GET: Dishes
        public ActionResult Index()
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Отображение списка блюд");
            var dish = db.Dish.Include(d => d.DishTypes);
            return View(dish.ToList());
        }

        // GET: Dishes/Create
        public ActionResult Create()
        {
            ViewBag.Type = new SelectList(db.DishTypes, "Id", "Name");
            return View();
        }

        // POST: Dishes/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Cost,Type,Frequency")] Dish dish)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Создание нового блюда");
            if (ModelState.IsValid)
            {
                db.Dish.Add(dish);
                db.SaveChanges();
                {
                    DishComposition dishComposition = new DishComposition();
                    dishComposition.Dish = db.Dish.FirstOrDefault(m=>(m.Name==dish.Name && m.Type==dish.Type)).Id;
                    db.DishComposition.Add(dishComposition);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.Type = new SelectList(db.DishTypes, "Id", "Name", dish.Type);
            return View(dish);
        }

        // GET: Dishes/Edit/5
        public ActionResult Edit(int? id)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Изменение блюда");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dish dish = db.Dish.Find(id);
            if (dish == null)
            {
                return HttpNotFound();
            }
            ViewBag.Type = new SelectList(db.DishTypes, "Id", "Name", dish.Type);
            return View(dish);
        }

        // POST: Dishes/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Cost,Type,Frequency")] Dish dish)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dish).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Type = new SelectList(db.DishTypes, "Id", "Name", dish.Type);
            return View(dish);
        }

        // GET: Dishes/Delete/5
        public ActionResult Delete(int? id)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Удаление блюда");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dish dish = db.Dish.Find(id);
            if (dish == null)
            {
                return HttpNotFound();
            }
            return View(dish);
        }

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info("Подтверждение удаление блюда");
            Dish dish = db.Dish.Find(id);
            db.Dish.Remove(dish);
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
