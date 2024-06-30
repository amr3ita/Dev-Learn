using Code_Road.Dto.Account;
using System.ComponentModel.DataAnnotations;

namespace Code_Road.Dto.Comment
{
    public class CommentDto
    {
        public StateDto? State { get; set; }
        public int? Id { get; set; }
        public string? UserName { get; set; }
        [Required]
        public string? Content { get; set; }
        public int? Up { get; set; }
        public int? Down { get; set; }
        public DateTime? Date { get; set; }

    }
}
