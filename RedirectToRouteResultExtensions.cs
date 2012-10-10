using System.Web.Mvc;

namespace MVCWhenThenFramework
{
    public static class RedirectToRouteResultExtensions
    {
        public static bool ControllerIs(this RedirectToRouteResult result, string expectedController)
        {
            return (string)result.RouteValues["controller"] == expectedController;
        }

        public static bool ActionIs(this RedirectToRouteResult result, string expectedAction)
        {
            return (string)result.RouteValues["action"] == expectedAction;
        }

        public static bool HasValue(this RedirectToRouteResult result, string key, string expectedValue)
        {
            return (string)result.RouteValues[key] == expectedValue;
        }
    }
}