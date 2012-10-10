namespace MVCWhenThenFramework
{
    public interface ISpecifyUrl
    {
        /// <summary>
        /// Provide the url to exercise you wish to test against
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        IRouteExpectation GivenUrl(string url);
    }
}