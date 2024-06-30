using Code_Road.Dto.Post;
using Code_Road.Dto.User;
using Code_Road.Models;

namespace Code_Road.Dto.Account
{
    public class AllUserDataDto
    {
        public ApplicationUser? userInfo { get; set; }
        public bool? IsAdmin { get; set; }
        public List<PostDto>? posts { get; set; }
        public List<FinishedLessonDetailsDto>? finishedLessons { get; set; }
        public string? userImage { get; set; }
        public UserVotesDto UserVotes { get; set; }
    }
}
