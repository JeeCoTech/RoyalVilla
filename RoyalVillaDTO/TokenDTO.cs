using System;
using System.Collections.Generic;
using System.Text;

namespace RoyalVilla.DTO
{
    public class TokenDTO
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
