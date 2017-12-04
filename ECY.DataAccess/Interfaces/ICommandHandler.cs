using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECY.DataAccess.Interfaces
{
    public interface ICommandHandler<TCommand>
    {
        object Execute(TCommand command);
    }
}
