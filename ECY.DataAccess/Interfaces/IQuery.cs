namespace ECY.DataAccess.Interfaces
{
    public interface IQuery<out T>
    {
        T Execute(ISession session);
    }
}
