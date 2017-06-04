using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using Excel = Microsoft.Office.Interop.Excel;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Calabonga.Xml.Exports;
using Coursach;

namespace Coursach.Controllers
{
    public class EntrancesController : Controller
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
            List<EntranceComposition> entranceCompositions = db.EntranceComposition.ToList();
            Worksheet ws3 = new Worksheet("Поставки");
            ws3.AddCell(0, 0, "Поставка");
            ws3.AddCell(0, 1, "Ингредиент");
            ws3.AddCell(0, 2, "Количество");
            ws3.AddCell(0, 3, "Цена");
            int totalRows = 0;

            // appending rows with data
            for (int i = 0; i < entranceCompositions.Count; i++)
            {
                ws3.AddCell(i + 1, 0, entranceCompositions[i].Entrance);
                ws3.AddCell(i + 1, 1, entranceCompositions[i].Ingredient1.Name.ToString());
                ws3.AddCell(i + 1, 2, entranceCompositions[i].Count);
                ws3.AddCell(i + 1, 3, entranceCompositions[i].Cost);
                totalRows++;
            }

            wb.AddWorksheet(ws3);

            // generate xml 
            string workstring = wb.ExportToXML();

            // Send to user file
            return new ExcelResult("Entrances.xls", workstring);
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
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase excelfile)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            if (excelfile == null || excelfile.ContentLength == 0)
            {
                ViewBag.Error = "Файл не выбран! <br>";
                return View("Index", db.Entrance.ToList());
            }
            else
            {
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {
                    string path = Server.MapPath("~/Import/" + excelfile.FileName);
                    excelfile.SaveAs(path);
                    //Читаем из файла
                    // Excel.Application ap = new Excel.Application();

                    Excel.Application application = new Excel.Application();
                    Excel.Workbook workbook = application.Workbooks.Open(path);
                    Excel.Worksheet worksheet = workbook.ActiveSheet;
                    Excel.Range range = worksheet.UsedRange;
                    List<EntranceComposition> entranceCompositions = new List<EntranceComposition>();
                    for (int row = 2; row <= range.Rows.Count; row++)
                    {
                        EntranceComposition entranceComposition = new EntranceComposition();
                        string name = ((Excel.Range) range.Cells[row, 2]).Text;
                        entranceComposition.Entrance = Convert.ToInt32(((Excel.Range)range.Cells[row, 1]).Text);
                        entranceComposition.Ingredient = db.Ingredient.FirstOrDefault(m=>m.Name== name).Id;
                        entranceComposition.Count = Convert.ToDouble(((Excel.Range)range.Cells[row, 3]).Text);
                        entranceComposition.Cost = Convert.ToDecimal(((Excel.Range)range.Cells[row, 4]).Text);
                        db.EntranceComposition.Add(entranceComposition);
                        db.SaveChanges();
                    }
                    workbook.Close();
                    ViewBag.Error = "Данные загружены <br>";
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    return View("Index", db.Entrance.ToList());
                }
                else
                {
                    ViewBag.Error = "Это не Excel! <br>";
                    return View("Index", db.Entrance.ToList());
                }
            }

        }
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
