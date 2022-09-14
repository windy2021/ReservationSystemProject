using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReservationSystemProject.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ReservationSystemTestProject.Controllers.Tests
{
    [TestClass()]
    public class HomeControllerTests
    {
        [TestMethod]
        public void IndexTestNotNull()
        {
            HomeController controller = new HomeController(null);
            var actualResult = controller.Index();
            Assert.IsNotNull(actualResult);
        }
        [TestMethod]
        public void IndexTitleTest()
        {
            HomeController controller = new HomeController(null);
            ViewResult actualResult = (ViewResult)controller.Index();
            Assert.IsNotNull(actualResult.ViewData["Title"]);
            Assert.IsInstanceOfType(actualResult.ViewData["Title"], typeof(string));
            Assert.IsTrue(actualResult.ViewData["Title"].ToString().Contains("Home"));
        }
        [TestMethod]
        public void RedirectUnregisteredUserTest()
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m => m.User.IsInRole("")).Returns(true);

            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            var controller = new HomeController(null)
            {
                ControllerContext = context
            };
            RedirectToPageResult result = (RedirectToPageResult)controller.RedirectUser();
            Assert.IsInstanceOfType(result, typeof(RedirectToPageResult));
            RedirectToPageResult redirect = result as RedirectToPageResult; //<--cast here
            Assert.AreEqual("Register", redirect.PageName);
        }
        [TestMethod]
        public void RedirectManagerTest()
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m => m.User.IsInRole("Manager")).Returns(true);

            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            var controller = new HomeController(null)
            {
                ControllerContext = context
            };

            RedirectToActionResult result = (RedirectToActionResult)controller.RedirectUser();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.IsTrue(result.RouteValues.ContainsKey("area"));
            Assert.IsTrue(result.RouteValues.Values.Contains("Admin"));
        }
        [TestMethod]
        public void RedirectMemberTest()
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m => m.User.IsInRole("Member")).Returns(true);

            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            var controller = new HomeController(null)
            {
                ControllerContext = context
            };

            RedirectToActionResult result = (RedirectToActionResult)controller.RedirectUser();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.IsTrue(result.RouteValues.ContainsKey("area"));
            Assert.IsTrue(result.RouteValues.Values.Contains(""));
        }
        [TestMethod]
        public void RedirectStaffTest()
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m => m.User.IsInRole("Staff")).Returns(true);

            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            var controller = new HomeController(null)
            {
                ControllerContext = context
            };

            RedirectToActionResult result = (RedirectToActionResult)controller.RedirectUser();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
            Assert.IsTrue(result.RouteValues.ContainsKey("area"));
            Assert.IsTrue(result.RouteValues.Values.Contains("Admin"));
        }
    }
}
