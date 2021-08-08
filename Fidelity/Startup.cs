using Microsoft.Owin;
using Owin;
using System;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Jwt;  
using Microsoft.Owin.Security;  
using Microsoft.IdentityModel.Tokens;  
using System.Text;  

[assembly: OwinStartup(typeof(Fidelity.Startup))]

namespace Fidelity
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "https://localhost:44387", //some string, normally web url,  
                        ValidAudience = "https://localhost:44387",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_secret_key_12345"))
                    }
                });
        }
    }
}
