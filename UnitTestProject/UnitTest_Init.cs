using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataAccess;
using System.Collections.Generic;
using Models;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest_Init
    {
        [TestMethod]
        public void Test_Init()
        {
            Test_Auth();
            Test_Role();
            Test_RoleAuth();
            Test_User();
            Test_UserAuth();
        }

        [TestMethod]
        public void Test_Auth()
        {
            using (var dao = new DataDbContext())
            {
                var auths = new List<Auth>()
                {
                    new Auth {AuthName="查看公司财务报表",AuthUrl="/Pages/Finance/ViewRpt",AuthMemo="公司高层和财务经理可以查看公司财务报告" },
                    new Auth {AuthName="批阅请示报告",AuthUrl="/Pages/Dept/ViewRequest",AuthMemo="公司高层和部门经理可以批阅请示" },
                    new Auth {AuthName="编写请示报告",AuthUrl="/Pages/Dept/WriteRequest",AuthMemo="部门内的人员、部门经理都可以编写请示报告" },
                };
                dao.Set<Auth>().AddRange(auths);
                dao.SaveChanges();
            }
        }

        [TestMethod]
        public void Test_Role()
        {
            using (var dao = new DataDbContext())
            {
                var roles = new List<Role>()
                {
                    new Role {RoleName="总经理" },
                    new Role {RoleName="部门经理" },
                    new Role {RoleName="普通职员" },
                };
                dao.Set<Role>().AddRange(roles);
                dao.SaveChanges();

            }
        }

        [TestMethod]
        public void Test_RoleAuth()
        {
            /*
                drop table if exists RoleAuth;
                create table RoleAuth (RoleId int not null comment '角色Id',
                AuthId int not null comment '权限Id',
                AuthFlag int not null comment '权限标志,0-AuthId代表权限, 1-AuthId代表角色',
                primary key(RoleId, AuthId, AuthFlag))
                comment '角色中包含的权限和角色';
                -- 总经理角色
                insert into RoleAuth values(1, 1, 0);
                insert into RoleAuth values(1, 2, 0);
                -- 部门经理角色
                insert into RoleAuth values(2, 2, 0);
                insert into RoleAuth values(2, 3, 1);   -- 部门经理角色包含普通职员角色
                -- 普通职员角色
                insert into RoleAuth values(3, 3, 0);
             */
            using (var dao = new DataDbContext())
            {
                var roldAuths = new List<RoleAuth>()
                {
                    new RoleAuth {RoleId=1,AuthId=1,RoleAuthFlag=0 },//总经理角色，可以查看公司财务报表
                    new RoleAuth {RoleId=1,AuthId=2,RoleAuthFlag=0  },//总经理角色，可以批阅请示报告
                    new RoleAuth {RoleId=2,AuthId=2,RoleAuthFlag=0 },//部门经理角色，可以批阅请示报告
                    new RoleAuth {RoleId=2,AuthId=3,RoleAuthFlag=1  },//部门经理角色，包含普通职员角色
                    new RoleAuth {RoleId=3,AuthId=3,RoleAuthFlag=0 },//普通职员角色，可以编写请示报告
                };
                dao.Set<RoleAuth>().AddRange(roldAuths);
                dao.SaveChanges();

            }
        }

        [TestMethod]
        public void Test_User()
        {
            /*
                create table User (UserId int primary key comment '用户Id',
                LoginName varchar(20) not null comment '登录名',
                LoginPwd varchar(20) not null comment '登录密码',
                UserName varchar(20) not null comment '用户姓名',
                DeptId varchar(20) not null comment '所在部门Id',
                PositionId int not null comment '职位Id',
                Status int not null default 1 comment '用户状态,1-正常,0-停用',
                Memo varchar(200) comment '备注')
                comment '用户表';
                insert into user values(1, 'wangzong', 'wz123', '王总', '00', 1, 1, '公司总经理');
                insert into user values(2, 'zhangfei', 'zf123', '张飞', '0002', 2, 1, '研发部经理');
                insert into user values(3, 'huangyun', 'hy123', '黄云', '0003', 2, 1, '财务部经理');
                insert into user values(4, 'lilan', 'pw123', '李兰', '0002', 3, 1, '研发部工程师');
             */
            using (var dao = new DataDbContext())
            {
                var users = new List<UserInfo>()
                {
                    new UserInfo {LoginName="wangzong",LoginPwd="wz123",UserName="王总",DeptId="00",Status=1,Memo="公司总经理" },
                    new UserInfo {LoginName="zhangfei",LoginPwd="zf123",UserName="张飞",DeptId="00",Status=1,Memo="研发部经理" },
                    new UserInfo {LoginName="huangyun",LoginPwd="hy123",UserName="黄云",DeptId="00",Status=1,Memo="财务部经理" },
                    new UserInfo {LoginName="lilan",LoginPwd="pw123",UserName="李兰",DeptId="00",Status=1,Memo="研发部工程师" },
                };
                dao.Set<UserInfo>().AddRange(users);
                dao.SaveChanges();

            }
        }

        [TestMethod]
        public void Test_UserAuth()
        {
            /*
                drop table if exists UserAuth;
                create table UserAuth (UserId int not null comment '用户Id',
                AuthId int not null comment '权限Id',
                AuthFlag int not null comment '权限标志,0-AuthId代表权限, 1-AuthId代表角色',
                primary key(UserId, AuthId, AuthFlag))
                comment '用户所拥有的权限和角色';
                insert into UserAuth values(1, 1, 1);
                insert into UserAuth values(2, 2, 1);
                insert into UserAuth values(3, 2, 1);
                insert into UserAuth values(3, 1, 0);	-- 财务部经理，直接分配权限
                insert into UserAuth values(4, 3, 0);
             */
            using (var dao = new DataDbContext())
            {
                var userAuths = new List<UserAuth>()
                {
                    new UserAuth {UserId=1,AuthId=1,RoleAuthFlag=1},
                    new UserAuth {UserId=2,AuthId=2,RoleAuthFlag=1},
                    new UserAuth {UserId=3,AuthId=2,RoleAuthFlag=1},
                    new UserAuth {UserId=3,AuthId=1,RoleAuthFlag=0},
                    new UserAuth {UserId=4,AuthId=3,RoleAuthFlag=0},
                };
                dao.Set<UserAuth>().AddRange(userAuths);
                dao.SaveChanges();

            }
        }
    }
}
