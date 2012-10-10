using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCWhenThenFramework
{
    public static class ControllerExtensions
    {
        public static void SetFakeControllerContext(this Controller controller, HttpContextBase httpContext)
        {
            SetFakeControllerContext(controller, httpContext, new RouteData());
        }
        public static void SetFakeControllerContext(this Controller controller, HttpContextBase httpContext, RouteData routeData)
        {
            var context = new ControllerContext(new RequestContext(httpContext, routeData), controller);
            controller.ControllerContext = context;
            controller.Url = new UrlHelper(controller.ControllerContext.RequestContext);
        }
    }
}