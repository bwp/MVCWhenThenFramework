using System.Web.Routing;

namespace MVCWhenThenFramework
{
    public interface ISpecifyRouteCollection
    {
        /// <summary>
        /// Provide the RouteCollection you wish to test against
        /// </summary>
        /// <param name="routes"></param>
        /// <returns></returns>
        IRouteExpectation UsingRoutes(RouteCollection routes);
    }
}