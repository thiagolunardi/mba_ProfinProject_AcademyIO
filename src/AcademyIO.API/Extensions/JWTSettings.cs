﻿namespace AcademyIO.API.Extensions
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public int ExpirationHours { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
