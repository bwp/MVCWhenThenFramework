namespace MVCWhenThenFramework
{
    public interface IRouteExpectation
    {
        /// <summary>
        /// Provide a set of route values to ecpect as an anonymous type.
        /// </summary>
        /// <param name="expectations"></param>
        /// <returns></returns>
        /// <example> .Expect(new { controller = "Home", action = "Index" }) </example>
        IRouteExpectation Expect(object expectations);
    }
}