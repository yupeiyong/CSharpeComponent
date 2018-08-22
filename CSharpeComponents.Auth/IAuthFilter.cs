using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpeComponents.Auth
{
    public interface IAuthFilter
    {
        bool CanAccess(object userId, string url);
    }
}
