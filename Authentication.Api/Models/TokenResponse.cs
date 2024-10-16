using System;

namespace Authentication.Api.Models;

public class TokenResponse
{
    public string ResponseCode {get; set;}
    public string ResponseMessage {get; set;}
    public string Token {get; set;}
    public object ErrorResponse {get; set;}
}
