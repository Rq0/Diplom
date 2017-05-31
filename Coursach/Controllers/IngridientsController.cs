using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Calabonga.Xml.Exports;
using Coursach;

namespace Coursach.Controllers
{
    public class IngridientsController : Controller
    {
        private MenuUnitEntities db = new MenuUnitEntities();

        public ActionResult Export()
        {
            string result = string.Empty;
            Workbook wb = new Workbook();

            // properties
            wb.Properties.Author = "Calabonga";
            wb.Properties.Created = DateTime.Today;
            wb.Properties.LastAutor = "Calabonga";
            wb.Properties.Version = "14";

            // options sheets
            wb.ExcelWorkbook.ActiveSheet = 1;
            wb.ExcelWorkbook.DisplayInkNotes = false;
            wb.ExcelWorkbook.FirstVisibleSheet = 1;
            wb.ExcelWorkbook.ProtectStructure = false;
            wb.ExcelWorkbook.WindowHeight = 800;
            wb.ExcelWorkbook.WindowTopX = 0;
            wb.ExcelWorkbook.WindowTopY = 0;
            wb.ExcelWorkbook.WindowWidth = 600;


            // get data
            List<Ingredient> ingredients = db.Ingredient.ToList();
            Worksheet ws3 = new Worksheet("Ингредиенты");
            ws3.AddCell(0, 0, "Название");
            ws3.AddCell(0, 1, "Количество");
            ws3.AddCell(0, 2, "Цена");
            int totalRows = 0;

            // appending rows with data
            for (int i = 0; i < ingredients.Count; i++)
            {
                ws3.AddCell(i + 1, 0, ingredients[i].Name);
                ws3.AddCell(i + 1, 1, ingredients[i].Count);
                ws3.AddCell(i + 1, 2, ingredients[i].Cost);
                totalRows++;
            }

            wb.AddWorksheet(ws3);

            // generate xml 
            string workstring = wb.ExportToXML();

            // Send to user file
            return new ExcelResult("Ingredients.xls", workstring);
        }
        public class ExcelResult : ActionResult
        {
            /// <summary>
            /// Создает экземпляр класса, которые выдает файл Excel
            /// </summary>
            /// <param name="fileName">наименование файла для экспорта</param>
            /// <param name="report">готовый набор данные для экпорта</param>
            public ExcelResult(string fileName, string report)
            {
                this.Filename = fileName;
                this.Report = report;
            }
            public string Report { get; private set; }
            public string Filename { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var ctx1 = context.HttpContext;
                ctx1.Response.Clear();
                ctx1.Response.ContentType = "application/vnd.ms-excel";
                ctx1.Response.BufferOutput = true;
                ctx1.Response.AddHeader("content-disposition",
                    string.Format("attachment; filename={0}", Filename));
                ctx1.Response.ContentEncoding = Encoding.UTF8;
                ctx1.Response.Charset = "utf-8";
                ctx1.Response.Write(Report);
                ctx1.Response.Flush();
                ctx1.Response.End();
            }
        }

        // GET: Ingridients
        public ActionResult Index()
        {
            return View(db.Ingredient.ToList());
        }

        // GET: Ingridients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ingridients/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Cost")] Ingredient ingridient)
        {
            if (ModelState.IsValid)
            {
                db.Ingredient.Add(ingridient);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ingridient);
        }

        // GET: Ingridients/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingridient = db.Ingredient.Find(id);
            if (ingridient == null)
            {
                return HttpNotFound();
            }
            return View(ingridient);
        }

        // POST: Ingridients/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Cost")] Ingredient ingridient)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ingridient).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ingridient);
        }

        // GET: Ingridients/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ingredient ingridient = db.Ingredient.Find(id);
            if (ingridient == null)
            {
                return HttpNotFound();
            }
            return View(ingridient);
        }

        // POST: Ingridients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ingredient ingridient = db.Ingredient.Find(id);
            db.Ingredient.Remove(ingridient);
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
