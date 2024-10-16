using System;

namespace Authentication.Api.Models;

public class TokenUser
{
    public required string Username {get; set;}
    public required string Password {get; set;}
}
