using System.Collections.Generic;
using System.Data;

namespace ECY.DataAccess.Interfaces
{
    /// <summary>
    /// Entity model
    /// </summary>
    /// <typeparam name="T">Model type(same name as model) so a Person would implement IEntity of type Person</typeparam>
    public interface IEntity<T> where T : class, IEntity<T>, new()
    {
        /// <summary>
        /// Method to map the DataTable result to the return model
        /// </summary>
        /// <param name="table">Results datatable</param>
        /// <returns></returns>
        IEnumerable<T> Mapper(DataTable table);
    }
}
