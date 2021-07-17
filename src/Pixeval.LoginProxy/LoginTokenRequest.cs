﻿using System.Text.Json.Serialization;

namespace Pixeval.LoginProxy
{
    public class LoginTokenRequest
    {
        [JsonPropertyName("errno")]
        public int Errno { get; set; }

        [JsonPropertyName("cookie")]
        public string? Cookie { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("verifier")]
        public string? Verifier { get; set; }
    }
}