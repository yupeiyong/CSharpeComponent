﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Role : BaseModel
    {
        public String RoleName { get; set; }

        // 权限说明
        public String RoleMemo { get; set; }
    }
}
