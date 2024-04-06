namespace CombinationAPI.Entity
{
    public class Answer
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool isTrue { get; set; }
        public int QuestionId { get; set; }

    }
}
