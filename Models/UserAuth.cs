using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UserAuth:BaseModel
    {
        public long UserId { get; set; }

        public long AuthId { get; set; }

        /// <summary>
        /// 权限标志，=0为权限，=1为角色
        /// </summary>
        public int RoleAuthFlag { get; set; }
    }
}
