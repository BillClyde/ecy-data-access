namespace ECY.DataAccess.Interfaces
{
    public interface IQuery<out TResult>
    {
        TResult Execute(ISession session);
    }
}
