using Core.Enums;

namespace DataAccess.Services.Response
{
    public class AuthResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IList<string> Role { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }

        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
