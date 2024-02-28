namespace Code_Road.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<Lesson> Lesson { get; set; }
    }
}
