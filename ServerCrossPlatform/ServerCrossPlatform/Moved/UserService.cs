using LitJson;
using ServerCrossPlatform.Test;
using WebSocketSharp;
using Google.Protobuf;

namespace Ymck
{
    public class UserService : Singleton<UserService>
    {
        public void Init()
        {
            MsgProcessor<Session, WebSocketBehaviorEx<Session>>.Instance.RegHandler<UserRegReq>(OnUserRegReq);
            MsgProcessor<Session, WebSocketBehaviorEx<Session>>.Instance.RegHandler<UserLoginReq>(OnUserLoginReq);
            MsgProcessor<Session, WebSocketBehaviorEx<Session>>.Instance.RegHandler<UserCreateTravalerReq>(OnUserCreateTravalerReq);
        }

        public void OnUserRegReq(WebSocketBehaviorEx<Session> sender, UserRegReq req)
        {
            // => MsgHandler统一打印日志
            Console.WriteLine($"REQ: sender = {sender.ID}, req = {JsonMapper.ToJson(req)}");

            var nm = new NetMsg();
            nm.Resp = new RespMsg();
            nm.Resp.UserReg = new UserRegResp();

            // Check user:
            // 原来name是User表的key，所以可以Find，现在不行了，每张表默认key是id，所以要用Where
            var dbUser = EntityService.Instance.Model.DbUserSet.Where(x => x.Name == req.Name).FirstOrDefault();
            if (dbUser != null)
            //if (UserMgr.Instance.GetUser(req.Name)!=null)
            {
                nm.Resp.UserReg.Ok = false;
                nm.Resp.UserReg.ErrMsg = "用户已存在"; // TODO: 多语言
            }
            else
            {
                var newUser = new User()
                {
                    Name = req.Name,
                    Pwd = req.Pwd,
                    // NOTE: 注册时，player还没有创建，是null对象
                };

                // 数据库：
                var dbPlayer = new DbPlayer(); // id字段不用填，系统在插入数据时自动填充
                EntityService.Instance.Model.DbPlayerSet.Add(dbPlayer); // 玩家加到数据库

                dbUser = new DbUser() // id字段不用填，系统在插入数据时自动填充
                {
                    Name = newUser.Name,
                    Pwd = newUser.Pwd,
                    Player = dbPlayer,
                };
                EntityService.Instance.Model.DbUserSet.Add(dbUser);
                EntityService.Instance.Model.SaveChanges();

                // 协议：
                nm.Resp.UserReg.User = newUser;
                nm.Resp.UserReg.Ok = true; // NOTE: 这里不要忘记（在MMO训练营课程里这句没有加）
            }

            sender.SendPBO(nm.ToByteArray()); // 返回注册结果
        }

        public void OnUserLoginReq(WebSocketBehaviorEx<Session> sender, UserLoginReq req)
        {
            // 登录和注册的逻辑相似，直接复制并修改
            var nm = new NetMsg();
            nm.Resp = new RespMsg();
            nm.Resp.UserLogin = new UserLoginResp();

            // 查找用户:
            var dbUser = EntityService.Instance.Model.DbUserSet.Where(x => x.Name == req.Name).FirstOrDefault();
            if (dbUser != null) // 已注册
            {
                // Session中填充用户信息
                sender.session.dbUser = dbUser;

                // 用户信息初始化
                nm.Resp.UserLogin.User = new User(); // 注意：这里是协议中定义的User，不是数据库的User
                nm.Resp.UserLogin.User.Id = dbUser.Id;
                nm.Resp.UserLogin.User.Name = req.Name; // 这里也可以不填充，客户端用不到
                nm.Resp.UserLogin.User.Pwd = string.Empty; // 密码不用返回
                nm.Resp.UserLogin.User.Player = new Player(); // 确保要在注册时已创建玩家
                nm.Resp.UserLogin.User.Player.Id = dbUser.Player.Id;

                // 填充旅行者和队伍成员
                foreach (var member in dbUser.Player.TeamMembers)
                {
                    Character chr = new Character();
                    chr.Id = member.Id;
                    chr.TemplateId = member.TemplateId;
                    chr.Name = member.Name;
                    // 坐标位置暂时没有，可以用默认值0.0f
                    // 这里会出现teamMembers为null的异常，之所以没有暴露，是因为dbUser.Player.TeamMembers数组大小为0
                    // 解决办法是从协议入手
                    nm.Resp.UserLogin.User.Player.TeamMembers.Add(chr);
                }

                nm.Resp.UserLogin.Ok = true;
            }
            else // 未注册
            {
                nm.Resp.UserLogin.Ok = false;
                nm.Resp.UserLogin.ErrMsg = "用户未注册"; // TODO: 多语言
            }

            sender.SendPBO(nm.ToByteArray()); // 返回登录结果
        }

        public void OnUserCreateTravalerReq(WebSocketBehaviorEx<Session> sender, UserCreateTravalerReq req)
        {
            // 登录和注册的逻辑相似，直接复制并修改
            var nm = new NetMsg();
            nm.Resp = new RespMsg();
            nm.Resp.UserCreateTravalerResp = new UserCreateTravalerResp();

            // 初始化dbTravaler
            var dbTravaler = new DbCharacter()
            {
                MapId = 1,
                MapPosX = 138.2f,
                MapPosY = 11f,
                MapPosZ = 73.9f,
                Name = "优梦创客",
                TemplateId = req.IsMale ? 49 : 52,
            };

            // 数据库保存
            EntityService.Instance.Model.DbCharacterSet.Add(dbTravaler);
            sender.session.dbUser.Player.TeamMembers.Add(dbTravaler);
            EntityService.Instance.Model.SaveChanges();

            nm.Resp.UserCreateTravalerResp.Ok = true;

            // 通过dbTravaler去构造travaler

            // 方法1：通过初始化列表去初始化
            //var travaler = new Character()
            //{
            //    id = dbTravaler.Id,
            //    mapId = dbTravaler.MapId,
            //    mapPosX = dbTravaler.MapPosX,
            //    mapPosY = dbTravaler.MapPosY,
            //    mapPosZ = dbTravaler.MapPosZ,
            //    templateId = dbTravaler.TemplateId,
            //    name = dbTravaler.Name,
            //};

            // 方法2：通过构造器
            //var travaler = new Character(dbTravaler);

            // 方法3：通过深拷贝（逐字段的拷贝）
            dbTravaler.DeepCopyTo(out Character travaler);

            nm.Resp.UserCreateTravalerResp.NewTravaler = travaler;

            sender.SendPBO(nm.ToByteArray()); // 返回登录结果
        }
    }
}
