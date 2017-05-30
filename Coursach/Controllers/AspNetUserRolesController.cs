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
    public class AspNetUserRolesController : Controller
    {
        private MenuUnitEntities db = new MenuUnitEntities();

        // GET: AspNetUserRoles
        public ActionResult Index()
        {
            var aspNetUserRoles = db.AspNetUserRoles.Include(a => a.AspNetRoles).Include(a => a.AspNetUsers);
            return View(aspNetUserRoles.ToList());
        }

        // GET: AspNetUserRoles/Create
        public ActionResult Create()
        {
            ViewBag.RoleId = new SelectList(db.AspNetRoles, "Id", "Name");
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: AspNetUserRoles/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserId,RoleId,UserRoleId")] AspNetUserRoles aspNetUserRoles)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUserRoles.Add(aspNetUserRoles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleId = new SelectList(db.AspNetRoles, "Id", "Name", aspNetUserRoles.RoleId);
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", aspNetUserRoles.UserId);
            return View(aspNetUserRoles);
        }

        // GET: AspNetUserRoles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUserRoles aspNetUserRoles = db.AspNetUserRoles.Find(id);
            if (aspNetUserRoles == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleId = new SelectList(db.AspNetRoles, "Id", "Name", aspNetUserRoles.RoleId);
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", aspNetUserRoles.UserId);
            return View(aspNetUserRoles);
        }

        // POST: AspNetUserRoles/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,RoleId,UserRoleId")] AspNetUserRoles aspNetUserRoles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUserRoles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(db.AspNetRoles, "Id", "Name", aspNetUserRoles.RoleId);
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", aspNetUserRoles.UserId);
            return View(aspNetUserRoles);
        }

        // GET: AspNetUserRoles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUserRoles aspNetUserRoles = db.AspNetUserRoles.Find(id);
            if (aspNetUserRoles == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUserRoles);
        }

        // POST: AspNetUserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AspNetUserRoles aspNetUserRoles = db.AspNetUserRoles.Find(id);
            db.AspNetUserRoles.Remove(aspNetUserRoles);
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
