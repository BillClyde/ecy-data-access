namespace ECY.DataAccess.Interfaces
{
    public interface IDatabase
    {
        T Query<T>(IQuery<T> query);
        object Execute(ICommand command);
    }
}
