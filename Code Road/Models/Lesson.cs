using System.ComponentModel.DataAnnotations.Schema;

namespace Code_Road.Models
{
    public class Lesson
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Explanation { get; set; }
        public string Level { get; set; }
        public List<Image>? Images { get; set; }

        [ForeignKey(nameof(topic))]
        public int TopicId { get; set; }
        public virtual Topic topic { get; set; }
        public string? ApplicationUserId { get; set; }
        public virtual Quiz Quiz { get; set; }

    }
}