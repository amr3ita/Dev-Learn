using Code_Road.Dto.Post;

namespace Code_Road.Dto.User
{
    public class UserProfileDto
    {
        public UsersReactDto? UserInfo { get; set; } = new UsersReactDto();
        public List<PostDto>? Posts { get; set; } = new List<PostDto>();
    }
}
