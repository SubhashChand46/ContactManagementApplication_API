﻿using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;

namespace CMS.JwtGenerator
{
    public interface IJwtAuthManager
    {
        JwtAuthResult GenerateTokens(string username, Claim[] claims, DateTime now);
    }
}
