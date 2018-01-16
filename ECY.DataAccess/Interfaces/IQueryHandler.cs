namespace ECY.DataAccess.Interfaces
{
    /// <summary>
    /// Interface for processing a database query
    /// </summary>
    /// <typeparam name="TQuery">Query object to execute</typeparam>
    /// <typeparam name="TResult">Result model of the query</typeparam>
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Executes the database query
        /// </summary>
        /// <param name="query">Query to be executed</param>
        /// <returns></returns>
        TResult Query(TQuery query);
    }
}
