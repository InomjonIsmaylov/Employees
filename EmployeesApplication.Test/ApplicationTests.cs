using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using EmployeesApplication.Controllers;
using EmployeesApplication.Models;
using EmployeesApplication.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EmployeesApplication.Test
{
    [TestClass]
    public class EmployeeControllerTest
    {
        private EmployeeController _controller;
        private ViewResult _result;
        private readonly IRepository _repo;

        public EmployeeControllerTest()
        {
            _repo = new Repository(new EmployeesEntities());
            _controller = new EmployeeController(_repo);
        }

        [TestMethod]
        public void IndexViewResultNotNull()
        {
            _result = _controller.Index() as ViewResult;
            Assert.IsNotNull(_result);
        }

        [TestMethod]
        public void IndexViewModelNotNull()
        {
            ViewResult result = _controller.Index() as ViewResult;

            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public void IndexViewModelNotNull_Mock()
        {
            var mock = new Mock<IRepository>();
            mock.Setup(a => a.GetEmployeeList()).Returns(new List<Employees>());
            var controller = new EmployeeController(mock.Object);

            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public void EmployeeController_Returns_EmployeeVM()
        {
            EmployeeController controller = new EmployeeController(_repo);
            var result = controller.Index() as ViewResult;
            Assert.AreEqual(typeof(EmployeesVM).ToString(), result.Model.ToString());
        }

        [TestMethod]
        public void GetList_ResultNotNULL()
        {
            var mock = new Mock<IRepository>();
            EmployeeController controller = new EmployeeController(mock.Object);
            var result = controller.GetList();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetList_Return_Json()
        {
            var mock = new Mock<IRepository>();
            mock.Object.AddEmployee(new Employees());
            EmployeeController controller = new EmployeeController(mock.Object);

            var result = controller.GetList();

            Assert.AreEqual(typeof(JsonResult), result.GetType());
        }

        [TestMethod]
        public void ImportCsv_RedirectsTo_IndexMethod()
        {
            var mock = new Mock<HttpPostedFileBase>();
            string expected = "Index";

            var result = _controller.ImportCsv(mock.Object) as RedirectToRouteResult;
            Assert.IsNotNull(result);

            string actual = result.RouteValues["action"].ToString();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ImportCsv_DoesntSaveIntoRepository_IfFileIsEmpty_And_RedirectsToIndex()
        {
            string expected = "Index";
            var mock = new Mock<IRepository>();
            var mockUpload = new Mock<HttpPostedFileBase>();
            mock.Object.AddEmployee(new Employees());
            
            var controller = new EmployeeController(mock.Object);
            var route = controller.ImportCsv(mockUpload.Object) as RedirectToRouteResult;

            mock.Verify(x => x.SaveChanges(), Times.Never);
            
            Assert.AreEqual(expected, route.RouteValues["action"].ToString());
        }

        [TestMethod]
        public void DetailsMethod_Returns_BadRequest_IfIdIsNULL()
        {
            int expected = 400; // BadRequest

            var result = _controller.Details(null) as HttpStatusCodeResult;
            
            Assert.AreEqual(expected, result.StatusCode);
        }

        [TestMethod]
        public void DetailsMethod_Returns_NotFound_IfIdIsInvalid()
        {
            int expected = 404; // NotFound
            var mock = new Mock<IRepository>();
            mock.Object.AddEmployee(new Employees());

            var controller = new EmployeeController(mock.Object);
            var result = controller.Details(1000000) as HttpStatusCodeResult;

            Assert.AreEqual(expected, result.StatusCode);
        }

        [TestMethod]
        public void CreatePostAction_ModelError()
        {
            var mock = new Mock<IRepository>();
            Employees emp = new Employees();
            EmployeeController controller = new EmployeeController(mock.Object);
            controller.ModelState.AddModelError("Name", "Название модели не установлено");
            
            var result = controller.Create(emp) as ViewResult;
            
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreatePOSTMethod_SavesIntoDatabase()
        {
            var mock = new Mock<IRepository>();
            var controller = new EmployeeController(mock.Object);
            var emp = new Employees();

            controller.Create(emp);

            mock.Verify(repo => repo.AddEmployee(emp));
            mock.Verify(repo => repo.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void EditMethod_Returns_NotFound_IfIdIsInvalid()
        {
            int expected = 404; // NotFound
            var mock = new Mock<IRepository>();

            var controller = new EmployeeController(mock.Object);
            var result = controller.Edit(1000000) as HttpStatusCodeResult;

            Assert.AreEqual(expected, result.StatusCode);
        }

        [TestMethod]
        public void EditPOSTMethod_SavesIntoDatabase()
        {
            var mock = new Mock<IRepository>();
            var controller = new EmployeeController(mock.Object);
            var emp = new Employees();

            controller.Edit(emp);

            mock.Verify(repo => repo.Update(emp));
            mock.Verify(repo => repo.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void DeleteMethod_Returns_NotFound_IfIdIsInvalid()
        {
            int expected = 404; // NotFound
            var mock = new Mock<IRepository>();

            var controller = new EmployeeController(mock.Object);
            var result = controller.Delete(1000000) as HttpStatusCodeResult;

            Assert.AreEqual(expected, result.StatusCode);
        }

        [TestMethod]
        public void DeleteConfirmedPOSTMethod_SavesIntoDatabase()
        {
            var mock = new Mock<IRepository>();
            var controller = new EmployeeController(mock.Object);
            var emp = new Employees { Id = 5 };

            controller.DeleteConfirmed(emp.Id);

            mock.Verify(repo => repo.Delete(emp.Id));
            mock.Verify(repo => repo.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void AboutMethod_ReturnsView()
        {
            ViewResult result = _controller.About() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ContactMethod_ReturnsView()
        {
            ViewResult result = _controller.Contact() as ViewResult;

            Assert.IsNotNull(result);
        }

    }
}
