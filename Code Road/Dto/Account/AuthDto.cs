namespace Code_Road.Dto.Account
{
    public class AuthDto
    {
        public StateDto Status { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public int? LastCountinusActiveDays { get; set; }
        public List<string>? Roles { get; set; }
        public string? Token { get; set; }
        public DateTime? TokenExpiresOn { get; set; }
    }
}
