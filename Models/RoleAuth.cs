using CSharpeComponents.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class RoleAuth : BaseModel, IRoleAuth
    {
        public long RoleId { get; set; }

        public long AuthId { get; set; }

        public RoleAuthFlagEnum RoleAuthFlag { get; set; }

        public object GetAuthId()
        {
            return AuthId;
        }

        public object GetRoleId()
        {
            return RoleId;
        }
    }
}
