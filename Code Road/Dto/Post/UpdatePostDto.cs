using System.ComponentModel.DataAnnotations;

namespace Code_Road.Dto.Post
{
    public class UpdatePostDto
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Content { get; set; }
        public IFormFileCollection? Images { get; set; }

    }
}
