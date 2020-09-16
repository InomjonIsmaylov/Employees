using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using EmployeesApplication.Models;

namespace EmployeesApplication.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IRepository _repository;

        public EmployeeController(IRepository repository)
        {
            _repository = repository;
        }

        // GET: Employee
        public ActionResult Index()
        {
            // Default case
            return View(new EmployeesVM());
        }

        // POST: Employee/Getlist
        [HttpPost]
        public ActionResult GetList()
        {
            var EmpList = _repository.GetEmployeeList();

            // New object List to send to AJAX DataTable
            var employees = new List<object>();

            if (EmpList.Any())
                foreach (var emp in EmpList)
                    employees.Add(new
                    {
                        Address = emp.Address,
                        Address_2 = emp.Address_2 ?? "",
                        // To display in a correct Format
                        Date_of_Birth = emp.Date_of_Birth.Value.Date.ToShortDateString() ?? "",
                        EMail_Home = emp.EMail_Home ?? "",
                        Forenames = emp.Forenames,
                        Id = emp.Id,
                        Mobile = emp.Mobile ?? "",
                        Payroll_Number = emp.Payroll_Number,
                        Postcode = emp.Postcode ?? "",
                        // To display in a correct Format
                        Start_Date = emp.Start_Date.Date.ToShortDateString(),
                        Surname = emp.Surname,
                        Telephone = emp.Telephone ?? ""
                    });

            return Json(new { data = employees }, JsonRequestBehavior.AllowGet);
        }

        // Action that Imports records from csv file
        // POST: Employee/ImportCsv/
        [HttpPost]
        public ActionResult ImportCsv(HttpPostedFileBase upload)
        {
            
            upload = upload ?? Request.Files.Get(0);

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

                    // Add to Database
                    foreach (var employee in EmListFromFile)
                        _repository.AddEmployee(employee);

                    var count = _repository.SaveChanges();
                    return Json(new { message = $"{count} Employee Record{(count > 1 ? "s have" : " has")} been added!" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Response.StatusCode = 400;
                    return Json(new { message = " Could not parse the data from the file, because:\n" + ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            Response.StatusCode = 400;
            return Json(new { message = $"The file of the {upload?.ContentType ?? "null"} type can not be parsed!" }, JsonRequestBehavior.AllowGet);
        }

        // GET: Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = _repository.FindById(id);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Payroll_Number,Forenames,Surname,Date_of_Birth,Telephone,Mobile,Address,Address_2,Postcode,EMail_Home,Start_Date")] Employees employees)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = 400;
                return View(employees);

            }
            else
            {
                _repository.AddEmployee(employees);
                _repository.SaveChanges();

                return Json(new { message = "New Employee Record has been created successfully!" });
                //              return RedirectToAction("Index");x
            }

            //            return Json(new { success = false, @object = employees });
            //return View(employees);
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = _repository.FindById(id);
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
                _repository.Update(employees);
                _repository.SaveChanges();

                return Json(new { success = true, message = $"The Record of the Employee has been Updated" }, JsonRequestBehavior.AllowGet);

                //return RedirectToAction("Index");
            }

            return Json(new { success = false, message = "Something went wrong. Could not Update the Record!" }, JsonRequestBehavior.AllowGet);
//            return View(employees);
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employees employees = _repository.FindById(id);
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
            try
            {
                _repository.Delete(id);
                _repository.SaveChanges();
                return Json(new { message = "The Record of the Employee has been Deleted" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 400;
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

            //return RedirectToAction("Index");
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
                _repository.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
