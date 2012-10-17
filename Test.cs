using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCWhenThenFramework
{
    public class Test
    {
        /// <summary>
        /// Provide a controller to test
        /// QueryString is extracted from URL automatically if it exists
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="useHttpContextConstructor"> </param>
        /// <param name="url"> </param>
        /// <param name="userHostAddress"> </param>
        /// <returns></returns>
        public static ControllerTestContext<T> This<T>(T controller, 
            string url = null, bool useHttpContextConstructor = true,
            string userHostAddress = "127.0.0.1") where T : Controller
        {

            MockHttpContextContainer container = useHttpContextConstructor == false
                ? new MockHttpContextContainer(userHostAddress) : new MockHttpContextNewContainer(url: url, userHostAddress: userHostAddress);
            
            var controllerEx = new ControllerTestContext<T>
                                   {
                                       Controller = controller,
                                       MockContext = container
                                   };

            controllerEx.Controller
                .SetFakeControllerContext(controllerEx.MockContext.GetHttpContext());

            //controllerEx.Controller.Request.SetupUserHostAddress(userHostAddress);

            return controllerEx;
        }

        public static ControllerTestContext<T> This<T>(T controller, HttpVerbs verb, string cookieNameToSet = null,
            HttpCookieCollection cookies = null, string userName = null, 
            bool useHttpContextConstructor = true, string url = null,
            string queryString = null, string userHostAddress = "127.0.0.1") where T : Controller
        {
            var controllerEx = new ControllerTestContext<T>();

            controllerEx.Controller = controller;

            if (!string.IsNullOrEmpty(userName))
            {
                controllerEx.MockContext = useHttpContextConstructor == false
                                               ? new MockHttpContextContainer(userName, verb, userHostAddress)
                                               : new MockHttpContextNewContainer(userName: userName, verb: verb, userHostAddress: userHostAddress);
            }
            else if (cookies == null)
            {
                controllerEx.MockContext = useHttpContextConstructor == false ? new MockHttpContextContainer(userHostAddress) : new MockHttpContextNewContainer(url, userHostAddress);
            }
            else
            {
                controllerEx.MockContext = useHttpContextConstructor == false ? new MockHttpContextContainer(verb, cookies, userHostAddress, cookieNameToSet) : new MockHttpContextNewContainer(verb, cookies, userHostAddress, cookieNameToSet);
            }

            controllerEx.Controller
                .SetFakeControllerContext(controllerEx.MockContext.GetHttpContext());

            //controllerEx.Controller.Request.SetupUserHostAddress(userHostAddress);

            //HttpContext.Current = controllerEx.MockContext.Context.Object.ApplicationInstance.Context;

            return controllerEx;
        }

        /// <summary>
        /// Provide a url to test for route values
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static ISpecifyRouteCollection This(string url)
        {
            return new RouteTestContext { Url = url };
        }

        /// <summary>
        /// Provide a route collection to test against a url for route values
        /// </summary>
        /// <param name="routes"></param>
        /// <returns></returns>
        public static ISpecifyUrl This(RouteCollection routes)
        {
            return new RouteTestContext { Routes = routes };
        }

    }
}