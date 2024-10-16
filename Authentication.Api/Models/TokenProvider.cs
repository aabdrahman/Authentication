using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Authentication.Api.Models;

public class TokenProvider(IConfiguration configuration, ILogger<TokenProvider> logger)
{
    public string GenerateToken(TokenUser user)
    {
        logger.LogInformation("Getting all the token users from appsettings file.");
        var TokenUsers = configuration.GetSection("Jwt-Users").Get<List<TokenUser>>()!;
        logger.LogInformation($"All users gotten from appsetings: {GetDetails(TokenUsers)}");

        var details = JsonConvert.SerializeObject(user);
        if(!TokenUsers.Any(u => u.Username == user.Username))
        {
            logger.LogInformation("No User found in list for specified username.");
            throw new ArgumentNullException(nameof(user.Username), "No User found with provided usrname.");
        }
        
        logger.LogInformation("Getting user secret key from the configuration.");
        var Key = configuration["Jwt-Secret"]!;
        logger.LogInformation("User secret fetched successfully.");
        logger.LogInformation("Encoding the user secret as Key");
        var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
        logger.LogInformation("Security key created successfully using the key");
        logger.LogInformation("Cretating credentials for the token generator");
        var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
        logger.LogInformation($"Credentials created successfully:{GetDetails(credentials)}");
        logger.LogInformation("Generating a user token descriptor");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim("Username", user.Username.ToString()),
                new Claim("Password", user.Password)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(configuration.GetValue<int>("JwT:ExpirationInMinute")),
            SigningCredentials = credentials,
            Issuer = configuration["JwT:Issuer"],
            Audience = user.Username
        };
        logger.LogInformation($"Token descriptor created successfully:{tokenDescriptor}");
        logger.LogInformation("Handler creation");
        var TokenHandler = new JsonWebTokenHandler();
        logger.LogInformation("Generate token using the handler.");
        var token = TokenHandler.CreateToken(tokenDescriptor);
        logger.LogInformation($"Token Generated Successfully,.{GetDetails(Encoding.UTF8.GetBytes(token))}");
        logger.LogInformation($"Expires at: {tokenDescriptor.Expires}");

        return token;
    }

    private string GetDetails(Object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.None);
    }

}
