using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CMS.JwtGenerator
{
    public class JwtTokenConfig
    {
        [JsonPropertyName("secret")]
        public string Secret { get; set; } = string.Empty;

        [JsonPropertyName("issuer")]
        public string Issuer { get; set; } = string.Empty;

        [JsonPropertyName("audience")]
        public string Audience { get; set; } = string.Empty;

        [JsonPropertyName("accessTokenExpiration")]
        public int AccessTokenExpiration { get; set; }
       
    }
}
