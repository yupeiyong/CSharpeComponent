using CSharpeComponents.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class UserInfo : BaseModel, IUserInfo
    {
        public object GetUserId()
        {
            return Id;
        }


        // 登录名
        public string LoginName { get; set; }


        // 登录密码
        public string LoginPwd { get; set; }


        // 用户姓名
        public string UserName { get; set; }


        // 所在部门Id
        public string DeptId { get; set; }


        // 职位Id
        public int PositionId { get; set; }


        // 用户状态,1-正常,0-停用
        public int Status { get; set; }


        // 备注
        public string Memo { get; set; }

    }
}
