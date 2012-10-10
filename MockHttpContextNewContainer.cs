using System;
using System.Collections;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Moq;

namespace MVCWhenThenFramework
{

    public class MockHttpContextNewContainer
    {

        #region " variables "
        private Mock<Mock<HttpContextBase>> MockContext { get; set; }
        private Mock<Mock<HttpRequestBase>> MockRequest { get; set; }
        private Mock<Mock<HttpResponseBase>> MockResponse { get; set; }
        private Mock<Mock<HttpSessionStateBase>> MockSessionState { get; set; }
        private Mock<Mock<HttpServerUtilityBase>> MockServerUtility { get; set; }
        private Mock<Mock<HttpApplicationStateBase>> MockApplication { get; set; }
        private Mock<Mock<IPrincipal>> MockUser { get; set; }
        private Mock<Mock<IIdentity>> MockIdentity { get; set; }
        
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
        #endregion


        // CONSTRUCTORS

        /// <summary>
        /// Constructor used for conversion to MockHttpContextContainer
        /// </summary>
        public MockHttpContextNewContainer()
        {
            Context = new Mock<HttpContextBase> { CallBase = true };
            Request = new Mock<HttpRequestBase> { CallBase = true };
            Response = new Mock<HttpResponseBase> { CallBase = true };
            SessionState = new Mock<HttpSessionStateBase> { CallBase = true };
            ServerUtility = new Mock<HttpServerUtilityBase> { CallBase = true };
            Application = new Mock<HttpApplicationStateBase> { CallBase = true };
            User = new Mock<IPrincipal> {CallBase = true};
            Identity = new Mock<IIdentity> {CallBase = true};
            UrlHelper = new Mock<UrlHelper>() {CallBase = true};
        }

        public MockHttpContextNewContainer(string userName, HttpVerbs verb, string userHostAddress)
        {
            // Instantiate variables
            HttpCookieCollection cookies = new HttpCookieCollection();
            NameValueCollection formData = new NameValueCollection();
            Hashtable items = new Hashtable();

            Mock<IPrincipal> user = new Mock<IPrincipal>();

            InitMocksOfMocks();

            HttpContext context = HttpContext();

            // --------------------------Setup Mock of Mocks 

            CreateMockContext(userName, context, user);

            //CreateMockContextSession();

            CreateMockUserIdentity(context, user);

            //MockRequest.Setup(r => r.Object).Returns(new HttpRequestWrapper(context.Request));
            HttpRequestBase request = new HttpRequestWrapper(context.Request);

            SetMockRequestCookies(cookies);

            SetMockRequestServerVariables(userHostAddress);

            SetMockRequestForm(formData);

            SetMockRequestUserHostAddress(userHostAddress);

            SetMockRequestRequestType(verb);

            MockRequest.Setup(r => r.Object.TotalBytes).Returns(request.TotalBytes);
            MockRequest.Setup(r => r.Object.Unvalidated).Returns(request.Unvalidated);
            MockRequest.Setup(r => r.Object.Url).Returns(request.Url);
            MockRequest.Setup(r => r.Object.UrlReferrer).Returns(request.UrlReferrer);
            MockRequest.Setup(r => r.Object.UserAgent).Returns(request.UserAgent);
            MockRequest.Setup(r => r.Object.UserHostName).Returns(request.UserHostName);
            MockRequest.Setup(r => r.Object.UserLanguages).Returns(request.UserLanguages);
            MockRequest.Setup(r => r.Object.AcceptTypes).Returns(request.AcceptTypes);
            MockRequest.Setup(r => r.Object.AnonymousID).Returns(request.AnonymousID);
            MockRequest.Setup(r => r.Object.ApplicationPath).Returns(request.ApplicationPath);
            MockRequest.Setup(r => r.Object.ClientCertificate).Returns(request.ClientCertificate);
            MockRequest.Setup(r => r.Object.ContentLength).Returns(request.ContentLength);
            MockRequest.Setup(r => r.Object.ContentType).Returns(request.ContentType);
            MockRequest.Setup(r => r.Object.CurrentExecutionFilePath).Returns(request.CurrentExecutionFilePath);
            MockRequest.Setup(r => r.Object.CurrentExecutionFilePathExtension).Returns(request.CurrentExecutionFilePathExtension);
            MockRequest.Setup(r => r.Object.FilePath).Returns(request.FilePath);
            MockRequest.Setup(r => r.Object.Files).Returns(request.Files);
            MockRequest.Setup(r => r.Object.Filter).Returns(request.Filter);
            MockRequest.Setup(r => r.Object.Headers).Returns(request.Headers);
            MockRequest.Setup(r => r.Object.HttpMethod).Returns(request.HttpMethod);
            MockRequest.Setup(r => r.Object.InputStream).Returns(request.InputStream);
            MockRequest.Setup(r => r.Object.IsAuthenticated).Returns(request.IsAuthenticated);
            MockRequest.Setup(r => r.Object.IsLocal).Returns(request.IsLocal);
            MockRequest.Setup(r => r.Object.IsSecureConnection).Returns(request.IsSecureConnection);
            MockRequest.Setup(r => r.Object.LogonUserIdentity).Returns(request.LogonUserIdentity);
            MockRequest.Setup(r => r.Object.Params).Returns(request.Params);
            MockRequest.Setup(r => r.Object.Path).Returns(request.Path);
            MockRequest.Setup(r => r.Object.PathInfo).Returns(request.PathInfo);
            MockRequest.Setup(r => r.Object.PhysicalPath).Returns(request.PhysicalPath);
            MockRequest.Setup(r => r.Object.QueryString).Returns(request.QueryString);
            MockRequest.Setup(r => r.Object.RawUrl).Returns(request.RawUrl);
            MockRequest.Setup(r => r.Object.ReadEntityBodyMode).Returns(request.ReadEntityBodyMode);
            MockRequest.Setup(r => r.Object.RequestContext).Returns(request.RequestContext);
            MockRequest.Setup(r => r.Object.TimedOutToken).Returns(request.TimedOutToken);

            //if (request.AppRelativeCurrentExecutionFilePath != null)
            //{
            //    MockRequest.Setup(r => r.Object.AppRelativeCurrentExecutionFilePath).Returns(
            //        request.AppRelativeCurrentExecutionFilePath); // potential null ref
            //}

            //if (request.Browser != null)
            //{
            //    MockRequest.Setup(r => r.Object.Browser).Returns(request.Browser);
            //}

            //MockRequest.Setup(r => r.Object.ContentEncoding).Returns(request.ContentEncoding);
            //MockRequest.Setup(r => r.Object.HttpChannelBinding).Returns(request.HttpChannelBinding);
            //MockRequest.Setup(r => r.Object.PhysicalApplicationPath).Returns(request.PhysicalApplicationPath);

            MockResponse.Setup(r => r.Object).Returns(new HttpResponseWrapper(context.Response));
            MockResponse.Setup(r => r.Object.Cookies).Returns(cookies);

            MockSessionState.Setup(ss => ss.Object).Returns(new HttpSessionStateWrapper(context.Session));
            //MockSessionState.Setup(s => s.Object.SessionID).Returns(Guid.NewGuid().ToString());

            MockServerUtility.Setup(su => su.Object).Returns(new HttpServerUtilityWrapper(context.Server));

            MockApplication.Setup(a => a.Object).Returns(new HttpApplicationStateWrapper(context.Application));

            SetMocksFromMocksofMocksV2();

        }

        public MockHttpContextNewContainer(string url, string queryString, string userHostAddress)
        {
            // Instantiate variables
            HttpCookieCollection cookies = new HttpCookieCollection();
            NameValueCollection formData = new NameValueCollection();

            // Instantiate Partial Mock of Mocks
            InitMocksOfMocks();

            HttpContext context = HttpContext(url);

            // --------------------------Setup Mock of Mocks 

            CreateMockContext(context);

            //CreateMockContextSession();

            SetupUserIdentity(context);

            //Request.Object.SetupUserHostAddress(userHostAddress);

            HttpRequestBase request = new HttpRequestWrapper(context.Request);
            //MockRequest.Setup(r => r.Object).Returns(new HttpRequestWrapper(context.Request));

            SetMockRequestServerVariables(userHostAddress);

            SetMockRequestUserHostAddress(userHostAddress);

            SetMockRequestCookies(cookies);

            SetMockRequestForm(formData);

            if(!string.IsNullOrEmpty(queryString))
            {
                MockRequest.Setup(r => r.Object.QueryString).Returns(HttpUtility.ParseQueryString(queryString));
                //MockRequest.Setup(r => r.Object.QueryString).Returns(request.QueryString);
            }

            if (!string.IsNullOrEmpty(url))
            {
                MockRequest.Setup(r => r.Object.Url).Returns(new Uri(url));
                //MockRequest.Setup(r => r.Object.Url).Returns(request.Url);
            }

            MockRequest.Setup(r => r.Object.TotalBytes).Returns(request.TotalBytes);
            MockRequest.Setup(r => r.Object.Unvalidated).Returns(request.Unvalidated);
            MockRequest.Setup(r => r.Object.UrlReferrer).Returns(request.UrlReferrer);
            MockRequest.Setup(r => r.Object.UserAgent).Returns(request.UserAgent);
            MockRequest.Setup(r => r.Object.UserHostName).Returns(request.UserHostName);
            MockRequest.Setup(r => r.Object.UserLanguages).Returns(request.UserLanguages);
            MockRequest.Setup(r => r.Object.AcceptTypes).Returns(request.AcceptTypes);
            MockRequest.Setup(r => r.Object.AnonymousID).Returns(request.AnonymousID);
            MockRequest.Setup(r => r.Object.ApplicationPath).Returns(request.ApplicationPath);
            MockRequest.Setup(r => r.Object.ClientCertificate).Returns(request.ClientCertificate);
            MockRequest.Setup(r => r.Object.ContentLength).Returns(request.ContentLength);
            MockRequest.Setup(r => r.Object.ContentType).Returns(request.ContentType);
            MockRequest.Setup(r => r.Object.CurrentExecutionFilePath).Returns(request.CurrentExecutionFilePath);
            MockRequest.Setup(r => r.Object.CurrentExecutionFilePathExtension).Returns(request.CurrentExecutionFilePathExtension);
            MockRequest.Setup(r => r.Object.FilePath).Returns(request.FilePath);
            MockRequest.Setup(r => r.Object.Files).Returns(request.Files);
            MockRequest.Setup(r => r.Object.Filter).Returns(request.Filter);
            MockRequest.Setup(r => r.Object.Headers).Returns(request.Headers);
            MockRequest.Setup(r => r.Object.HttpMethod).Returns(request.HttpMethod);
            MockRequest.Setup(r => r.Object.InputStream).Returns(request.InputStream);
            MockRequest.Setup(r => r.Object.IsAuthenticated).Returns(request.IsAuthenticated);
            MockRequest.Setup(r => r.Object.IsLocal).Returns(request.IsLocal);
            MockRequest.Setup(r => r.Object.IsSecureConnection).Returns(request.IsSecureConnection);
            MockRequest.Setup(r => r.Object.LogonUserIdentity).Returns(request.LogonUserIdentity);
            MockRequest.Setup(r => r.Object.Params).Returns(request.Params); 
            MockRequest.Setup(r => r.Object.Path).Returns(request.Path);
            MockRequest.Setup(r => r.Object.PathInfo).Returns(request.PathInfo);
            MockRequest.Setup(r => r.Object.PhysicalPath).Returns(request.PhysicalPath);
            MockRequest.Setup(r => r.Object.RawUrl).Returns(request.RawUrl);
            MockRequest.Setup(r => r.Object.ReadEntityBodyMode).Returns(request.ReadEntityBodyMode);
            MockRequest.Setup(r => r.Object.RequestContext).Returns(request.RequestContext);
            MockRequest.Setup(r => r.Object.RequestType).Returns(request.RequestType);
            MockRequest.Setup(r => r.Object.TimedOutToken).Returns(request.TimedOutToken);

            //if (request.AppRelativeCurrentExecutionFilePath != null)
            //{
            //    MockRequest.Setup(r => r.Object.AppRelativeCurrentExecutionFilePath).Returns(
            //        request.AppRelativeCurrentExecutionFilePath); // potential null ref
            //}

            //if (request.Browser != null)
            //{
            //    MockRequest.Setup(r => r.Object.Browser).Returns(request.Browser);
            //}

            //MockRequest.Setup(r => r.Object.ContentEncoding).Returns(request.ContentEncoding);
            //MockRequest.Setup(r => r.Object.HttpChannelBinding).Returns(request.HttpChannelBinding);
            //MockRequest.Setup(r => r.Object.PhysicalApplicationPath).Returns(request.PhysicalApplicationPath);
            
            MockResponse.Setup(r => r.Object).Returns(new HttpResponseWrapper(context.Response));
            //MockResponse.Setup(r => r.Object.Cookies).Returns(cookies);

            MockSessionState.Setup(ss => ss.Object).Returns(new HttpSessionStateWrapper(context.Session));
            //MockSessionState.Setup(s => s.Object.SessionID).Returns(Guid.NewGuid().ToString());

            MockServerUtility.Setup(su => su.Object).Returns(new HttpServerUtilityWrapper(context.Server));

            MockApplication.Setup(a => a.Object).Returns(new HttpApplicationStateWrapper(context.Application));

            SetMocksFromMocksofMocksV3();

        }

        public MockHttpContextNewContainer(HttpVerbs verb, HttpCookieCollection cookies, string userHostAddress, string cookieNameToSet)
        {
            // Instantiate variables
            cookies = cookies ?? new HttpCookieCollection();
            NameValueCollection formData = new NameValueCollection();
            Hashtable items = new Hashtable();

            InitMocksOfMocks();

            HttpContext context = HttpContext();

            // --------------------------Setup Mock of Mocks 

            CreateMockContext(context);

            //CreateMockContextSession();

            CreateMockUserIdentity(context);

            //MockRequest.Setup(r => r.Object).Returns(new HttpRequestWrapper(context.Request));
            HttpRequestBase request = new HttpRequestWrapper(context.Request);

            SetMockRequestUserHostAddress(userHostAddress);

            SetMockRequestRequestType(verb);

            SetMockRequestServerVariables(userHostAddress);

            SetMockRequestCookies(cookies);

            SetMockRequestForm(formData);

            MockRequest.Setup(r => r.Object.TotalBytes).Returns(request.TotalBytes);
            MockRequest.Setup(r => r.Object.Unvalidated).Returns(request.Unvalidated);
            MockRequest.Setup(r => r.Object.Url).Returns(request.Url);
            MockRequest.Setup(r => r.Object.UrlReferrer).Returns(request.UrlReferrer);
            MockRequest.Setup(r => r.Object.UserAgent).Returns(request.UserAgent);
            MockRequest.Setup(r => r.Object.UserHostName).Returns(request.UserHostName);
            MockRequest.Setup(r => r.Object.UserLanguages).Returns(request.UserLanguages);
            MockRequest.Setup(r => r.Object.AcceptTypes).Returns(request.AcceptTypes);
            MockRequest.Setup(r => r.Object.AnonymousID).Returns(request.AnonymousID);
            MockRequest.Setup(r => r.Object.ApplicationPath).Returns(request.ApplicationPath);
            MockRequest.Setup(r => r.Object.ClientCertificate).Returns(request.ClientCertificate);
            MockRequest.Setup(r => r.Object.ContentLength).Returns(request.ContentLength);
            MockRequest.Setup(r => r.Object.ContentType).Returns(request.ContentType);           
            MockRequest.Setup(r => r.Object.CurrentExecutionFilePath).Returns(request.CurrentExecutionFilePath);
            MockRequest.Setup(r => r.Object.CurrentExecutionFilePathExtension).Returns(request.CurrentExecutionFilePathExtension);
            MockRequest.Setup(r => r.Object.FilePath).Returns(request.FilePath);
            MockRequest.Setup(r => r.Object.Files).Returns(request.Files);
            MockRequest.Setup(r => r.Object.Filter).Returns(request.Filter);
            MockRequest.Setup(r => r.Object.Headers).Returns(request.Headers);
            MockRequest.Setup(r => r.Object.HttpMethod).Returns(request.HttpMethod);
            MockRequest.Setup(r => r.Object.InputStream).Returns(request.InputStream);
            MockRequest.Setup(r => r.Object.IsAuthenticated).Returns(request.IsAuthenticated);
            MockRequest.Setup(r => r.Object.IsLocal).Returns(request.IsLocal);
            MockRequest.Setup(r => r.Object.IsSecureConnection).Returns(request.IsSecureConnection);
            MockRequest.Setup(r => r.Object.LogonUserIdentity).Returns(request.LogonUserIdentity);
            MockRequest.Setup(r => r.Object.Params).Returns(request.Params);
            MockRequest.Setup(r => r.Object.Path).Returns(request.Path);
            MockRequest.Setup(r => r.Object.PathInfo).Returns(request.PathInfo);
            MockRequest.Setup(r => r.Object.PhysicalPath).Returns(request.PhysicalPath);
            MockRequest.Setup(r => r.Object.QueryString).Returns(request.QueryString);
            MockRequest.Setup(r => r.Object.RawUrl).Returns(request.RawUrl);
            MockRequest.Setup(r => r.Object.ReadEntityBodyMode).Returns(request.ReadEntityBodyMode);
            MockRequest.Setup(r => r.Object.RequestContext).Returns(request.RequestContext);
            MockRequest.Setup(r => r.Object.TimedOutToken).Returns(request.TimedOutToken);

            //if (request.AppRelativeCurrentExecutionFilePath != null)
            //{
            //    MockRequest.Setup(r => r.Object.AppRelativeCurrentExecutionFilePath).Returns(
            //        request.AppRelativeCurrentExecutionFilePath); // potential null ref
            //}

            //if (request.Browser != null)
            //{
            //    MockRequest.Setup(r => r.Object.Browser).Returns(request.Browser);
            //}

            //MockRequest.Setup(r => r.Object.ContentEncoding).Returns(request.ContentEncoding);
            //MockRequest.Setup(r => r.Object.HttpChannelBinding).Returns(request.HttpChannelBinding);
            //MockRequest.Setup(r => r.Object.PhysicalApplicationPath).Returns(request.PhysicalApplicationPath);
 
            
            MockResponse.Setup(r => r.Object).Returns(new HttpResponseWrapper(context.Response));

            MockResponse.Setup(r => r.Object.Cookies).Returns(cookies);

            SetMockResponseCookie(cookies, cookieNameToSet);

            MockSessionState.Setup(ss => ss.Object).Returns(new HttpSessionStateWrapper(context.Session));
            //MockSessionState.Setup(s => s.Object.SessionID).Returns(Guid.NewGuid().ToString());

            MockServerUtility.Setup(su => su.Object).Returns(new HttpServerUtilityWrapper(context.Server));

            MockApplication.Setup(a => a.Object).Returns(new HttpApplicationStateWrapper(context.Application));

            SetMocksFromMocksofMocks();

        }

        private void CreateMockContextSession()
        {
            var session = new Mock<HttpSessionStateBase>(MockBehavior.Loose);
            session.Setup(s => s.SessionID).Returns(Guid.NewGuid().ToString());
            MockContext.Setup(c => c.Object.Session).Returns(session.Object);
        }

        // PUBLIC

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

        public HttpContextBase GetHttpContext()
        {
            return Context.Object;
        }

        // PRIVATE

        private void SetMocksFromMocksofMocks()
        {
            // Assign Mocks Mock to Mock
            Context = Mock.Get(MockContext.Object.Object);
            User = Mock.Get(MockUser.Object.Object);
            Identity = Mock.Get(MockIdentity.Object.Object);
            Request = Mock.Get(MockRequest.Object.Object);
            Response = Mock.Get(MockResponse.Object.Object);
            SessionState = Mock.Get(MockSessionState.Object.Object);
            ServerUtility = Mock.Get(MockServerUtility.Object.Object);
            Application = Mock.Get(MockApplication.Object.Object);
        }

        private void SetMocksFromMocksofMocksV2()
        {
            // Assign Mocks Mock to Mock
            Context = Mock.Get(MockContext.Object.Object);
            User = Mock.Get(MockUser.Object.Object);
            Identity = Mock.Get(MockIdentity.Object.Object);
            Request = Mock.Get(MockRequest.Object.Object);
            Response = Mock.Get(MockResponse.Object.Object);
            SessionState = MockSessionState.Object;
            ServerUtility = MockServerUtility.Object;
            Application = MockApplication.Object;
        }

        private void SetMocksFromMocksofMocksV3()
        {
            // Assign Mocks Mock to Mock
            Context = MockContext.Object;
            User = Mock.Get(MockUser.Object.Object);
            Identity = Mock.Get(MockIdentity.Object.Object);
            Request = Mock.Get(MockRequest.Object.Object);
            Response = MockResponse.Object;
            SessionState = MockSessionState.Object;
            ServerUtility = MockServerUtility.Object;
            Application = MockApplication.Object;
        }


        private void SetMockRequestForm(NameValueCollection formData)
        {
            MockRequest.Setup(r => r.Object.Form).Returns(formData); // custom
        }


        private void SetMockRequestCookies(HttpCookieCollection cookies)
        {
            MockRequest.Setup(x => x.Object.Cookies).Returns(cookies); // custom
        }


        private void SetMockResponseCookie(HttpCookieCollection cookies, string cookieName)
        {
            MockResponse.Setup(r => r.Object.SetCookie(cookies[cookieName]));
        }


        private void SetMockRequestServerVariables(string userHostAddress)
        {
            //{ "SERVER_NAME", "localhost" },
            //{ "SCRIPT_NAME", "localhost" },
            //{ "SERVER_PORT", "80" },
            //{ "HTTPS", "www.melaos.com" },
            MockRequest.Setup(r => r.Object.ServerVariables).Returns(new NameValueCollection
                {
                    // custom
                    {"REMOTE_ADDR", userHostAddress},
                    {"REMOTE_HOST", userHostAddress}, // default ip location
                    {"HTTP_X_FORWARDED_FOR", userHostAddress}, // behind proxy or router ip location
                    {"HTTP_FORWARDED", userHostAddress} // behind proxy or router ip location
                });
        }


        private void SetMockRequestRequestType(HttpVerbs verb)
        {
            MockRequest.Setup(y => y.Object.RequestType).Returns(verb == HttpVerbs.Get ? "GET" : "POST"); // custom
        }


        private void SetMockRequestUserHostAddress(string userHostAddress)
        {
            MockRequest.Setup(r => r.Object.UserHostAddress).Returns(userHostAddress); // custom
        }


        private void CreateMockUserIdentity(HttpContext context)
        {
            // If FakeHttpContext created a User
            if (context.User != null)
            {
                // Use User
                MockUser.Setup(u => u.Object).Returns(context.User);
                //TODO should i check for and mock out User.Identity if its null
                // Use Identity
                MockIdentity.Setup(i => i.Object).Returns(context.User.Identity);
            }
            else
            {
                // Mock User
                Mock<IPrincipal> user = new Mock<IPrincipal>();
                MockUser.Setup(u => u.Object).Returns(user.Object);
                //TODO is mocking identity duplicating the effort of mocking the user
                // Mock Identity on User
                Mock<IIdentity> identity = new Mock<IIdentity>();
                MockUser.Setup(u => u.Object.Identity).Returns(identity.Object);
                // Mock Identity 
                MockIdentity.Setup(i => i.Object).Returns(identity.Object);
            }
        }

        private void CreateMockUserIdentity(HttpContext context, Mock<IPrincipal> user)
        {
            // If FakeHttpContext created a User
            if (context.User != null)
            {
                // Use User
                MockUser.Setup(u => u.Object).Returns(context.User);
                //TODO should i check for and mock out User.Identity if its null
                // Use Identity
                MockIdentity.Setup(i => i.Object).Returns(context.User.Identity);
            }
            else
            {
                // Mock User
                MockUser.Setup(u => u.Object).Returns(user.Object);
                //TODO is mocking identity duplicating the effort of mocking the user
                // Mock Identity on User
                Mock<IIdentity> identity = new Mock<IIdentity>();
                MockUser.Setup(u => u.Object.Identity).Returns(identity.Object);
                // Mock Identity 
                MockIdentity.Setup(i => i.Object).Returns(identity.Object);
            }
        }


        private static HttpContext GetHttpContext(HttpContextBase context)
        {
            var app = (HttpApplication)context.GetService(typeof(HttpApplication));
            return app.Context;
        }


        private static HttpContext HttpContext(string url = "http://google.com")
        {
            // Using HttpContext private constructor
            // Instantiate an HttpContext, HttpRequest, HttpResponse, and HttpContextSession
            HttpContext context = MockHttpContext.FakeHttpContext(url:url);
            // Assign the Session HttpContext to the static Request HttpContext
            System.Web.HttpContext.Current = context;
            return context;
        }


        private void InitMocksOfMocks()
        {
            // Instantiate Partial Mock of Mocks
            MockContext = new Mock<Mock<HttpContextBase>> {CallBase = true};
            MockUser = new Mock<Mock<IPrincipal>> {CallBase = true};
            MockIdentity = new Mock<Mock<IIdentity>> {CallBase = true};
            MockRequest = new Mock<Mock<HttpRequestBase>> {CallBase = true};
            MockResponse = new Mock<Mock<HttpResponseBase>> {CallBase = true};
            MockSessionState = new Mock<Mock<HttpSessionStateBase>> {CallBase = true};
            MockServerUtility = new Mock<Mock<HttpServerUtilityBase>> {CallBase = true};
            MockApplication = new Mock<Mock<HttpApplicationStateBase>> {CallBase = true};
        }
        

        private void CreateMockContext(string userName, HttpContext context, Mock<IPrincipal> user)
        {
            CreateMockContext(context);

            MockContext.Setup(c => c.Object.User).Returns(user.Object);
            MockContext.Setup(y => y.Object.User.Identity.IsAuthenticated).Returns(true);
            MockContext.Setup(y => y.Object.User.Identity.Name).Returns(userName);
        }
        
        private void CreateMockContext(HttpContext context)
        {
            MockContext.Setup(c => c.Object).Returns(new HttpContextWrapper(context));
        }


        private void SetupUserIdentity(HttpContext context)
        {
            // If FakeHttpContext created a User
            if (context.User != null)
            {
                // Use User
                MockUser.Setup(u => u.Object).Returns(context.User);
                //TODO should i check for and mock out User.Identity if its null
                // Use Identity
                MockIdentity.Setup(i => i.Object).Returns(context.User.Identity);
            }
            else
            {
                // Mock User
                Mock<IPrincipal> user = new Mock<IPrincipal>();
                MockUser.Setup(u => u.Object).Returns(user.Object);
                //TODO is mocking identity duplicating the effort of mocking the user
                // Mock Identity on User
                Mock<IIdentity> identity = new Mock<IIdentity>();
                MockUser.Setup(u => u.Object.Identity).Returns(identity.Object);
                // Mock Identity 
                MockIdentity.Setup(i => i.Object).Returns(identity.Object);
            }
        }
        
        #region " Mock Examples "
        //private HttpContextBase GetMockedHttpContext()
        //{
        //    var context = new Mock<HttpContextBase>();
        //    var request = new Mock<HttpRequestBase>();
        //    var response = new Mock<HttpResponseBase>();
        //    var session = new Mock<HttpSessionStateBase>();
        //    var server = new Mock<HttpServerUtilityBase>();
        //    var user = new Mock<IPrincipal>();
        //    var identity = new Mock<IIdentity>();
        //    var urlHelper = new Mock<UrlHelper>();

        //    var routes = new RouteCollection();
        //    MvcApplication.RegisterRoutes(routes);
        //    var requestContext = new Mock<RequestContext>();
        //    requestContext.Setup(x => x.HttpContext).Returns(context.Object);
        //    context.Setup(ctx => ctx.Request).Returns(request.Object);
        //    context.Setup(ctx => ctx.Response).Returns(response.Object);
        //    context.Setup(ctx => ctx.Session).Returns(session.Object);
        //    context.Setup(ctx => ctx.Server).Returns(server.Object);
        //    context.Setup(ctx => ctx.User).Returns(user.Object);
        //    user.Setup(ctx => ctx.Identity).Returns(identity.Object);
        //    identity.Setup(id => id.IsAuthenticated).Returns(true);
        //    identity.Setup(id => id.Name).Returns("test");
        //    request.Setup(req => req.Url).Returns(new Uri("http://www.google.com"));
        //    request.Setup(req => req.RequestContext).Returns(requestContext.Object);
        //    requestContext.Setup(x => x.RouteData).Returns(new RouteData());

        //    return context.Object;
        //}

        // Using Moq to Mock HttpContextBase
        //----------------------------------------
        //var controller = new MyController();
 
        //var server = new Mock<HttpServerUtilityBase>(MockBehavior.Loose);
        //var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
 
        //var request = new Mock<HttpRequestBase>(MockBehavior.Strict);            
        //request.Setup(r => r.UserHostAddress).Returns("127.0.0.1");
 
        //var session = new Mock<HttpSessionStateBase>();
        //session.Setup(s => s.SessionID).Returns(Guid.NewGuid().ToString());
 
        //var context = new Mock<HttpContextBase>();
        //context.SetupGet(c => c.Request).Returns(request.Object);
        //context.SetupGet(c => c.Response).Returns(response.Object);
        //context.SetupGet(c => c.Server).Returns(server.Object);
        //context.SetupGet(c => c.Session).Returns(session.Object);
 
        //controller.ControllerContext = new ControllerContext(context.Object, 
        //                            new RouteData(), controller);
        #endregion

    }
}
