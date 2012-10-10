using System;
using System.Web;
using System.Web.Mvc;

namespace MVCWhenThenFramework
{
    public interface IActionExpectation<T>
    {
        /// <summary>
        /// This is the actual result returned by your call in When(...)
        /// </summary>
        ActionResult Result { get; }
        /// <summary>
        /// Asserts an expectation on the HttpResponse
        /// </summary>
        /// <param name="responseMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectResponse(Predicate<HttpResponseBase> responseMatch);
        /// <summary>
        /// Asserts an expectation on the HttpContext
        /// </summary>
        /// <param name="httpMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectHttp(Predicate<HttpContextBase> httpMatch);
        /// <summary>
        /// Asserts an expectation on the ViewData
        /// </summary>
        /// <param name="viewDataMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectViewData(Predicate<ViewDataDictionary> viewDataMatch);
        /// <summary>
        /// Asserts an expectation on the ModelState
        /// </summary>
        /// <param name="modelStateMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectModelState(Predicate<ModelStateDictionary> modelStateMatch);
        /// <summary>
        /// Asserts an expectation on the ViewData.Model
        /// </summary>
        /// <param name="modelMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectModel<TModel>(Predicate<TModel> modelMatch);
        /// <summary>
        /// Asserts an expectation on the TempData
        /// </summary>
        /// <param name="tempDataMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectTempData(Predicate<TempDataDictionary> tempDataMatch);
        /// <summary>
        /// Asserts an expectation on the ActionResult
        /// </summary>
        /// <param name="resultMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectResult(Predicate<ActionResult> resultMatch);
        /// <summary>
        /// Asserts an expectation on the ViewResult
        /// </summary>
        /// <param name="resultMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectViewResult(Predicate<ViewResult> resultMatch);
        /// <summary>
        /// Asserts an expectation on the ViewResult providing the name of the action you called in 'when'. This method will also
        /// set the ViewResult.ViewName property to be equal to the action name you called in When() to allow easy equality comparison.
        /// </summary>
        /// <param name="resultMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectViewResult(Func<ViewResult, string, bool> resultMatch);
        /// <summary>
        /// Asserts an expectation on the ViewResult.ViewName, asserting it is empty or equal to the name of the action you called in When()
        /// </summary>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectViewResultWhereNameIsActionNameOrEmpty();
        /// <summary>
        /// Asserts an expectation on the ContentResult
        /// </summary>
        /// <param name="resultMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectContentResult(Predicate<ContentResult> resultMatch);
        /// <summary>
        /// Asserts an expectation on the RedirectResult
        /// </summary>
        /// <param name="resultMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectRedirectResult(Predicate<RedirectResult> resultMatch);
        /// <summary>
        /// Asserts an expectation on the RedirectResult
        /// </summary>
        /// <param name="resultMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectRedirectToRouteResult(Predicate<RedirectToRouteResult> resultMatch);
        /// <summary>
        /// You should provide any assertions or verifications on your external dependencies here
        /// </summary>
        /// <param name="verify"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenVerifyDependencies(Action verify);
        /// <summary>
        /// Asserts an expectation on the HttpVerbs contained on the action you called in When()
        /// </summary>
        /// <param name="verbs"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectVerb(params HttpVerbs[] verbs);
        /// <summary>
        /// Asserts an expectation on the controller state.
        /// </summary>
        /// <param name="controllerMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpect(Predicate<T> controllerMatch);
        /// <summary>
        /// Asserts an expectation on the dynamic viewbag of the controller
        /// </summary>
        /// <param name="viewBagMatch"></param>
        /// <returns></returns>
        IActionExpectation<T> ThenExpectViewBag(Predicate<object> viewBagMatch);
    }
}