using Code_Road.Dto.Account;

namespace Code_Road.Dto.User
{
    public class FollowingDto
    {
        public StateDto? State { get; set; }
        public int? Count { get; set; }
        public List<UserDetailsDto>? FollowingList { get; set; } = new List<UserDetailsDto>();
    }
}
