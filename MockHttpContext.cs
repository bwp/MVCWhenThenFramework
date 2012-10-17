using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using Microsoft.CSharp.RuntimeBinder;
using Moq;

namespace MVCWhenThenFramework
{
    public static class MockHttpContext
    {
        /// <summary>  
        /// Specify the flags for accessing members  
        /// </summary>  
        private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        private static Mock<Mock<HttpRequest>> MyMockHttpRequest { get; set; }

        public static object GetDynamicMember(object myObject, string memberName)
        {
            var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, memberName, myObject.GetType(),
                new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
            return callsite.Target(callsite, myObject);
        }

        public static HttpRequest CreateHttpRequestPartialMock(string url, string queryString, string userHostAddress)
        {
            MyMockHttpRequest = new Mock<Mock<HttpRequest>>(MockBehavior.Loose);

            // Instantiate Concrete instance
            var httpRequest = new HttpRequest("", url, queryString);


            // Note that the type of 'wrapper' is dynamic  
            dynamic request = new AccessPrivateWrapper(httpRequest);

            // Hit the property ServerVariables to fire late binding
            var property = request.ServerVariables;

            // attempt to access RequestWorker to verify its null
            var wr = request._wr;


            // Grab assembly reference
            var assembly = typeof(HttpRequest).Assembly;


            // Grab reference to internal HttpServerVarsCollection 
            var type = assembly.GetType("System.Web.HttpServerVarsCollection");
            // Grab type reference to internal IIS7WorkerRequest
            var type2 = assembly.GetType("System.Web.Hosting.IIS7WorkerRequest");
            var type3 = assembly.GetType("System.Web.HttpValueCollection");

            var serverVariables = InstantiateServerVariablesViaReflection(type, wr, httpRequest);
            
            //---------------------------------------------
            //// Not supported
            //MethodInfo add = typeof(NameValueCollection).GetMethod("Add", flags,
            //                                       null, CallingConventions.Standard,
            //                                       new Type[] {typeof (NameValueCollection)},
            //                                       null);

            //add.Invoke(serverVariables as object, new object[] {serverVar});
            ////add.Invoke(serverVariables as object, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] {});
            //---------------------------------------------

            // Unlock and Add ServerVariables

            AddServerVariableViaReflection("REMOTE_ADDR", userHostAddress, ref serverVariables, type3); // alternate method - SetServerVariableManagedOnly
            AddServerVariableViaReflection("REMOTE_HOST", userHostAddress, ref serverVariables, type3);// default ip location
            AddServerVariableViaReflection("HTTP_X_FORWARDED_FOR", userHostAddress, ref serverVariables, type3); // behind proxy or router ip location
            AddServerVariableViaReflection("HTTP_FORWARDED", userHostAddress, ref serverVariables, type3); // behind proxy or router ip location         

            // Relock ServerVariables
            UseReflectionToLockServerVariable(ref serverVariables, type3);


            // Assign our collection instance to the private field of our concrete request
            request._serverVariables = serverVariables;

            //// Update our fields from the serverVariables collection 
            //MethodInfo fillInServerVariables = typeof(HttpRequest).GetMethod("FillInServerVariablesCollection",
            //                                                                  BindingFlags.NonPublic |
            //                                                                  BindingFlags.Instance,
            //                                                                  null, CallingConventions.Standard,
            //                                                                  new Type[0],
            //                                                                  null);

            #region " Clone HttpRequest "

            ////request.FillInServerVariablesCollection();

            ////Object bob = new object();
            ////GetDynamicMember(bob, "_serverVariables");

            ////myMockHttpRequest.Setup(r => r.Object).Returns(httpRequest);

            ////_myMockHttpRequest.Setup(r => r.Object.UserHostAddress).Returns(userHostAddress); // custom

            ////if (!string.IsNullOrEmpty(queryString))
            ////{
            ////    MyMockHttpRequest.Setup(r => r.Object.QueryString).Returns(HttpUtility.ParseQueryString(queryString));
            ////    //MockRequest.Setup(r => r.Object.QueryString).Returns(request.QueryString);
            ////}

            ////if (!string.IsNullOrEmpty(url))
            ////{
            ////    MyMockHttpRequest.SetupGet(r => r.Object.Url).Returns(new Uri(url));
            ////    //MockRequest.Setup(r => r.Object.Url).Returns(request.Url);
            ////}

            //////{ "SERVER_NAME", "localhost" },
            //////{ "SCRIPT_NAME", "localhost" },
            //////{ "SERVER_PORT", "80" },
            //////{ "HTTPS", "www.melaos.com" },
            ////MyMockHttpRequest.Setup(r => r.Object.ServerVariables).Returns(new NameValueCollection
            ////    {
            ////        // custom
            ////        {"REMOTE_ADDR", userHostAddress},
            ////        {"REMOTE_HOST", userHostAddress}, // default ip location
            ////        {"HTTP_X_FORWARDED_FOR", userHostAddress}, // behind proxy or router ip location
            ////        {"HTTP_FORWARDED", userHostAddress} // behind proxy or router ip location
            ////    });

            //MyMockHttpRequest.Setup(r => r.Object.TotalBytes).Returns(httpRequest.TotalBytes);
            //MyMockHttpRequest.Setup(r => r.Object.Unvalidated).Returns(httpRequest.Unvalidated);
            ////MyMockHttpRequest.Setup(r => r.Object.Unvalidated).Returns(new UnvalidatedRequestValuesWrapper(httpRequest.Unvalidated));
            //MyMockHttpRequest.Setup(r => r.Object.UrlReferrer).Returns(httpRequest.UrlReferrer);
            //MyMockHttpRequest.Setup(r => r.Object.UserAgent).Returns(httpRequest.UserAgent);
            //MyMockHttpRequest.Setup(r => r.Object.UserHostName).Returns(httpRequest.UserHostName);
            //MyMockHttpRequest.Setup(r => r.Object.UserLanguages).Returns(httpRequest.UserLanguages);
            //MyMockHttpRequest.Setup(r => r.Object.AcceptTypes).Returns(httpRequest.AcceptTypes);
            //MyMockHttpRequest.Setup(r => r.Object.AnonymousID).Returns(httpRequest.AnonymousID);
            //MyMockHttpRequest.Setup(r => r.Object.ApplicationPath).Returns(httpRequest.ApplicationPath);
            //MyMockHttpRequest.Setup(r => r.Object.ClientCertificate).Returns(httpRequest.ClientCertificate);
            //MyMockHttpRequest.Setup(r => r.Object.ContentLength).Returns(httpRequest.ContentLength);
            //MyMockHttpRequest.Setup(r => r.Object.ContentType).Returns(httpRequest.ContentType);
            //MyMockHttpRequest.Setup(r => r.Object.Cookies).Returns(httpRequest.Cookies);
            //MyMockHttpRequest.Setup(r => r.Object.CurrentExecutionFilePath).Returns(httpRequest.CurrentExecutionFilePath);
            //MyMockHttpRequest.Setup(r => r.Object.CurrentExecutionFilePathExtension).Returns(httpRequest.CurrentExecutionFilePathExtension);
            //MyMockHttpRequest.Setup(r => r.Object.FilePath).Returns(httpRequest.FilePath);
            //MyMockHttpRequest.Setup(r => r.Object.Files).Returns(httpRequest.Files);
            ////MyMockHttpRequest.Setup(r => r.Object.Files).Returns(new HttpFileCollectionWrapper(httpRequest.Files));
            //MyMockHttpRequest.Setup(r => r.Object.Filter).Returns(httpRequest.Filter);
            //MyMockHttpRequest.Setup(r => r.Object.Form).Returns(httpRequest.Form);
            //MyMockHttpRequest.Setup(r => r.Object.Headers).Returns(httpRequest.Headers);
            //MyMockHttpRequest.Setup(r => r.Object.HttpMethod).Returns(httpRequest.HttpMethod);
            //MyMockHttpRequest.Setup(r => r.Object.InputStream).Returns(httpRequest.InputStream);
            //MyMockHttpRequest.Setup(r => r.Object.IsAuthenticated).Returns(httpRequest.IsAuthenticated);
            //MyMockHttpRequest.Setup(r => r.Object.IsLocal).Returns(httpRequest.IsLocal);
            //MyMockHttpRequest.Setup(r => r.Object.IsSecureConnection).Returns(httpRequest.IsSecureConnection);
            //MyMockHttpRequest.Setup(r => r.Object.LogonUserIdentity).Returns(httpRequest.LogonUserIdentity);
            //MyMockHttpRequest.Setup(r => r.Object.Params).Returns(httpRequest.Params);
            //MyMockHttpRequest.Setup(r => r.Object.Path).Returns(httpRequest.Path);
            //MyMockHttpRequest.Setup(r => r.Object.PathInfo).Returns(httpRequest.PathInfo);
            //MyMockHttpRequest.Setup(r => r.Object.PhysicalPath).Returns(httpRequest.PhysicalPath);
            //MyMockHttpRequest.Setup(r => r.Object.RawUrl).Returns(httpRequest.RawUrl);
            //MyMockHttpRequest.Setup(r => r.Object.ReadEntityBodyMode).Returns(httpRequest.ReadEntityBodyMode);
            //MyMockHttpRequest.Setup(r => r.Object.RequestContext).Returns(httpRequest.RequestContext);
            //MyMockHttpRequest.Setup(r => r.Object.RequestType).Returns(httpRequest.RequestType);
            //MyMockHttpRequest.Setup(r => r.Object.TimedOutToken).Returns(httpRequest.TimedOutToken);

            ////if (request.AppRelativeCurrentExecutionFilePath != null)
            ////{
            ////    MockRequest.Setup(r => r.Object.AppRelativeCurrentExecutionFilePath).Returns(
            ////        request.AppRelativeCurrentExecutionFilePath); // potential null ref
            ////}

            ////if (request.Browser != null)
            ////{
            ////    MockRequest.Setup(r => r.Object.Browser).Returns(request.Browser);
            ////}

            ////MockRequest.Setup(r => r.Object.ContentEncoding).Returns(request.ContentEncoding);
            ////MockRequest.Setup(r => r.Object.HttpChannelBinding).Returns(request.HttpChannelBinding);
            ////MockRequest.Setup(r => r.Object.PhysicalApplicationPath).Returns(request.PhysicalApplicationPath);

            //Mock<HttpRequest> mockHttpRequest = MyMockHttpRequest.Object;

            ////mockHttpRequest.Setup(r => r.UserHostAddress).Returns(userHostAddress);

            #endregion

            return httpRequest; //mockHttpRequest.Object;

        }

        private static void AddServerVariableViaReflection(string variableName, string userHostAddress, ref dynamic serverVariables, Type unlockCollectionType)
        {

            UseReflectionToUnlockServerVariable(ref serverVariables, unlockCollectionType);

            MethodInfo add = typeof(NameValueCollection).GetMethod("BaseSet", Flags,
                                                   null, CallingConventions.Standard,
                                                   new[] { typeof(string), typeof(object) },
                                                   null);

                        // Grab assembly reference
            var assembly = typeof(HttpRequest).Assembly;


            // Grab reference to internal HttpServerVarsCollectionEntry 
            var type = assembly.GetType("System.Web.HttpServerVarsCollectionEntry");

            var constructor = type.GetConstructor(Flags, null, CallingConventions.Standard, new[] { typeof(string), typeof(string) }, null);

            dynamic serverVarEntry = constructor.Invoke(new object[] {variableName, userHostAddress});

            add.Invoke(serverVariables as object, Flags, null, new object[] {variableName, serverVarEntry}, null);

            //add.Invoke(serverVariables as object, new object[] {serverVar});
            ////add.Invoke(serverVariables as object, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] {});

            //// Add ServerVariables
            //type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance,
            //               null, CallingConventions.Standard,
            //               new[] { typeof(String), typeof(String) },
            //               null).Invoke(serverVariables as object, new object[] { variableNAme, userHostAddress });
        }

        private static dynamic InstantiateServerVariablesViaReflection(Type type, dynamic wr, HttpRequest httpRequest)
        {
            // Grab reference to private HttpServerVarsCollection constructor
            var constructor = type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, CallingConventions.Standard,
                new[] { typeof(HttpWorkerRequest), typeof(HttpRequest) },
                null);

            //HttpServerVarsCollection => HttpValueCollection => NameValueCollection => NameObjectCollectionBase => ICollection, IEnumerable, ISerializable, IDeserializationCallback

            // Instantiate instance of HttpServerVarsCollection
            dynamic serverVariables = constructor.Invoke(new object[] { wr, httpRequest });
            return serverVariables;
        }

        private static void UseReflectionToLockServerVariable(ref dynamic serverVariables, Type type)
        {
            // Relock ServerVariables
            MethodInfo makeReadOnly = type.GetMethod("MakeReadOnly", BindingFlags.NonPublic | BindingFlags.Instance,
                                                     null, CallingConventions.Standard,
                                                     new Type[0],
                                                     null);
            
            makeReadOnly.Invoke(serverVariables as object, BindingFlags.NonPublic | BindingFlags.Instance, null,
                              null, null);

            // Make collection writable via Property (NameValueCollection level)
            PropertyInfo isReadOnly = typeof(NameValueCollection).GetProperty("IsReadOnly", Flags);
            isReadOnly.SetValue(serverVariables, true);

            //// Make collection writable via Field (NameValueCollection level)
            //FieldInfo readOnly = typeof(NameObjectCollectionBase).GetField("_readOnly", flags);
            //Debug.Assert(readOnly != null, "readOnly != null");
            //readOnly.SetValue(serverVariables, true, flags, null, null);
        }

        private static void UseReflectionToUnlockServerVariable(ref dynamic serverVariables, Type type)
        {


            // Make collection writable via Method (HttpValueCollection level)
            MethodInfo makeWritable = type.GetMethod("MakeReadWrite", BindingFlags.NonPublic | BindingFlags.Instance,
                                                      null, CallingConventions.Standard,
                                                      new Type[0],
                                                      null);

            makeWritable.Invoke(serverVariables as object, BindingFlags.NonPublic | BindingFlags.Instance, null,
                                null, null);

            // Make collection writable via Property (NameValueCollection level)
            PropertyInfo isReadOnly = typeof(NameValueCollection).GetProperty("IsReadOnly", Flags);
            isReadOnly.SetValue(serverVariables, false);

            //// Make collection writable via Field (NameValueCollection level)
            //FieldInfo readOnly = typeof(NameObjectCollectionBase).GetField("_readOnly", flags);
            //Debug.Assert(readOnly != null, "readOnly != null");
            //readOnly.SetValue(serverVariables, false, flags, null, null);

        }

        public static HttpContext FakeHttpContext(string userHostAddress, string url = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                url = "http://google.com";
            }

            string queryString;

            int iqs = url.IndexOf('?');

            if (iqs >= 0)
            {
                queryString = (iqs < url.Length - 1) ? url.Substring(iqs + 1) : "";
            }else
            {
                queryString = "";
            }

            var httpRequest = CreateHttpRequestPartialMock(url, queryString, userHostAddress);

            //var httpRequest = new HttpRequest("", url, queryString);

            //NameValueCollection serverVariables = new NameValueCollection();
            //httpRequest.ServerVariables.Add("REMOTE_ADDR", userHostAddress); // default ip location
            //httpRequest.ServerVariables.Add("HTTP_X_FORWARDED_FOR", userHostAddress); // behind proxy or router ip location
            //httpRequest.ServerVariables.Add("HTTP_FORWARDED", userHostAddress); // behind proxy or router ip location

            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                                 new HttpStaticObjectsCollection(), 10, true,
                                                                 HttpCookieMode.AutoDetect,
                                                                 SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, CallingConventions.Standard,
                new[] { typeof(HttpSessionStateContainer) },
                null)
                .Invoke(new object[] { sessionContainer });

            //Note that the type of 'wrapper' is dynamic  
            //dynamic request = new AccessPrivateWrapper(httpRequest);
            //request.UserHostAddress = userHostAddress;

            return httpContext;
        }

        public static HttpContextBase CustomFakeHttpContext(Controller current)
        {

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                     new HttpStaticObjectsCollection(), 10, true,
                                                     HttpCookieMode.AutoDetect,
                                                     SessionStateMode.InProc, false);

            current.HttpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, CallingConventions.Standard,
                new[] { typeof(HttpSessionStateContainer) },
                null)
                .Invoke(new object[] { sessionContainer });

            return current.HttpContext;
        }

        public static void InvokeBaseClass(Type baseClassType, string methodName, NameValueCollection myCollection, ref dynamic baseClassInstance)
        {
            object[] serverVars = new object[] { myCollection };

            baseClassType.GetMethod(methodName, new[] { typeof(NameValueCollection) }).Invoke(baseClassInstance as object, serverVars);
        }

        public static void SetServerVariable(Type baseClassType, string methodName, string name, string value, ref dynamic baseClassInstance)
        {
            object[] serverVars = new[] { name, value };

            MethodInfo method = baseClassType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance,
                null, CallingConventions.Standard,
                new Type[] { typeof(String), typeof(String) },
                null);

            //.Invoke(baseClassInstance as object, serverVars);
        }

    }
}
