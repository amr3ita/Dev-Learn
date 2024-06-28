
using Code_Road.Dto.Account;

namespace Code_Road.Dto.Post
{
    public class PostDto
    {
        public StateDto Status { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string Content { get; set; }
        public int? Up { get; set; }
        public int? Down { get; set; }
        public DateTime Date { get; set; }
        public List<string> Image_url { get; set; }
    }
}
