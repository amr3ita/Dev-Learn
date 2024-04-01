using Code_Road.Dto.Account;
using Code_Road.Dto.Comment;

namespace Code_Road.Services.CommentService
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetComments(int PostId);
        Task<CommentDto> EditComment(int commentId, string userId,EditDto model);
        Task<StateDto> DeleteComment(int commentId, int postId , string userId);
        Task<StateDto> AddComment(int postId, string userId, EditDto model);
    }
}
