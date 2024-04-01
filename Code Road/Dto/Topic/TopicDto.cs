using Code_Road.Dto.Account;

namespace Code_Road.Dto.Topic
{
    public class TopicDto
    {
        public StateDto? State { get; set; }
        public int? Id { get; set; }
        public string? Name { get; set; }
        public List<string>? Lessons { get; set; }
    }
}
