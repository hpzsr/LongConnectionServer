using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Consts_Code
{
    public enum Code
    {
        Code_Ok = 1000,
        Code_ParamError,                // 参数错误
        Code_RealNameFail,              // 实名认证失败
        Code_ErrorNetInterface,         // 错误的网络接口
        Code_ServerError,               // 服务端内部错误
        Code_BuQianError,               // 补签失败
        Code_KeyError,                  // 兑换码错误
        Code_HasKey,                    // 已经兑换过
    };

    public enum HeroAction
    {
        HeroAction_Idle = 1,
        HeroAction_Walk,
        HeroAction_Run,
        HeroAction_Attack,
        HeroAction_Skill1,
        HeroAction_Skill2,
        HeroAction_Skill3,
    };
}
