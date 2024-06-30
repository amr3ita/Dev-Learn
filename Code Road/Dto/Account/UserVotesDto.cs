using Code_Road.Dto.Comment;
using Code_Road.Dto.Post;

namespace Code_Road.Dto.Account
{
    public class UserVotesDto
    {
        public UserPostVotesDto? PostVotesId { get; set; }
        public UserCommentVotesDto? CommentVotesId { get; set; }
    }
}
