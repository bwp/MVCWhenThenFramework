using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MVCWhenThenFramework
{
    public class ControllerTestContext<T> : IActionExpectation<T> where T : Controller
    {
        public T Controller { get; set; }
        public MockHttpContextContainer MockContext { get; set; }
        private ActionResult _result;
        private string _actionName;
        private List<object> _actionAttributes = new List<object>();

        ActionResult IActionExpectation<T>.Result { get { return _result; } }

        private readonly Queue<Action> _setupActions = new Queue<Action>();
        private readonly Queue<Action<T>> _setupActionsTakingController = new Queue<Action<T>>();
        private readonly Queue<Action<MockHttpContextContainer>> _setupHttp = new Queue<Action<MockHttpContextContainer>>();
        private readonly Queue<Action<Mock<HttpRequestBase>>> _setupRequest = new Queue<Action<Mock<HttpRequestBase>>>();

        /// <summary>
        /// Provide your external dependency setups here. This is where you should be doing your Mock['IDependency].Setup(...) stuff.
        /// </summary>
        /// <param name="setup">The setups action</param>
        /// <returns></returns>
        public ControllerTestContext<T> GivenDependenciesAreSetupAs(Action setup)
        {
            _setupActions.Enqueue(setup);
            return this;
        }
        /// <summary>
        /// Provide your external dependency setups here, you are given the controller instance. 
        /// This is where you should be doing your Mock['IDependency].Setup(...) stuff.
        /// </summary>
        /// <param name="setup">The setups action</param>
        /// <returns></returns>
        public ControllerTestContext<T> GivenDependenciesAreSetupAs(Action<T> setup)
        {
            _setupActionsTakingController.Enqueue(setup);
            return this;
        }

        /// <summary>
        /// Provide your external dependency setups here, you are given the controller instance. 
        /// This is where you should be doing your setups on the controller, stuff like ModelState etc.
        /// </summary>
        /// <param name="setup">The setups action</param>
        /// <returns></returns>
        public ControllerTestContext<T> Given(Action<T> setup)
        {
            _setupActionsTakingController.Enqueue(setup);
            return this;
        }

        /// <summary>
        /// Provide any setups Mock setups or otherwise on the HttpContext or related objects
        /// </summary>
        /// <param name="httpSetup">The setups action</param>
        /// <returns></returns>
        public ControllerTestContext<T> GivenHttp(Action<MockHttpContextContainer> httpSetup)
        {
            _setupHttp.Enqueue(httpSetup);
            return this;
        }

        /// <summary>
        /// Provide any setups Mock setups or otherwise on the HttpRequestBase
        /// </summary>
        /// <param name="httpSetup">The setups action</param>
        /// <returns></returns>
        public ControllerTestContext<T> GivenRequest(Action<Mock<HttpRequestBase>> httpSetup)
        {
            _setupRequest.Enqueue(httpSetup);
            return this;
        }

        /// <summary>
        /// Provide your controller action call here, this will execute the action immediately
        /// </summary>
        /// <param name="actionCalled">The controller action</param>
        /// <returns></returns>
        public IActionExpectation<T> When(Expression<Func<T, ActionResult>> actionCalled)
        {
            RunGivenSetups();

            var methodInfo = (actionCalled.Body as MethodCallExpression).Method;
            _actionName = methodInfo.Name;
            _actionAttributes = methodInfo.GetCustomAttributes(false).ToList();

            _result = actionCalled.Compile()(Controller);

            return this;
        }

        /// <summary>
        /// Will execute the setups provided in Given methods
        /// </summary>
        /// <returns></returns>
        public ControllerTestContext<T> RunGivenSetups()
        {
            while (_setupActions.Count > 0)
                _setupActions.Dequeue()();
            while (_setupActionsTakingController.Count > 0)
                _setupActionsTakingController.Dequeue()(Controller);
            while (_setupHttp.Count > 0)
                _setupHttp.Dequeue()(MockContext);
            while (_setupRequest.Count > 0)
                _setupRequest.Dequeue()(MockContext.Request);

            return this;
        }

        private void AssertWeHaveAResult()
        {
            if (_result == null)
                throw new Exception("The result is null, ensure you have called When() before any expectations");
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectResponse(Predicate<HttpResponseBase> responseMatch)
        {
            Assert.IsTrue(responseMatch(MockContext.Response.Object), "ExpectResponse assertion failed");
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectHttp(Predicate<HttpContextBase> httpMatch)
        {
            Assert.IsTrue(httpMatch(MockContext.Context.Object), "ExpectHttp assertion failed");
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectViewData(Predicate<ViewDataDictionary> viewDataMatch)
        {
            Assert.IsTrue(viewDataMatch(Controller.ViewData), "ExpectViewData assertion failed");
            return this;
        }

        public IActionExpectation<T> ThenExpectModelState(Predicate<ModelStateDictionary> modelStateMatch)
        {
            Assert.IsTrue(modelStateMatch(Controller.ModelState), "ThenExpectModelState assertion failed");
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectModel<TModel>(Predicate<TModel> modelMatch)
        {
            Assert.IsTrue(modelMatch((TModel)Controller.ViewData.Model), "ExpectModel assertion failed");
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectViewBag(Predicate<object> viewBagMatch)
        {
            Assert.IsTrue(viewBagMatch(Controller.ViewBag), "ThenExpectViewBag assertion failed");
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectTempData(Predicate<TempDataDictionary> tempDataMatch)
        {
            Assert.IsTrue(tempDataMatch(Controller.TempData), "ExpectTempData assertion failed");
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectResult(Predicate<ActionResult> resultMatch)
        {
            AssertWeHaveAResult();
            Assert.IsTrue(resultMatch(_result), "ExpectResult assertion failed");
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectViewResult(Predicate<ViewResult> resultMatch)
        {
            AssertWeHaveAResult();
            Assert.IsTrue(resultMatch(_result as ViewResult), "ExpectViewResult assertion failed");
            return this;
        }

        private ViewResult SetViewName(ViewResult view)
        {
            //When returing ViewResults using the controler method View() the ViewName property is empty
            //untill the ViewResult is executed, so we provide the actioName as the 
            //viewResult.Name to make testing easier
            if (string.IsNullOrEmpty(view.ViewName))
                view.ViewName = _actionName;
            return view;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectViewResult(Func<ViewResult, string, bool> resultMatch)
        {
            AssertWeHaveAResult();
            Assert.IsTrue(resultMatch(SetViewName(_result as ViewResult), _actionName), "ExpectViewResult with actionName assertion failed");
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectViewResultWhereNameIsActionNameOrEmpty()
        {
            AssertWeHaveAResult();

            var view = _result as ViewResult;

            Func<bool> isActionNameOrEmpty =
                () => view.ViewName == string.Empty ||
                      view.ViewName.Equals(_actionName, StringComparison.OrdinalIgnoreCase);

            Assert.IsTrue(isActionNameOrEmpty(), "ExpectViewNameIsActionNameOrEmpty assertion failed");
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectContentResult(Predicate<ContentResult> resultMatch)
        {
            AssertWeHaveAResult();
            Assert.IsTrue(resultMatch(_result as ContentResult), "ExpectContentResult assertion failed");
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectRedirectResult(Predicate<RedirectResult> resultMatch)
        {
            AssertWeHaveAResult();
            Assert.IsTrue(resultMatch(_result as RedirectResult), "ExpectRedirectResult  assertion failed");
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenVerifyDependencies(Action verify)
        {
            AssertWeHaveAResult();
            verify();
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectVerb(params HttpVerbs[] verbs)
        {
            foreach (var verb in verbs)
            {
                switch (verb)
                {
                    case HttpVerbs.Get:
                        Assert.IsTrue(_actionAttributes.Any(v => v is HttpGetAttribute), "Expected HttpVerb: " + verb);
                        break;
                    case HttpVerbs.Post:
                        Assert.IsTrue(_actionAttributes.Any(v => v is HttpPostAttribute), "Expected HttpVerb: " + verb);
                        break;
                    case HttpVerbs.Put:
                        Assert.IsTrue(_actionAttributes.Any(v => v is HttpPutAttribute), "Expected HttpVerb: " + verb);
                        break;
                    case HttpVerbs.Delete:
                        Assert.IsTrue(_actionAttributes.Any(v => v is HttpDeleteAttribute), "Expected HttpVerb: " + verb);
                        break;
                }
            }
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpectRedirectToRouteResult(Predicate<RedirectToRouteResult> resultMatch)
        {
            AssertWeHaveAResult();
            Assert.IsTrue(resultMatch(_result as RedirectToRouteResult), "ExpectRedirectToRouteResult  assertion failed");
            return this;
        }

        IActionExpectation<T> IActionExpectation<T>.ThenExpect(Predicate<T> controllerMatch)
        {
            AssertWeHaveAResult();
            Assert.IsTrue(controllerMatch(Controller));
            return this;
        }
    }
}