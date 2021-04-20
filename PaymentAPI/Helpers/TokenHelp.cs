using Microsoft.IdentityModel.Tokens;
using PaymentAPI.Models.Dto;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAPI.Helpers
{
    public class TokenHelp
    {
        public static ClaimEntity GetUserInfo(string authorization)
        { // 此方法用解码字符串token，并返回秘钥的信息对象
            try
            {
                ClaimEntity entity = new ClaimEntity();
                string token = String.Empty;
                string bearer = "Bearer ";
                if (authorization != null && authorization.StartsWith(bearer))
                {
                    token = authorization.Substring(bearer.Length);
                }
                if (token != null)
                {
                    // 获取Claims
                    IEnumerator<Claim> ienumerator = new JwtSecurityToken(token).Claims.GetEnumerator();
                    // 遍历
                    while (ienumerator.MoveNext())
                    {
                        var claim = ienumerator.Current;
                        if (claim.Type.ToLower().Equals("id"))
                        {
                            entity.Id = int.Parse(claim.Value);
                        }
                        if (claim.Type.ToLower().Equals("name"))
                        {
                            entity.Name = claim.Value;
                        }
                        if (claim.Type.ToLower().Equals("realname"))
                        {
                            entity.RealName = claim.Value;
                        }
                    }
                }
                // 返回
                return entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
