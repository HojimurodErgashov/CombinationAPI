using System.ComponentModel.DataAnnotations.Schema;

namespace CombinationAPI.Entity
{
    [Table("QuestionList")]
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Answer> Answers { get; set; }
        public string?  ImageURL { get; set; }
        public string FileName { get; set; }
    }
}