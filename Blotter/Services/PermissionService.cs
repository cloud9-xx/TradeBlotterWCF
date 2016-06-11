using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blotter.Model;

namespace Blotter.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly string username;
        public PermissionService(string username)
        {
            this.username = username;
        }

        public bool IsEditable(Trade trade)
        {
            return trade.CreatedBy == username;
        }
    }
}
