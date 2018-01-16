namespace ECY.DataAccess.Interfaces
{
    /// <summary>
    /// Query interface
    /// </summary>
    /// <typeparam name="TResult">Type of the result model</typeparam>
    public interface IQuery<out TResult>
    {
        /// <summary>
        /// Execute the query
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        TResult Execute(ISession session);
    }
}
