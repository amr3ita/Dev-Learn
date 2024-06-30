using Code_Road.Dto.Account;
using Code_Road.Dto.Comment;
using Code_Road.Dto.User;

namespace Code_Road.Services.CommentService
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetComments(int PostId);
        Task<CommentDto> EditComment(int commentId, string userId, EditDto model);
        Task<CommentDto> GetCommentById(int id, string userId);
        Task<StateDto> DeleteComment(int commentId, int postId, string userId);
        Task<StateDto> AddComment(int postId, string Content);
        Task<List<UsersReactDto>> GetUpVotes(int commentId);
        Task<List<UsersReactDto>> GetDownVotes(int commentId);
    }
}
