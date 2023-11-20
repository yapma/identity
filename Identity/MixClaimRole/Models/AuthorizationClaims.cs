using System.Reflection;
using System.Security.Claims;

namespace MixClaimRole.Models
{
    public class AuthorizationClaims
    {
        public static readonly Claim ReadEmail = new Claim("ReadEmail", "ReadEmailValue");
        public static readonly Claim WriteEmail = new Claim("WriteEmail", "WriteEmailValue");
        public static readonly Claim SendEmail = new Claim("SendEmail", "SendEmailValue");

        public static List<Claim> ToList()
        {
            Type t = typeof(AuthorizationClaims);
            FieldInfo[] fields = t.GetFields(BindingFlags.Static | BindingFlags.Public);
            List<Claim> claims = new List<Claim>();
            foreach (FieldInfo fi in fields)
            {
                if (fi.GetValue(t) is Claim claim)
                    claims.Add(claim);
            }
            return claims;
        }
    }
}
