using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmployeesApplication.Models;

namespace EmployeesApplication.Controllers
{
    public class EmployeeController : Controller
    {
        private EmployeesEntities db = new EmployeesEntities();

        // GET: Employee
        public ActionResult Index()
        {
            /* For Notifications when Redirected into this controller */
            // Added Records Into Database
            if (TempData["AddedItemsCount"] != null)
            {
                var VM = new EmployeesVM
                {
                    Succeeded = true,
                    Added = int.Parse(TempData["AddedItemsCount"].ToString())
                };
                return View(VM);
            }
            // A Record created and Added Into Database
            if (TempData["CreatedSuccessfully"] != null)
            {
                var VM = new EmployeesVM
                {
                    Succeeded = true,
                    Created = bool.Parse(TempData["CreatedSuccessfully"].ToString())
                };
                return View(VM);
            }
            // The Record edited and the Database updated
            if (TempData["EditedSuccessfully"] != null)
            {
                var VM = new EmployeesVM
                {
                    Succeeded = true,
                    Edited = bool.Parse(TempData["EditedSuccessfully"].ToString())
                };
                return View(VM);
            }
            // The Record deleted and the Database updated
            if (TempData["DeletedSuccessfully"] != null)
            {
                var VM = new EmployeesVM
                {
                    Succeeded = true,
                    Deleted = bool.Parse(TempData["DeletedSuccessfully"].ToString())
                };
                return View(VM);
            }

            // Default case
            return View(new EmployeesVM());
        }

        // POST: Employee/Getlist
        [HttpPost]
        public ActionResult GetList()
        {
            var EmpList = db.Employees.ToList();

            // New object List to send to AJAX DataTable
            var employees = new List<object>();

            foreach (var emp in EmpList)
            {
                employees.Add(new
                {
                    Address = emp.Address,
                    Address_2 = emp.Address_2,
                    // To display in a correct Format
                    Date_of_Birth = emp.Date_of_Birth.Value.Date.ToShortDateString(),
                    EMail_Home = emp.EMail_Home,
                    Forenames = emp.Forenames,
                    Id = emp.Id,
                    Mobile = emp.Mobile,
                    Payroll_Number = emp.Payroll_Number,
                    Postcode = emp.Postcode,
                    // To display in a correct Format
                    Start_Date = emp.Start_Date.Date.ToShortDateString(),
                    Surname = emp.Surname,
                    Telephone = emp.Telephone
                });
            }

            return Json(new { data = employees }, JsonRequestBehavior.AllowGet);
        }

        // Action that Imports records from csv file
        // POST: Employee/ImportCsv/
        [HttpPost]
        public ActionResult ImportCsv(HttpPostedFileBase upload)
        {
            var AddedItemsCount = 0;
            if (upload != null && (upload.ContentType == "text/csv" || upload.ContentType == "application/vnd.ms-excel"))
            {
                try
                {
                    var fileName = Server.MapPath("~/Files/" + Path.GetFileName(upload.FileName));
                    upload.SaveAs(fileName);

                    string[] Lines = System.IO.File.ReadAllLines(fileName);
                    string[] Fields;
                    var EmListFromFile = new List<Employees>();

                    // Skip Headers
                    Lines = Lines.Skip(1).ToArray();

                    // Parsing the data taken from the file
                    foreach (var line in Lines)
                    {
                        Fields = line.Split(new char[] { ',' });
                        EmListFromFile.Add(new Employees
                        {
                            Payroll_Number = Fields[0],
                            Forenames = Fields[1],
                            Surname = Fields[2],
                            Date_of_Birth = DateTime.Parse(Fields[3]),
                            Telephone = Fields[4],
                            Mobile = Fields[5],
                            Address = Fields[6],
                            Address_2 = Fields[7],
                            Postcode = Fields[8],
                            EMail_Home = Fields[9],
                            Start_Date = DateTime.Parse(Fields[10])
                        });
                    }

                    AddedItemsCount = EmListFromFile.Count;
                    // Add to Database
                    foreach (var employee in EmListFromFile)
                        db.Employees.Add(employee);

                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw ex;
                }
            }

            // Temporarly persists the data that needs for notifying
            TempData["AddedItemsCount"] = AddedItemsCount;
            return RedirectToAction("Index");
        }




        // GET: Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Payroll_Number,Forenames,Surname,Date_of_Birth,Telephone,Mobile,Address,Address_2,Postcode,EMail_Home,Start_Date")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                db.Employees.Add(employees);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(employees);
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Payroll_Number,Forenames,Surname,Date_of_Birth,Telephone,Mobile,Address,Address_2,Postcode,EMail_Home,Start_Date")] Employees employees)
        {
            if (ModelState.IsValid)
            {
                db.Entry(employees).State = EntityState.Modified;
                db.SaveChanges();

                // Temporarly persists the data that needs for notifying
                TempData["EditedSuccessfully"] = true;
                return RedirectToAction("Index");
            }
            return View(employees);
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = db.Employees.Find(id);
            if (employees == null)
            {
                return HttpNotFound();
            }
            return View(employees);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employees employees = db.Employees.Find(id);
            db.Employees.Remove(employees);
            db.SaveChanges();

            // Temporarly persists the data that needs for notifying
            TempData["DeletedSuccessfully"] = true;
            return RedirectToAction("Index");
        }

        // GET: Employee/About
        public ActionResult About()
        {
            ViewBag.Message = "Application description page.";

            return View();
        }

        // GET: Employee/Contacts
        public ActionResult Contact()
        {
            ViewBag.Message = "Contact page.";
            return View();
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
