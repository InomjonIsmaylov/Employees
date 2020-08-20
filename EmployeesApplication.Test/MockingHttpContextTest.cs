using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Rhino.Mocks;
using System.Web;
using System.Web.Routing;
using MockRepository = Rhino.Mocks.MockRepository;

namespace EmployeesApplication.Test
{
    [TestClass]
    public class MockingHttpContextTest
    {
        private HttpContextBase rmContext;
        private HttpRequestBase rmRequest;
        private Mock<HttpContextBase> moqContext;
        private Mock<HttpRequestBase> moqRequest;
        [TestInitialize]
        public void SetupTests()
        {
            rmContext = MockRepository.GenerateMock<HttpContextBase>();
            rmRequest = MockRepository.GenerateMock<HttpRequestBase>();
            rmContext.Stub(x => x.Request).Return(rmRequest);
            
            moqContext = new Mock<HttpContextBase>();
            moqRequest = new Mock<HttpRequestBase>();
            moqContext.Setup(x => x.Request).Returns(moqRequest.Object);
        }

        [TestMethod]
        public void RhinoMocksRoutingTest()
        {
            RouteCollection routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
            rmRequest.Stub(e => e.AppRelativeCurrentExecutionFilePath).Return("~/Employee/Index");
            
            RouteData routeData = routes.GetRouteData(rmContext);
            
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Employee", routeData.Values["controller"]);
            Assert.AreEqual("Index", routeData.Values["action"]);
        }

        [TestMethod]
        public void MoqRoutingTest()
        {
            // Arrange
            RouteCollection routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);
            moqRequest.Setup(e => e.AppRelativeCurrentExecutionFilePath).Returns("~/Employee/Index");
            // Act
            RouteData routeData = routes.GetRouteData(moqContext.Object);
            // Assert
            Assert.IsNotNull(routeData);
            Assert.AreEqual("Employee", routeData.Values["controller"]);
            Assert.AreEqual("Index", routeData.Values["action"]);
        }
    }
}
