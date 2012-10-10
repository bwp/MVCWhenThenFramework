﻿using System.Collections;
using System.Collections.Specialized;
﻿using System.Diagnostics;
﻿using System.Security.Principal;
﻿using System.Web;
using System.Web.Mvc;
﻿using Moq;
 
namespace MVCWhenThenFramework
{
    /// <summary>
    /// Shamelessly borrowed some code from http://www.codethinked.com/post/2008/03/23/Simplified-Aspnet-MVC-Controller-Testing-with-Moq.aspx
    /// </summary>
    public class MockHttpContextContainer
    {         
        public Mock<HttpContextBase> Context { get; set; }
        public Mock<HttpRequestBase> Request { get; set; }
        public Mock<HttpResponseBase> Response { get; set; }
        public Mock<HttpSessionStateBase> SessionState { get; set; }
        public Mock<HttpServerUtilityBase> ServerUtility { get; set; }
        public Mock<HttpApplicationStateBase> Application { get; set; }
        public Mock<IPrincipal> User { get; set; }
        public Mock<IIdentity> Identity { get; set; }
        public Mock<UrlHelper> UrlHelper { get; set; }

        private NameValueCollection _formData;
        public NameValueCollection FormData
        {
            get
            {
                if (_formData == null)
                {
                    _formData = new NameValueCollection();
                    Request.Setup(r => r.Form).Returns(FormData);
                }
                return _formData;
            }
        }

        public MockHttpContextContainer()
        {
            HttpCookieCollection cookies = new HttpCookieCollection();
            NameValueCollection formData = new NameValueCollection();
            const string ip = "166777210"; // US ip 

            Context = new Mock<HttpContextBase> { CallBase = true };
            Request = new Mock<HttpRequestBase>(MockBehavior.Strict) { CallBase = true };
            Response = new Mock<HttpResponseBase>(MockBehavior.Strict) { CallBase = true };
            SessionState = new Mock<HttpSessionStateBase> { CallBase = true };
            ServerUtility = new Mock<HttpServerUtilityBase>(MockBehavior.Loose) { CallBase = true };
            Application = new Mock<HttpApplicationStateBase> { CallBase = true };
            User = new Mock<IPrincipal> { CallBase = true };
            Identity = new Mock<IIdentity> { CallBase = true };
            UrlHelper = new Mock<UrlHelper> {CallBase = true};

            Context.Setup(c => c.Request).Returns(Request.Object);
            Context.Setup(c => c.Response).Returns(Response.Object);
            Context.Setup(c => c.Session).Returns(SessionState.Object);
            Context.Setup(c => c.Server).Returns(ServerUtility.Object);
            Context.Setup(c => c.Application).Returns(Application.Object);
            Context.Setup(c => c.User).Returns(User.Object);

            Request.Setup(x => x.Cookies).Returns(cookies);
            Request.Setup(r => r.Form).Returns(formData);

            NameValueCollection serverVariables = new NameValueCollection();
            serverVariables.Add("REMOTE_ADDR", ip); // default ip location
            serverVariables.Add("HTTP_X_FORWARDED_FOR", ip); // behind proxy or router ip location
            serverVariables.Add("HTTP_FORWARDED", ip); // behind proxy or router ip location
            Request.Setup(r => r.ServerVariables).Returns(serverVariables);
          
            Request.Setup(r => r.UserHostAddress).Returns(ip); // Used for ip lookup

            Response.Setup(r => r.Cookies).Returns(cookies);

        } 

        public MockHttpContextContainer(string userHostAddress)
        {
            HttpCookieCollection cookies = new HttpCookieCollection();
            NameValueCollection formData = new NameValueCollection(); 
 
            Context = new Mock<HttpContextBase> {CallBase = true};
            Request = new Mock<HttpRequestBase>(MockBehavior.Strict) {CallBase = true};
            Response = new Mock<HttpResponseBase>(MockBehavior.Strict) {CallBase = true};
            SessionState = new Mock<HttpSessionStateBase> {CallBase = true};
            ServerUtility = new Mock<HttpServerUtilityBase>(MockBehavior.Loose) {CallBase = true};
            Application = new Mock<HttpApplicationStateBase> {CallBase = true};
            User = new Mock<IPrincipal> {CallBase = true};
            Identity = new Mock<IIdentity> {CallBase = true};
            UrlHelper = new Mock<UrlHelper> {CallBase = true};
 
            Context.Setup(c => c.Request).Returns(Request.Object);
            Context.Setup(c => c.Response).Returns(Response.Object);
            Context.Setup(c => c.Session).Returns(SessionState.Object);
            Context.Setup(c => c.Server).Returns(ServerUtility.Object);
            Context.Setup(c => c.Application).Returns(Application.Object);
            Context.Setup(c => c.User).Returns(User.Object);
 
            Request.Setup(x => x.Cookies).Returns(cookies);
            Request.Setup(r => r.Form).Returns(formData);

            Request.Setup(r => r.ServerVariables).Returns(new NameValueCollection{
                { "REMOTE_ADDR", userHostAddress },
                { "REMOTE_HOST", userHostAddress }, // default ip location
                { "HTTP_X_FORWARDED_FOR", userHostAddress}, // behind proxy or router ip location
                { "HTTP_FORWARDED", userHostAddress} // behind proxy or router ip location
            });

            Request.Setup(r => r.UserHostAddress).Returns(userHostAddress); // Used for ip lookup
            
            Response.Setup(r => r.Cookies).Returns(cookies);  
            
        } 
 
        private static HttpContext GetHttpContext(HttpContextBase context)
        {
            var app = (HttpApplication)context.GetService(typeof(HttpApplication));
            Debug.Assert(app != null, "app != null");
            return app.Context;   
        }

        public MockHttpContextContainer(HttpVerbs verb, HttpCookieCollection cookies, string userHostAddress, string cookieNameToSet)
        {
            cookies = cookies ?? new HttpCookieCollection();
            NameValueCollection formData = new NameValueCollection(); 
 
            Context = new Mock<HttpContextBase> { CallBase = true };
            Request = new Mock<HttpRequestBase>(MockBehavior.Strict) { CallBase = true };
            Response = new Mock<HttpResponseBase>(MockBehavior.Strict) { CallBase = true };
            SessionState = new Mock<HttpSessionStateBase> { CallBase = true };
            ServerUtility = new Mock<HttpServerUtilityBase>(MockBehavior.Loose) { CallBase = true };
            Application = new Mock<HttpApplicationStateBase> { CallBase = true };
            User = new Mock<IPrincipal> { CallBase = true };
            Identity = new Mock<IIdentity> { CallBase = true };
            UrlHelper = new Mock<UrlHelper> { CallBase = true };
 
            Context.Setup(c => c.Request).Returns(Request.Object);
            Context.Setup(c => c.Response).Returns(Response.Object);
            Context.Setup(c => c.Session).Returns(SessionState.Object);
            Context.Setup(c => c.Server).Returns(ServerUtility.Object);
            Context.Setup(c => c.Application).Returns(Application.Object);
            Context.Setup(c => c.User).Returns(User.Object);
                        
            Request.Setup(r => r.Form).Returns(formData);
            Request.Setup(y => y.RequestType).Returns(verb == HttpVerbs.Get ? "GET" : "POST");
            Request.Setup(x => x.Cookies).Returns(cookies);

            Request.Setup(r => r.ServerVariables).Returns(new NameValueCollection{
                { "REMOTE_ADDR", userHostAddress },
                { "REMOTE_HOST", userHostAddress }, // default ip location
                { "HTTP_X_FORWARDED_FOR", userHostAddress}, // behind proxy or router ip location
                { "HTTP_FORWARDED", userHostAddress} // behind proxy or router ip location
            });

            Request.Setup(r => r.UserHostAddress).Returns(userHostAddress); // Used for ip lookup
 
            Response.Setup(r => r.Cookies).Returns(cookies);
            Response.Setup(r => r.SetCookie(cookies[cookieNameToSet]));
            
        }

        public MockHttpContextContainer(string userName, HttpVerbs verb, string userHostAddress)
        {
            HttpCookieCollection cookies = new HttpCookieCollection();
            NameValueCollection formData = new NameValueCollection();
            Hashtable items = new Hashtable();
 
            Context = new Mock<HttpContextBase> { CallBase = true };
            Request = new Mock<HttpRequestBase>(MockBehavior.Strict);
            Response = new Mock<HttpResponseBase>(MockBehavior.Strict);
            SessionState = new Mock<HttpSessionStateBase>(MockBehavior.Loose);
            ServerUtility = new Mock<HttpServerUtilityBase>(MockBehavior.Loose);
            Application = new Mock<HttpApplicationStateBase> { CallBase = true };
            User = new Mock<IPrincipal> { CallBase = true };
            Identity = new Mock<IIdentity> { CallBase = true };
            UrlHelper = new Mock<UrlHelper> { CallBase = true };
 
            Context.Setup(c => c.Items).Returns(items);
            Context.Setup(c => c.Request).Returns(Request.Object);
            Context.Setup(c => c.Response).Returns(Response.Object);
            Context.Setup(c => c.Session).Returns(SessionState.Object);
            Context.Setup(c => c.Server).Returns(ServerUtility.Object);
            Context.Setup(c => c.Application).Returns(Application.Object);
            Context.Setup(c => c.User).Returns(User.Object);
            Context.Setup(c => c.User.Identity.IsAuthenticated).Returns(true);
            Context.Setup(c => c.User.Identity.Name).Returns(userName);
                     
            Request.Setup(r => r.Form).Returns(formData);
            Request.Setup(y => y.RequestType).Returns(verb == HttpVerbs.Get ? "GET" : "POST");
            Request.Setup(x => x.Cookies).Returns(cookies);

            Request.Setup(r => r.ServerVariables).Returns(new NameValueCollection{
                { "REMOTE_ADDR", userHostAddress },
                { "REMOTE_HOST", userHostAddress }, // default ip location
                { "HTTP_X_FORWARDED_FOR", userHostAddress}, // behind proxy or router ip location
                { "HTTP_FORWARDED", userHostAddress} // behind proxy or router ip location
            });

            Request.Setup(r => r.UserHostAddress).Returns(userHostAddress); // Used for ip lookup
 
            Response.Setup(r => r.Cookies).Returns(cookies);
 
        }
 
        public HttpContextBase GetHttpContext()
        {
            return Context.Object;
        }

        public static implicit operator MockHttpContextContainer(MockHttpContextNewContainer convert)
        {
            MockHttpContextContainer container = new MockHttpContextContainer();

            container.Context = convert.Context;
            container.Request = Mock.Get(convert.Request.Object);
            container.Response = convert.Response;
            container.SessionState = convert.SessionState;
            container.Application = convert.Application;
            container.ServerUtility = convert.ServerUtility;
            container.User = Mock.Get(convert.User.Object);
            container.Identity = Mock.Get(convert.Identity.Object);
            //container.UrlHelper = Mock.Get(convert.UrlHelper.Object);

            return container;
        }
 
    }
}


 
