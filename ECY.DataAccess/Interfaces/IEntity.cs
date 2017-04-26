using System.Collections.Generic;
using System.Data;

namespace ECY.DataAccess.Interfaces
{
    public interface IEntity<T> where T : class, IEntity<T>, new()
    {
        IEnumerable<T> Mapper(DataTable table);
    }
}
