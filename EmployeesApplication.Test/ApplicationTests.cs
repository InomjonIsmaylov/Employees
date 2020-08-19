using System;
using System.Collections.Generic;
using System.Web.Mvc;
using EmployeesApplication.Controllers;
using EmployeesApplication.Models;
using EmployeesApplication.Repositories;
using EmptyWebApplicationForAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace EmployeesApplication.Test
{
    [TestClass]
    public class EmployeeControllerTest
    {
        private EmployeeController _controller;
        private ViewResult _result;
        private IRepository _repo;

        public EmployeeControllerTest()
        {
            _controller = new EmployeeController(new Repository(new EmployeesEntities()));
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

            // Assert
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public void IndexViewModelNotNull_Mock()
        {
            // Arrange
            var mock = new Mock<IRepository>();
            mock.Setup(a => a.GetEmployeeList()).Returns(new List<Employees>());
            var controller = new EmployeeController(mock.Object);

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result.Model);
        }


    }
}
