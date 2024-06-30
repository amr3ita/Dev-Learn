using Code_Road.Dto.Account;

namespace Code_Road.Dto.Comment
{
    public class UserCommentVotesDto
    {
        public StateDto Status { get; set; }
        public List<int> UpComments { get; set; }
        public List<int> DownComments { get; set; }
    }
}
