namespace ECY.DataAccess.Interfaces
{
    /// <summary>
    /// Interface for non queries
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Execute non-query
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        object Execute(ISession session);
    }
}
