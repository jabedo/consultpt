using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace app
{
    internal class TokenProviderOptions
    {
        public string Path { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public SigningCredentials SigningCredentials { get; set; }
        public Func<string, string, Task<ClaimsIdentity>> IdentityResolver { get; set; }
    }
}