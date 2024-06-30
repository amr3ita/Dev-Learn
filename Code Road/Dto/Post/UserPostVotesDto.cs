using Code_Road.Dto.Account;

namespace Code_Road.Dto.Post
{
    public class UserPostVotesDto
    {
        public StateDto Status { get; set; }
        public List<int> UpPosts { get; set; }
        public List<int> DownPosts { get; set; }
    }
}
