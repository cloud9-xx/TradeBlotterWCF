using Blotter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blotter.Services
{
    public interface IPermissionService
    {
        bool IsEditable(Trade trade);
    }
}
