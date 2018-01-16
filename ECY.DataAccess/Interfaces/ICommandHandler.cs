namespace ECY.DataAccess.Interfaces
{
    /// <summary>
    /// Handler for processesing database commands
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public interface ICommandHandler<TCommand>
    {
        /// <summary>
        /// Execute method for the database commands.
        /// </summary>
        /// <param name="command">Command to process</param>
        /// <returns></returns>
        object Execute(TCommand command);
    }
}
