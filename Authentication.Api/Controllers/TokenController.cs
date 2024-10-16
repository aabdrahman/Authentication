using Authentication.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly TokenProvider _tokenProvider;
        private readonly ILogger<TokenController> _logger;
        public TokenController(TokenProvider tokenProvider, ILogger<TokenController> logger)
        {
            _tokenProvider = tokenProvider;
            _logger = logger;
        }   

        [HttpPost("login")]
        public async Task<ActionResult> GenerateToken([FromBody] TokenUser user)
        {
            try
            {
                _logger.LogInformation($"Call Token Generator using: {user}");
                //var token = _tokenProvider.GenerateToken(user);
                var token = await Task.Run(() => _tokenProvider.GenerateToken(user));
                _logger.LogInformation("Token generation successful.");
                return Ok(new TokenResponse(){
                    ResponseCode = "00",
                    ResponseMessage = $"Token Generated Successfully. Expires at: {DateTime.UtcNow.AddMinutes(30)}",
                    ErrorResponse = null,
                    Token = token
                });
            }
            catch(ArgumentNullException ex)
            {
                _logger.LogInformation(ex, ex.Message);
                 return Ok(new TokenResponse(){
                    ResponseCode = "90",
                    ResponseMessage = $"{user.Username} is an invalid user.",
                    ErrorResponse = new {ErrorResponse= ex.Message, ErrorDescription="Invalid User Provided."},
                    Token = null
                });
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.Message, "An Error Occurred generating token.");
                return Ok(new TokenResponse(){
                    ResponseCode = "99",
                    ResponseMessage = "An error occurred generating token",
                    ErrorResponse =  new {ErrorResponse= ex.Message, ErrorDescription="Token Not Generated."},
                    Token = null
                });
            }

        }
    }   
}
