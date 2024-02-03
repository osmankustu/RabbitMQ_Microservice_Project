using IdentityService.Api.Application.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityService.Api.Application.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IConfiguration _config;
        public IdentityService(IConfiguration config)
        {
            _config = config;
        }

        public Task<LoginResponseModel> Login(LoginRequestModel requestModel)
        {
            //DB Process will be here

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier,requestModel.UserName),
                new Claim(ClaimTypes.Name,"Osman Kustu")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["AuthConfig:Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.Now.AddDays(10);

            var token = new JwtSecurityToken(claims: claims, expires: exp, signingCredentials: credentials, notBefore: DateTime.Now);
            
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

            LoginResponseModel responseModel = new()
            {
                UserName = requestModel.UserName,
                UserToken = encodedJwt
            };

            return Task.FromResult(responseModel);
        }
    }
}
