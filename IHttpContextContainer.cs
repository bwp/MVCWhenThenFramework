using System.Web;
using Moq;

namespace MVCWhenThenFramework
{
    public interface IHttpContextContainer
    {
        HttpContext HttpContext { get; set; }
        HttpRequest Request { get; set; }
        HttpResponse Response { get; set; }
        HttpServerUtility ServerUtility { get; set; }
        HttpApplicationState Application { get; set; }
    }

    public class VirtualContextContainer : IHttpContextContainer
    {
        public HttpContext HttpContext { get; set; }

        public virtual HttpContextBase HttpContextBase
        {
            get
            {
              return new HttpContextWrapper(HttpContext); 
    
            }
            set { throw new System.NotImplementedException(); }
        }

        public HttpRequest Request { get; set; }
        public HttpRequestBase RequestBase { get; set; }
        public HttpResponse Response { get; set; }
        public HttpResponseBase ResponseBase { get; set; }
        public HttpSessionStateBase SessionState { get; set; }
        public HttpServerUtility ServerUtility { get; set; }
        public HttpServerUtilityBase ServerUtilityBase { get; set; }
        public HttpApplicationState Application { get; set; }
        public HttpApplicationStateBase ApplicationStateBase { get; set; }
    }



    public static class ContextContainer
    {
        public static Mock<IHttpContextContainer> GetMockContextContainer()
        {
            Mock<IHttpContextContainer> contextContainer = new Mock<IHttpContextContainer>();

            return contextContainer;
        }
    }

} 