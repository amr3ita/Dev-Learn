using Code_Road.Dto.Account;
using Code_Road.Dto.Comment;

namespace Code_Road.Dto.Post
{
    public class PostAndCommentsDto
    {
        public StateDto? State { get; set; }
        public PostDto? post { get; set; }
        public List<CommentDto>? Comments { get; set; }
    }
}
