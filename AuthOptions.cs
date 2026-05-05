using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UniRequestAPI
{
    public class AuthOptions
    {
        public const string ISSUER = "RequestAPI_App_Server";
        public const string AUDIENCE = "RequestAPI_App_Client";
        private const string KEY = "5B90F37D-3EB5-4704-96E9-6522EDEDD030";
        public const int LIFETIME = 480;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
