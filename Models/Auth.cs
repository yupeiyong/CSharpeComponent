using CSharpeComponents.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Auth : BaseModel, IAuth
    {

        public object GetAuthId()
        {
            return Id;
        }

        public string GetUrl()
        {
            return AuthUrl;
        }

        // 权限名称
        public string AuthName { get; set; }

        // URL或者内容权限URI
        public string AuthUrl { get; set; }

        // 权限说明
        public string AuthMemo { get; set; }
    }
}
