using Code_Road.Dto.Account;

namespace Code_Road.Dto.User
{
    public class FollowersDto
    {
        public StateDto? State { get; set; }
        public int? Count { get; set; }
        public List<UserDetailsDto>? FollowersList { get; set; } = new List<UserDetailsDto>();

    }
}
