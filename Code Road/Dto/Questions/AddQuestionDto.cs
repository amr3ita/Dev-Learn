namespace Code_Road.Dto.Questions
{
    public class AddQuestionDto
    {
        public string QuestionContent { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string? Option3 { get; set; }
        public string? Option4 { get; set; }
        public string CorrectAnswer { get; set; }
        public int Degree { get; set; }
    }
}
