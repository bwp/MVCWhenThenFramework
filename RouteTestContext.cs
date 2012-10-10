using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MVCWhenThenFramework
{
    public class RouteTestContext : ISpecifyRouteCollection, IRouteExpectation, ISpecifyUrl
    {
        public string Url { get; internal set; }

        public RouteCollection Routes { get; internal set; }

        public static void AssertRoute(RouteCollection routes, string url, object expectations)
        {
            var httpContextMock = new Mock<HttpContextBase>();
            httpContextMock.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath)
                .Returns(url);

            RouteData routeData = routes.GetRouteData(httpContextMock.Object);
            Assert.IsNotNull(routeData, "Should have found the route");

            foreach (PropertyValue property in GetProperties(expectations))
            {
                Assert.IsTrue(string.Equals(property.Value.ToString(),
                                            routeData.Values[property.Name].ToString(),
                                            StringComparison.OrdinalIgnoreCase)
                              , string.Format("Expected '{0}', not '{1}' for '{2}'.",
                                              property.Value, routeData.Values[property.Name], property.Name));
            }
        }

        private static IEnumerable<PropertyValue> GetProperties(object obj)
        {
            if (obj != null)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
                foreach (PropertyDescriptor prop in props)
                {
                    object val = prop.GetValue(obj);
                    if (val != null)
                    {
                        yield return new PropertyValue { Name = prop.Name, Value = val };
                    }
                }
            }
        }

        private sealed class PropertyValue
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        IRouteExpectation ISpecifyRouteCollection.UsingRoutes(RouteCollection routes)
        {
            Routes = routes;
            return this;
        }

        IRouteExpectation IRouteExpectation.Expect(object expectations)
        {
            AssertRoute(Routes, Url, expectations);
            return this;
        }

        IRouteExpectation ISpecifyUrl.GivenUrl(string url)
        {
            Url = url;
            return this;
        }
    }
}