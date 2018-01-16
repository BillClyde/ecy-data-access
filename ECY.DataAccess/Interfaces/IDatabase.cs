namespace ECY.DataAccess.Interfaces
{
    /// <summary>
    /// Overall database interface. Deprecated, use ICommandHandler and IQueryHandler instead
    /// </summary>
    [System.Obsolete("Use command and query handlers instead")]
    public interface IDatabase
    {
        /// <summary>
        /// Typed query
        /// </summary>
        /// <typeparam name="T">Query return model</typeparam>
        /// <param name="query">Query to execute</param>
        /// <returns></returns>
        [System.Obsolete("Use query handler instead")]
        T Query<T>(IQuery<T> query);
        /// <summary>
        /// Database command
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns></returns>
        [System.Obsolete("Use command handler instead")]
        object Execute(ICommand command);
    }
}
